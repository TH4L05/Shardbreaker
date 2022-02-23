# Mac Wwise Unity Build module
import BuildUtil
import BuildWwiseUnityIntegration
import GenerateApiBinding
import collections
from os import path
from BuildWwiseUnityIntegration import XCodeBuilder
from GenerateApiBinding import AppleSwigCommand
from PrepareSwigInput import SwigPlatformHandlerPOSIX, SwigApiHeaderBlobber, PlatformStructProfile

class MacBuilder(XCodeBuilder):
	def __init__(self, platformName, arches, configs, generateSwig, generatePremake):
		XCodeBuilder.__init__(self, platformName, arches, configs, generateSwig, generatePremake)

class MacSwigCommand(AppleSwigCommand):
	def __init__(self, pathMan):
		AppleSwigCommand.__init__(self, pathMan)

		self.platformDefines.append('-D__i386__') # Does not have to be 64bit, just to suppress error for SWIG.

		self.platformSdkName = 'MacOSX'
		self.SdkCompatibilityNodes = collections.OrderedDict() # Nodes in the "greater and equal too" sense.
		self._InitPlatformSdkInfo()

class SwigApiHeaderBlobberMac(SwigApiHeaderBlobber):
	def __init__(self, pathMan):
		SwigApiHeaderBlobber.__init__(self, pathMan)

		self.inputHeaders.append(path.normpath(path.join(self.SdkIncludeDir, 'AK/SoundEngine/Platforms/Mac/AkMacSoundEngine.h')))

class SwigPlatformHandlerMac(SwigPlatformHandlerPOSIX):
	def __init__(self, pathMan):
		SwigPlatformHandlerPOSIX.__init__(self, pathMan)

def Init(argv=None):
	BuildUtil.BankPlatforms['Mac'] = 'Mac'
	BuildUtil.PremakeParameters['Mac'] = { 'os': 'macosx', 'generator': 'xcode4' }
	BuildUtil.PlatformSwitches['Mac'] = '#if (UNITY_STANDALONE_OSX && !UNITY_EDITOR) || UNITY_EDITOR_OSX'
	BuildUtil.SupportedPlatforms['Darwin'].append('Mac')

def CreatePlatformBuilder(platformName, arches, configs, generateSwig, generatePremake):
	return MacBuilder(platformName, arches, configs, generateSwig, generatePremake)

def CreateSwigCommand(pathMan, arch):
	return MacSwigCommand(pathMan)

def CreateSwigPlatformHandler(pathMan):
	return SwigPlatformHandlerMac(pathMan)

def CreateSwigApiHeaderBlobber(pathMan):
	return SwigApiHeaderBlobberMac(pathMan)

if __name__ == '__main__':
	pass
