using System.Linq;
using EvilMask.Elin.ModOptions.UI;

namespace NoTentRestrictions;

internal static class TentLandExpansionUI
{
    private const string LandTopicId = "topic06";
    private const string GoldTopicId = "topic01";
    private const string BuyLandButtonId = "button01";
    private const string GoldBarCurrencyId = "money2";
    private const string NotEnoughGoldBarsTextId = "notEnoughMoney2";
    private const string ExpandConfirmDialogId = "expand1";
    private const string ExpandCompleteDialogId = "expand2";
    private const string ExpandUnavailableDialogId = "expand3";

    internal static bool Build(OptionUIBuilder builder)
    {
        var landTopic = UIController.GetRequiredPreBuild<OptTopic>(builder: builder, id: LandTopicId);
        var goldTopic = UIController.GetRequiredPreBuild<OptTopic>(builder: builder, id: GoldTopicId);
        var buyLandButton = UIController.GetRequiredPreBuild<OptButton>(builder: builder, id: BuyLandButtonId);
        if (landTopic == null || goldTopic == null || buyLandButton == null)
        {
            return false;
        }

        landTopic.Base.SetActive(enable: false);
        goldTopic.Base.SetActive(enable: false);
        buyLandButton.Enabled = false;

        if (EClass.core?.IsGameStarted == true &&
            EClass._zone is Zone_Tent zone)
        {
            string landTitle = "land".lang();
            Map? map = zone.map;

            landTopic.Base.SetActive(enable: true);
            goldTopic.Base.SetActive(enable: true);

            string goldBarTitle = GoldBarCurrencyId.lang().ToTitleCase(wholeText: true);
            string goldBarPlural = GoldBarCurrencyId.langPlural(i: 2);
            RefreshLandExpansionText(
                landTopic: landTopic,
                goldTopic: goldTopic,
                map: map,
                landTitle: landTitle,
                goldBarTitle: goldBarTitle,
                goldBarPlural: goldBarPlural);

            buyLandButton.Enabled = true;
            buyLandButton.Text = "daBuyLand".lang();

            buyLandButton.OnClicked += () =>
            {
                if (EClass._zone is Zone_Tent currentZone == false)
                {
                    return;
                }

                Map? map = currentZone.map;
                if (map?.bounds == null)
                {
                    return;
                }

                bool canExpand = map.bounds.CanExpand(a: 1);
                int costLand = CalcGold.ExpandLand();
                int goldBars = EClass.pc.GetCurrency(id: GoldBarCurrencyId);

                if (canExpand == false)
                {
                    ShowExpansionDialog(idTopic: ExpandUnavailableDialogId);
                    return;
                }

                if (goldBars < costLand)
                {
                    Dialog.Ok(langDetail: NotEnoughGoldBarsTextId.langGame());
                    return;
                }

                string goldBarText = $"{goldBarTitle}: {goldBars} {goldBarPlural}";

                GameLang.refDrama1 = "";
                GameLang.refDrama2 = costLand.ToString();
                string expandDialog = GetDialogBody(idTopic: ExpandConfirmDialogId);

                Dialog.YesNo(
                    langDetail: $"{landTitle}: {map.bounds.Width * map.bounds.Height / 100f} ㎢\n" +
                                $"{goldBarText}\n" +
                                $"{expandDialog}",
                    actionYes: delegate
                    {
                        SE.Pay();
                        EClass.pc.ModCurrency(a: -costLand, id: GoldBarCurrencyId);
                        map.bounds.Expand(a: 1);
                        SE.Play(id: "good");
                        map.RefreshAllTiles();
                        WidgetMinimap.UpdateMap();
                        ScreenEffect.Play(id: "Firework");

                        RefreshLandExpansionText(
                            landTopic: landTopic,
                            goldTopic: goldTopic,
                            map: map,
                            landTitle: landTitle,
                            goldBarTitle: goldBarTitle,
                            goldBarPlural: goldBarPlural);

                        expandDialog = GetDialogBody(idTopic: ExpandCompleteDialogId);
                        Dialog.Ok(langDetail: expandDialog);
                    },
                    actionNo: delegate { },
                    langYes: "yes",
                    langNo: "no"
                );
            };
        }

        return true;
    }

    private static void RefreshLandExpansionText(
        OptTopic landTopic,
        OptTopic goldTopic,
        Map? map,
        string landTitle,
        string goldBarTitle,
        string goldBarPlural)
    {
        string landArea = string.Empty;
        if (map?.bounds != null)
        {
            landArea = (map.bounds.Width * map.bounds.Height / 100f).ToString();
        }

        landTopic.Text = $"{landTitle}: {landArea} ㎢";
        goldTopic.Text = $"{goldBarTitle}: {EClass.pc.GetCurrency(id: GoldBarCurrencyId)} {goldBarPlural}";
    }

    private static void ShowExpansionDialog(string idTopic)
    {
        Dialog.Ok(langDetail: GetDialogBody(idTopic: idTopic));
    }

    private static string GetDialogBody(string idTopic)
    {
        string[] dialogLines = Lang.GetDialog(idSheet: "general", idTopic: idTopic);
        string dialogDrama = dialogLines.First();
        return GameLang.ConvertDrama(text: dialogDrama, c: EClass.pc);
    }
}
