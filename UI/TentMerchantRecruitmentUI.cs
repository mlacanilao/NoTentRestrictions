using System;
using System.Collections.Generic;
using System.Linq;
using EvilMask.Elin.ModOptions.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NoTentRestrictions;

internal static class TentMerchantRecruitmentUI
{
    private const string MerchantDropdownId = "dropdown01";
    private const string GoldTopicId = "topic03";
    private const string RecruitButtonId = "button03";
    private const string GoldBarCurrencyId = "money2";
    private const string NotEnoughGoldTextId = "notEnoughMoney2";
    private const string HireTextId = "hire";
    private const string CostTextId = "cost";
    private const string GoldBarCostTextId = "u2_money2";
    private const string MerchantIdPrefix = "merchant";

    private static readonly Dictionary<string, string> MerchantDisplayToIdMap = new Dictionary<string, string>();
    private static readonly HashSet<string> MerchantBlocklist =
        new HashSet<string>(comparer: StringComparer.OrdinalIgnoreCase) { "merchant_plat" };

    internal static bool Build(OptionUIBuilder builder)
    {
        var merchantDropdown = PopulateDropdown(builder: builder, dropdownId: MerchantDropdownId);
        var goldTopic = UIController.GetRequiredPreBuild<OptTopic>(builder: builder, id: GoldTopicId);
        var recruitButton = UIController.GetRequiredPreBuild<OptButton>(builder: builder, id: RecruitButtonId);
        if (goldTopic == null || recruitButton == null)
        {
            return false;
        }

        goldTopic.Base.SetActive(enable: false);
        recruitButton.Enabled = false;

        if (EClass.core?.IsGameStarted == true &&
            HasDropdownOptions(dropdown: merchantDropdown) == true &&
            EClass._zone is Zone_Tent)
        {
            goldTopic.Base.SetActive(enable: true);

            string goldBarTitle = GoldBarCurrencyId.lang().ToTitleCase(wholeText: true);
            string goldBarPlural = GoldBarCurrencyId.langPlural(i: 2);

            RefreshGoldText(
                goldTopic: goldTopic,
                goldBarTitle: goldBarTitle,
                goldBarPlural: goldBarPlural);

            recruitButton.Enabled = true;
            recruitButton.Text = "daAccept".lang();

            recruitButton.OnClicked += () =>
            {
                Zone_Tent? zone = EClass._zone as Zone_Tent;
                if (zone == null)
                {
                    return;
                }

                int goldBars = EClass.pc.GetCurrency(id: GoldBarCurrencyId);
                if (TryGetSelectedMerchantId(dropdown: merchantDropdown, selectedId: out string selectedId) == false)
                {
                    recruitButton.Enabled = false;
                    return;
                }

                Chara chara = CharaGen.Create(id: selectedId, lv: -1);

                int charaCost = CalcGold.Hire(c: chara);

                if (goldBars < charaCost)
                {
                    Dialog.Ok(langDetail: NotEnoughGoldTextId.langGame());
                    return;
                }

                string goldBarText = $"{goldBarTitle}: {goldBars} {goldBarPlural}";

                Dialog.YesNo(
                    langDetail: $"{chara.Name}\n" +
                                $"{goldBarText}\n" +
                                $"{CostTextId.lang()}: {GoldBarCostTextId.lang(ref1: charaCost.ToString())}",
                    actionYes: delegate
                    {
                        Point point = EClass.pc.pos.GetNearestPoint(allowChara: false) ?? EClass.pc.pos;
                        SE.Pay();
                        EClass.pc.ModCurrency(a: -charaCost, id: GoldBarCurrencyId);
                        chara.SetBool(18, true);
                        chara.SetHomeZone(zone: zone);
                        zone.AddCard(t: chara, point: point);
                        SE.Play(id: "good");
                        RefreshGoldText(
                            goldTopic: goldTopic,
                            goldBarTitle: goldBarTitle,
                            goldBarPlural: goldBarPlural);
                        Dialog.Ok(langDetail: HireTextId.langGame(ref1: chara.Name));
                    },
                    actionNo: delegate { },
                    langYes: "yes",
                    langNo: "no"
                );
            };
        }

        return true;
    }

    private static void RefreshGoldText(
        OptTopic goldTopic,
        string goldBarTitle,
        string goldBarPlural)
    {
        goldTopic.Text = $"{goldBarTitle}: {EClass.pc.GetCurrency(id: GoldBarCurrencyId)} {goldBarPlural}";
    }

    private static bool HasDropdownOptions(OptDropdown? dropdown)
    {
        return dropdown?.Base != null && dropdown.Base.options.Count > 0;
    }

    private static bool TryGetSelectedMerchantId(OptDropdown? dropdown, out string selectedId)
    {
        selectedId = string.Empty;

        Dropdown? baseDropdown = dropdown?.Base;
        if (baseDropdown == null || baseDropdown.options.Count == 0)
        {
            return false;
        }

        int selectedIndex = Mathf.Clamp(value: baseDropdown.value, min: 0, max: baseDropdown.options.Count - 1);
        string selectedDisplay = baseDropdown.options[index: selectedIndex].text;
        return MerchantDisplayToIdMap.TryGetValue(key: selectedDisplay, value: out selectedId);
    }

    private static OptDropdown? PopulateDropdown(OptionUIBuilder builder, string dropdownId)
    {
        var dropdown = UIController.GetRequiredPreBuild<OptDropdown>(builder: builder, id: dropdownId);
        if (dropdown?.Base == null)
        {
            MerchantDisplayToIdMap.Clear();
            return null;
        }

        int previousIndex = Mathf.Clamp(value: dropdown.Base.value, min: 0, max: Mathf.Max(a: 0, b: dropdown.Base.options.Count - 1));

        var rows = EClass.sources?.charas?.rows?
            .Where(predicate: row => row != null
                                     && string.IsNullOrEmpty(value: row.id) == false
                                     && row.id.StartsWith(value: MerchantIdPrefix, comparisonType: StringComparison.OrdinalIgnoreCase)
                                     && MerchantBlocklist.Contains(item: row.id) == false)
            .ToList();

        if (rows == null || rows.Count == 0)
        {
            MerchantDisplayToIdMap.Clear();
            dropdown.Base.options.Clear();
            dropdown.Base.RefreshShownValue();
            return dropdown;
        }

        var list = new List<(string Text, string Id)>(capacity: rows.Count);
        foreach (var row in rows)
        {
            string text = GetMerchantDisplayText(row: row);
            if (string.IsNullOrWhiteSpace(value: text) == false)
            {
                list.Add(item: (Text: text.Trim(), Id: row.id));
            }
        }

        if (list.Count == 0)
        {
            MerchantDisplayToIdMap.Clear();
            dropdown.Base.options.Clear();
            dropdown.Base.RefreshShownValue();
            return dropdown;
        }

        list.Sort(comparison: (a, b) => string.Compare(strA: a.Text, strB: b.Text, comparisonType: StringComparison.CurrentCulture));

        var seen = new HashSet<string>(comparer: StringComparer.CurrentCulture);
        for (int i = 0; i < list.Count; i++)
        {
            var text = list[index: i].Text;
            if (seen.Add(item: text) == false)
            {
                list[index: i] = (Text: $"{text} - {list[index: i].Id}", Id: list[index: i].Id);
            }
        }

        MerchantDisplayToIdMap.Clear();
        dropdown.Base.options.Clear();
        foreach (var item in list)
        {
            MerchantDisplayToIdMap[key: item.Text] = item.Id;
            dropdown.Base.options.Add(item: new Dropdown.OptionData(text: item.Text));
        }

        dropdown.Base.value = Mathf.Clamp(value: previousIndex, min: 0, max: dropdown.Base.options.Count - 1);
        dropdown.Base.RefreshShownValue();
        return dropdown;
    }

    private static string GetMerchantDisplayText(SourceChara.Row row)
    {
        string displayName = row.GetName(c: null, full: true);
        if (string.IsNullOrWhiteSpace(value: displayName))
        {
            displayName = row.id;
        }

        return $"{displayName} ({row.id})";
    }
}
