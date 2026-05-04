using System;
using System.Runtime.CompilerServices;
using BepInEx;
using HarmonyLib;

namespace NoTentRestrictions;

internal static class ModInfo
{
    internal const string Guid = "omegaplatinum.elin.notentrestrictions";
    internal const string Name = "No Tent Restrictions";
    internal const string Version = "3.1.0";
    internal const string ModOptionsGuid = "evilmask.elinplugins.modoptions";
}

[BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
internal class NoTentRestrictions : BaseUnityPlugin
{
    internal static NoTentRestrictions? Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        NoTentRestrictionsConfig.LoadConfig(config: Config);
        Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: ModInfo.Guid);

        if (HasModOptionsPlugin() == false)
        {
            return;
        }

        try
        {
            UIController.RegisterUI();
        }
        catch (Exception ex)
        {
            LogError(message: $"An error occurred during UI registration: {ex}");
        }
    }

    internal static void Log(object payload)
    {
        LogInfo(message: payload);
    }

    internal static void LogDebug(object message, [CallerMemberName] string caller = "")
    {
        Instance?.Logger.LogDebug(data: $"[{caller}] {message}");
    }

    internal static void LogInfo(object message)
    {
        Instance?.Logger.LogInfo(data: message);
    }

    internal static void LogError(object message)
    {
        Instance?.Logger.LogError(data: message);
    }

    private static bool HasModOptionsPlugin()
    {
        try
        {
            foreach (var obj in ModManager.ListPluginObject)
            {
                if (obj is not BaseUnityPlugin plugin)
                {
                    continue;
                }

                if (plugin.Info.Metadata.GUID == ModInfo.ModOptionsGuid)
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            LogError(message: $"Error while checking for Mod Options: {ex}");
            return false;
        }
    }
}
