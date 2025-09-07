using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace NoTentRestrictions
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.notentrestrictions";
        internal const string Name = "No Tent Restrictions";
        internal const string Version = "2.1.0.0";
        internal const string ModOptionsGuid = "evilmask.elinplugins.modoptions";
        internal const string ModOptionsAssemblyName = "ModOptions";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal class NoTentRestrictions : BaseUnityPlugin
    {
        internal static NoTentRestrictions Instance { get; private set; }
        
        private void Start()
        {
            Instance = this;
            NoTentRestrictionsConfig.LoadConfig(config: Config);
            Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: ModInfo.Guid);
        }
        
        private void Awake()
        {
            if (IsModOptionsInstalled())
            {
                try
                {
                    UIController.RegisterUI();
                }
                catch (Exception ex)
                {
                    Log(payload: $"An error occurred during UI registration: {ex.Message}");
                }
            }
            else
            {
                Log(payload: "Mod Options is not installed. Skipping UI registration.");
            }
        }
        
        internal static void Log(object payload)
        {
            Instance.Logger.LogInfo(data: payload);
        }
        
        private bool IsModOptionsInstalled()
        {
            try
            {
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Any(predicate: (Assembly assembly) => assembly.GetName().Name == ModInfo.ModOptionsAssemblyName);
            }
            catch (Exception ex)
            {
                Log(payload: $"Error while checking for Mod Options: {ex.Message}");
                return false;
            }
        }
    }
}