using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text newBestText;   // "NEW BEST!" 텍스트 (신기록 아니면 숨김)
    public Button retryButton;
    public Button mainMenuButton;

    // 런타임에 생성되는 텍스트 참조
    private TMP_Text _scoreValueText;
    private TMP_Text _bestValueText;

    private void Start()
    {
        retryButton.onClick.AddListener(OnRetry);
        mainMenuButton.onClick.AddListener(OnMainMenu);

        BuildScoreBestRow();

        gameOverPanel.SetActive(false);
        GameManager.Instance.OnGameOver += ShowGameOver;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= ShowGameOver;
    }

    /// <summary>
    /// GameOverPanel 안에 SCORE / BEST 나란히 표시할 행을 런타임에 생성.
    /// VerticalLayoutGroup의 sibling 순서로 배치됨:
    ///   0: GameOverText → 1: ScoreBestRow → 2: NewBestText → 3: Retry → 4: MainMenu
    /// </summary>
    private void BuildScoreBestRow()
    {
        var panelRT = gameOverPanel.GetComponent<RectTransform>();

        // --- ScoreBestRow (가로 배치 컨테이너) ---
        var row = CreateUIObject("ScoreBestRow", panelRT);
        var rowRT = row.GetComponent<RectTransform>();
        rowRT.sizeDelta = new Vector2(500, 120);

        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 20;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        // --- SCORE 박스 ---
        _scoreValueText = CreateStatBox(row.transform, "GOScoreBox", "SCORE\n0",
            new Color(0.05f, 0.05f, 0.1f, 0.85f));

        // --- BEST 박스 ---
        _bestValueText = CreateStatBox(row.transform, "GOBestBox", "BEST\n0",
            new Color(0.05f, 0.05f, 0.1f, 0.85f));

        // sibling 순서 조정: GameOverText(0) 다음인 1번에 배치
        row.transform.SetSiblingIndex(1);

        // NewBestText를 2번으로 이동 (ScoreBestRow 바로 아래)
        newBestText.transform.SetSiblingIndex(2);
    }

    private TMP_Text CreateStatBox(Transform parent, string name, string defaultText, Color bgColor)
    {
        var box = CreateUIObject(name, parent);

        // 배경 이미지
        var img = box.AddComponent<Image>();
        img.color = bgColor;

        // Outline (HUD 박스와 동일한 스타일)
        var outline = box.AddComponent<Outline>();
        outline.effectColor = new Color(0.6f, 0.6f, 0.7f, 0.9f);
        outline.effectDistance = new Vector2(2, -2);

        // 텍스트
        var textObj = CreateUIObject(name + "Text", box.transform);
        var textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.offsetMin = new Vector2(8, 8);
        textRT.offsetMax = new Vector2(-8, -8);

        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = defaultText;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 18;
        tmp.fontSizeMax = 48;
        tmp.color = Color.white;

        return tmp;
    }

    private GameObject CreateUIObject(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private void ShowGameOver()
    {
        var sm = ScoreManager.Instance;
        _scoreValueText.text = $"SCORE\n{sm.Score:N0}";
        _bestValueText.text = $"BEST\n{sm.BestScore:N0}";
        newBestText.gameObject.SetActive(sm.IsNewBest);
        gameOverPanel.SetActive(true);

        if (TouchInputHandler.Instance != null)
            TouchInputHandler.Instance.SetInputActive(false);
    }

    private void OnRetry()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
