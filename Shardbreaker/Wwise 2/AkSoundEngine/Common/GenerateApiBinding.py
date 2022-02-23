# //////////////////////////////////////////////////////////////////////
# //
# // Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
# //
# //////////////////////////////////////////////////////////////////////
import os, sys, platform, re, collections
from os.path import join, exists
import BuildUtil
import PrepareSwigInput, PostprocessSwigOutput
from BuildInitializer import BuildInitializer

class SwigCommand(object):
	def __init__(self, pathMan):
		self.pathMan = pathMan

		self.EXE = self.pathMan.Paths['SWIGEXE']
		self.platformDefines = []
		self.compilerDefines = []
		self.outputLanguage = ['-csharp']
		self.dllName = []
		self.Includes = []

		self.logger = BuildUtil.CreateLogger(pathMan.Paths['Log'], __file__, self.__class__.__name__)

	def CreatePlatformCommand(self):
		return BuildUtil.BuildModules[self.pathMan.PlatformName].CreateSwigCommand(self.pathMan, self.pathMan.Arch)

	def Run(self):
		exe = [self.EXE]

		output = ['-o', self.pathMan.Paths['SWIG_OutputCWrapper']]
		includes = ['-I{}'.format(self.pathMan.Paths['SWIG_WwiseSdkInclude']), '-I{}'.format(self.pathMan.Paths['SWIG_PlatformInclude']), '-I{}'.format(self.pathMan.Paths['SWIG_CSharpModule'])] + self.Includes
		links = ['-l{}'.format(self.pathMan.Paths['SWIG_WcharModule'])]
		excludeExceptionHandling = ['-DSWIG_CSHARP_NO_EXCEPTION_HELPER', '-DSWIG_CSHARP_NO_WSTRING_HELPER', '-DSWIG_CSHARP_NO_STRING_HELPER']
		outdir = ['-outdir', self.pathMan.Paths['SWIG_OutputApiDir']]
		inputLanguage = ['-c++']
		interface = [self.pathMan.Paths['SWIG_Interface']]

		if not exists(self.pathMan.Paths['SWIG_OutputApiDir']):
			self.logger.info("Output C# API directory does not exist, creating {}".format(self.pathMan.Paths['SWIG_OutputApiDir']))
			try:
				os.makedirs(self.pathMan.Paths['SWIG_OutputApiDir'])
			except:
				pass

		cmd = exe + output + includes + links + self.platformDefines + self.compilerDefines + excludeExceptionHandling + self.dllName + outdir + inputLanguage + self.outputLanguage + interface

		self.logger.debug(' '.join(cmd))

		try:
			(stdOut, stdErr, returnCode) = BuildUtil.RunCommand(cmd)
			self.logger.debug('stdout:\n{}'.format(stdOut))
			self.logger.debug('stderr:\n{}'.format(stdErr))
			isCmdFailed = returnCode != 0
			if isCmdFailed:
				msg = 'SWIG failed with return code: {}, Command: {}'.format(returnCode, ' '.join(cmd))
				raise RuntimeError(msg)
		except Exception as err:
			# Note: logger.exception() is buggy in py3.
			self.logger.error(err)
			raise RuntimeError(err)

class GccSwigCommand(SwigCommand):
	def __init__(self, pathMan):
		SwigCommand.__init__(self, pathMan)
		self.compilerDefines = ['-D__GNUC__', '-DGCC_HASCLASSVISIBILITY']

class AppleSwigCommand(GccSwigCommand):
	def __init__(self, pathMan):
		GccSwigCommand.__init__(self, pathMan)
		self.platformDefines = ['-D__APPLE__', '-DAK_APPLE']

		# Sorted in subclasses. Descending versions.
		self.SdkCompatibilityNodes = collections.OrderedDict()

	def _InitPlatformSdkInfo(self):
		# For UnitTest that runs outside of Xcode on Mac OS X Mountain Lion
		SdkRoot = None

		# Parse versions
		pattern = re.compile(r'{}(?P<version>[0-9]+\.[0-9]+)\.sdk'.format(self.platformSdkName))

		isSdkRootEnvVarExist = 'SDKROOT' in os.environ and os.environ['SDKROOT'] != ''
		if isSdkRootEnvVarExist:
			SdkRoot = os.environ['SDKROOT']
			foundSdks = [os.path.split(SdkRoot)[1]]
			latestVersion = None
			for i, s in enumerate(foundSdks):
				match = pattern.search(s)
				if not match is None:
					latestVersion = match.groupdict()['version']
					break
		else: # search first valid Mac SDK
			self.logger.debug('Detected: Running in command line outside Xcode IDE environment. Proceed to find SDKROOT ourselves.')

			XcodeDeveloperDir = None
			cmd = ['/usr/bin/xcode-select', '--print-path']
			self.logger.debug(' '.join(cmd))
			try:
				(stdOut, stdErr, returnCode) = BuildUtil.RunCommand(cmd)
				outMsgs = stdOut.split(BuildUtil.SpecialChars['PosixLineEnd'])
				errMsgs = stdErr.split(BuildUtil.SpecialChars['PosixLineEnd'])
				for o in outMsgs:
					self.logger.debug('stdout: {}'.format(o))
				for e in errMsgs:
					self.logger.debug('stderr: {}'.format(e))
				isCmdFailed = returnCode != 0
				if isCmdFailed:
					msg = 'Command failed with return code: {}, Command: {}'.format(returnCode, ' '.join(cmd))
					raise RuntimeError(msg)
			except Exception as err:
				# Note: logger.exception() is buggy in py3.
				self.logger.error(err)
				raise RuntimeError(err)
			XcodeDeveloperDir = outMsgs[3]

			# Find latest SDK in SDKROOT folder.
			latestSdkRoot = None
			sdkRootParentDir = '{}/Platforms/{}.platform/Developer/SDKs'.format(XcodeDeveloperDir, self.platformSdkName)
			foundSdks = os.listdir(sdkRootParentDir)
			latestVersion = None
			for i, s in enumerate(foundSdks):
				match = pattern.search(s)
				if not match is None:
					latestVersion = match.groupdict()['version']
					break
			if latestVersion is None:
				msg = 'Failed to find platform SDK for: {}. Aborted.'.format(self.platformSdkName)
				self.logger.error(msg)
				raise RuntimeError(msg)
			latestSdkRoot = join(sdkRootParentDir, '{}{}.sdk'.format(self.platformSdkName, latestVersion))
			SdkRoot = latestSdkRoot

		# Compare latest version with nodes (descending versions).
		headerSwitch = None
		for verNode in self.SdkCompatibilityNodes.keys():
			isGreaterThanNodeVersion = float(latestVersion)+0.00001 > float(verNode)
			if isGreaterThanNodeVersion:
				headerSwitch = self.SdkCompatibilityNodes[verNode]
		if not headerSwitch is None:
			self.platformDefines.append(headerSwitch)

		self.Includes = ['-I{}'.format(join(SdkRoot, 'usr', 'include'))]

def main():
	pathMan = BuildUtil.Init()
	logger = BuildUtil.CreateLogger(pathMan.Paths['Log'], __file__, main.__name__)

	logger.info('Using WWISESDK: {}'.format(pathMan.Paths['Wwise_SDK']))
	logger.info('Generating SWIG binding [{}] [{}]'.format(pathMan.PlatformName, pathMan.Arch))
	logger.info('Remove existing API bindings to avoid conflicts.')
	initer = BuildInitializer(pathMan)
	initer.Initialize()

	logger.info('Prepare SDK header blob for generating API bindings, and platform source code for building plugin.')
	PrepareSwigInput.main(pathMan)

	logger.info('Generate API binding for Wwise SDK.')
	swigCmd = SwigCommand(pathMan).CreatePlatformCommand()
	swigCmd.Run()

	logger.info('Postprocess generated API bindings to make platform and architecture switchable and make iOS API work in Unity.')
	PostprocessSwigOutput.main(pathMan)

	logger.info('SUCCESS. API binding generated under {}.'.format(pathMan.Paths['Deploy_API_Generated']))

if __name__ == '__main__':
	main()
