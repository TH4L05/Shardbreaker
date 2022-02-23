newoption
{
	trigger     = "wwiseroot",
	value       = "PATH",
	description = "Use this path as the root for Wwise folder. Will set up WwiseSDK and disallow using the environment variable."
}


if _OPTIONS["wwiseroot"] then
	wwiseRoot = _OPTIONS["wwiseroot"]
	wwiseSDK = path.getabsolute(wwiseRoot .. "/SDK")
	wwiseSDKEnv = wwiseSDK
	print("Hard-coding wwiseSDK path to: " .. wwiseSDK)
else
	wwiseSDK = path.getabsolute(os.getenv("WWISESDK"))
	wwiseSDKEnv = "$(WWISESDK)"
	print("Using default wwiseSDK environment variable: " .. wwiseSDK)
end


package.path = package.path .. ";" .. path.getabsolute(wwiseSDK .. "/source/Build") .. "/?.lua" .. ';'.. path.getabsolute(wwiseSDK .. "/../Scripts/Build") .. "/?.lua"

require "PremakePlatforms"
require "Premake5Helper"
require "Premake5PathHelpers"

_AK_ROOT_DIR = path.getabsolute(wwiseSDK .. "/source/Build") .. "/"

local unityPlatforms = {}
for index, folder in ipairs(os.matchdirs("../*")) do
	local platformModule = path.getabsolute(path.join(folder, "platformPremake.lua"))
	if os.isfile(platformModule) then
		unityPlatform = require(path.replaceextension(platformModule, ""))
		unityPlatforms[path.getbasename(folder)] = unityPlatform
	end
end

local platform = AK.Platform

function SoundEngineInternals(suffix, communication)
	local d = {
		"AkMusicEngine"..suffix,
		"AkSpatialAudio"..suffix,
		"AkMemoryMgr"..suffix,
		"AkStreamMgr"..suffix,
	}

	if communication then
		d[#d+1] = "CommunicationCentral"..suffix
	end

	d[#d+1] = "AkSoundEngine"..suffix
	d[#d+1] = "AkSilenceSource"..suffix
	d[#d+1] = "AkSineSource"..suffix
	d[#d+1] = "AkToneSource"..suffix
	d[#d+1] = "AkSynthOneSource"..suffix
	d[#d+1] = "AkVorbisDecoder"..suffix
	d[#d+1] = "AkOpusDecoder"..suffix
	d[#d+1] = "AkAudioInputSource"..suffix

	return d
end

function Dependencies(suffix, communication)
	local d = SoundEngineInternals(suffix, communication)

	d[#d+1] = "AkCompressorFX"..suffix
	d[#d+1] = "AkDelayFX"..suffix
	d[#d+1] = "AkExpanderFX"..suffix
	d[#d+1] = "AkFlangerFX"..suffix
	d[#d+1] = "AkGainFX"..suffix
	d[#d+1] = "AkGuitarDistortionFX"..suffix
	d[#d+1] = "AkHarmonizerFX"..suffix
	d[#d+1] = "AkMatrixReverbFX"..suffix
	d[#d+1] = "AkMeterFX"..suffix
	d[#d+1] = "AkParametricEQFX"..suffix
	d[#d+1] = "AkPeakLimiterFX"..suffix
	d[#d+1] = "AkPitchShifterFX"..suffix
	d[#d+1] = "AkRecorderFX"..suffix
	d[#d+1] = "AkReflectFX"..suffix
	d[#d+1] = "AkRoomVerbFX"..suffix
	d[#d+1] = "AkStereoDelayFX"..suffix
	d[#d+1] = "AkTimeStretchFX"..suffix
	d[#d+1] = "AkTremoloFX"..suffix

	return d
end

function SetupTargetAndLibDir(platformName)
	if platformName == "Mac" or platformName == "iOS" or platformName == "tvOS" then
		xcodebuildsettings {
			WWISESDK = wwiseSDK
		}
	end

	local baseTargetDir = "../../Integration/Assets/Wwise/API/Runtime/Plugins/"

	targetdir(baseTargetDir .. platformName .. "/%{cfg.architecture}/%{cfg.buildcfg}")

	if unityPlatforms[platformName] ~= nil then
		unityPlatforms[platformName].setupTargetAndLibDir(baseTargetDir)
	else
		libdirs(wwiseSDKEnv .. "/%{cfg.platform}_%{cfg.architecture}/%{cfg.buildcfg}/lib")
	end
end

function FixupPropsheets(prj)
	-- Need to set the propsheets that way because the Wwise premake scripts insert a relative path.
	-- Since the Wwise SDK can be installed anywhere on a customer's machine, we need to use the
	-- environment variable instead. Simply using vs_propsheet() does not work, because it appends 
	-- to the internal array. We need to actually get to the internal array, and set it to what we
	-- need.
	for cfg in premake.project.eachconfig(prj) do
		for i, ps in ipairs(cfg.vs_propsheet) do
			cfg.vs_propsheet[i] = string.gsub(ps, wwiseSDK, "$(WWISESDK)")
		end
	end
end

function CreateCommonProject(platformName, suffix)
	-- A project defines one build target
	local pfFolder = "../"..platformName.."/";
	local prj = AK.Platform.CreateProject("AkSoundEngine" .. platformName, "AkSoundEngine" .. platformName, pfFolder, GetSuffixFromCurrentAction(), nil, "SharedLib")
		targetname "AkSoundEngine"
		-- Files must be explicitly specified (no wildcards) because our build system calls premake before copying the sample files
		files {
			"../Common/AkCallbackSerializer.cpp",
			pfFolder .. "AkDefaultIOHookBlocking.cpp",
			"../Common/AkFileLocationBase.cpp",
			"../Common/AkMultipleFileLocation.cpp",
			"../Common/AkFilePackage.cpp",
			"../Common/AkFilePackageLUT.cpp",
			"../Common/AkSoundEngineStubs.cpp",
			pfFolder .. "SoundEngine_wrap.cxx",
			pfFolder .. "stdafx.cpp",
		}

		defines {
			"AUDIOKINETIC",
		}

		includedirs {
			"../"..platformName,
			wwiseSDKEnv .. "/include",
			"."
		}

		if platformName == "Mac" then
			sharedlibtype "OSXBundle"
		end

		SetupTargetAndLibDir(platformName)
		
		configuration "Debug"
			defines { "_DEBUG" }
			symbols "On"
			links(Dependencies(suffix, true))

		configuration "Profile"
			defines { "NDEBUG" }
			optimize "On"
			links(Dependencies(suffix, true))

		configuration "Release"
			defines { "NDEBUG", "AK_OPTIMIZED" }
			optimize "On"
			links(Dependencies(suffix, false))

		configuration "*"
		if unityPlatforms[platformName] ~= nil then
			unityPlatforms[platformName].platformSpecificConfiguration()
		end

		if wwiseSDKEnv ~= wwiseSDK then
			FixupPropsheets(prj)
		end
	return prj
end

-- This is the main() of the script
if table.find(platform.validActions, _ACTION) then
	local unityPlatformName = platform.name

	if platform.name == "UWP" then
		unityPlatformName = "WSA"
	elseif platform.name == "NX" then
		unityPlatformName = "Switch"
	elseif platform.name == "GGP" then
		unityPlatformName = "Stadia"
	end

	solution("AkSoundEngine" .. unityPlatformName)
	configurations { "Debug", "Profile", "Release" }

	if unityPlatformName == "Android" then
		location("../Android/jni/")
	else
		location("../" .. unityPlatformName)
	end

	local cfgplatforms = AK.Platform.platforms
	if type(cfgplatforms) == 'function' then
		cfgplatforms()
	else
		platforms(cfgplatforms)

		-- mlarouche: Workaround AK.Platforms not specifying platforms and architecture for Linux
		if unityPlatformName == "Linux" then
			platforms { "x64" }

			filter "platforms:x64"
				architecture "x86_64"
		end
	end
	
	CreateCommonProject(unityPlatformName, "")
else
	if _ACTION ~= nil then
		premake.error(string.format("No valid action specified for platform %s, valid actions are: %s", platform.name, table.concat(platform.validActions, ",")))
	end
end
