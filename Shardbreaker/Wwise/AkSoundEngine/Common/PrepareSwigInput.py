# //////////////////////////////////////////////////////////////////////
# //
# // Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
# //
# //////////////////////////////////////////////////////////////////////
import sys, os, os.path, shutil, subprocess, copy, re, inspect, platform, json
import BuildUtil
from os.path import join, normpath, exists

class FunctionMacroExpander(object):
	def __init__(self, inputLines, lineRegEx, macroDict, toStripOpenBracket=True, toStripCloseBracket=True):
		self.__lines = inputLines
		self.__LineRegEx = lineRegEx
		self.__macroDict = macroDict
		self.__toStripOpenBracket = toStripOpenBracket
		self.__toStripCloseBracket = toStripCloseBracket

	def Process(self):
		''' Iterate through input lines and find function line,
			e.g., AK_EXTERNAPIFUNC( bool, IsInitialized )();
			e.g., AK_EXTERNAPIFUNC( void, GetDefaultInitSettings )(
			e.g., AK_CALLBACK( void, AkCallbackFunc )(
			substitute and output:
			e.g., extern  bool __cdecl IsInitialized();
			e.g., extern  void __cdecl GetDefaultInitSettings(
			e.g., typedef void ( __cdecl *AkCallbackFunc )(
		'''
		for l in range(len(self.__lines)):
			line = self.__lines[l]
			match = re.search(self.__LineRegEx, line)
			isTargetLine = match != None and len(match.string) != 0
			if isTargetLine:
				self.__lines[l] = self.__SubstituteMacros__(line)
				self.__lines[l] = self.__StripBrackets__(self.__lines[l])

		return self.__lines

	def __SubstituteMacros__(self, line):
		keys = self.__macroDict.keys()
		for key in keys:
			line = line.replace(key, self.__macroDict[key])
		return line

	def __StripBrackets__(self, line):
		if self.__toStripOpenBracket:
			openBracket = BuildUtil.SpecialChars['FunctionBrackets']['Open']
			firstOpenBracketCol = line.find(openBracket)
			if firstOpenBracketCol != -1:
				line = line.replace(openBracket, BuildUtil.SpecialChars['WhiteSpace'], 1)

		if self.__toStripCloseBracket:
			closeBracket = BuildUtil.SpecialChars['FunctionBrackets']['Close']
			firstCloseBracketCol = line.find(closeBracket)
			if firstCloseBracketCol != -1:
				line = line.replace(closeBracket, BuildUtil.SpecialChars['WhiteSpace'], 1)

		return line

class MemberListExpander(object):
	s_NotFound = None
	def __init__(self, inputLines, lineRegEx, linesToInsert, brackets, separater=BuildUtil.SpecialChars['Comma']):
		self.__lines = inputLines
		self.__LineRegEx = lineRegEx
		self.__BlockBrackets = brackets
		self.__Separater = separater
		self.__linesToInsert = linesToInsert

	def Process(self):
		for l in range(len(self.__lines)):
			line = self.__lines[l]
			match = re.search(self.__LineRegEx, line)
			isTitleLine = match != None and len(match.string) != 0
			if isTitleLine:
				closureLineNumber = self.__FindClosure__(l)
				assert(closureLineNumber != MemberListExpander.s_NotFound)
				if closureLineNumber == MemberListExpander.s_NotFound: #NOTE: this should not happen!
					continue

				self.__InsertMembers__(closureLineNumber)

	def __FindClosure__(self, startLineNumber):
		for l in range(startLineNumber, len(self.__lines)):
			line = self.__lines[l]
			if self.__IsClosureLine__(line):
				return l

		return MemberListExpander.s_NotFound

	def __InsertMembers__(self, closureLineNumber):
		itemCount = len(self.__linesToInsert)
		for l in range(itemCount):
			# insert in reverse order to have correct order after sequential insert
			itemNumber = len(self.__linesToInsert) - l - 1
			item = self.__linesToInsert[itemNumber]
			self.__lines.insert(closureLineNumber, item)

	def __IsClosureLine__(self, line):
		'''A line that contains '};' and not part of C++ comment,
		   NOTE: We don't check for C comment for now
		'''
		foundCol = line.find(self.__BlockBrackets['Close']+BuildUtil.SpecialChars['StatementEnd'])
		hasClosure = foundCol != -1

		commentCol = line.find(BuildUtil.SpecialChars['CppComment'])
		hasComment = commentCol != -1
		isPartOfCppComment = hasComment and commentCol < foundCol

		return hasClosure and not isPartOfCppComment

def SimpleReplace(inputLines, inPattern, outPattern):
	for l in range(len(inputLines)):
		line = inputLines[l]
		match = re.search(inPattern, line)
		isTargetLine = match != None and len(match.string) != 0
		if isTargetLine:
			inputLines[l] = re.sub(inPattern, outPattern, line)

class SwigApiHeaderBlobber(object):
	def __init__(self, pathMan):
		self.pathMan = pathMan
		self.SdkIncludeDir = self.pathMan.Paths['Wwise_SDK_Include']
		self.PluginCodeDir = self.pathMan.Paths['Src_Common']
		self.inputHeaders = \
		[
			normpath(join(self.SdkIncludeDir, 'AK/SoundEngine/Common/AkSoundEngine.h')),
			normpath(join(self.SdkIncludeDir, 'AK/SoundEngine/Common/AkCommonDefs.h')),
			normpath(join(self.SdkIncludeDir, 'AK/SoundEngine/Common/AkCallback.h')),
			normpath(join(self.SdkIncludeDir, 'AK/SoundEngine/Common/AkMidiTypes.h')),
			normpath(join(self.SdkIncludeDir, 'AK/SoundEngine/Common/AkMidiWwiseCmds.h')),
			normpath(join(self.SdkIncludeDir, 'AK/MusicEngine/Common/AkMusicEngine.h')),
			normpath(join(self.PluginCodeDir, '../Common/AkCallbackSerializer.h')),
			normpath(join(self.SdkIncludeDir, 'AK/Tools/Common/AkMonitorError.h')),
			normpath(join(self.SdkIncludeDir, 'AK/Tools/Common/AkPlatformFuncs.h')),
			normpath(join(self.SdkIncludeDir, 'AK/SoundEngine/Common/AkDynamicDialogue.h')),
			normpath(join(self.SdkIncludeDir, 'AK/SoundEngine/Common/AkQueryParameters.h')),
			normpath(join(self.SdkIncludeDir, 'AK/SoundEngine/Common/AkSpeakerConfig.h')),
			normpath(join(self.SdkIncludeDir, 'AK/Plugin/AkReflectGameData.h')),
			normpath(join(self.SdkIncludeDir, 'AK/SpatialAudio/Common/AkSpatialAudioTypes.h')),
			normpath(join(self.SdkIncludeDir, 'AK/SpatialAudio/Common/AkSpatialAudio.h'))
		]

		self.OutputHeader = join(self.pathMan.Paths['Src_Platform'], 'AkUnityApiHeader.h')

		self.lines = []

	def CombineHeaders(self):
		self.lines = []
		for fullPath in self.inputHeaders:
			rawLines = BuildUtil.ImportFile(fullPath)
			self.lines += rawLines


	def Export(self, externalLines = []):
		outputLines = []

		toUseExternalLines = len(externalLines) != 0
		if toUseExternalLines:
			outputLines = externalLines
		else:
			outputLines = self.lines

		BuildUtil.ExportFile(self.OutputHeader, outputLines)

class CodeBlockExtractor(object):
	def __init__(self, header, startKey, endKey):
		self.header = header
		self.startKey = startKey
		self.endKey = endKey

	def Extract(self):
		lines = BuildUtil.ImportFile(self.header)
		blockRange = [0, len(lines)]
		for ll in range(len(lines)):
			foundStart = lines[ll].find(self.startKey) != -1
			if foundStart:
				blockRange[0] = ll
				break
		ll = len(lines)-1
		while ll >= 0:
			foundStart = lines[ll].find(self.endKey) != -1
			if foundStart:
				blockRange[1] = ll
				break
			ll -= 1
		assert(blockRange[1] >= blockRange[0])
		return lines[blockRange[0] : blockRange[1]+1]

class AkArrayProxyGenerater(object):
	'''Copy Wwise SDK AkArray header and decorate it with SWIG instructions'''
	def __init__(self, pathMan):
		self.pathMan = pathMan

	def Generate(self):
		src = join(self.pathMan.Paths['Wwise_SDK_Include'], 'AK', 'Tools', 'Common', 'AkArray.h')
		dest = join(self.pathMan.Paths['Src_Common'], 'AkArrayProxy.h')

		ExceptionDecorator = [\
			'''\n#include "SwigExceptionSwitch.h"
#ifdef SWIG
PAUSE_SWIG_EXCEPTIONS
#endif\n\n''',
			'''\n#ifdef SWIG
RESUME_SWIG_EXCEPTIONS
#endif // #ifdef SWIG\n\n'''
		]

		inLines = BuildUtil.ImportFile(src)
		outLines = copy.deepcopy(inLines)
		includeRegEx = re.compile(r'^[\s]*#include')
		endifRegEx = re.compile(r'^[\s]*#endif')
		includeLineIndices = []
		endifLineIndices = []
		for i, l in enumerate(inLines):
			if not includeRegEx.search(l) is None:
				includeLineIndices.append(i)
			if not endifRegEx.search(l) is None:
				endifLineIndices.append(i)

		# Find the last #include line and insert exceptinon wrapper opening after it.
		includeTail = includeLineIndices[len(includeLineIndices)-1]
		endifTail = endifLineIndices[len(endifLineIndices)-1]

		# Find the last #endif line and insert exception wrapper ending before it
		outLines[includeTail] += ExceptionDecorator[0]
		outLines[endifTail] = ExceptionDecorator[1] + outLines[endifTail]

		inputPattern = 'AkForceInline'
		outputPattern = BuildUtil.SpecialChars['Null']
		SimpleReplace(outLines, inputPattern, outputPattern)
		SimpleReplace(outLines, 'Iterator', 'AkIterator')

		BuildUtil.ExportFile(dest, outLines)

class PlaylistExtractor(object):
	'''AkDynamicSequence classes definitions need to be split for SWIG template to work.'''
	def __init__(self, IncludeDir):
		self.inputHeader = join(IncludeDir, 'AK/SoundEngine/Common/AkDynamicSequence.h')

		thisScriptDir = os.path.dirname(__file__)
		outputDir = thisScriptDir
		self.outputHeaderDict = \
		{
			'PlaylistItem': join(outputDir, "AkDynamicSequence_PlaylistItem.h"),
			'Playlist': join(outputDir, "AkDynamicSequence_Playlist.h")
		}

	def GenerateNewHeaders(self):
		lines = BuildUtil.ImportFile(self.inputHeader)

		itemStartKey = '/// Playlist Item for Dynamic Sequence Playlist.'
		itemEndKey = '/// List of items to play in a Dynamic Sequence.'
		playlistItemLines = CodeBlockExtractor(self.inputHeader, itemStartKey, itemEndKey).Extract()

		listStartKey = itemEndKey
		listEndKey = ');' # closing parenthesis of the last API in the file.
		playlistLines = CodeBlockExtractor(self.inputHeader, listStartKey, listEndKey).Extract()

		playlistLines = self.ReplaceFunctionMacros(playlistLines)

		includeLines = self.ExtractIncludeLines()
		[includeGuard, namespaceStart, namespaceEnd] = self.CreateEnclosure(['AK', 'SoundEngine', 'DynamicSequence'])

		dependenceForwardDecl = ['class AkExternalSourceArray;']

		# Add includes and namespaces
		playlistItemLines = includeGuard+includeLines+dependenceForwardDecl+namespaceStart + playlistItemLines + namespaceEnd
		playlistLines = includeGuard+includeLines+namespaceStart + playlistLines + namespaceEnd

		self.Export('PlaylistItem', playlistItemLines)
		self.Export('Playlist', playlistLines)

	def ExtractIncludeLines(self):
		includeLines = CodeBlockExtractor(self.inputHeader, '#include', '#include').Extract()

		# Replace AkArray includes with our local proxy (with "ifndef SWIG")
		for ll in range(len(includeLines)):
			if includeLines[ll].find('AkArray') != -1:
				includeLines[ll] = '#include "AkArrayProxy.h"'

		return includeLines

	def CreateEnclosure(self, namespaces):
		includeGuard = ['#pragma once\n']
		namespaceStart = ['namespace %s\n{\n\tnamespace %s\n\t{\n\t\tnamespace %s\n\t\t{\n\t' % (namespaces[0], namespaces[1], namespaces[2])]
		namespaceEnd = ['\t\t}\n\t}\n}']
		return [includeGuard, namespaceStart, namespaceEnd]

	def ReplaceFunctionMacros(self, inputLines):
		lineRegEx = '(AK_EXTERNAPIFUNC)([ \t]*\()([ \t]*[_\w* \t]+)(,[ \t]*)([_\w]+)([ \t]*\))'
		macroDict = \
		{
			'AK_EXTERNAPIFUNC': 'extern'+BuildUtil.SpecialChars['WhiteSpace'],
			BuildUtil.SpecialChars['Comma']: BuildUtil.SpecialChars['WhiteSpace']
		}

		inputPattern = 'AkForceInline'
		outputPattern = BuildUtil.SpecialChars['Null']
		SimpleReplace(inputLines, inputPattern, outputPattern)

		return FunctionMacroExpander(inputLines, lineRegEx, macroDict).Process()

	def Export(self, blockKey, lines):
		outputFile = self.outputHeaderDict[blockKey]
		BuildUtil.ExportFile(outputFile, lines)

class SimpleStructExtractor(object):
	def __init__(self, inputHeader, structRegEx):
		self.__InputHeader = inputHeader
		self.__StructRegEx = structRegEx

	def ExtractStructAsOneString(self):
		fileString = ''
		with open(self.__InputHeader) as f:
			fileString = f.read()
			f.close()

		outputString = ''
		match = re.search(self.__StructRegEx, fileString)
		foundStruct = match != None and len(match.string) != 0
		if foundStruct:
			outputString = match.string[match.start() : match.end()]+BuildUtil.SpecialChars['PosixLineEnd']
			structLines = outputString.split(BuildUtil.SpecialChars['PosixLineEnd'])

		return outputString

class PlatformStructProfile(object):
	def __init__(self, pathMan, headerRelpath, structRegEx, ignoreMemberKeys=[]):
		self.pathMan = pathMan
		self.headerRelpath = headerRelpath
		self.regExKey = structRegEx
		self.ignoredMemberKeys = ignoreMemberKeys

	def CompleteHeaderFullPath(self):
		return join(self.pathMan.Paths['Wwise_SDK_Include'], normpath(self.headerRelpath))

class PlatformApiProfile(PlatformStructProfile):
	def __init__(self, DirDict, headerRelpath, structRegEx, ignoreMemberKeys=[]):
		PlatformStructProfile.__init__(self, DirDict, headerRelpath, structRegEx, ignoreMemberKeys)

class SwigPlatformHandler(object):
	PlatformInitSettingsRegEx = '(struct[ \t]+AkPlatformInitSettings)([\w\W]*?)(\};)'
	ThreadPropertiesRegEx = '(struct[ \t]+AkThreadProperties)([\w\W]*?)(\};)'
	def __init__(self, pathMan):
		self.pathMan = pathMan

		# A list of platform-specific structs to extract
		StreamingSettingHeader = 'AK/SoundEngine/Common/AkStreamMgrModule.h'
		self.PlatformStructProfiles = \
		[
			PlatformStructProfile(self.pathMan, StreamingSettingHeader, '(struct[ \t]+AkStreamMgrSettings)([\w\W]*?)(\};)'),
			PlatformStructProfile(self.pathMan, StreamingSettingHeader, '(struct[ \t]+AkDeviceSettings)([\w\W]*?)(\};)')
		]

		self.PlatformApiProfiles = []

		self.commonFileSource = \
		[
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkFileLocationBase.cpp'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkFileLocationBase.h'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkMultipleFileLocation.cpp'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkMultipleFileLocation.h'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkFilePackageLowLevelIO.h'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkFilePackageLowLevelIO.inl'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkFilePackageLUT.h'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkFilePackageLUT.cpp'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkFilePackage.h'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkFilePackage.cpp'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine/Common/AkFileHelpersBase.h'),

		]
		self.commonFileDestDir = self.pathMan.Paths['Src_Common']

		self.ioFileSources = ['']
		self.ioFileDestDir = self.pathMan.Paths['Src_Platform']
		self.logger = BuildUtil.CreateLogger(self.pathMan.Paths['Log'], __file__, self.__class__.__name__)

	def CopyCommonSdkSourceFiles(self):		
		self._BatchCopyFiles(self.commonFileSource, self.commonFileDestDir)

	def CopyPlatformSdkSourceFiles(self):
		self._BatchCopyFiles(self.ioFileSources, self.ioFileDestDir)

	def _BatchCopyFiles(self, srcs, destDir):
		for src in srcs:
			try:				
				shutil.copy(src, destDir)
			except Exception as e:
				msg = 'Failed to copy source to destination. Src: {}, Dest: {}'.format(src, destDir)
				self.logger.error(msg)
				raise RuntimeError(msg)

	def GetPlatformSettings(self):
		structsLines = []
		for r in self.PlatformStructProfiles:
			structsLines += self.__ExtractStructWithStrip(r)

		return structsLines

	def GetPlatformAPIs(self):
		apiLines = ['// Platform-specific APIs\n']
		for r in self.PlatformApiProfiles:
			apiLines += self.__ExtractStructWithStrip(r)

		return apiLines

	def __ExtractStructWithStrip(self, structRecord):
		structString = self.__ExtractStruct(structRecord)
		structLines = self.__CovertStructStringToLines(structString)

		return self.StripIgnoredStructMembers(structRecord.ignoredMemberKeys, structLines)

	def __CovertStructStringToLines(self, structString):
		structLines = structString.split(BuildUtil.SpecialChars['PosixLineEnd'])
		for ss in range(len(structLines)):
			structLines[ss] += BuildUtil.SpecialChars['PosixLineEnd']
		return structLines

	def StripIgnoredStructMembers(self, ignoredMemberKeys, structLines):
		linesLeft = []
		for memberLine in structLines:
			isToKeepMember = True
			for keyword in ignoredMemberKeys:
				isTargetLine = memberLine.find(keyword) != -1
				if isTargetLine:
					isToKeepMember = False
					self.logger.info( "Strip members from: key: {} memberLine: {}".format(keyword, memberLine) )
					break
			if isToKeepMember:
				linesLeft.append(memberLine)

		return linesLeft

	def __ExtractStruct(self, structRecord):
		'''Extract a struct in a platform-specific setting header, each as a single string, Ignore other helper functions'''
		headerFullpath = structRecord.CompleteHeaderFullPath()
		return SimpleStructExtractor(headerFullpath, structRecord.regExKey).ExtractStructAsOneString()

class SwigPlatformHandlerPOSIX(SwigPlatformHandler):
	def __init__(self, pathMan):
		SwigPlatformHandler.__init__(self, pathMan)

		ThreadPropertyHeader = 'AK/Tools/POSIX/AkPlatformFuncs.h'
		self.PlatformStructProfiles += \
		[
			PlatformStructProfile(self.pathMan, ThreadPropertyHeader, SwigPlatformHandler.ThreadPropertiesRegEx)
		]

		self.ioFileSources = \
		[
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine', 'POSIX', 'stdafx.cpp'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine', 'POSIX', 'stdafx.h'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine', 'POSIX', 'AkFileHelpers.h'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine', 'POSIX', 'AkDefaultIOHookBlocking.cpp'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine', 'POSIX', 'AkDefaultIOHookBlocking.h'),
			join(self.pathMan.Paths['Wwise_SDK_Samples'], 'SoundEngine', 'POSIX', 'AkFilePackageLowLevelIOBlocking.h')
		]

def CreateSwigPlatformHandler(PlatformName, pathMan):
	return BuildUtil.BuildModules[PlatformName].CreateSwigPlatformHandler(pathMan)

def CreateSwigApiHeaderBlobber(PlatformName, pathMan):
	return BuildUtil.BuildModules[PlatformName].CreateSwigApiHeaderBlobber(pathMan)

def GenerateExtraCallbacks(pathMan):
	callbackBlobStrings = []
	overview = '// Unity-only callback type codes for native-side callback delegate. \n// Note: _Val suffix was added to \#define header avoid name clashes with the hacked blob input to SWIG. Do not attempt to rename, and follow this convention when adding new callbacks.\n'
	callbackHeaderStrings = [overview]
	for k, v in iter(BuildUtil.ExtraCallbackTypes.items()):
		callbackBlobStrings.append(', {} = {}'.format(k, v))
		callbackHeaderStrings.append('#define {}_Val {}'.format(k, v))

	BuildUtil.ExportFile(pathMan.Paths['ExtraCallbacks'], callbackHeaderStrings)
	return callbackBlobStrings

def main(pathMan=None):
	if pathMan is None:
		pathMan = BuildUtil.Init()

	logger = BuildUtil.CreateLogger(pathMan.Paths['Log'], __file__, main.__name__)

	PlatformName = pathMan.PlatformName

	# Create C++ output dir if it doesn't exists
	if not exists(pathMan.Paths['Src_Platform']):
		logger.info("Platform Source directory does not exist, creating {}".format(pathMan.Paths['Src_Platform']))
		try:
			os.makedirs(pathMan.Paths['Src_Platform'])
		except:
			pass

	logger.info('Blob necessary API headers.')
	headerBlobber = CreateSwigApiHeaderBlobber(PlatformName, pathMan)
	headerBlobber.CombineHeaders()

	#platform-specific structs
	logger.info('Add misc platform-specific settings.')
	SwigPlatformHandler = CreateSwigPlatformHandler(PlatformName, pathMan)
	SwigPlatformHandler.CopyCommonSdkSourceFiles()
	SwigPlatformHandler.CopyPlatformSdkSourceFiles()
	headerBlobber.lines += SwigPlatformHandler.GetPlatformSettings()
	headerBlobber.lines += SwigPlatformHandler.GetPlatformAPIs()

	# Post-processing: Replace macros with c++ primitive statements.
	# AK_EXTERNAPIFUNC
	logger.info('Expand extern API macro.')
	lineRegEx = r'(AK_EXTERNAPIFUNC)([ \t]*\()([ \t]*[_\w* \t]+)(,[ \t]*)([_\w]+)([ \t]*\))'
	macroDict = \
	{
		# 'AK_EXTERNAPIFUNC': 'extern'+BuildUtil.SpecialChars['WhiteSpace']+'__declspec dllexport'+BuildUtil.SpecialChars['WhiteSpace'],
		'AK_EXTERNAPIFUNC': 'extern'+BuildUtil.SpecialChars['WhiteSpace'],
		BuildUtil.SpecialChars['Comma']: BuildUtil.SpecialChars['WhiteSpace']+'__cdecl'+BuildUtil.SpecialChars['WhiteSpace']
	}
	toStripBrackets = (True, True)
	externExpander = FunctionMacroExpander(headerBlobber.lines, lineRegEx, macroDict)
	outputLines = externExpander.Process()

	# AK_CALLBACK
	logger.info('Expand callback macro.')
	lineRegEx = r'(AK_CALLBACK)([ \t]*\()([ \t]*[_\w \t]+)(,[ \t]*)([_\w]+)([ \t]*\))'
	macroDict = \
	{
		'AK_CALLBACK': 'typedef'+BuildUtil.SpecialChars['WhiteSpace'],
		BuildUtil.SpecialChars['Comma']: '( __cdecl *'
	}
	toStripBrackets = (True, False)
	callbackExpander = FunctionMacroExpander(headerBlobber.lines, lineRegEx, macroDict, toStripBrackets[0], toStripBrackets[1])
	outputLines = callbackExpander.Process()

	# insert members for an enum
	logger.info('Add members for callback types.')
	lineRegEx = '(enum)([ \t]+)(AkCallbackType)'
	memberInserter = MemberListExpander(headerBlobber.lines, lineRegEx, GenerateExtraCallbacks(pathMan), BuildUtil.SpecialChars['CurlyBrackets'])
	memberInserter.Process()

	# remove forceInline
	logger.info('Remove force inline derectives.')
	inputPattern = 'AkForceInline'
	outputPattern = BuildUtil.SpecialChars['Null']
	SimpleReplace(headerBlobber.lines, inputPattern, outputPattern)
	
	logger.info('Generate AkArray proxy header.')
	AkArrayProxyGenerater(pathMan).Generate()

	logger.info('Split PlaylistItem and Playlist for SWIG template.')
	PlaylistExtractor(pathMan.Paths['Wwise_SDK_Include']).GenerateNewHeaders()

	logger.info('Export SWIG input header.')
	headerBlobber.Export(outputLines)

	logger.info('SUCCESS. Required Wwise SDK info for SWIG prepared at: {}'.format(headerBlobber.OutputHeader))

	return 0


#if not __debug__:
#	'''See GenerateApiBinding.py'''
if __name__ == "__main__":
	sys.exit(main())
