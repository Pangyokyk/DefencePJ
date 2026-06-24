// GoldManager.cs
// 플레이어의 골드를 관리하는 싱글턴.
// 몬스터 처치 시 AddGold(), 유닛 구매 시 SpendGold()를 호출한다.

using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [SerializeField] private int startingGold = 50;

    // 현재 골드. 외부에서 읽기만 허용 (차감/추가는 이 클래스 메서드로만)
    public int CurrentGold { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        CurrentGold = startingGold;
    }

    /// <summary>몬스터 처치 등으로 골드를 추가한다.</summary>
    public void AddGold(int amount)
    {
        CurrentGold += amount;
        Debug.Log($"[GoldManager] +{amount} 골드 획득! 보유 골드: {CurrentGold}");
    }

    /// <summary>
    /// 골드를 소비한다. 잔액이 부족하면 false를 반환하고 골드를 차감하지 않는다.
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (CurrentGold < amount)
        {
            Debug.Log($"[GoldManager] 골드 부족! 필요: {amount}, 보유: {CurrentGold}");
            return false;
        }
        CurrentGold -= amount;
        Debug.Log($"[GoldManager] -{amount} 골드 사용. 보유 골드: {CurrentGold}");
        return true;
    }
}
