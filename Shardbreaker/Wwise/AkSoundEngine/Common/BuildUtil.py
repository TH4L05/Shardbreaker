# //////////////////////////////////////////////////////////////////////
# //
# // Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
# //
# //////////////////////////////////////////////////////////////////////
import sys, os, os.path, shutil, subprocess, logging, logging.handlers, argparse, platform, json, copy, codecs
import pkgutil
import importlib
from os.path import abspath, dirname, join, basename, normpath, exists
from os import pardir as Parent

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(os.path.realpath(__file__)), "..")))

ScriptDir = abspath(dirname(__file__))
IsPython3 = sys.version_info[0] > 2
DefaultCodec = 'ascii'

def CheckPythonVersion():
	isPython27OrAbove = sys.version_info[:2] == (2, 7) or sys.version_info[0] > 2
	if not isPython27OrAbove:
		raise RuntimeError('Unexpected Python version: Expected 2.7 or above, but running Python{}.{}. Install an expected version from python.org and try again. Aborted.'.format(sys.version_info[0], sys.version_info[1]))

Version = '2014.1' # Update this by hand before releasing.

MetadataBaseName = 'BuildWwiseUnityIntegration'
DefaultLogFile = normpath(join(ScriptDir, Parent, Parent, 'Logs', '{}.log'.format(MetadataBaseName)))
IsVerbose = False
DefaultPrefFile = join(ScriptDir, '{}.json'.format(MetadataBaseName))

BuildModules = {}

# Keys are output of platform.system()
# Values are populated inside BuildModule.py of each platform
SupportedPlatforms = \
{
	'Windows': [],
	'Darwin': [],
	'Linux': []
}

# Unity platform vs. Wwise native bank platforms (WwiseCLI -platform switch).
BankPlatforms = \
{
	'placeholder': ['placeholder']
}

# Multi-arch platforms that require swtiching dynamic loading targets using Script Define Symbols.
# Other multi-arch platforms have designated plugin folders for arches.
AkMultiArchPlatforms = \
{
	'Windows': ['Android'],
	'Darwin': ['Android'],
	'Linux' : []
}

SupportedArches = \
{
	'placeholder': ['placeholder']
}

PremakeParameters = \
{
	'placeholder': { 'os': 'placeholder', 'generator': 'vs2017' }
}

DefaultArches = \
{	
}

ApiExt = 'cs'

# Post-SWIG
PlatformSwitches = \
{
	'Common': '#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.'
}

PlatformDependentFilenames = \
[
	'AkSoundEngine.{}'.format(ApiExt),
	'AkSoundEnginePINVOKE.{}'.format(ApiExt),
	'AkUnityPlatformSpecificSettings.{}'.format(ApiExt),
	'AkCommunicationSettings.{}'.format(ApiExt),
	'AkPlatformInitSettings.{}'.format(ApiExt),
	'AkThreadProperties.{}'.format(ApiExt),
	'AkSpeakerVolumes.{}'.format(ApiExt),
	'AkMemPoolAttributes.{}'.format(ApiExt),
	'AkAudioAPI.{}'.format(ApiExt),
	'AkAudioOutputType.{}'.format(ApiExt),
]

MultiArchSwitches = \
{
}

# The single entry for adding custom callback IDs.
# Make sure every key ends with "_Val".
ExtraCallbackTypes = \
{
	'AK_Monitoring': '0x20000000',
	'AK_Bank': '0x40000000', 
	'AK_AudioInterruption': '0x22000000',
	'AK_AudioSourceChange': '0x23000000'
}

Messages = \
	{
		'PlatformOptions': 'available options: {}.'.format(', '.join(SupportedPlatforms[platform.system()])),
		'Error_NoSdkDir': 'Failed to find $WWISESDK folder.',
		'Prog_Success': 'Build succeeded.',
		'Prog_Failed': 'Build failed.',
		'NotImplemented': 'Not implemented. Implement this in a subclass.',
		'CheckLogs': 'Check complete logs under: {}.'.format(normpath(dirname(DefaultLogFile)))
	}

SupportedConfigs = ['Debug', 'Profile', 'Release']

WwiseSdkEnvVar = 'WWISESDK'
AndroidNdkEnvVar = 'NDKROOT'
AndroidSdkEnvVar = 'ANDROID_HOME'
ApacheAntEnvVar = 'ANT_HOME'

# define target header and C++ declaration keywords
SpecialChars = {}
SpecialChars['Null'] = ''
SpecialChars['Comma'] = ','
SpecialChars['WhiteSpace'] = ' '
SpecialChars['StatementEnd'] = ';'
SpecialChars['PosixLineEnd'] = '\n'
SpecialChars['WindowsLineEnd'] = '\r\n'
SpecialChars['FunctionBrackets'] = {'Open': '(', 'Close': ')'}
SpecialChars['CurlyBrackets'] = {'Open': '{', 'Close': '}'}
SpecialChars['CComment'] = {'Open': '/*', 'Close': '*/'}
SpecialChars['CppComment'] = '//'


def CreateLogger(logPath, sourceFile, moduleName):
	'''Create a module-scope logger that writes comprehensive info to a log file and print error messages on screen.'''

	# Note: individual loggers can't share names otherwise they duplicate each other's logging messages.
	logger = logging.getLogger('{} ({})'.format(basename(sourceFile), moduleName))
	logger.setLevel(logging.DEBUG)

	# Avoid duplicated logs caused by duplicated handlers each time we create logger from a different module.
	if len(logger.handlers) > 0:
		return logger

	consoleHanlder = logging.StreamHandler()
	if IsVerbose:
		consoleHanlder.setLevel(logging.DEBUG)
	else:
		consoleHanlder.setLevel(logging.WARNING)
	formatter = logging.Formatter('Wwise: %(levelname)s: %(name)s: %(lineno)d: %(message)s')
	consoleHanlder.setFormatter(formatter)
	logger.addHandler(consoleHanlder)

	if not exists(logPath):
		logDir = os.path.split(logPath)[0]
		try:
			os.makedirs(normpath(logDir))
		except:
			pass

	isOnWindows = platform.system() == 'Windows'
	if isOnWindows:
		# Note: log file rotation is buggy on Windows due to file locking. Don't use it.
		fileHandler = logging.FileHandler(logPath)
	else:
		fileHandler = logging.handlers.TimedRotatingFileHandler(logPath, when='H', interval=1)
	fileHandler.setLevel(logging.DEBUG)
	formatter = logging.Formatter('%(asctime)s: %(levelname)s: %(name)s: %(lineno)d: %(message)s')
	fileHandler.setFormatter(formatter)
	logger.addHandler(fileHandler)

	return logger

def RunCommand(cmd):
	process = subprocess.Popen(cmd, shell=False, stdout=subprocess.PIPE, stderr=subprocess.PIPE, bufsize=0)
	(stdOut, stdErr) = process.communicate()

	if IsPython3:
		stdOut = stdOut.decode(DefaultCodec, 'ignore')
		stdErr = stdErr.decode(DefaultCodec, 'ignore')
	stdOut = '\ncmd: {}\n\n'.format(' '.join(cmd)) + stdOut
	return (stdOut, stdErr, process.returncode)

def ImportFile(inputFile):
	rawLines = []
	with open(inputFile) as f:
		rawLines = f.readlines()
		f.close()

	return rawLines

def ImportCSharpFile(inputFile):
	if sys.version_info[0] < 3:
		import cStringIO
		rawLines = []
		with open(inputFile) as f:
			s = f.read()
			f.close()

		HasUtf8Bom = s.startswith(codecs.BOM_UTF8)
		u = s.decode("utf-8-sig")
		buf = cStringIO.StringIO(u.encode("utf-8"))
		rawLines = buf.readlines()

		return rawLines, HasUtf8Bom
		
	else:
		rawLines = open(inputFile, mode='r', encoding='utf-8-sig').readlines()
		HasUtf8Bom = open(inputFile, 'rb').read(4).startswith(codecs.BOM_UTF8)
		return rawLines, HasUtf8Bom
		

def ExportFile(outputFile, outputLines):
	lines = copy.deepcopy(outputLines)
	# append line separators if none
	nLines = len(lines)
	for ll in range(nLines):
		hasLinebreak = os.linesep in lines[ll] or SpecialChars['PosixLineEnd'] in lines[ll]
		if ll == nLines-1:
			continue
		if not hasLinebreak:
			lines[ll] += SpecialChars['PosixLineEnd']

	with open(outputFile, 'w') as f:
		f.writelines(lines)
		f.close()

def RecursiveReplace(src, dest):
	logger = CreateLogger(DefaultLogFile, __file__, RecursiveReplace.__name__)

	if abspath(src) == abspath(dest): # Bail out to avoid removing src by mistake.
		logger.warning('Src and dest point to same location: {}. Cancelled.'.format(abspath(src)))
		return

	# Remove destination
	if os.path.isdir(dest):
		try:
			shutil.rmtree(dest)
		except Exception as e:
			msg = 'Failed to remove destination folder: {} before replacing it. Error: {}. Aborted.'.format(dest, e)
			logger.error(msg)
			raise RuntimeError(msg)
	elif os.path.isfile(dest):
		try:
			os.remove(dest)
		except Exception as e:
			msg = 'Failed to remove destination file: {} before replacing it. Error: {}. Aborted.'.format(dest, e)
			logger.error(msg)
			raise RuntimeError(msg)
	else:
		# Not exist, proceed to copy.
		pass

	# Copy source to destination: Exclude SCM folders
	if os.path.isdir(src):
		try:
			shutil.copytree(src, dest, ignore=shutil.ignore_patterns('.svn', '.hg', '.git'))
		except Exception as e:
			msg = 'Failed to copy source to destination: Src: {}, Dest: {}. Error: {}. Aborted.'.format(src, dest, e)
			logger.error(msg)
			raise RuntimeError(msg)
	elif os.path.isfile(src):
		# Create dest parent folder if none exists
		destDir = os.path.dirname(dest)
		if not os.path.isdir(destDir):
			try:
				os.makedirs(destDir)
			except Exception as e:
				logger.warning('Failed to create folder destination {}. Error: {}. Ignored'.format(destDir, e))
		try:
			shutil.copy(src, dest)
		except Exception as e:
			msg = 'Failed to copy source to destination: Src: {}, Dest: {}. Error: {}. Aborted.'.format(src, dest, e)
			logger.error(msg)
			raise RuntimeError(msg)
	else:
		msg = 'Source is missing: {}. Aborted.'.format(src)
		logger.error(msg)
		raise RuntimeError(msg)

def WritePreferenceField(key, value):
	data = []
	if exists(DefaultPrefFile):
		with open(DefaultPrefFile) as f:
			dataStr = f.read()
			data = json.loads(dataStr)
			# Update field
			data[0][key] = value
	else:
		# Write as new data
		data = [{key:value}]

	dataStr = json.dumps(data, sort_keys=True, indent=4)
	with open(DefaultPrefFile, 'w') as f:
		f.write(dataStr)

def ReadPreferenceField(key):
	value = None
	data = []
	if exists(DefaultPrefFile):
		try:
			with open(DefaultPrefFile) as f:
				dataStr = f.read()
				data = json.loads(dataStr)
				# Update field
				if key in data[0]:
					value = data[0][key]
		except:
			return None
	return value

class PathManager(object):
	def __init__(self, PlatformName, Arch=None, wwiseSdkRootDir=None):
		'''Initialize all plugin related paths for other scripts to use.'''

		self.Paths = {}
		self.Paths['Root'] = self.NormJoin(os.path.abspath(os.path.dirname(__file__)), Parent, Parent)
		self.Paths['Log'] = DefaultLogFile
		self.logger = CreateLogger(self.Paths['Log'], __file__, self.__class__.__name__)

		if PlatformName == 'Android' and SpecialChars['WhiteSpace'] in self.Paths['Root']:
			msg = 'Unity Integration root path contains white spaces. Android build will fail. Consider removing all white spaces in Unity Integration root path: {}. Aborted.'.format(self.Paths['Root'])
			self.logger.error(msg)
			raise RuntimeError(msg)

		if not PlatformName in SupportedPlatforms[platform.system()]:
			msg = 'Unsupported target platform: {}, {}. Aborted.'.format(PlatformName, Messages['PlatformOptions'])
			self.logger.error(msg)
			raise RuntimeError(msg)

		self.PlatformName = PlatformName
		self.Arch = Arch

		self.Paths['Platform'] = self.PlatformName

		self.ProductName = 'AkSoundEngine'

		# NOTE: Assume this script stays in Common code folder!
		# We have to derive project dir here because on Android, Eclipse uses Win32 path, while Cygwin does not
		# while they use the same script.

		self.Paths['Src'] = self.NormJoin(self.Paths['Root'], self.ProductName)
		self.Paths['Src_Platform'] = self.NormJoin(self.Paths['Src'], self.Paths['Platform'])

		#
		# NOTE: Input arg overrides environment variable.
		#

		# Test if WWISESDK dir is defined.
		isWwiseSdkDirGiven = not wwiseSdkRootDir is None
		if isWwiseSdkDirGiven:
			os.environ[WwiseSdkEnvVar] = wwiseSdkRootDir
		else:
			if not WwiseSdkEnvVar in os.environ:
				msg = Messages['Error_NoSdkDir']
				self.logger.error(msg)
				raise RuntimeError(msg)

		# Test if WWISESDK dir exists
		# NOTE: On Windows, the valid path needs to be like D:\Wwise v2012.1.4 build 4260\SDK, without doublequotes.
		wwiseSdkRootDir = os.environ[WwiseSdkEnvVar]
		isUseCygwinStylePath = sys.platform == 'cygwin'
		if isUseCygwinStylePath:
			msg = 'Running under Cygwin Python. This would confuse SWIG. Try again using system Python instead. Aborted.'
			self.logger.error(msg)
			raise RuntimeError(msg)
		if not exists(wwiseSdkRootDir):
			msg = Messages['Error_NoSdkDir']
			self.logger.error(msg)
			raise RuntimeError(msg)

		self.Paths['Wwise_SDK'] = wwiseSdkRootDir
		self.Paths['Wwise_SDK_Include'] = self.NormJoin(self.Paths['Wwise_SDK'], 'include')
		self.Paths['Wwise_SDK_Samples'] = self.NormJoin(self.Paths['Wwise_SDK'], 'samples')
		self.Paths['Wwise_SDK_Doxygen'] = self.NormJoin(self.Paths['Wwise_SDK'], 'private')

		self.Paths['Src_Common'] = self.NormJoin(self.Paths['Src'], 'Common')

		self.Paths['Doc'] = self.NormJoin(self.Paths['Root'], 'Documentation')
		self.Paths['Installer'] = self.NormJoin(self.Paths['Root'], 'Installers')

		self.Paths['Deploy'] = self.NormJoin(self.Paths['Root'], 'Integration', 'Assets', 'Wwise')
		self.Paths['Deploy_API'] = self.NormJoin(self.Paths['Deploy'], 'API', 'Runtime')
		self.Paths['Deploy_API_Generated'] = self.NormJoin(self.Paths['Deploy_API'], 'Generated')
		self.Paths['Deploy_API_Generated_Common'] = self.NormJoin(self.Paths['Deploy_API_Generated'], 'Common')
		if sys.platform == 'darwin':
			self.Paths['Deploy_API_Generated_Desktop'] = self.NormJoin(self.Paths['Deploy_API_Generated'], 'Mac')
		else:
			self.Paths['Deploy_API_Generated_Desktop'] = self.NormJoin(self.Paths['Deploy_API_Generated'], 'Windows')
		self.Paths['Deploy_API_Generated_Platform'] = self.NormJoin(self.Paths['Deploy_API_Generated'], self.Paths['Platform'])
		self.Paths['Deploy_API_Generated_Platform_PInvoke'] = self.NormJoin(self.Paths['Deploy_API_Generated_Platform'], '{}PINVOKE.{}'.format(self.ProductName, ApiExt))
		self.Paths['Deploy_API_Generated_Platform_Module'] = self.NormJoin(self.Paths['Deploy_API_Generated_Platform'], '{}.{}'.format(self.ProductName, ApiExt))

		self.Paths['Deploy_API_Handwritten'] = self.NormJoin(self.Paths['Deploy_API'], 'Handwritten')
		self.Paths['Deploy_Plugins'] = self.NormJoin(self.Paths['Deploy'], 'API', 'Runtime', 'Plugins', self.Paths['Platform'])

		self.Paths['Deploy_Components'] = self.NormJoin(self.Paths['Deploy'], 'MonoBehaviour', 'Runtime')
		self.Paths['Deploy_Dependencies'] = self.NormJoin(self.Paths['Deploy'], 'Dependencies')

		self.Paths['Editor'] = self.NormJoin(self.Paths['Root'], 'Editor')

		if sys.platform == 'darwin':
			self.Paths['SWIGEXE'] = '/usr/local/bin/swig'
			self.Paths['SWIG_CSharpModule'] = '/usr/local/Cellar/swig@3/3.0.12/share/swig/3.0.12/csharp'
			self.Paths['SWIG_WcharModule'] = self.NormJoin(self.Paths['SWIG_CSharpModule'], 'wchar.i')
		elif sys.platform == 'linux':
			self.Paths['SWIGEXE'] = '/usr/local/bin/swig'
			self.Paths['SWIG_CSharpModule'] = '/usr/local/share/swig/3.0.12/csharp'
			self.Paths['SWIG_WcharModule'] = self.NormJoin(self.Paths['SWIG_CSharpModule'], 'wchar.i')
		else:
			self.Paths['SWIG'] = 'c:\\swigwin-3.0.12'
			self.Paths['SWIGEXE'] = self.NormJoin(self.Paths['SWIG'], 'swig.exe')
			self.Paths['SWIG_CSharpModule'] = self.NormJoin(self.Paths['SWIG'], 'Lib', 'csharp')
			self.Paths['SWIG_WcharModule'] = self.NormJoin(self.Paths['SWIG_CSharpModule'], 'wchar.i')
		
		self.Paths['SWIG_Interface'] = self.NormJoin(self.Paths['Src_Common'], 'SoundEngine.swig')
		self.Paths['SWIG_OutputApiDir'] = self.Paths['Deploy_API_Generated_Platform']
		self.Paths['SWIG_OutputCWrapper'] = self.NormJoin(self.Paths['Src_Platform'], 'SoundEngine_wrap.cxx')
		self.Paths['SWIG_WwiseSdkInclude'] = self.Paths['Wwise_SDK_Include']
		self.Paths['SWIG_PlatformInclude'] = self.Paths['Src_Platform']

		self.Paths['Src_Script_GenApi'] = self.NormJoin(self.Paths['Src_Common'], 'GenerateApiBinding.py')

		self.Paths['ExtraCallbacks'] = self.NormJoin(self.Paths['Src_Common'], 'ExtraCallbacks.h')

	def ConvertWindowsToCygwinPath(self, path):
		return subprocess.check_output(['cygpath', '-u', path]).strip(SpecialChars['PosixLineEnd'])

	def ConvertCygwinToWindowsPath(self, path):
		return subprocess.check_output(['cygpath', '-w', path]).strip(SpecialChars['PosixLineEnd'])

	def NormJoin(self, *args):
		return normpath(join(*args))

def InitPlatforms(argv=None):
	fileDirPath = os.path.dirname(os.path.realpath(__file__))
	akSoundEnginePath = os.path.abspath(os.path.join(fileDirPath, ".."))
	allPlatforms = sorted([d for d in os.listdir(akSoundEnginePath) if os.path.isdir(os.path.join(akSoundEnginePath, d)) and d != "Common"], key=str.lower)
	for platform in allPlatforms:
		modulename = platform + '.BuildModule'
		try:
			if pkgutil.find_loader(modulename) is not None:
				Module = importlib.import_module(modulename)
				BuildModules[platform] = Module
				Module.Init()
		except ImportError:
			continue

def Init(argv=None):
	InitPlatforms()
	CheckPythonVersion()

	logger = CreateLogger(DefaultLogFile, __file__, Init.__name__)

	parser = argparse.ArgumentParser(description='Pre-build event to generate Unity script binding of Wwise SDK')
	parser.add_argument('targetPlatform', metavar='platform', action='store', help='Target platform name, available options: {}.'.format(', '.join(SupportedPlatforms[platform.system()])))
	parser.add_argument('-a', '--arch', nargs=1, dest='arch', default=None, help='The target architecture to build for certain platforms, available options: Windows: {}, Android: {}; WSA: {}, default to None if none given.'.format(', '.join(SupportedArches['Windows']), ', '.join(SupportedArches['Android']), ', '.join(SupportedArches['WSA'])))
	Undefined = 'Undefined'
	envWwiseSdkDir = Undefined
	if WwiseSdkEnvVar in os.environ:
		envWwiseSdkDir = os.environ[WwiseSdkEnvVar]
	parser.add_argument('-w', '--wwisesdkdir', nargs=1, dest='wwiseSdkDir', default=None, help='The Wwise SDK folder to build the Integration against. Required if -u is used. Abort if the specified path is not found; if no path specified in arguments, fall back to the environment variable WWISESDK. Current WWISESDK = {}. For Android, no white spaces are allowed in the Wwise SDK folder path.'.format(DefaultPrefFile, envWwiseSdkDir))
	parser.add_argument('-V', '--verbose', action='store_true', dest='isVerbose', default=False, help='Set the flag to show all log messages to console, default to unset (quiet).')

	if argv is None:
		args = parser.parse_args()
	else: # for unittest
		args = parser.parse_args(argv[1:])

	targetPlatform = args.targetPlatform
	targetArch = None
	if not args.arch is None:
		targetArch = args.arch[0]

	wwiseSdkDir = None
	if args.wwiseSdkDir is None:
		logger.warning('No Wwise SDK folder specified (-w). Fall back to use environment variable: {} = {}'.format(WwiseSdkEnvVar, envWwiseSdkDir))
	else:
		wwiseSdkDir = args.wwiseSdkDir[0]

	IsVerbose = args.isVerbose

	pathMan = PathManager(targetPlatform, targetArch, wwiseSdkDir)

	return pathMan

if __name__ == '__main__':
	pass
