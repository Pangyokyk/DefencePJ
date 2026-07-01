// UnitUpgradeRow.cs
// 업그레이드 패널에서 유닛 한 줄(행)을 담당하는 컴포넌트.
// 이 스크립트를 행 오브젝트에 붙이고 Inspector에서 UI 요소들을 연결한다.
// 폰트·크기·위치는 Unity 에디터에서 직접 조정하면 된다.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitUpgradeRow : MonoBehaviour
{
    [Header("이 행이 나타낼 유닛 데이터")]
    public UnitData unitData;

    [Header("UI 연결")]
    public TMP_Text infoText;    // 유닛 이름 · 레벨 · 스탯 표시
    public TMP_Text costText;    // 업그레이드 버튼 안의 비용 텍스트
    public Button   upgradeBtn;  // 업그레이드 버튼
}
