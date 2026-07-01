// GameUI.cs
// 골드·목숨·웨이브를 화면에 표시하고, 뽑기 버튼·웨이브 시작 버튼 이벤트를 처리하는 UI 매니저.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("상태 텍스트")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text pendingUnitText;

    [Header("뽑기 버튼")]
    [SerializeField] private Button   drawButton;
    [SerializeField] private TMP_Text drawButtonText;

    [Header("웨이브 시작 버튼")]
    // Hierarchy에서 Canvas 아래에 만든 웨이브 시작 버튼을 여기에 연결한다.
    [SerializeField] private Button   waveStartButton;

    private void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        goldText.text  = $"골드: {GoldManager.Instance.CurrentGold}G";
        livesText.text = $"목숨: {LevelManager.Instance.CurrentLives}";
        waveText.text  = $"웨이브: {WaveManager.Instance.CurrentWave}";

        UnitData pending = GachaSystem.Instance.PendingUnit;

        if (pending != null)
        {
            pendingUnitText.text    = $"배치 대기: [{pending.unitName}]";
            drawButton.interactable = false;
        }
        else
        {
            pendingUnitText.text    = "배치 대기: 없음  (타일 클릭으로 배치)";
            drawButton.interactable =
                GoldManager.Instance.CurrentGold >= GachaSystem.Instance.GachaCost;
        }

        drawButtonText.text = $"뽑기  ({GachaSystem.Instance.GachaCost}G)";

        // 웨이브가 시작되면 시작 버튼을 숨긴다.
        if (waveStartButton != null)
            waveStartButton.gameObject.SetActive(!WaveManager.Instance.HasStarted);
    }

    // ─────────────────────────────────────────────
    // 뽑기 버튼 OnClick
    // ─────────────────────────────────────────────
    public void OnDrawButtonClicked()
    {
        GachaSystem.Instance.TryDraw();
    }

    // ─────────────────────────────────────────────
    // 웨이브 시작 버튼 OnClick
    // ─────────────────────────────────────────────
    public void OnWaveStartButtonClicked()
    {
        WaveManager.Instance.StartWaves();
    }
}
