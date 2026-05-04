using System.Collections;
using System.IO;
using System.Reflection;
using BepInEx.Configuration;
using EvilMask.Elin.ModOptions;
using EvilMask.Elin.ModOptions.UI;
using UnityEngine.UI;

namespace NoTentRestrictions;

internal static class UIController
{
    private const int FertilityModeVanilla = 0;
    private const int FertilityModeMax = 1;
    private const int FertilityModeWrench = 2;

    public static void RegisterUI()
    {
        ModOptionController controller = ModOptionController.Register(guid: ModInfo.Guid, tooptipId: "mod.tooltip");
        if (controller == null)
        {
            NoTentRestrictions.LogError(message: "Failed to register Mod Options controller.");
            return;
        }

        string assemblyLocation = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        string xmlPath = Path.Combine(path1: assemblyLocation, path2: "NoTentRestrictionsConfig.xml");
        string xlsxPath = Path.Combine(path1: assemblyLocation, path2: "translations.xlsx");

        NoTentRestrictionsConfig.InitializeXmlPath(xmlPath: xmlPath);
        NoTentRestrictionsConfig.InitializeTranslationXlsxPath(xlsxPath: xlsxPath);

        if (File.Exists(path: NoTentRestrictionsConfig.XmlPath))
        {
            controller.SetPreBuildWithXml(xml: File.ReadAllText(path: NoTentRestrictionsConfig.XmlPath));
        }
        else
        {
            NoTentRestrictions.LogError(message: $"Mod Options XML not found: {xmlPath}");
        }

        if (File.Exists(path: NoTentRestrictionsConfig.TranslationXlsxPath))
        {
            controller.SetTranslationsFromXslx(path: NoTentRestrictionsConfig.TranslationXlsxPath);
        }
        else
        {
            NoTentRestrictions.LogError(message: $"Mod Options translations not found: {xlsxPath}");
        }

        RegisterEvents(controller: controller);
    }

    private static void RegisterEvents(ModOptionController controller)
    {
        controller.OnBuildUI += (OptionUIBuilder builder) =>
        {
            BindConfigToggle(
                toggle: GetRequiredPreBuild<OptToggle>(builder: builder, id: "enablePlaceTentToggle"),
                configEntry: NoTentRestrictionsConfig.EnablePlaceTent);
            BindConfigToggle(
                toggle: GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableWeightlessTentsToggle"),
                configEntry: NoTentRestrictionsConfig.EnableWeightlessTents);
            BindConfigToggle(
                toggle: GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableStorageChestToggle"),
                configEntry: NoTentRestrictionsConfig.EnableStorageChest);
            BindConfigToggle(
                toggle: GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableDeliveryBoxToggle"),
                configEntry: NoTentRestrictionsConfig.EnableDeliveryBox);
            BindConfigToggle(
                toggle: GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableShippingChestToggle"),
                configEntry: NoTentRestrictionsConfig.EnableShippingChest);
            BindConfigToggle(
                toggle: GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableMaxElectricityToggle"),
                configEntry: NoTentRestrictionsConfig.EnableMaxElectricity);
            BindConfigToggle(
                toggle: GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableTeleporterToggle"),
                configEntry: NoTentRestrictionsConfig.EnableTeleporter);
            BindFertilityModeToggleGroup(
                toggleGroup: GetRequiredPreBuild<OptToggleGroup>(builder: builder, id: "fertilityModeToggleGroup"));
            BindConfigToggle(
                toggle: GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableDiningSpotSignToggle"),
                configEntry: NoTentRestrictionsConfig.EnableDiningSpotSign);

            if (TentLandExpansionUI.Build(builder: builder) == false)
            {
                return;
            }

            if (TentInvestmentUI.Build(builder: builder) == false)
            {
                return;
            }

            if (TentMerchantRecruitmentUI.Build(builder: builder) == false)
            {
                return;
            }

            if (TentLandFeatsUI.Build(builder: builder) == false)
            {
                return;
            }
        };
    }

    private static void BindConfigToggle(OptToggle? toggle, ConfigEntry<bool> configEntry)
    {
        if (toggle == null)
        {
            return;
        }

        toggle.Checked = configEntry.Value;
        toggle.OnValueChanged += isChecked =>
        {
            configEntry.Value = isChecked;
        };
    }

    private static void BindFertilityModeToggleGroup(OptToggleGroup? toggleGroup)
    {
        if (toggleGroup?.Base == null)
        {
            return;
        }

        IList? buttons = GetToggleGroupButtons(toggleGroup: toggleGroup);
        if (buttons == null || buttons.Count < 3)
        {
            NoTentRestrictions.LogError(message: "Fertility mode toggle group is missing expected options.");
            return;
        }

        bool isUpdating = false;

        SetFertilityModeToggleGroupSelection(
            toggleGroup: toggleGroup,
            mode: GetFertilityModeIndex(),
            buttons: buttons,
            isUpdating: ref isUpdating);

        for (int i = 0; i < 3; i++)
        {
            int selectedMode = i;
            if (buttons[index: i] is not UIButton button)
            {
                continue;
            }

            button.onClick.AddListener(call: () =>
            {
                if (isUpdating == true)
                {
                    return;
                }

                SetFertilityModeToggleGroupSelection(
                    toggleGroup: toggleGroup,
                    mode: selectedMode,
                    buttons: buttons,
                    isUpdating: ref isUpdating);
            });
        }
    }

    private static int GetFertilityModeIndex()
    {
        if (NoTentRestrictionsConfig.EnableMaxFertility.Value == true)
        {
            return FertilityModeMax;
        }

        if (NoTentRestrictionsConfig.EnableNoSoilUpgradeLimit.Value == true)
        {
            return FertilityModeWrench;
        }

        return FertilityModeVanilla;
    }

    private static void ApplyFertilityMode(int mode)
    {
        if (mode == FertilityModeMax)
        {
            NoTentRestrictionsConfig.EnableMaxFertility.Value = true;
            NoTentRestrictionsConfig.EnableNoSoilUpgradeLimit.Value = false;
            return;
        }

        if (mode == FertilityModeWrench)
        {
            NoTentRestrictionsConfig.EnableMaxFertility.Value = false;
            NoTentRestrictionsConfig.EnableNoSoilUpgradeLimit.Value = true;
            return;
        }

        NoTentRestrictionsConfig.EnableMaxFertility.Value = false;
        NoTentRestrictionsConfig.EnableNoSoilUpgradeLimit.Value = false;
    }

    private static void SetFertilityModeToggleGroupSelection(
        OptToggleGroup toggleGroup,
        int mode,
        IList buttons,
        ref bool isUpdating)
    {
        isUpdating = true;

        int selectedMode = ClampFertilityMode(mode: mode);
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[index: i] is UIButton button)
            {
                button.SetCheck(i == selectedMode);
            }
        }

        toggleGroup.Base.CurrentSelected = 1;

        isUpdating = false;
        ApplyFertilityMode(mode: selectedMode);
    }

    private static int ClampFertilityMode(int mode)
    {
        if (mode == FertilityModeMax)
        {
            return FertilityModeMax;
        }

        if (mode == FertilityModeWrench)
        {
            return FertilityModeWrench;
        }

        return FertilityModeVanilla;
    }

    private static IList? GetToggleGroupButtons(OptToggleGroup toggleGroup)
    {
        PropertyInfo? toggles = typeof(UIToggleGroup).GetProperty(
            name: "Toggles",
            bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

        return toggles?.GetValue(obj: toggleGroup.Base) as IList;
    }

    internal static T? GetRequiredPreBuild<T>(OptionUIBuilder builder, string id) where T : OptUIElement
    {
        T? element = builder.GetPreBuild<T>(id: id);
        if (element == null)
        {
            NoTentRestrictions.LogError(message: $"Missing Mod Options prebuilt element: {id}");
        }

        return element;
    }
}
