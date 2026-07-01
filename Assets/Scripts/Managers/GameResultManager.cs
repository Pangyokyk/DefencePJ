// GameResultManager.cs
// 게임 결과(클리어 / 게임오버) 패널을 보여주고, 스테이지 선택 씬으로 돌아가는 매니저.
//
// [사용 방법]
//   - SampleScene의 Canvas 아래에 GameOverPanel, StageClearPanel을 만든다.
//   - 빈 GameObject에 이 컴포넌트를 붙이고, Inspector에서 두 패널을 연결한다.
//   - 각 패널의 "돌아가기" 버튼 OnClick → 이 컴포넌트의 OnBackToStageSelect()를 연결한다.

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance { get; private set; }

    [Header("결과 패널 (Canvas 자식 오브젝트를 연결)")]
    [SerializeField] private GameObject gameOverPanel;   // 게임 오버 패널
    [SerializeField] private GameObject stageClearPanel; // 스테이지 클리어 패널

    [Header("씬 이름")]
    [SerializeField] private string stageSelectSceneName = "StageSelect";

    [Header("클리어 보상")]
    [SerializeField] private int clearGoldReward = 200; // 스테이지 클리어 시 지급할 지속 골드

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // 시작할 때는 결과 패널을 숨긴다.
        gameOverPanel?.SetActive(false);
        stageClearPanel?.SetActive(false);
    }

    /// <summary>
    /// LevelManager에서 목숨이 0이 됐을 때 호출한다.
    /// </summary>
    public void ShowGameOver()
    {
        // timeScale 을 0으로 하면 Update·FixedUpdate가 정지한다 (코루틴도 멈춤).
        // UI 애니메이션처럼 unscaledTime이 필요한 것은 계속 동작한다.
        Time.timeScale = 0f;
        gameOverPanel?.SetActive(true);
    }

    /// <summary>
    /// WaveManager에서 모든 웨이브를 클리어했을 때 호출한다.
    /// </summary>
    public void ShowStageClear()
    {
        // 클리어 보상 골드를 지속 골드에 추가한다.
        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.AddPersistentGold(clearGoldReward);

        Time.timeScale = 0f;
        stageClearPanel?.SetActive(true);
    }

    // ─────────────────────────────────────────────
    // 패널 안의 "스테이지 선택으로" 버튼 OnClick 에 연결한다.
    // ─────────────────────────────────────────────
    public void OnBackToStageSelect()
    {
        // 씬을 전환하기 전에 timeScale을 반드시 1로 복구한다.
        // 복구하지 않으면 다음 씬도 일시정지 상태로 시작된다.
        Time.timeScale = 1f;
        SceneManager.LoadScene(stageSelectSceneName);
    }
}
