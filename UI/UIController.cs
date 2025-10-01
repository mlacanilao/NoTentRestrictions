using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using EvilMask.Elin.ModOptions;
using EvilMask.Elin.ModOptions.UI;
using UnityEngine;

namespace NoTentRestrictions
{
    public class UIController
    {
        private static Dictionary<string, string> akaToIdMap = new Dictionary<string, string>();
        private static readonly HashSet<string> MerchantBlocklist =
            new HashSet<string>(comparer: StringComparer.OrdinalIgnoreCase) { "merchant_plat" };
        
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
            controller.OnBuildUI += (OptionUIBuilder builder) =>
            {
                var enablePlaceTentToggle = builder.GetPreBuild<OptToggle>(id: "enablePlaceTentToggle");
                enablePlaceTentToggle.Checked = NoTentRestrictionsConfig.EnablePlaceTent.Value;
                enablePlaceTentToggle.OnValueChanged += (bool isChecked) =>
                {
                    NoTentRestrictionsConfig.EnablePlaceTent.Value = isChecked;
                };
                
                var enableStorageChestToggle = builder.GetPreBuild<OptToggle>(id: "enableStorageChestToggle");
                enableStorageChestToggle.Checked = NoTentRestrictionsConfig.EnableStorageChest.Value;
                enableStorageChestToggle.OnValueChanged += (bool isChecked) =>
                {
                    NoTentRestrictionsConfig.EnableStorageChest.Value = isChecked;
                };
                
                var enableDeliveryBoxToggle = builder.GetPreBuild<OptToggle>(id: "enableDeliveryBoxToggle");
                enableDeliveryBoxToggle.Checked = NoTentRestrictionsConfig.EnableDeliveryBox.Value;
                enableDeliveryBoxToggle.OnValueChanged += (bool isChecked) =>
                {
                    NoTentRestrictionsConfig.EnableDeliveryBox.Value = isChecked;
                };
                
                var enableShippingChestToggle = builder.GetPreBuild<OptToggle>(id: "enableShippingChestToggle");
                enableShippingChestToggle.Checked = NoTentRestrictionsConfig.EnableShippingChest.Value;
                enableShippingChestToggle.OnValueChanged += (bool isChecked) =>
                {
                    NoTentRestrictionsConfig.EnableShippingChest.Value = isChecked;
                };
                
                var enableMaxElectricityToggle = builder.GetPreBuild<OptToggle>(id: "enableMaxElectricityToggle");
                enableMaxElectricityToggle.Checked = NoTentRestrictionsConfig.EnableMaxElectricity.Value;
                enableMaxElectricityToggle.OnValueChanged += (bool isChecked) =>
                {
                    NoTentRestrictionsConfig.EnableMaxElectricity.Value = isChecked;
                };
                
                var enableTeleporterToggle = builder.GetPreBuild<OptToggle>(id: "enableTeleporterToggle");
                enableTeleporterToggle.Checked = NoTentRestrictionsConfig.EnableTeleporter.Value;
                enableTeleporterToggle.OnValueChanged += (bool isChecked) =>
                {
                    NoTentRestrictionsConfig.EnableTeleporter.Value = isChecked;
                };
                
                var enableMaxFertilityToggle = builder.GetPreBuild<OptToggle>(id: "enableMaxFertilityToggle");
                enableMaxFertilityToggle.Checked = NoTentRestrictionsConfig.EnableMaxFertility.Value;
                enableMaxFertilityToggle.OnValueChanged += (bool isChecked) =>
                {
                    NoTentRestrictionsConfig.EnableMaxFertility.Value = isChecked;
                };
                
                var enableNoSoilUpgradeLimitToggle = builder.GetPreBuild<OptToggle>(id: "enableNoSoilUpgradeLimitToggle");
                enableNoSoilUpgradeLimitToggle.Checked = NoTentRestrictionsConfig.EnableNoSoilUpgradeLimit.Value;
                enableNoSoilUpgradeLimitToggle.OnValueChanged += (bool isChecked) =>
                {
                    NoTentRestrictionsConfig.EnableNoSoilUpgradeLimit.Value = isChecked;
                };
                
                var enableDiningSpotSignToggle = builder.GetPreBuild<OptToggle>(id: "enableDiningSpotSignToggle");
                enableDiningSpotSignToggle.Checked = NoTentRestrictionsConfig.EnableDiningSpotSign.Value;
                enableDiningSpotSignToggle.OnValueChanged += (bool isChecked) =>
                {
                    NoTentRestrictionsConfig.EnableDiningSpotSign.Value = isChecked;
                };
                
                var topic06 = builder.GetPreBuild<OptTopic>(id: "topic06");
                var topic01 = builder.GetPreBuild<OptTopic>(id: "topic01");
                var button01 = builder.GetPreBuild<OptButton>(id: "button01");

                topic06.Base.SetActive(enable: false);
                topic01.Base.SetActive(enable: false);
                button01.Enabled = false;
                
                if (EClass.core?.IsGameStarted == true &&
                    EClass._zone is Zone_Tent)
                {
                    string landTitle = "land".lang();
                    
                    topic06.Base.SetActive(enable: true);
                    topic01.Base.SetActive(enable: true);

                    topic06.Text = $"{landTitle}: {EClass._map?.bounds?.Width * EClass._map?.bounds?.Height / 100f} ㎢";
                    
                    string goldBarTitle = "money2".lang().ToTitleCase(wholeText: true);
                    string goldBarPlural = "money2".langPlural(i: 2);
                    topic01.Text = $"{goldBarTitle}: {EClass.pc.GetCurrency(id: "money2")} {goldBarPlural}";
                    
                    button01.Enabled = true;
                    button01.Text = "daBuyLand".lang();
                    
                    button01.OnClicked += () =>
                    {
                        bool canExpand = EClass._map.bounds.CanExpand(a: 1);
                        int costLand = CalcGold.ExpandLand();
                        int goldBars = EClass.pc.GetCurrency(id: "money2");

                        if (goldBars < costLand)
                        {
                            Dialog.Ok(langDetail: "notEnoughMoney2".langGame());
                            return;
                        }

                        if (canExpand == false)
                        {
                            ShowDialog(idTopic: "expand3");
                            return;
                        }
                        
                        string goldBarText = $"{goldBarTitle}: {goldBars} {goldBarPlural}";
                        
                        GameLang.refDrama1 = "";
                        GameLang.refDrama2 = costLand.ToString();
                        string[] expandDialogArray = Lang.GetDialog(idSheet: "general", idTopic: "expand1");
                        string expandDialogDrama = expandDialogArray.First();
                        string expandDialog = GameLang.ConvertDrama(text: expandDialogDrama, c: EClass.pc);
                        
                        Dialog.YesNo(
                            langDetail: $"{landTitle}: {EClass._map?.bounds?.Width * EClass._map?.bounds?.Height / 100f} ㎢\n" +
                                        $"{goldBarText}\n" +
                                        $"{expandDialog}",
                            actionYes: delegate
                            {
                                SE.Pay();
                                EClass.pc.ModCurrency(a: -costLand, id: "money2");
                                EClass._map.bounds.Expand(a: 1);
                                SE.Play(id: "good");
                                EClass._map.RefreshAllTiles();
                                WidgetMinimap.UpdateMap();
                                ScreenEffect.Play(id: "Firework");
                                
                                topic06.Text = $"{landTitle}: {EClass._map?.bounds?.Width * EClass._map?.bounds?.Height / 100f} ㎢";
                                topic01.Text = $"{goldBarTitle}: {EClass.pc.GetCurrency(id: "money2")} {goldBarPlural}";
                                
                                expandDialogArray = Lang.GetDialog(idSheet: "general", idTopic: "expand2");
                                expandDialogDrama = expandDialogArray.First();
                                expandDialog = GameLang.ConvertDrama(text: expandDialogDrama, c: EClass.pc);
                                Dialog.Ok(langDetail: expandDialog);
                            },
                            actionNo: delegate { },
                            langYes: "yes",
                            langNo: "no"
                        );
                    };
                    
                    void ShowDialog(string idTopic)
                    {
                        var arr  = Lang.GetDialog(idSheet: "general", idTopic: idTopic);
                        var body = GameLang.ConvertDrama(text: arr.First(), c: EClass.pc);
                        Dialog.Ok(langDetail: body);
                    }
                }
                
                var topic07 = builder.GetPreBuild<OptTopic>(id: "topic07");
                var topic02 = builder.GetPreBuild<OptTopic>(id: "topic02");
                var button02 = builder.GetPreBuild<OptButton>(id: "button02");
                
                topic07.Base.SetActive(enable: false);
                topic02.Base.SetActive(enable: false);
                button02.Enabled = false;
                
                if (EClass.core?.IsGameStarted == true &&
                    EClass._zone is Zone_Tent)
                {
                    topic07.Base.SetActive(enable: true);
                    topic02.Base.SetActive(enable: true);

                    string developmentTitle = "development".lang();
                    topic07.Text = $"{developmentTitle}: {EClass._zone?.development}";
                
                    string orenTitle = "money".lang();
                    topic02.Text = $"{orenTitle}: {Lang._currency(a: EClass.pc.GetCurrency(id: "money"), showUnit: true, unitSize: 14)}";
                    
                    button02.Enabled = true;
                    button02.Text = "daInvest".lang();
                    
                    button02.OnClicked += () =>
                    {
                        int investCost = CalcMoney.InvestZone(c: EClass.pc);
                        int orens = EClass.pc.GetCurrency(id: "money");
                        
                        if (orens < investCost)
                        {
                            Dialog.Ok(langDetail: "notEnoughMoney".langGame());
                            return;
                        }
                        
                        GameLang.refDrama1 = investCost.ToString();
                        GameLang.refDrama2 = (EClass._zone.investment < 0 ? int.MaxValue : EClass._zone.investment).ToString();
                        string[] expandDialogArray = Lang.GetDialog(idSheet: "general", idTopic: "invest1");
                        string expandDialogDrama = expandDialogArray.First();
                        string expandDialog = GameLang.ConvertDrama(text: expandDialogDrama, c: EClass.pc);
                        
                        Dialog.YesNo(
                            langDetail: $"{developmentTitle}: {EClass._zone?.development}\n" +
                                        $"{"money".lang()}: {Lang._currency(a: EClass.pc.GetCurrency(id: "money"), showUnit: true, unitSize: 14)}\n" +
                                        $"{expandDialog}",
                            actionYes: delegate
                            {
                                SE.Pay();
                                EClass.pc.ModCurrency(a: -investCost);
                                EClass._zone.investment += investCost;
                                EClass._zone.ModDevelopment(a: 5 + EClass.rnd(a: 5));
                                EClass._zone.ModInfluence(a: 2);
                                EClass.pc.ModExp(ele: 292, a: 100 + EClass._zone.development * 2);
                                
                                topic07.Text = $"{developmentTitle}: {EClass._zone?.development}";
                                topic02.Text = $"{orenTitle}: {Lang._currency(a: EClass.pc.GetCurrency(id: "money"), showUnit: true, unitSize: 14)}";
                                
                                expandDialogArray = Lang.GetDialog(idSheet: "general", idTopic: "invest2");
                                expandDialogDrama = expandDialogArray.First();
                                expandDialog = GameLang.ConvertDrama(text: expandDialogDrama, c: EClass.pc);
                                Dialog.Ok(langDetail: expandDialog);
                            },
                            actionNo: delegate { },
                            langYes: "yes",
                            langNo: "no"
                        );
                    };
                }
                
                var dropdown01 = PopulateDropdown(builder: builder, dropdownId: "dropdown01", targetMap: akaToIdMap);
                var topic03 = builder.GetPreBuild<OptTopic>(id: "topic03");
                var button03 = builder.GetPreBuild<OptButton>(id: "button03");
                
                topic03.Base.SetActive(enable: false);
                button03.Enabled = false;
                
                if (EClass.core?.IsGameStarted == true &&
                    EClass._zone is Zone_Tent)
                {
                    topic03.Base.SetActive(enable: true);
                
                    string goldBarTitle = "money2".lang().ToTitleCase(wholeText: true);
                    string goldBarPlural = "money2".langPlural(i: 2);
                
                    topic03.Text = $"{goldBarTitle}: {EClass.pc.GetCurrency(id: "money2")} {goldBarPlural}";
                    
                    button03.Enabled = true;
                    button03.Text = "daAccept".lang();
                    
                    button03.OnClicked += () =>
                    {
                        int goldBars = EClass.pc.GetCurrency(id: "money2");
                        int selectedIndex = dropdown01.Base.value;
                        string selectedAka = dropdown01.Base.options[index: selectedIndex].text;
                        string selectedId = akaToIdMap[key: selectedAka];
                        Chara chara = CharaGen.Create(id: selectedId, lv: -1);
                        
                        int charaCost = CalcGold.Hire(c: chara);
                        
                        if (goldBars < charaCost)
                        {
                            Dialog.Ok(langDetail: "notEnoughMoney2".langGame());
                            return;
                        }
                        
                        string goldBarText = $"{goldBarTitle}: {goldBars} {goldBarPlural}";
                        
                        Dialog.YesNo(
                            langDetail: $"{chara.Name}\n" +
                                        $"{goldBarText}\n" +
                                        $"{"cost".lang()}: {"u2_money2".lang(ref1: charaCost.ToString())}",
                            actionYes: delegate
                            {
                                SE.Pay();
                                EClass.pc.ModCurrency(a: -charaCost, id: "money2");
                                Point point = EClass.pc.pos.GetNearestPoint(allowChara: false);
                                chara.homeZone = EClass._zone;
                                chara.isImported = true;
                                EClass._zone?.AddCard(t: chara, point: point);
                                SE.Play(id: "good");
                                topic03.Text = $"{goldBarTitle}: {EClass.pc.GetCurrency(id: "money2")} {goldBarPlural}";
                                Dialog.Ok(langDetail: "hire".langGame(ref1: chara.Name));
                            },
                            actionNo: delegate { },
                            langYes: "yes",
                            langNo: "no"
                        );
                    };
                }
                
                var topic04 = builder.GetPreBuild<OptTopic>(id: "topic04");
                var topic05 = builder.GetPreBuild<OptTopic>(id: "topic05");
                
                var dropdown02 = builder.GetPreBuild<OptDropdown>(id: "dropdown02");
                var dropdown03 = builder.GetPreBuild<OptDropdown>(id: "dropdown03");
                var dropdown04 = builder.GetPreBuild<OptDropdown>(id: "dropdown04");
                
                var availableFeats = EClass.sources?.elements?.rows?
                    .Where(predicate: e => e != null && e.category == "landfeat")
                    .ToList();

                var baseFeats  = availableFeats.Where(predicate: e => e.chance == 0).ToList();
                var extraFeats = availableFeats.Where(predicate: e => e.chance != 0).ToList();
                
                var map02 = new Dictionary<string,int>(comparer: StringComparer.CurrentCulture);
                var map03 = new Dictionary<string,int>(comparer: StringComparer.CurrentCulture);
                var map04 = new Dictionary<string,int>(comparer: StringComparer.CurrentCulture);
                
                BuildDropdown(dd: dropdown02, map: map02, rows: baseFeats.Count > 0 ? baseFeats : availableFeats, preserveId: null);
                
                RebuildExtras();
                
                dropdown02.Base.onValueChanged.AddListener(call: _ => RebuildExtras());
                dropdown03.Base.onValueChanged.AddListener(call: _ => RebuildExtra2());
                
                void RebuildExtras()
                {
                    var baseId = SelectedId(dd: dropdown02, map: map02);
                    var baseAlias= GetAliasForElementId(id: baseId) ?? string.Empty;
                    var cand     = extraFeats.Where(predicate: e => IsExtraForBase(e: e, baseTag: baseAlias)).ToList();

                    var sel3Prev = SelectedId(dd: dropdown03,  map: map03);
                    var sel4Prev = SelectedId(dd: dropdown04, map: map04);

                    BuildDropdown(dd: dropdown03,  map: map03, rows: cand.Where(predicate: e => e.id != baseId && e.id != sel4Prev), preserveId: sel3Prev);
                    var sel3Now = SelectedId(dd: dropdown03, map: map03);
                    
                    BuildDropdown(dd: dropdown04, map: map04, rows: cand.Where(predicate: e => e.id != baseId && e.id != sel3Now),  preserveId: sel4Prev);
                }

                void RebuildExtra2()
                {
                    var baseId = SelectedId(dd: dropdown02, map: map02);
                    var baseAlias= GetAliasForElementId(id: baseId) ?? string.Empty;
                    var cand = extraFeats.Where(predicate: e => IsExtraForBase(e: e, baseTag: baseAlias)).ToList();
                    var sel3 = SelectedId(dd: dropdown03, map: map03);

                    var sel4Prev= SelectedId(dd: dropdown04, map: map04);
                    BuildDropdown(dd: dropdown04, map: map04, rows: cand.Where(predicate: e => e.id != baseId && e.id != sel3), preserveId: sel4Prev);
                }
                
                var button04 = builder.GetPreBuild<OptButton>(id: "button04");
                
                topic04.Base.SetActive(enable: false);
                topic05.Base.SetActive(enable: false);
                button04.Enabled = false;
                
                if (EClass.core?.IsGameStarted == true &&
                    EClass._zone is Zone_Tent)
                {
                    topic04.Base.SetActive(enable: true);
                    topic05.Base.SetActive(enable: true);
                    
                    var feats = EClass._zone.ListLandFeats();

                    string sep = (Lang.langCode == "JP" || Lang.langCode == "CN") ? "、" : ", ";
                    var featNames = feats.Select(selector: e =>
                    {
                        var row = EClass.sources?.elements?.rows?.FirstOrDefault(predicate: r => r.id == e.id);
                        var name = row?.GetText(id: "name", returnNull: false) ?? row?.name_JP ?? row?.name;
                        return string.IsNullOrWhiteSpace(value: name) ? e.id.ToString() : name.Trim();
                    }).ToList();

                    topic04.Text = $"{"landfeat".lang()}: {string.Join(separator: sep, values: featNames)}";
                    
                    string orenTitle = "money".lang();
                    string orenPlural = Lang._currency(a: EClass.pc.GetCurrency(id: "money"), showUnit: true, unitSize: 14);
                
                    topic05.Text = $"{orenTitle}: {orenPlural}";
                    
                    button04.Enabled = true;
                    button04.Text = "dialog_claimLand".lang();
                    
                    button04.OnClicked += () =>
                    {
                        int orens = EClass.pc.GetCurrency(id: "money");

                        Thing deed = ThingGen.Create(id: "deed", idMat: -1, lv: -1);
                        int deedValue = deed.GetPrice();
                        
                        if (orens < deedValue)
                        {
                            Dialog.Ok(langDetail: "notEnoughMoney".langGame());
                            return;
                        }
                        
                        string SelectedName(OptDropdown dd)
                        {
                            if (dd?.Base == null || dd.Base.options.Count == 0) return null;
                            var txt = dd.Base.options[index: Mathf.Clamp(value: dd.Base.value, min: 0, max: dd.Base.options.Count - 1)].text;
                            var i = txt.LastIndexOf(value: '(');
                            return (i > 0 ? txt.Substring(startIndex: 0, length: i).TrimEnd() : txt).Trim();
                        }

                        var chosen = new[]
                            {
                                builder.GetPreBuild<OptDropdown>(id: "dropdown02"),
                                builder.GetPreBuild<OptDropdown>(id: "dropdown03"),
                                builder.GetPreBuild<OptDropdown>(id: "dropdown04"),
                            }
                            .Select(selector: SelectedName)
                            .Where(predicate: s => !string.IsNullOrWhiteSpace(value: s))
                            .ToList();
                        
                        Dialog.YesNo(
                            langDetail: $"{"landfeat".lang()}: {string.Join(separator: sep, values: chosen)}\n" +
                                        $"{orenTitle}: {orenPlural}\n" +
                                        $"{"cost".lang()}: {Lang._currency(a: deedValue, showUnit: true, unitSize: 14)}",
                            actionYes: delegate
                            {
                                var zone = EClass._zone;
                                var prevIds = zone.ListLandFeats()?.Select(selector: f => f.id).ToList() ?? new List<int>();
                                
                                SE.Play(id: "jingle_embark");
                                EClass.pc.PlaySound(id: "build", v: 1f, spatial: true);
                                EClass.pc.ModCurrency(a: -deedValue);
                                
                                var baseId = SelectedId(dd: dropdown02, map: map02);
                                var extraId = SelectedId(dd: dropdown03, map: map03);
                                var extraId2 = SelectedId(dd: dropdown04, map: map04);

                                var newIds = new[] { baseId, extraId, extraId2 }
                                    .Where(predicate: id => id > 0)
                                    .Distinct()
                                    .ToList();
                                
                                foreach (var id in prevIds.Except(second: newIds))
                                {
                                    zone.elements.SetBase(id: id, v: 0, potential: 0);
                                }

                                foreach (var id in newIds)
                                {
                                    zone.elements.SetBase(id: id, v: 1, potential: 0);
                                }

                                zone.landFeats = newIds;

                                zone.branch?.RefreshEfficiency();
                                EClass._map?.RefreshAllTiles();
                                WidgetMinimap.UpdateMap();
                                
                                feats = zone.ListLandFeats();

                                featNames = feats.Select(selector: e =>
                                {
                                    var row = EClass.sources?.elements?.rows?.FirstOrDefault(predicate: r => r.id == e.id);
                                    var name = row?.GetText(id: "name", returnNull: false) ?? row?.name_JP ?? row?.name;
                                    return string.IsNullOrWhiteSpace(value: name) ? e.id.ToString() : name.Trim();
                                }).ToList();
                                
                                string landFeatText = $"{"landfeat".lang()}: {string.Join(separator: sep, values: featNames)}";
                                
                                topic04.Text = landFeatText;
                                
                                orenPlural = Lang._currency(a: EClass.pc.GetCurrency(id: "money"), showUnit: true, unitSize: 14);
                                topic02.Text = $"{orenTitle}: {orenPlural}";
                                topic05.Text = $"{orenTitle}: {orenPlural}";
                                
                                Dialog.Ok(langDetail: landFeatText);
                            },
                            actionNo: delegate { },
                            langYes: "yes",
                            langNo: "no"
                        );
                    };
                }
                
            };
        }
        
        private static OptDropdown PopulateDropdown(
            OptionUIBuilder builder,
            string dropdownId,
            IDictionary<string, string> targetMap,
            string spawnListId = null)
        {
            var dropdown = builder.GetPreBuild<OptDropdown>(id: dropdownId);
            if (dropdown == null) return null;

            int prevIndex = Mathf.Clamp(value: dropdown.Base.value, min: 0, max: Mathf.Max(a: 0, b: dropdown.Base.options.Count - 1));

            var rows = (string.IsNullOrEmpty(value: spawnListId)
                    ? EClass.sources?.charas?.rows
                    : SpawnList.Get(id: spawnListId)?.rows?.OfType<SourceChara.Row>())
                ?.Where(predicate: r => r != null
                                        && !string.IsNullOrEmpty(value: r.id)
                                        && r.id.StartsWith(value: "merchant", comparisonType: StringComparison.OrdinalIgnoreCase)
                                        && !MerchantBlocklist.Contains(item: r.id))
                .ToList();

            if (rows == null || rows.Count == 0)
            {
                targetMap?.Clear();
                dropdown.Base.options.Clear();
                dropdown.Base.RefreshShownValue();
                return dropdown;
            }

            var list = new List<(string Text, string Id)>(capacity: rows.Count);
            foreach (var r in rows)
            {
                var text = GetLocalizedText(row: r, includeId: true);
                if (!string.IsNullOrWhiteSpace(value: text))
                    list.Add(item: (text.Trim(), r.id));
            }
            if (list.Count == 0)
            {
                targetMap?.Clear();
                dropdown.Base.options.Clear();
                dropdown.Base.RefreshShownValue();
                return dropdown;
            }

            list.Sort(comparison: (a, b) => string.Compare(strA: a.Text, strB: b.Text, comparisonType: StringComparison.CurrentCulture));

            var seen = new HashSet<string>(comparer: StringComparer.CurrentCulture);
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[index: i].Text;
                if (!seen.Add(item: t))
                    list[index: i] = ($"{t} — {list[index: i].Id}", list[index: i].Id);
            }

            targetMap?.Clear();
            dropdown.Base.options.Clear();
            foreach (var item in list)
            {
                if (targetMap != null) targetMap[key: item.Text] = item.Id;
                dropdown.Base.options.Add(item: new UnityEngine.UI.Dropdown.OptionData(text: item.Text));
            }

            dropdown.Base.value = Mathf.Clamp(value: prevIndex, min: 0, max: dropdown.Base.options.Count - 1);
            dropdown.Base.RefreshShownValue();
            return dropdown;
        }

        private static string GetLocalizedText(SourceChara.Row row, bool includeId = true)
        {
            if (row == null) return null;

            string San(string s) =>
                string.IsNullOrWhiteSpace(value: s) ? null : s.Replace(oldValue: "\r", newValue: "").Replace(oldValue: "\n", newValue: "").Trim();

            string aka  = San(s: row.GetText(id: "aka",  returnNull: true))  ?? San(s: row.aka_JP)  ?? San(s: row.aka);
            string name = San(s: row.GetText(id: "name", returnNull: true))  ?? San(s: row.name_JP) ?? San(s: row.name);

            if (aka  == "*r") aka  = null;
            if (name == "*r") name = null;

            string display = aka ?? name ?? row.id;

            if (!string.IsNullOrEmpty(value: aka) && !string.IsNullOrEmpty(value: name) &&
                !aka.Equals(value: name, comparisonType: StringComparison.CurrentCulture))
            {
                var qL = (Lang.langCode == "JP") ? "「" : "“";
                var qR = (Lang.langCode == "JP") ? "」" : "”";
                display = $"{aka} {qL}{name}{qR}";
            }

            return includeId ? $"{display} ({row.id})" : display;
        }
        
        private static bool IsExtraForBase(SourceElement.Row e, string baseTag)
        {
            bool result = true;
            foreach (var t in e.tag)
            {
                if (t != null && t.StartsWith(value: "bf", comparisonType: StringComparison.Ordinal))
                {
                    result = false;
                    if (!string.IsNullOrEmpty(value: baseTag) && t == baseTag)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private static string GetAliasForElementId(int id)
        {
            foreach (var kv in EClass.sources.elements.alias)
                if (kv.Value?.id == id) return kv.Key;
            return null;
        }

        private static string GetElementDisplay(SourceElement.Row r, bool includeId = true)
        {
            var name = r?.GetText(id: "name", returnNull: false) ?? r?.name_JP ?? r?.name ?? r?.id.ToString();
            var disp = name?.Trim() ?? r.id.ToString();
            return includeId ? $"{disp} ({r.id})" : disp;
        }

        private static int SelectedId(OptDropdown dd, IDictionary<string,int> map)
        {
            if (dd?.Base == null || dd.Base.options.Count == 0) return -1;
            var idx = Mathf.Clamp(value: dd.Base.value, min: 0, max: dd.Base.options.Count - 1);
            var key = dd.Base.options[index: idx].text;
            return (map != null && map.TryGetValue(key: key, value: out var id)) ? id : -1;
        }

        private static void BuildDropdown(OptDropdown dd,
            IDictionary<string,int> map,
            IEnumerable<SourceElement.Row> rows,
            int? preserveId = null)
        {
            var list = rows
                .Where(predicate: r => r != null)
                .Select(selector: r => (Text: GetElementDisplay(r: r, includeId: true), Id: r.id))
                .OrderBy(keySelector: p => p.Text, comparer: StringComparer.CurrentCulture)
                .ToList();

            var seen = new HashSet<string>(comparer: StringComparer.CurrentCulture);
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[index: i].Text;
                if (!seen.Add(item: t)) list[index: i] = ($"{t} — {list[index: i].Id}", list[index: i].Id);
            }

            map?.Clear();
            dd.Base.options.Clear();
            foreach (var (text, id) in list)
            {
                if (map != null) map[key: text] = id;
                dd.Base.options.Add(item: new UnityEngine.UI.Dropdown.OptionData(text: text));
            }

            int newIndex = 0;
            if (preserveId.HasValue)
            {
                var target = preserveId.Value;
                for (int i = 0; i < list.Count; i++)
                    if (list[index: i].Id == target) { newIndex = i; break; }
            }
            dd.Base.value = Mathf.Clamp(value: newIndex, min: 0, max: Mathf.Max(a: 0, b: dd.Base.options.Count - 1));
            dd.Base.RefreshShownValue();
        }
    }
}