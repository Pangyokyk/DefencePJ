// EnemyHealth.cs
// 몬스터의 체력을 관리하는 컴포넌트.
// 스폰 시 WaveManager가 Initialize(EnemyData)를 호출해 스탯을 주입한다.

using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("HP바 (프리팹 안의 BarUI 컴포넌트 연결)")]
    [SerializeField] private BarUI hpBar;

    public int CurrentHp { get; private set; }
    public int MaxHp     { get; private set; }

    private int goldReward;

    public void Initialize(EnemyData data)
    {
        MaxHp      = data.maxHp;
        CurrentHp  = data.maxHp;
        goldReward = data.goldReward;

        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = data.bodyColor;

        hpBar?.SetFill(CurrentHp, MaxHp); // 스폰 직후 바를 꽉 채운 상태로 초기화
    }

    public void TakeDamage(int amount)
    {
        CurrentHp -= amount;
        hpBar?.SetFill(CurrentHp, MaxHp); // 피해를 받을 때마다 바 갱신
        if (CurrentHp <= 0)
            Die();
    }

    private void Die()
    {
        GoldManager.Instance.AddGold(goldReward);
        WaveManager.Instance.OnEnemyRemoved();
        Destroy(gameObject);
    }
}
