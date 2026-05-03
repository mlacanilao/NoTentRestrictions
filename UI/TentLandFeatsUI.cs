using System;
using System.Collections.Generic;
using System.Linq;
using EvilMask.Elin.ModOptions.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NoTentRestrictions;

internal static class TentLandFeatsUI
{
    private const string LandFeatTopicId = "topic04";
    private const string OrenTopicId = "topic05";
    private const string InvestmentOrenTopicId = "topic02";
    private const string BaseLandFeatDropdownId = "dropdown02";
    private const string ExtraLandFeatDropdownId = "dropdown03";
    private const string SecondExtraLandFeatDropdownId = "dropdown04";
    private const string ApplyLandFeatButtonId = "button04";
    private const string LandFeatCategory = "landfeat";
    private const string BaseFeatTagPrefix = "bf";
    private const string OrenCurrencyId = "money";
    private const string DeedThingId = "deed";
    private const string CostTextId = "cost";
    private const string NotEnoughMoneyTextId = "notEnoughMoney";
    private const string LandFeatApplyTextId = "dialog_claimLand";
    private const string JapaneseChineseSeparator = "\u3001";

    private sealed class LandFeatOption
    {
        internal LandFeatOption(int id, string name)
        {
            Id = id;
            Name = name;
            Text = $"{name} ({id})";
        }

        internal int Id { get; }
        internal string Name { get; }
        internal string Text { get; }
    }

    internal static bool Build(OptionUIBuilder builder)
    {
        var landFeatTopic = UIController.GetRequiredPreBuild<OptTopic>(builder: builder, id: LandFeatTopicId);
        var orenTopic = UIController.GetRequiredPreBuild<OptTopic>(builder: builder, id: OrenTopicId);

        var baseDropdown = UIController.GetRequiredPreBuild<OptDropdown>(builder: builder, id: BaseLandFeatDropdownId);
        var extraDropdown = UIController.GetRequiredPreBuild<OptDropdown>(builder: builder, id: ExtraLandFeatDropdownId);
        var secondExtraDropdown = UIController.GetRequiredPreBuild<OptDropdown>(builder: builder, id: SecondExtraLandFeatDropdownId);
        var applyButton = UIController.GetRequiredPreBuild<OptButton>(builder: builder, id: ApplyLandFeatButtonId);
        if (landFeatTopic == null ||
            orenTopic == null ||
            baseDropdown == null ||
            extraDropdown == null ||
            secondExtraDropdown == null ||
            applyButton == null)
        {
            return false;
        }

        OptTopic? investmentOrenTopic = builder.GetPreBuild<OptTopic>(id: InvestmentOrenTopicId);

        var availableFeats = EClass.sources?.elements?.rows?
            .Where(predicate: e => e != null && e.category == LandFeatCategory)
            .ToList() ?? new List<SourceElement.Row>();

        var baseFeats = availableFeats.Where(predicate: e => e.chance == 0).ToList();
        var extraFeats = availableFeats.Where(predicate: e => e.chance != 0).ToList();

        var dropdownState = new LandFeatDropdownState(
            baseDropdown: baseDropdown,
            firstExtraDropdown: extraDropdown,
            secondExtraDropdown: secondExtraDropdown,
            extraFeats: extraFeats);

        Zone_Tent? currentTent = EClass._zone as Zone_Tent;
        if (availableFeats.Count > 0)
        {
            var currentLandFeatIds = currentTent?.ListLandFeats()?
                .Select(selector: e => e.id)
                .ToList() ?? new List<int>();

            dropdownState.Initialize(
                baseRows: GetBaseRows(baseFeats: baseFeats, availableFeats: availableFeats),
                selectedIds: currentLandFeatIds);
            baseDropdown.Base.onValueChanged.AddListener(call: _ => dropdownState.RebuildExtras());
            extraDropdown.Base.onValueChanged.AddListener(call: _ => dropdownState.RebuildSecondExtra());
        }

        landFeatTopic.Base.SetActive(enable: false);
        orenTopic.Base.SetActive(enable: false);
        applyButton.Enabled = false;

        if (EClass.core?.IsGameStarted == true &&
            availableFeats.Count > 0 &&
            currentTent != null)
        {
            landFeatTopic.Base.SetActive(enable: true);
            orenTopic.Base.SetActive(enable: true);

            string separator = GetLandFeatSeparator();
            string orenTitle = OrenCurrencyId.lang();

            RefreshLandFeatText(
                landFeatTopic: landFeatTopic,
                zone: currentTent,
                separator: separator);
            RefreshOrenText(topic: orenTopic, orenTitle: orenTitle);

            applyButton.Enabled = true;
            applyButton.Text = LandFeatApplyTextId.lang();

            applyButton.OnClicked += () =>
            {
                if (EClass._zone is Zone_Tent currentZone == false)
                {
                    return;
                }

                int orens = EClass.pc.GetCurrency(id: OrenCurrencyId);

                Thing deed = ThingGen.Create(id: DeedThingId, idMat: -1, lv: -1);
                int deedValue = deed.GetPrice();

                if (orens < deedValue)
                {
                    Dialog.Ok(langDetail: NotEnoughMoneyTextId.langGame());
                    return;
                }

                var chosen = dropdownState.GetSelectedNames();

                Dialog.YesNo(
                    langDetail: $"{LandFeatCategory.lang()}: {string.Join(separator: separator, values: chosen)}\n" +
                                $"{orenTitle}: {FormatOrens()}\n" +
                                $"{CostTextId.lang()}: {Lang._currency(a: deedValue, showUnit: true, unitSize: 14)}",
                    actionYes: delegate
                    {
                        ApplySelectedLandFeats(
                            zone: currentZone,
                            dropdownState: dropdownState,
                            deedValue: deedValue);

                        RefreshLandFeatText(
                            landFeatTopic: landFeatTopic,
                            zone: currentZone,
                            separator: separator);
                        RefreshOrenText(topic: orenTopic, orenTitle: orenTitle);
                        if (investmentOrenTopic != null)
                        {
                            RefreshOrenText(topic: investmentOrenTopic, orenTitle: orenTitle);
                        }

                        Dialog.Ok(langDetail: landFeatTopic.Text);
                    },
                    actionNo: delegate { },
                    langYes: "yes",
                    langNo: "no"
                );
            };
        }

        return true;
    }

    private sealed class LandFeatDropdownState
    {
        private readonly OptDropdown baseDropdown;
        private readonly OptDropdown firstExtraDropdown;
        private readonly OptDropdown secondExtraDropdown;
        private readonly List<SourceElement.Row> extraFeats;
        private readonly List<LandFeatOption> baseOptions = new List<LandFeatOption>();
        private readonly List<LandFeatOption> firstExtraOptions = new List<LandFeatOption>();
        private readonly List<LandFeatOption> secondExtraOptions = new List<LandFeatOption>();

        internal LandFeatDropdownState(
            OptDropdown baseDropdown,
            OptDropdown firstExtraDropdown,
            OptDropdown secondExtraDropdown,
            List<SourceElement.Row> extraFeats)
        {
            this.baseDropdown = baseDropdown;
            this.firstExtraDropdown = firstExtraDropdown;
            this.secondExtraDropdown = secondExtraDropdown;
            this.extraFeats = extraFeats;
        }

        internal void Initialize(IEnumerable<SourceElement.Row> baseRows, IReadOnlyList<int> selectedIds)
        {
            BuildDropdown(
                dropdown: baseDropdown,
                options: baseOptions,
                rows: baseRows,
                preserveId: GetSelectedId(selectedIds: selectedIds, index: 0));
            RebuildExtras(
                preserveFirstExtraId: GetSelectedId(selectedIds: selectedIds, index: 1),
                preserveSecondExtraId: GetSelectedId(selectedIds: selectedIds, index: 2));
        }

        internal void RebuildExtras(int? preserveFirstExtraId = null, int? preserveSecondExtraId = null)
        {
            int baseId = SelectedId(dropdown: baseDropdown, options: baseOptions);
            string baseAlias = GetAliasForElementId(id: baseId) ?? string.Empty;
            var candidates = extraFeats.Where(predicate: e => IsExtraForBase(e: e, baseTag: baseAlias)).ToList();

            int selectedExtraId = preserveFirstExtraId ?? SelectedId(dropdown: firstExtraDropdown, options: firstExtraOptions);
            int selectedSecondExtraId = preserveSecondExtraId ?? SelectedId(dropdown: secondExtraDropdown, options: secondExtraOptions);

            BuildDropdown(
                dropdown: firstExtraDropdown,
                options: firstExtraOptions,
                rows: candidates.Where(predicate: e => e.id != baseId && e.id != selectedSecondExtraId),
                preserveId: selectedExtraId);

            int selectedExtraIdNow = SelectedId(dropdown: firstExtraDropdown, options: firstExtraOptions);

            BuildDropdown(
                dropdown: secondExtraDropdown,
                options: secondExtraOptions,
                rows: candidates.Where(predicate: e => e.id != baseId && e.id != selectedExtraIdNow),
                preserveId: selectedSecondExtraId);
        }

        internal void RebuildSecondExtra()
        {
            int baseId = SelectedId(dropdown: baseDropdown, options: baseOptions);
            string baseAlias = GetAliasForElementId(id: baseId) ?? string.Empty;
            var candidates = extraFeats.Where(predicate: e => IsExtraForBase(e: e, baseTag: baseAlias)).ToList();
            int selectedExtraId = SelectedId(dropdown: firstExtraDropdown, options: firstExtraOptions);

            int selectedSecondExtraId = SelectedId(dropdown: secondExtraDropdown, options: secondExtraOptions);
            BuildDropdown(
                dropdown: secondExtraDropdown,
                options: secondExtraOptions,
                rows: candidates.Where(predicate: e => e.id != baseId && e.id != selectedExtraId),
                preserveId: selectedSecondExtraId);
        }

        internal List<string> GetSelectedNames()
        {
            var names = new List<string>();
            AddSelectedOptionName(
                names: names,
                dropdown: baseDropdown,
                options: baseOptions);
            AddSelectedOptionName(
                names: names,
                dropdown: firstExtraDropdown,
                options: firstExtraOptions);
            AddSelectedOptionName(
                names: names,
                dropdown: secondExtraDropdown,
                options: secondExtraOptions);
            return names;
        }

        internal List<int> GetSelectedIds()
        {
            return new[]
                {
                    SelectedId(dropdown: baseDropdown, options: baseOptions),
                    SelectedId(dropdown: firstExtraDropdown, options: firstExtraOptions),
                    SelectedId(dropdown: secondExtraDropdown, options: secondExtraOptions),
                }
                .Where(predicate: id => id > 0)
                .Distinct()
                .ToList();
        }
    }

    private static IEnumerable<SourceElement.Row> GetBaseRows(
        List<SourceElement.Row> baseFeats,
        List<SourceElement.Row> availableFeats)
    {
        if (baseFeats.Count > 0)
        {
            return baseFeats;
        }

        return availableFeats;
    }

    private static void ApplySelectedLandFeats(
        Zone_Tent zone,
        LandFeatDropdownState dropdownState,
        int deedValue)
    {
        var previousIds = zone.ListLandFeats()?.Select(selector: f => f.id).ToList() ?? new List<int>();

        SE.Play(id: "jingle_embark");
        EClass.pc.PlaySound(id: "build", v: 1f, spatial: true);
        EClass.pc.ModCurrency(a: -deedValue);

        var newIds = dropdownState.GetSelectedIds();

        foreach (int id in previousIds.Except(second: newIds))
        {
            zone.elements.SetBase(id: id, v: 0, potential: 0);
        }

        foreach (int id in newIds)
        {
            zone.elements.SetBase(id: id, v: 1, potential: 0);
        }

        zone.landFeats = newIds;

        zone.branch?.RefreshEfficiency();
        EClass._map?.RefreshAllTiles();
        WidgetMinimap.UpdateMap();
    }

    private static void RefreshLandFeatText(
        OptTopic landFeatTopic,
        Zone_Tent zone,
        string separator)
    {
        var feats = zone.ListLandFeats();
        var featNames = GetLandFeatNames(feats: feats);

        landFeatTopic.Text = $"{LandFeatCategory.lang()}: {string.Join(separator: separator, values: featNames)}";
    }

    private static void RefreshOrenText(OptTopic topic, string orenTitle)
    {
        topic.Text = $"{orenTitle}: {FormatOrens()}";
    }

    private static string FormatOrens()
    {
        return Lang._currency(a: EClass.pc.GetCurrency(id: OrenCurrencyId), showUnit: true, unitSize: 14);
    }

    private static string GetLandFeatSeparator()
    {
        if (Lang.langCode == "JP" || Lang.langCode == "CN")
        {
            return JapaneseChineseSeparator;
        }

        return ", ";
    }

    private static LandFeatOption? GetSelectedOption(OptDropdown dropdown, List<LandFeatOption> options)
    {
        if (dropdown?.Base == null || dropdown.Base.options.Count == 0)
        {
            return null;
        }

        if (options.Count == 0)
        {
            return null;
        }

        int index = Mathf.Clamp(value: dropdown.Base.value, min: 0, max: options.Count - 1);
        return options[index: index];
    }

    private static void AddSelectedOptionName(
        List<string> names,
        OptDropdown dropdown,
        List<LandFeatOption> options)
    {
        LandFeatOption? option = GetSelectedOption(dropdown: dropdown, options: options);
        if (option == null)
        {
            return;
        }

        names.Add(item: option.Name);
    }

    private static bool IsExtraForBase(SourceElement.Row e, string baseTag)
    {
        bool hasBaseRestriction = false;
        foreach (string tag in e.tag ?? Array.Empty<string>())
        {
            if (tag != null && tag.StartsWith(value: BaseFeatTagPrefix, comparisonType: StringComparison.Ordinal))
            {
                if (string.IsNullOrEmpty(value: baseTag) == false && tag == baseTag)
                {
                    return true;
                }

                hasBaseRestriction = true;
            }
        }

        if (hasBaseRestriction == true)
        {
            return false;
        }

        return true;
    }

    private static List<string> GetLandFeatNames(IEnumerable<Element> feats)
    {
        return feats
            .Where(predicate: e => e != null)
            .Select(selector: e =>
            {
                string name = e.Name;
                if (string.IsNullOrWhiteSpace(value: name))
                {
                    return e.id.ToString();
                }

                return name.Trim();
            })
            .ToList();
    }

    private static string? GetAliasForElementId(int id)
    {
        foreach (var item in EClass.sources.elements.alias)
        {
            if (item.Value?.id == id)
            {
                return item.Key;
            }
        }

        return null;
    }

    private static string GetLandFeatName(SourceElement.Row row)
    {
        string name = row.GetName();
        if (string.IsNullOrWhiteSpace(value: name))
        {
            return row.id.ToString();
        }

        return name.Trim();
    }

    private static LandFeatOption CreateLandFeatOption(SourceElement.Row row)
    {
        return new LandFeatOption(id: row.id, name: GetLandFeatName(row: row));
    }

    private static int? GetSelectedId(IReadOnlyList<int> selectedIds, int index)
    {
        if (index >= selectedIds.Count)
        {
            return null;
        }

        return selectedIds[index: index];
    }

    private static int SelectedId(OptDropdown dropdown, List<LandFeatOption> options)
    {
        LandFeatOption? option = GetSelectedOption(dropdown: dropdown, options: options);
        if (option == null)
        {
            return -1;
        }

        return option.Id;
    }

    private static void BuildDropdown(
        OptDropdown dropdown,
        List<LandFeatOption> options,
        IEnumerable<SourceElement.Row> rows,
        int? preserveId)
    {
        var list = rows
            .Where(predicate: r => r != null)
            .Select(selector: CreateLandFeatOption)
            .OrderBy(keySelector: option => option.Text, comparer: StringComparer.CurrentCulture)
            .ToList();

        options.Clear();
        options.AddRange(collection: list);
        dropdown.Base.options.Clear();
        foreach (LandFeatOption option in options)
        {
            dropdown.Base.options.Add(item: new Dropdown.OptionData(text: option.Text));
        }

        int newIndex = GetPreservedIndex(options: options, preserveId: preserveId);
        dropdown.Base.value = Mathf.Clamp(value: newIndex, min: 0, max: Mathf.Max(a: 0, b: dropdown.Base.options.Count - 1));
        dropdown.Base.RefreshShownValue();
    }

    private static int GetPreservedIndex(List<LandFeatOption> options, int? preserveId)
    {
        if (preserveId.HasValue == false)
        {
            return 0;
        }

        int target = preserveId.Value;
        for (int i = 0; i < options.Count; i++)
        {
            if (options[index: i].Id == target)
            {
                return i;
            }
        }

        return 0;
    }
}
