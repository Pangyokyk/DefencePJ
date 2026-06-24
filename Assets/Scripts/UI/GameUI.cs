// GameUI.cs
// 골드·목숨·웨이브를 화면에 표시하고, 뽑기 버튼 이벤트를 처리하는 UI 매니저.
// Canvas 아래의 UI 오브젝트들을 Inspector에서 연결해 사용한다.
//
// TextMeshPro(TMP)를 사용한다.
// 처음 사용 시 Unity 메뉴 Window → TextMeshPro → Import TMP Essential Resources 를 실행해야 한다.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("상태 텍스트")]
    [SerializeField] private TMP_Text goldText;        // 보유 골드 표시
    [SerializeField] private TMP_Text livesText;       // 남은 목숨 표시
    [SerializeField] private TMP_Text waveText;        // 현재 웨이브 표시
    [SerializeField] private TMP_Text pendingUnitText; // 손패(배치 대기) 유닛 표시

    [Header("뽑기 버튼")]
    [SerializeField] private Button    drawButton;     // 뽑기 버튼 컴포넌트
    [SerializeField] private TMP_Text  drawButtonText; // 버튼 안의 텍스트 ("뽑기 15G")

    private void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        // 매 프레임 각 매니저에서 최신 값을 읽어 텍스트를 갱신한다.
        goldText.text  = $"골드: {GoldManager.Instance.CurrentGold}G";
        livesText.text = $"목숨: {LevelManager.Instance.CurrentLives}";
        waveText.text  = $"웨이브: {WaveManager.Instance.CurrentWave}";

        UnitData pending = GachaSystem.Instance.PendingUnit;

        if (pending != null)
        {
            // 손패에 유닛이 있으면 이름을 표시하고 뽑기 버튼을 비활성화
            pendingUnitText.text        = $"배치 대기: [{pending.unitName}]";
            drawButton.interactable     = false;
        }
        else
        {
            pendingUnitText.text = "배치 대기: 없음  (타일 클릭으로 배치)";

            // 골드가 충분할 때만 뽑기 버튼 활성화
            drawButton.interactable =
                GoldManager.Instance.CurrentGold >= GachaSystem.Instance.GachaCost;
        }

        drawButtonText.text = $"뽑기  ({GachaSystem.Instance.GachaCost}G)";
    }

    // ─────────────────────────────────────────────
    // Button의 OnClick() 이벤트에 연결하는 콜백 메서드.
    // Inspector에서 Button → OnClick → 이 함수를 선택하면 클릭 시 호출된다.
    // ─────────────────────────────────────────────
    public void OnDrawButtonClicked()
    {
        GachaSystem.Instance.TryDraw();
    }
}
