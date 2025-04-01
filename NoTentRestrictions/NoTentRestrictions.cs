using BepInEx;
using HarmonyLib;

namespace NoTentRestrictions
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.notentrestrictions";
        internal const string Name = "No Tent Restrictions";
        internal const string Version = "1.0.1.0";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal class NoTentRestrictions : BaseUnityPlugin
    {
        internal static NoTentRestrictions Instance { get; private set; }
        
        private void Start()
        {
            Instance = this;
            
            Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: ModInfo.Guid);
        }
        
        internal static void Log(object payload)
        {
            Instance.Logger.LogInfo(data: payload);
        }
    }
}