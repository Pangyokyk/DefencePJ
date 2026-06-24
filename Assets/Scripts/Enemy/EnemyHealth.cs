// EnemyHealth.cs
// 몬스터의 체력을 관리하는 컴포넌트.
// 유닛의 공격을 받으면 TakeDamage()가 호출되고, HP가 0 이하가 되면 처치된다.
// Enemy 프리팹에 EnemyMover와 함께 붙여 사용한다.

using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp      = 100; // Inspector에서 몬스터 종류별로 조절
    [SerializeField] private int goldReward = 10;  // 처치 시 지급할 골드

    // 현재 HP. 외부에서 읽을 수 있지만 직접 대입은 불가 (TakeDamage를 통해서만 감소)
    public int CurrentHp { get; private set; }
    public int MaxHp => maxHp;

    private void Awake()
    {
        CurrentHp = maxHp;
    }

    /// <summary>
    /// 유닛이 공격할 때 호출한다. amount만큼 HP를 깎는다.
    /// </summary>
    public void TakeDamage(int amount)
    {
        CurrentHp -= amount;
        Debug.Log($"[EnemyHealth] {gameObject.name} — 피해 {amount}, 남은 HP: {CurrentHp}/{maxHp}");

        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[EnemyHealth] {gameObject.name} 처치!");

        // 골드 지급
        GoldManager.Instance.AddGold(goldReward);

        // WaveManager에 "몬스터 한 마리 제거됨"을 알린다.
        // 웨이브 클리어 판정(살아있는 몬스터 수 추적)에 사용된다.
        WaveManager.Instance.OnEnemyRemoved();

        Destroy(gameObject);
    }
}
