using System.IO;
using BepInEx.Configuration;

namespace NoTentRestrictions
{
    internal static class NoTentRestrictionsConfig
    {
        internal static ConfigEntry<bool> EnablePlaceTent;
        internal static ConfigEntry<bool> EnableStorageChest;
        internal static ConfigEntry<bool> EnableDeliveryBox;
        internal static ConfigEntry<bool> EnableShippingChest;
        internal static ConfigEntry<bool> EnableMaxFertility;
        internal static ConfigEntry<bool> EnableNoSoilUpgradeLimit;
        internal static ConfigEntry<bool> EnableMaxElectricity;
        
        public static string XmlPath { get; private set; }
        public static string TranslationXlsxPath { get; private set; }

        internal static void LoadConfig(ConfigFile config)
        {
            EnablePlaceTent = config.Bind(
                section: ModInfo.Name,
                key: "Enable Place Tent",
                defaultValue: true,
                description:
                "Enable or disable placing tents inside other tents.\n" +
                "Set to 'true' to allow tents inside tents, or 'false' to block it.\n" +
                "テントの中に他のテントを設置する機能を有効または無効にします。\n" +
                "'true' に設定すると設置でき、'false' に設定するとできません。\n" +
                "启用或禁用在帐篷内放置其他帐篷。\n" +
                "设置为 'true' 可放置，设置为 'false' 禁止放置。"
            );

            EnableStorageChest = config.Bind(
                section: ModInfo.Name,
                key: "Enable Storage Chest",
                defaultValue: true,
                description:
                "Enable or disable using storage chests inside tents.\n" +
                "Set to 'true' to allow access, or 'false' to disable it.\n" +
                "テント内で収納箱を使用する機能を有効または無効にします。\n" +
                "'true' に設定すると使用可能になり、'false' に設定すると使用できません。\n" +
                "启用或禁用在帐篷内使用储物箱。\n" +
                "设置为 'true' 允许使用，设置为 'false' 禁用使用。"
            );

            EnableDeliveryBox = config.Bind(
                section: ModInfo.Name,
                key: "Enable Delivery Box",
                defaultValue: true,
                description:
                "Enable or disable delivery box functionality inside tents.\n" +
                "Set to 'true' to enable, or 'false' to disable it.\n" +
                "テント内で宅配ボックスを機能させるかどうかを有効または無効にします。\n" +
                "'true' に設定すると有効になり、'false' に設定すると無効になります。\n" +
                "启用或禁用帐篷内的宅配箱功能。\n" +
                "设置为 'true' 启用，设置为 'false' 禁用。"
            );

            EnableShippingChest = config.Bind(
                section: ModInfo.Name,
                key: "Enable Shipping Chest",
                defaultValue: true,
                description:
                "Enable or disable using shipping chests inside tents.\n" +
                "Set to 'true' to enable, or 'false' to disable it.\n" +
                "テント内で出荷箱を使用する機能を有効または無効にします。\n" +
                "'true' に設定すると使用可能になり、'false' に設定すると使用できません。\n" +
                "启用或禁用在帐篷内使用出货箱。\n" +
                "设置为 'true' 启用，设置为 'false' 禁用。"
            );

            EnableMaxFertility = config.Bind(
                section: ModInfo.Name,
                key: "Enable Max Fertility",
                defaultValue: false,
                description:
                "Enable or disable max fertility inside tents (99999).\n" +
                "Set to 'true' to boost fertility to 99999, or 'false' to use the default.\n" +
                "テント内の最大肥沃度（99999）を有効または無効にします。\n" +
                "'true' に設定すると肥沃度を99999に増加し、'false' に設定するとデフォルトのままになります。\n" +
                "启用或禁用帐篷内的最大肥沃度（99999）。\n" +
                "设置为 'true' 将肥沃度提高至 99999，设置为 'false' 保持默认值。"
            );
            
            EnableNoSoilUpgradeLimit = config.Bind(
                section: ModInfo.Name,
                key: "Enable No Soil Upgrade Limit",
                defaultValue: false,
                description:
                "Enable or disable unlimited soil upgrades using wrenches in tents.\n" +
                "Set to 'true' to remove the usage limit, or 'false' to keep the default.\n" +
                "テント用の土壌レンチの使用制限を有効または無効にします。\n" +
                "'true' に設定すると制限が解除され、'false' に設定するとデフォルトのままになります。\n" +
                "启用或禁用帐篷中土壤升级扳手的无限使用。\n" +
                "设置为 'true' 移除使用限制，设置为 'false' 保持默认。"
            );
            
            EnableMaxElectricity = config.Bind(
                section: ModInfo.Name,
                key: "Enable Max Electricity",
                defaultValue: false,
                description:
                "Enable or disable max electricity inside tents (99999).\n" +
                "Set to 'true' to boost electricity to 99999, or 'false' to use the default.\n" +
                "テント内の最大電力（99999）を有効または無効にします。\n" +
                "'true' に設定すると電力を99999に増加し、'false' に設定するとデフォルトのままになります。\n" +
                "启用或禁用帐篷内的最大电力（99999）。\n" +
                "设置为 'true' 将电力提高至 99999，设置为 'false' 保持默认值。"
            );
        }
        
        public static void InitializeXmlPath(string xmlPath)
        {
            if (File.Exists(path: xmlPath))
            {
                XmlPath = xmlPath;
            }
            else
            {
                XmlPath = string.Empty;
            }
        }
        
        public static void InitializeTranslationXlsxPath(string xlsxPath)
        {
            if (File.Exists(path: xlsxPath))
            {
                TranslationXlsxPath = xlsxPath;
            }
            else
            {
                TranslationXlsxPath = string.Empty;
            }
        }
    }
}
