// UnitUpgradeUI.cs
// StageSelect 씬의 유닛 업그레이드 패널 매니저.
// UI 레이아웃은 Unity 에디터에서 직접 만들고, 이 스크립트는 데이터만 채운다.
// 폰트·크기·위치는 에디터에서 자유롭게 조정할 수 있다.

using TMPro;
using UnityEngine;

public class UnitUpgradeUI : MonoBehaviour
{
    [Header("유닛 행 목록 (UnitUpgradeRow 컴포넌트가 붙은 오브젝트 연결)")]
    [SerializeField] private UnitUpgradeRow[] rows;

    [Header("보유 골드 텍스트 (선택)")]
    [SerializeField] private TMP_Text goldText;

    private void Start()
    {
        foreach (var row in rows)
        {
            // 클로저 캡처 문제를 방지하기 위해 로컬 변수에 복사한다.
            // foreach 루프 변수를 람다에서 직접 쓰면 루프 종료 후의 값이 참조될 수 있다.
            var captured = row;
            row.upgradeBtn.onClick.AddListener(() => OnUpgradeClicked(captured));
            RefreshRow(row);
        }

        RefreshGoldText();
    }

    private void OnUpgradeClicked(UnitUpgradeRow row)
    {
        if (PlayerDataManager.Instance.TryUpgradeUnit(row.unitData))
        {
            RefreshRow(row);
            RefreshGoldText();
        }
    }

    private void RefreshRow(UnitUpgradeRow row)
    {
        int level = PlayerDataManager.Instance.GetUnitLevel(row.unitData.unitName);
        int atk   = row.unitData.attack + row.unitData.attackPerLevel * level;
        int cost  = row.unitData.upgradeBaseCost * (level + 1);

        row.infoText.text = $"[{row.unitData.unitName}]  Lv.{level}\n" +
                            $"공격력 {atk}  사거리 {row.unitData.attackRange}  속도 {row.unitData.attackSpeed}/s";

        row.costText.text        = $"업그레이드\n{cost}G";
        row.upgradeBtn.interactable = PlayerDataManager.Instance.PersistentGold >= cost;
    }

    private void RefreshGoldText()
    {
        if (goldText != null)
            goldText.text = $"보유 골드: {PlayerDataManager.Instance.PersistentGold}G";
    }
}
