// EnemyHealth.cs
// 몬스터의 체력을 관리하는 컴포넌트.
// 스폰 시 WaveManager가 Initialize(EnemyData)를 호출해 스탯을 주입한다.

using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int CurrentHp { get; private set; }
    public int MaxHp     { get; private set; }

    private int goldReward;

    /// <summary>
    /// WaveManager가 스폰 직후 호출한다. EnemyData의 스탯을 이 오브젝트에 적용한다.
    /// </summary>
    public void Initialize(EnemyData data)
    {
        MaxHp      = data.maxHp;
        CurrentHp  = data.maxHp;
        goldReward = data.goldReward;

        // 머티리얼 인스턴스를 생성해 색을 바꾼다.
        // sharedMaterial을 바꾸면 같은 머티리얼을 쓰는 모든 오브젝트가 영향을 받으므로
        // .material(인스턴스)을 사용한다.
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = data.bodyColor;
    }

    /// <summary>
    /// 유닛이 공격할 때 호출한다. amount만큼 HP를 깎는다.
    /// </summary>
    public void TakeDamage(int amount)
    {
        CurrentHp -= amount;
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
