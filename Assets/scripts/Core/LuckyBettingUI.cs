using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LuckyBettingUI : MonoBehaviour
{
    [Header("References")]
    public RaceManager raceManager;

    [Header("Optional Styling")]
    [Tooltip("Optional: Assign a pixel-art .ttf/.otf font here for authentic retro text!")]
    public Font customPixelFont;

    // Color Palette matching "Royal Derby" Emerald & Gold aesthetic
    private readonly Color colBgOverlay = new Color(0.02f, 0.09f, 0.06f, 0.94f);
    private readonly Color colPanelDark = new Color(0.04f, 0.16f, 0.11f, 0.98f);
    private readonly Color colCardNormal = new Color(0.06f, 0.22f, 0.15f, 1.00f);
    private readonly Color colCardSelected = new Color(0.14f, 0.36f, 0.22f, 1.00f);
    private readonly Color colGoldBright = new Color(0.96f, 0.78f, 0.26f, 1.00f);
    private readonly Color colGoldDim = new Color(0.65f, 0.50f, 0.18f, 1.00f);
    private readonly Color colCrimson = new Color(0.56f, 0.14f, 0.12f, 1.00f);
    private readonly Color colCreamText = new Color(0.98f, 0.96f, 0.88f, 1.00f);
    private readonly Color colMutedText = new Color(0.72f, 0.82f, 0.75f, 1.00f);
    private readonly Color colTurfBadge = new Color(0.14f, 0.52f, 0.26f, 1.00f);
    private readonly Color colDirtBadge = new Color(0.58f, 0.36f, 0.16f, 1.00f);

    private readonly Color[] laneColors = new Color[]
    {
        new Color(0.90f, 0.25f, 0.25f), // #1 Red
        new Color(0.25f, 0.55f, 0.95f), // #2 Blue
        new Color(0.95f, 0.80f, 0.20f), // #3 Yellow
        new Color(0.25f, 0.80f, 0.40f), // #4 Green
        new Color(0.72f, 0.35f, 0.90f), // #5 Purple
        new Color(0.95f, 0.52f, 0.18f), // #6 Orange
        new Color(0.95f, 0.40f, 0.70f), // #7 Pink
        new Color(0.25f, 0.85f, 0.90f)  // #8 Cyan
    };

    private Sprite roundedPanelSprite;
    private Canvas mainCanvas;
    private GameObject preRacePanel;
    private GameObject howToPlayPanel;
    private GameObject resultsPanel;

    private Text venueHeaderText;
    private Text selectionStatusText;
    private Button startRaceButton;
    private Image startButtonBg;
    private Text startButtonLabel;

    private Text outcomeBannerText;
    private RectTransform standingsContainer;

    private int selectedLane = -1;
    private List<HorseCardUI> horseCards = new List<HorseCardUI>();

    private class HorseCardUI
    {
        public Image outerBorder;
        public Image innerBg;
        public Image portraitImg;
        public Text nameText;
        public Text ultText;
        public Image surfaceBadgeBg;
        public Text surfaceBadgeText;
        public Image spdBar;
        public Image staBar;
        public Image accBar;
        public Image wisBar;
        public Text statsNumText;
    }

    void Awake()
    {
        roundedPanelSprite = CreateProceduralRoundedSprite(32, 32, 6);
        EnsureEventSystem();
        BuildUI();
    }

    private Font GetFont()
    {
        if (customPixelFont != null) return customPixelFont;
        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    private void BuildUI()
    {
        GameObject canvasObj = new GameObject("RoyalDerbyCanvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        BuildPreRaceScreen(canvasObj.transform);
        BuildHowToPlayModal(canvasObj.transform);
        BuildResultsScreen(canvasObj.transform);

        preRacePanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        resultsPanel.SetActive(false);
    }

    private void BuildPreRaceScreen(Transform parent)
    {
        preRacePanel = CreateFullScreenBackdrop(parent, "PreRaceBackdrop", colBgOverlay);

        // Main Royal Gold-Trimmed Container
        RectTransform frame = CreateFramedBox(preRacePanel.transform, "MainFrame", Vector2.zero, new Vector2(1740, 960), colGoldBright, colPanelDark, 5f);

        // Top Title Crest Banner
        RectTransform crest = CreateFramedBox(frame, "TitleCrest", new Vector2(0, 445), new Vector2(820, 82), colGoldBright, colCrimson, 4f);
        CreateStyledText(crest, "MainTitle", "♣ ROYAL DERBY — LUCKY PICK ♣", 34, FontStyle.Bold, Vector2.zero, new Vector2(780, 68), colCreamText, TextAnchor.MiddleCenter, true);

        // Venue & Track Conditions Sub-bar
        venueHeaderText = CreateStyledText(frame, "VenueInfo", "VENUE: TOKYO TURF ARENA  •  SURFACE: TURF  •  WEATHER: SUNNY", 22, FontStyle.Bold, new Vector2(0, 375), new Vector2(1200, 40), colGoldBright, TextAnchor.MiddleCenter, true);

        // How to Play Button (Top Right)
        Button helpBtn = CreateRoyalButton(frame, "HowToPlayBtn", "? LUCK RULES", new Vector2(720, 435), new Vector2(230, 56), colGoldDim, colCardNormal, colCreamText, 20);
        helpBtn.onClick.AddListener(() => howToPlayPanel.SetActive(true));

        // 2x4 Grid of Horse Cards
        horseCards.Clear();
        for (int i = 0; i < 8; i++)
        {
            int laneNum = i + 1;
            int col = i % 2;       // 0 = Left column, 1 = Right column
            int row = i / 2;       // 0..3 top to bottom

            float xPos = (col == 0) ? -415f : 415f;
            float yPos = 245f - (row * 162f);

            HorseCardUI card = CreateHorseCard(frame, laneNum, new Vector2(xPos, yPos));
            horseCards.Add(card);
        }

        // Bottom Selection Footer Bar
        RectTransform footerBar = CreateFramedBox(frame, "FooterBar", new Vector2(0, -405), new Vector2(1640, 96), colGoldDim, new Color(0.03f, 0.12f, 0.08f, 1f), 3f);
        selectionStatusText = CreateStyledText(footerBar, "StatusText", "SELECT A HORSE (#1 TOP TO #8 BOTTOM) TO PREDICT THE HIDDEN LUCKY NUMBER", 23, FontStyle.Bold, new Vector2(-230, 0), new Vector2(1100, 70), colCreamText, TextAnchor.MiddleLeft, true);

        startRaceButton = CreateRoyalButton(footerBar, "StartRaceBtn", "▶  START RACE", new Vector2(610, 0), new Vector2(360, 68), colGoldBright, new Color(0.18f, 0.58f, 0.26f, 1f), colCreamText, 28);
        startButtonBg = startRaceButton.transform.Find("Inner").GetComponent<Image>();
        startButtonLabel = startRaceButton.GetComponentInChildren<Text>();
        startRaceButton.interactable = false;
        startRaceButton.onClick.AddListener(OnConfirmStartRace);
    }

    private HorseCardUI CreateHorseCard(Transform parent, int laneNumber, Vector2 pos)
    {
        HorseCardUI ui = new HorseCardUI();

        GameObject cardObj = new GameObject($"HorseCard_{laneNumber}");
        cardObj.transform.SetParent(parent, false);
        RectTransform rt = cardObj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(800, 146);

        ui.outerBorder = cardObj.AddComponent<Image>();
        ui.outerBorder.sprite = roundedPanelSprite;
        ui.outerBorder.type = Image.Type.Sliced;
        ui.outerBorder.color = colGoldDim;

        Button btn = cardObj.AddComponent<Button>();
        btn.onClick.AddListener(() => OnSelectLane(laneNumber));

        // Inner Card Fill
        GameObject innerObj = new GameObject("InnerFill");
        innerObj.transform.SetParent(cardObj.transform, false);
        RectTransform innerRt = innerObj.AddComponent<RectTransform>();
        innerRt.anchorMin = Vector2.zero;
        innerRt.anchorMax = Vector2.one;
        innerRt.offsetMin = new Vector2(4, 4);
        innerRt.offsetMax = new Vector2(-4, -4);
        ui.innerBg = innerObj.AddComponent<Image>();
        ui.innerBg.sprite = roundedPanelSprite;
        ui.innerBg.type = Image.Type.Sliced;
        ui.innerBg.color = colCardNormal;

        // Lane Number Badge (Left)
        Color laneCol = laneColors[(laneNumber - 1) % laneColors.Length];
        RectTransform badgeRt = CreateFramedBox(innerRt, "LaneBadge", new Vector2(-345, 0), new Vector2(76, 112), colGoldBright, laneCol, 3f);
        CreateStyledText(badgeRt, "LaneNum", $"#{laneNumber}", 30, FontStyle.Bold, Vector2.zero, new Vector2(70, 90), Color.white, TextAnchor.MiddleCenter, true);

        // Portrait Box
        RectTransform portFrame = CreateFramedBox(innerRt, "PortraitFrame", new Vector2(-248, 0), new Vector2(96, 112), colGoldDim, new Color(0.03f, 0.11f, 0.08f, 1f), 2f);
        GameObject portImgObj = new GameObject("HorseSprite");
        portImgObj.transform.SetParent(portFrame, false);
        RectTransform pImgRt = portImgObj.AddComponent<RectTransform>();
        pImgRt.sizeDelta = new Vector2(76, 76);
        ui.portraitImg = portImgObj.AddComponent<Image>();
        ui.portraitImg.preserveAspect = true;
        ui.portraitImg.color = laneCol;

        // Horse Name & Ultimate
        ui.nameText = CreateStyledText(innerRt, "HorseName", "THUNDER BOLT", 25, FontStyle.Bold, new Vector2(-15, 36), new Vector2(340, 36), colCreamText, TextAnchor.MiddleLeft, true);
        ui.ultText = CreateStyledText(innerRt, "UltName", "★ ULT: LIGHTNING FLASH", 17, FontStyle.Italic, new Vector2(-15, 8), new Vector2(340, 26), colGoldBright, TextAnchor.MiddleLeft, false);

        // Surface Pill Badge (Top Right)
        ui.surfaceBadgeBg = CreateFramedBox(innerRt, "SurfaceBadge", new Vector2(315, 36), new Vector2(115, 32), colGoldDim, colTurfBadge, 2f).GetComponent<Image>();
        ui.surfaceBadgeText = CreateStyledText(ui.surfaceBadgeBg.transform, "SurfTxt", "TURF", 16, FontStyle.Bold, Vector2.zero, new Vector2(105, 28), Color.white, TextAnchor.MiddleCenter, true);

        // Numeric Stat Summary
        ui.statsNumText = CreateStyledText(innerRt, "StatsNums", "SPD 90 | STA 85 | ACC 88 | WIS 80", 16, FontStyle.Bold, new Vector2(265, 8), new Vector2(220, 26), colMutedText, TextAnchor.MiddleRight, false);

        // Visual Stat Bars (SPD, STA, ACC, WIS)
        ui.spdBar = CreateMiniStatBar(innerRt, "SPD", new Vector2(-110, -34), new Color(0.95f, 0.35f, 0.30f));
        ui.staBar = CreateMiniStatBar(innerRt, "STA", new Vector2(45, -34), new Color(0.30f, 0.85f, 0.45f));
        ui.accBar = CreateMiniStatBar(innerRt, "ACC", new Vector2(200, -34), new Color(0.35f, 0.70f, 0.98f));
        ui.wisBar = CreateMiniStatBar(innerRt, "WIS", new Vector2(335, -34), new Color(0.95f, 0.78f, 0.25f));

        return ui;
    }

    private Image CreateMiniStatBar(Transform parent, string label, Vector2 pos, Color fillColor)
    {
        GameObject root = new GameObject($"Bar_{label}");
        root.transform.SetParent(parent, false);
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(130, 28);

        CreateStyledText(rt, "Lbl", label, 14, FontStyle.Bold, new Vector2(-44, 0), new Vector2(42, 24), colCreamText, TextAnchor.MiddleLeft, false);

        GameObject bgObj = new GameObject("Track");
        bgObj.transform.SetParent(rt, false);
        RectTransform bgRt = bgObj.AddComponent<RectTransform>();
        bgRt.anchoredPosition = new Vector2(22, 0);
        bgRt.sizeDelta = new Vector2(82, 14);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.02f, 0.08f, 0.05f, 1f);

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(bgRt, false);
        RectTransform fillRt = fillObj.AddComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0, 0);
        fillRt.anchorMax = new Vector2(1, 1);
        fillRt.offsetMin = new Vector2(2, 2);
        fillRt.offsetMax = new Vector2(-2, -2);
        Image fillImg = fillObj.AddComponent<Image>();
        fillImg.color = fillColor;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 0.8f;

        return fillImg;
    }

    private void BuildHowToPlayModal(Transform parent)
    {
        howToPlayPanel = CreateFullScreenBackdrop(parent, "HowToPlayModal", new Color(0f, 0f, 0f, 0.85f));
        RectTransform box = CreateFramedBox(howToPlayPanel.transform, "RulesBox", Vector2.zero, new Vector2(1380, 780), colGoldBright, colPanelDark, 5f);

        RectTransform banner = CreateFramedBox(box, "Banner", new Vector2(0, 355), new Vector2(680, 68), colGoldBright, colCrimson, 3f);
        CreateStyledText(banner, "Title", "♣ LUCK FACTOR — HOW IT WORKS ♣", 30, FontStyle.Bold, Vector2.zero, new Vector2(640, 56), colCreamText, TextAnchor.MiddleCenter, true);

        // 3 Info Cards matching your design diagram
        CreateRuleColumn(box, new Vector2(-430, 30), new Vector2(410, 490), "1. THE HIDDEN NUMBER",
            "Before every race, the track secretly blesses ONE lane (#1 to #8) with the Hidden Lucky Number.\n\n" +
            "• The blessed horse gains a major speed & stamina surge.\n" +
            "• The number stays secret until all 8 horses cross the finish line!");

        CreateRuleColumn(box, new Vector2(0, 30), new Vector2(410, 490), "2. STATS VS. LUCK",
            "Every horse brings unique Speed, Stamina, Acceleration, Wisdom & a 50%-mark Ultimate.\n\n" +
            "• Matching Track Surface (Turf/Dirt) grants +8% speed.\n" +
            "• Muddy weather penalizes low-stamina runners.\n" +
            "• A clutch Ultimate can still spark a dramatic upset!");

        CreateRuleColumn(box, new Vector2(430, 30), new Vector2(410, 490), "3. TWO WAYS TO WIN",
            "Pick your lucky lane (#1 Top to #8 Bottom) before the gates open:\n\n" +
            "• LUCKY GUESS: Your chosen number matches the Hidden Lucky Number.\n" +
            "• UPSET WIN: Your horse outruns the field to take 1st place!");

        Button closeBtn = CreateRoyalButton(box, "CloseRulesBtn", "GOT IT — BACK TO LINEUP", new Vector2(0, -305), new Vector2(440, 68), colGoldBright, new Color(0.18f, 0.58f, 0.26f, 1f), colCreamText, 24);
        closeBtn.onClick.AddListener(() => howToPlayPanel.SetActive(false));
    }

    private void CreateRuleColumn(Transform parent, Vector2 pos, Vector2 size, string heading, string body)
    {
        RectTransform col = CreateFramedBox(parent, heading, pos, size, colGoldDim, colCardNormal, 3f);
        CreateStyledText(col, "Head", heading, 22, FontStyle.Bold, new Vector2(0, 195), new Vector2(370, 45), colGoldBright, TextAnchor.MiddleCenter, true);
        Text bTxt = CreateStyledText(col, "Body", body, 19, FontStyle.Normal, new Vector2(0, -25), new Vector2(360, 380), colCreamText, TextAnchor.UpperLeft, false);
        bTxt.lineSpacing = 1.2f;
    }

    private void BuildResultsScreen(Transform parent)
    {
        resultsPanel = CreateFullScreenBackdrop(parent, "ResultsBackdrop", colBgOverlay);
        RectTransform frame = CreateFramedBox(resultsPanel.transform, "ResultsFrame", Vector2.zero, new Vector2(1460, 920), colGoldBright, colPanelDark, 5f);

        RectTransform crest = CreateFramedBox(frame, "ResultsCrest", new Vector2(0, 425), new Vector2(780, 78), colGoldBright, colCrimson, 4f);
        CreateStyledText(crest, "ResTitle", "🏁 OFFICIAL RACE RESULTS & REVEAL 🏁", 32, FontStyle.Bold, Vector2.zero, new Vector2(740, 64), colCreamText, TextAnchor.MiddleCenter, true);

        // Outcome Banner Box
        RectTransform outcomeBox = CreateFramedBox(frame, "OutcomeBox", new Vector2(0, 315), new Vector2(1340, 92), colGoldBright, new Color(0.08f, 0.28f, 0.18f, 1f), 3f);
        outcomeBannerText = CreateStyledText(outcomeBox, "OutcomeTxt", "", 25, FontStyle.Bold, Vector2.zero, new Vector2(1290, 80), colGoldBright, TextAnchor.MiddleCenter, true);

        // Standings Rows Container
        GameObject listObj = new GameObject("StandingsContainer");
        listObj.transform.SetParent(frame, false);
        standingsContainer = listObj.AddComponent<RectTransform>();
        standingsContainer.anchoredPosition = new Vector2(0, -35);
        standingsContainer.sizeDelta = new Vector2(1340, 560);

        Button nextBtn = CreateRoyalButton(frame, "NextRaceBtn", "▶  NEXT RACE", new Vector2(0, -395), new Vector2(400, 72), colGoldBright, new Color(0.18f, 0.58f, 0.26f, 1f), colCreamText, 28);
        nextBtn.onClick.AddListener(() => {
            if (raceManager != null) raceManager.RestartRace();
        });
    }

    public void ShowPreRacePanel(List<HorseRacer> racers, VenueData venue)
    {
        selectedLane = -1;
        startRaceButton.interactable = false;
        startButtonBg.color = new Color(0.22f, 0.28f, 0.24f, 1f);
        startButtonLabel.color = colMutedText;

        resultsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        preRacePanel.SetActive(true);

        if (venue != null)
        {
            venueHeaderText.text = $"VENUE: {venue.venueName.ToUpper()}   •   SURFACE: {venue.trackSurface.ToUpper()}   •   WEATHER: {venue.weatherCondition.ToString().ToUpper()}";
        }

        for (int i = 0; i < racers.Count && i < horseCards.Count; i++)
        {
            HorseData d = racers[i].data;
            HorseCardUI card = horseCards[i];

            card.outerBorder.color = colGoldDim;
            card.innerBg.color = colCardNormal;
            card.nameText.text = d.horseName.ToUpper();
            card.ultText.text = $"★ ULT: {d.ultimateName.ToUpper()}";

            if (d.horseSprite != null)
            {
                card.portraitImg.sprite = d.horseSprite;
                card.portraitImg.color = Color.white;
            }
            else
            {
                card.portraitImg.sprite = roundedPanelSprite;
                card.portraitImg.color = laneColors[i % laneColors.Length];
            }

            bool isTurf = d.preferredSurface.Equals("Turf", System.StringComparison.OrdinalIgnoreCase);
            card.surfaceBadgeBg.color = isTurf ? colTurfBadge : colDirtBadge;
            card.surfaceBadgeText.text = d.preferredSurface.ToUpper();

            card.statsNumText.text = $"SPD {d.speed:F0} | STA {d.stamina:F0} | ACC {d.acceleration:F0} | WIS {d.wisdom:F0}";
            card.spdBar.fillAmount = Mathf.Clamp01(d.speed / 100f);
            card.staBar.fillAmount = Mathf.Clamp01(d.stamina / 100f);
            card.accBar.fillAmount = Mathf.Clamp01(d.acceleration / 100f);
            card.wisBar.fillAmount = Mathf.Clamp01(d.wisdom / 100f);
        }
    }

    private void OnSelectLane(int laneNumber)
    {
        selectedLane = laneNumber;
        startRaceButton.interactable = true;
        startButtonBg.color = new Color(0.18f, 0.58f, 0.26f, 1f);
        startButtonLabel.color = colCreamText;

        selectionStatusText.text = $"★ LOCKED ON LANE #{selectedLane} — CLICK START RACE TO OPEN THE GATES!";

        for (int i = 0; i < horseCards.Count; i++)
        {
            bool isSelected = (i + 1 == selectedLane);
            horseCards[i].outerBorder.color = isSelected ? colGoldBright : colGoldDim;
            horseCards[i].innerBg.color = isSelected ? colCardSelected : colCardNormal;
        }
    }

    private void OnConfirmStartRace()
    {
        if (selectedLane < 1 || selectedLane > 8) return;
        preRacePanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        if (raceManager != null)
        {
            raceManager.LockInPickAndStartRace(selectedLane);
        }
    }

    public void ShowResultsPanel(List<HorseRacer> finishOrder, int hiddenLuckyNum, int playerPick, bool guessedLucky, bool pickedWinner)
    {
        resultsPanel.SetActive(true);

        if (guessedLucky && pickedWinner)
            outcomeBannerText.text = $"🎉 ROYAL JACKPOT! YOU GUESSED THE HIDDEN LUCKY NUMBER (#{hiddenLuckyNum}) AND WON THE RACE!";
        else if (guessedLucky)
            outcomeBannerText.text = $"♣ LUCKY PREDICTION! YOU GUESSED THE HIDDEN LUCKY NUMBER (#{hiddenLuckyNum})!";
        else if (pickedWinner)
            outcomeBannerText.text = $"🔥 UPSET VICTORY! HIDDEN LUCKY NUMBER WAS #{hiddenLuckyNum}, BUT YOUR PICK #{playerPick} WON!";
        else
            outcomeBannerText.text = $"HIDDEN LUCKY NUMBER WAS #{hiddenLuckyNum}  •  YOUR PICK WAS #{playerPick}";

        // Clear old rows
        foreach (Transform child in standingsContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < finishOrder.Count; i++)
        {
            HorseRacer r = finishOrder[i];
            float yPos = 240f - (i * 68f);

            bool isLucky = (r.horseNumber == hiddenLuckyNum);
            bool isPick = (r.horseNumber == playerPick);

            Color borderCol = (i == 0 || isLucky) ? colGoldBright : colGoldDim;
            Color rowFill = (i == 0) ? new Color(0.16f, 0.36f, 0.20f, 1f) : colCardNormal;

            RectTransform row = CreateFramedBox(standingsContainer, $"RankRow_{i + 1}", new Vector2(0, yPos), new Vector2(1300, 58), borderCol, rowFill, 2f);

            // Rank Medal
            string rankStr = i == 0 ? "1ST" : i == 1 ? "2ND" : i == 2 ? "3RD" : $"{i + 1}TH";
            Color rankCol = i == 0 ? colGoldBright : (i == 1 ? new Color(0.82f, 0.86f, 0.90f) : (i == 2 ? new Color(0.85f, 0.55f, 0.30f) : colMutedText));
            CreateStyledText(row, "Rank", rankStr, 22, FontStyle.Bold, new Vector2(-590, 0), new Vector2(80, 44), rankCol, TextAnchor.MiddleCenter, true);

            // Lane Badge
            Color laneCol = laneColors[(r.horseNumber - 1) % laneColors.Length];
            RectTransform lBadge = CreateFramedBox(row, "LBadge", new Vector2(-505, 0), new Vector2(64, 42), colGoldDim, laneCol, 2f);
            CreateStyledText(lBadge, "LNum", $"#{r.horseNumber}", 20, FontStyle.Bold, Vector2.zero, new Vector2(58, 36), Color.white, TextAnchor.MiddleCenter, true);

            // Horse Name
            CreateStyledText(row, "HName", r.data.horseName.ToUpper(), 22, FontStyle.Bold, new Vector2(-230, 0), new Vector2(440, 44), colCreamText, TextAnchor.MiddleLeft, true);

            // Tags (Hidden Lucky / Your Pick)
            if (isLucky)
            {
                RectTransform luckyTag = CreateFramedBox(row, "LuckyTag", new Vector2(220, 0), new Vector2(250, 38), colGoldBright, colTurfBadge, 2f);
                CreateStyledText(luckyTag, "LTxt", "♣ HIDDEN LUCKY", 17, FontStyle.Bold, Vector2.zero, new Vector2(240, 34), Color.white, TextAnchor.MiddleCenter, true);
            }

            if (isPick)
            {
                RectTransform pickTag = CreateFramedBox(row, "PickTag", new Vector2(495, 0), new Vector2(230, 38), colGoldBright, colCrimson, 2f);
                CreateStyledText(pickTag, "PTxt", "★ YOUR PICK", 17, FontStyle.Bold, Vector2.zero, new Vector2(220, 34), colCreamText, TextAnchor.MiddleCenter, true);
            }
        }
    }

    // --- UI Helper Builders ---

    private GameObject CreateFullScreenBackdrop(Transform parent, string name, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        Image img = obj.AddComponent<Image>();
        img.color = color;
        return obj;
    }

    private RectTransform CreateFramedBox(Transform parent, string name, Vector2 pos, Vector2 size, Color borderColor, Color fillColor, float borderThickness)
    {
        GameObject outer = new GameObject(name);
        outer.transform.SetParent(parent, false);
        RectTransform outerRt = outer.AddComponent<RectTransform>();
        outerRt.anchoredPosition = pos;
        outerRt.sizeDelta = size;
        Image outerImg = outer.AddComponent<Image>();
        outerImg.sprite = roundedPanelSprite;
        outerImg.type = Image.Type.Sliced;
        outerImg.color = borderColor;

        GameObject inner = new GameObject("Inner");
        inner.transform.SetParent(outer.transform, false);
        RectTransform innerRt = inner.AddComponent<RectTransform>();
        innerRt.anchorMin = Vector2.zero;
        innerRt.anchorMax = Vector2.one;
        innerRt.offsetMin = new Vector2(borderThickness, borderThickness);
        innerRt.offsetMax = new Vector2(-borderThickness, -borderThickness);
        Image innerImg = inner.AddComponent<Image>();
        innerImg.sprite = roundedPanelSprite;
        innerImg.type = Image.Type.Sliced;
        innerImg.color = fillColor;

        return innerRt;
    }

    private Button CreateRoyalButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color borderCol, Color fillCol, Color textCol, int fontSize)
    {
        RectTransform innerRt = CreateFramedBox(parent, name, pos, size, borderCol, fillCol, 3f);
        GameObject rootObj = innerRt.parent.gameObject;
        Button btn = rootObj.AddComponent<Button>();
        CreateStyledText(innerRt, "Label", label, fontSize, FontStyle.Bold, Vector2.zero, size - new Vector2(16, 10), textCol, TextAnchor.MiddleCenter, true);
        return btn;
    }

    private Text CreateStyledText(Transform parent, string name, string content, int fontSize, FontStyle style, Vector2 pos, Vector2 size, Color color, TextAnchor align, bool addShadow)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Text txt = obj.AddComponent<Text>();
        txt.font = GetFont();
        txt.text = content;
        txt.fontSize = fontSize;
        txt.fontStyle = style;
        txt.color = color;
        txt.alignment = align;

        if (addShadow)
        {
            Shadow shadow = obj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.85f);
            shadow.effectDistance = new Vector2(2f, -2f);
        }

        return txt;
    }

    // Generates a crisp 9-sliced rounded pixel-bevel sprite in memory
    private Sprite CreateProceduralRoundedSprite(int width, int height, int radius)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int dx = Mathf.Max(0, Mathf.Max(radius - x, x - (width - 1 - radius)));
                int dy = Mathf.Max(0, Mathf.Max(radius - y, y - (height - 1 - radius)));
                bool inside = (dx + dy) <= radius + 1; // Crisp pixel chamfered corner
                tex.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
        }
        tex.Apply();

        Vector4 border = new Vector4(radius + 2, radius + 2, radius + 2, radius + 2);
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
    }
}