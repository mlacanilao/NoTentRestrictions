using System.Linq;
using EvilMask.Elin.ModOptions.UI;

namespace NoTentRestrictions;

internal static class TentInvestmentUI
{
    private const string DevelopmentTopicId = "topic07";
    private const string OrenTopicId = "topic02";
    private const string InvestButtonId = "button02";
    private const string OrenCurrencyId = "money";
    private const string NotEnoughMoneyTextId = "notEnoughMoney";
    private const string InvestConfirmDialogId = "invest1";
    private const string InvestCompleteDialogId = "invest2";
    private const int NegotiationElementId = 292;

    internal static bool Build(OptionUIBuilder builder)
    {
        var developmentTopic = UIController.GetRequiredPreBuild<OptTopic>(builder: builder, id: DevelopmentTopicId);
        var orenTopic = UIController.GetRequiredPreBuild<OptTopic>(builder: builder, id: OrenTopicId);
        var investButton = UIController.GetRequiredPreBuild<OptButton>(builder: builder, id: InvestButtonId);
        if (developmentTopic == null || orenTopic == null || investButton == null)
        {
            return false;
        }

        developmentTopic.Base.SetActive(enable: false);
        orenTopic.Base.SetActive(enable: false);
        investButton.Enabled = false;

        if (EClass.core?.IsGameStarted == true &&
            EClass._zone is Zone_Tent zone)
        {
            developmentTopic.Base.SetActive(enable: true);
            orenTopic.Base.SetActive(enable: true);

            string developmentTitle = "development".lang();
            string orenTitle = OrenCurrencyId.lang();
            RefreshInvestmentText(
                developmentTopic: developmentTopic,
                orenTopic: orenTopic,
                zone: zone,
                developmentTitle: developmentTitle,
                orenTitle: orenTitle);

            investButton.Enabled = true;
            investButton.Text = "daInvest".lang();

            investButton.OnClicked += () =>
            {
                if (EClass._zone is Zone_Tent currentZone == false)
                {
                    return;
                }

                int investCost = CalcMoney.InvestZone(c: EClass.pc);
                int orens = EClass.pc.GetCurrency(id: OrenCurrencyId);

                if (orens < investCost)
                {
                    Dialog.Ok(langDetail: NotEnoughMoneyTextId.langGame());
                    return;
                }

                GameLang.refDrama1 = investCost.ToString();
                int investmentDisplay = currentZone.investment;
                if (investmentDisplay < 0)
                {
                    investmentDisplay = int.MaxValue;
                }

                GameLang.refDrama2 = investmentDisplay.ToString();
                string dialogBody = GetDialogBody(idTopic: InvestConfirmDialogId);

                Dialog.YesNo(
                    langDetail: $"{developmentTitle}: {currentZone.development}\n" +
                                $"{orenTitle}: {FormatOrens()}\n" +
                                $"{dialogBody}",
                    actionYes: delegate
                    {
                        SE.Pay();
                        EClass.pc.ModCurrency(a: -investCost);
                        currentZone.investment += investCost;
                        currentZone.ModDevelopment(a: 5 + EClass.rnd(a: 5));
                        currentZone.ModInfluence(a: 2);
                        EClass.pc.ModExp(ele: NegotiationElementId, a: 100 + currentZone.development * 2);

                        RefreshInvestmentText(
                            developmentTopic: developmentTopic,
                            orenTopic: orenTopic,
                            zone: currentZone,
                            developmentTitle: developmentTitle,
                            orenTitle: orenTitle);

                        dialogBody = GetDialogBody(idTopic: InvestCompleteDialogId);
                        Dialog.Ok(langDetail: dialogBody);
                    },
                    actionNo: delegate { },
                    langYes: "yes",
                    langNo: "no"
                );
            };
        }

        return true;
    }

    private static void RefreshInvestmentText(
        OptTopic developmentTopic,
        OptTopic orenTopic,
        Zone? zone,
        string developmentTitle,
        string orenTitle)
    {
        developmentTopic.Text = $"{developmentTitle}: {zone?.development}";
        orenTopic.Text = $"{orenTitle}: {FormatOrens()}";
    }

    private static string GetDialogBody(string idTopic)
    {
        string[] dialogLines = Lang.GetDialog(idSheet: "general", idTopic: idTopic);
        string dialogDrama = dialogLines.First();
        return GameLang.ConvertDrama(text: dialogDrama, c: EClass.pc);
    }

    private static string FormatOrens()
    {
        return Lang._currency(a: EClass.pc.GetCurrency(id: OrenCurrencyId), showUnit: true, unitSize: 14);
    }
}
