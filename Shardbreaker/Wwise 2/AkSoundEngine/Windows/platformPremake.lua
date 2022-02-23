-- Windows Unity premake module
local platform = {}

function platform.setupTargetAndLibDir(baseTargetDir)
    libdirs(wwiseSDKEnv .. "/%{cfg.platform}" .. GetSuffixFromCurrentAction() .. "/%{cfg.buildcfg}(StaticCRT)/lib")
end

function platform.platformSpecificConfiguration()
    links {
        "dxguid"
    }

    filter "Debug* or Profile*"
        links { "ws2_32" }
end

return platform