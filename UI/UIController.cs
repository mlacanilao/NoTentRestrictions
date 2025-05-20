using System.Configuration;
using System.IO;
using System.Reflection;
using BepInEx;
using EvilMask.Elin.ModOptions;
using EvilMask.Elin.ModOptions.UI;

namespace NoTentRestrictions
{
    public class UIController
    {
        public static void RegisterUI()
        {
            foreach (var obj in ModManager.ListPluginObject)
            {
                if (obj is BaseUnityPlugin plugin && plugin.Info.Metadata.GUID == ModInfo.ModOptionsGuid)
                {
                    var controller = ModOptionController.Register(guid: ModInfo.Guid, tooptipId: "mod.tooltip");
                    
                    var assemblyLocation = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().Location);
                    var xmlPath = Path.Combine(path1: assemblyLocation, path2: "NoTentRestrictionsConfig.xml");
                    NoTentRestrictionsConfig.InitializeXmlPath(xmlPath: xmlPath);
            
                    var xlsxPath = Path.Combine(path1: assemblyLocation, path2: "translations.xlsx");
                    NoTentRestrictionsConfig.InitializeTranslationXlsxPath(xlsxPath: xlsxPath);
                    
                    if (File.Exists(path: NoTentRestrictionsConfig.XmlPath))
                    {
                        using (StreamReader sr = new StreamReader(path: NoTentRestrictionsConfig.XmlPath))
                            controller.SetPreBuildWithXml(xml: sr.ReadToEnd());
                    }
                    
                    if (File.Exists(path: NoTentRestrictionsConfig.TranslationXlsxPath))
                    {
                        controller.SetTranslationsFromXslx(path: NoTentRestrictionsConfig.TranslationXlsxPath);
                    }
                    
                    RegisterEvents(controller: controller);
                }
            }
        }

        private static void RegisterEvents(ModOptionController controller)
        {
            controller.OnBuildUI += builder =>
            {
                var enablePlaceTentToggle = builder.GetPreBuild<OptToggle>(id: "enablePlaceTentToggle");
                enablePlaceTentToggle.Checked = NoTentRestrictionsConfig.EnablePlaceTent.Value;
                enablePlaceTentToggle.OnValueChanged += isChecked =>
                {
                    NoTentRestrictionsConfig.EnablePlaceTent.Value = isChecked;
                };
                
                var enableStorageChestToggle = builder.GetPreBuild<OptToggle>(id: "enableStorageChestToggle");
                enableStorageChestToggle.Checked = NoTentRestrictionsConfig.EnableStorageChest.Value;
                enableStorageChestToggle.OnValueChanged += isChecked =>
                {
                    NoTentRestrictionsConfig.EnableStorageChest.Value = isChecked;
                };
                
                var enableDeliveryBoxToggle = builder.GetPreBuild<OptToggle>(id: "enableDeliveryBoxToggle");
                enableDeliveryBoxToggle.Checked = NoTentRestrictionsConfig.EnableDeliveryBox.Value;
                enableDeliveryBoxToggle.OnValueChanged += isChecked =>
                {
                    NoTentRestrictionsConfig.EnableDeliveryBox.Value = isChecked;
                };
                
                var enableShippingChestToggle = builder.GetPreBuild<OptToggle>(id: "enableShippingChestToggle");
                enableShippingChestToggle.Checked = NoTentRestrictionsConfig.EnableShippingChest.Value;
                enableShippingChestToggle.OnValueChanged += isChecked =>
                {
                    NoTentRestrictionsConfig.EnableShippingChest.Value = isChecked;
                };
                
                var enableMaxElectricityToggle = builder.GetPreBuild<OptToggle>(id: "enableMaxElectricityToggle");
                enableMaxElectricityToggle.Checked = NoTentRestrictionsConfig.EnableMaxElectricity.Value;
                enableMaxElectricityToggle.OnValueChanged += isChecked =>
                {
                    NoTentRestrictionsConfig.EnableMaxElectricity.Value = isChecked;
                };
                
                var enableMaxFertilityToggle = builder.GetPreBuild<OptToggle>(id: "enableMaxFertilityToggle");
                enableMaxFertilityToggle.Checked = NoTentRestrictionsConfig.EnableMaxFertility.Value;
                enableMaxFertilityToggle.OnValueChanged += isChecked =>
                {
                    NoTentRestrictionsConfig.EnableMaxFertility.Value = isChecked;
                };
                
                var enableNoSoilUpgradeLimitToggle = builder.GetPreBuild<OptToggle>(id: "enableNoSoilUpgradeLimitToggle");
                enableNoSoilUpgradeLimitToggle.Checked = NoTentRestrictionsConfig.EnableNoSoilUpgradeLimit.Value;
                enableNoSoilUpgradeLimitToggle.OnValueChanged += isChecked =>
                {
                    NoTentRestrictionsConfig.EnableNoSoilUpgradeLimit.Value = isChecked;
                };
            };
        }
    }
}