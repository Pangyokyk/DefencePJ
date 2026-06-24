// GachaSystem.cs
// 전투 중 유닛 뽑기(가챠)를 담당하는 싱글턴.
//
// 동작 흐름:
//   플레이어가 "뽑기" 버튼 클릭
//   → 골드 gachaCost만큼 소비
//   → 가중치 기반 랜덤으로 UnitData 선택 → PendingUnit에 저장
//   → 플레이어가 타일을 클릭하면 PlacementManager가 PendingUnit을 배치
//   → 배치 완료 후 ClearPending() 호출로 손패를 비운다

using UnityEngine;

public class GachaSystem : MonoBehaviour
{
    public static GachaSystem Instance { get; private set; }

    [Header("뽑기 설정")]
    [SerializeField] private UnitData[] unitPool; // 뽑기 풀 — Inspector에서 UnitData 에셋들을 등록
    [SerializeField] private int gachaCost = 15;  // 뽑기 1회 비용 (골드)

    // 뽑아서 배치 대기 중인 유닛 데이터. null이면 손패가 없는 상태.
    public UnitData PendingUnit { get; private set; }

    // 뽑기 비용을 UI에서 읽을 수 있도록 프로퍼티로 노출
    public int GachaCost => gachaCost;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// UI의 뽑기 버튼에서 호출한다.
    /// 골드를 소비하고 랜덤 유닛을 PendingUnit에 저장한다.
    /// 골드 부족 또는 이미 손패가 있으면 false를 반환한다.
    /// </summary>
    public bool TryDraw()
    {
        if (PendingUnit != null)
        {
            Debug.Log("[GachaSystem] 손패에 유닛이 있습니다. 먼저 타일에 배치하세요.");
            return false;
        }

        if (unitPool == null || unitPool.Length == 0)
        {
            Debug.LogError("[GachaSystem] unitPool이 비어 있습니다. Inspector에서 UnitData 에셋을 등록하세요.");
            return false;
        }

        // GoldManager에 골드 소비 요청 — 잔액 부족이면 false 반환
        if (!GoldManager.Instance.SpendGold(gachaCost)) return false;

        PendingUnit = GetWeightedRandom();
        Debug.Log($"[GachaSystem] 뽑기 결과: {PendingUnit.unitName} (공격력:{PendingUnit.attack}, 사거리:{PendingUnit.attackRange})");
        return true;
    }

    /// <summary>
    /// 유닛을 타일에 배치한 뒤 PlacementManager에서 호출해 손패를 비운다.
    /// </summary>
    public void ClearPending()
    {
        PendingUnit = null;
    }

    // ─────────────────────────────────────────────
    // 가중치(weight) 기반 랜덤 선택 알고리즘
    //   총 가중치 합 안에서 난수를 뽑고,
    //   앞에서부터 누적 가중치를 더해 난수가 포함되는 구간의 항목을 선택한다.
    // ─────────────────────────────────────────────
    private UnitData GetWeightedRandom()
    {
        int totalWeight = 0;
        foreach (UnitData data in unitPool)
            totalWeight += data.weight;

        // Random.Range(int, int): min 이상 max 미만의 정수를 반환한다.
        int roll       = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (UnitData data in unitPool)
        {
            cumulative += data.weight;
            if (roll < cumulative) return data;
        }

        return unitPool[unitPool.Length - 1]; // 부동소수점 오차 안전 장치
    }
}
