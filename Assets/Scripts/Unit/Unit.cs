// Unit.cs
// 배치된 유닛의 자동 공격과 마나/스킬 로직을 담당하는 컴포넌트.
//
// 동작 흐름:
//   [기본 공격] 매 프레임 attackTimer 누적 → 쿨타임마다 가장 가까운 적 공격
//   [마나]      매 프레임 manaRegenPerSec 만큼 마나 회복
//   [스킬]      마나가 maxMana에 도달하면 스킬 자동 발동 → 마나 0 초기화

using UnityEngine;

public class Unit : MonoBehaviour
{
    // ── 기본 공격 스탯 ──────────────────────────────
    private string    unitName;
    private int       attack;
    private float     attackRange;
    private float     attackSpeed;

    // ── 마나 / 스킬 스탯 ────────────────────────────
    private float     maxMana;
    private float     manaRegenPerSec;
    private SkillType skillType;
    private int       skillDamage;
    private float     skillRange;

    [Header("마나바 (프리팹 안의 BarUI 컴포넌트 연결)")]
    [SerializeField] private BarUI manaBar;

    // ── 런타임 상태 ─────────────────────────────────
    private float attackTimer;

    // 현재 마나를 외부(UI 등)에서 읽을 수 있도록 프로퍼티로 노출한다.
    public float CurrentMana { get; private set; }
    public float MaxMana     => maxMana;
    public string UnitName   => unitName;

    /// <summary>
    /// GachaSystem이 뽑은 UnitData로 스탯을 설정한다. MapTile이 생성 직후 호출한다.
    /// </summary>
    public void Initialize(UnitData data)
    {
        unitName        = data.unitName;
        attack          = data.attack;
        attackRange     = data.attackRange;
        attackSpeed     = data.attackSpeed;

        maxMana         = data.maxMana;
        manaRegenPerSec = data.manaRegenPerSec;
        skillType       = data.skillType;
        skillDamage     = data.skillDamage;
        skillRange      = data.skillRange;

        if (PlayerDataManager.Instance != null)
        {
            // 업그레이드 보너스
            int level = PlayerDataManager.Instance.GetUnitLevel(data.unitName);
            attack += data.attackPerLevel * level;

            // 무기 보너스 — 장착된 무기가 있으면 스탯에 합산한다
            string equippedName = PlayerDataManager.Instance.GetEquippedWeaponName(data.unitName);
            if (!string.IsNullOrEmpty(equippedName) && WeaponDatabase.Instance != null)
            {
                WeaponData weapon = WeaponDatabase.Instance.GetByName(equippedName);
                if (weapon != null)
                {
                    attack          += weapon.attackBonus;
                    attackRange     += weapon.attackRangeBonus;
                    attackSpeed     += weapon.attackSpeedBonus;
                    manaRegenPerSec += weapon.manaRegenBonus;
                }
            }
        }

        CurrentMana = 0f;
        attackTimer = 1f / attackSpeed;
    }

    private void Update()
    {
        HandleAttack();
        HandleMana();
    }

    // ─────────────────────────────────────────────
    // 기본 공격
    // ─────────────────────────────────────────────
    private void HandleAttack()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer < 1f / attackSpeed) return;

        attackTimer = 0f;
        EnemyHealth target = FindNearestEnemy(attackRange);
        if (target == null) return;

        target.TakeDamage(attack);
        
        Debug.DrawLine(transform.position, target.transform.position, Color.red, 0.1f);
    }

    // ─────────────────────────────────────────────
    // 마나 회복 및 스킬 발동
    // ─────────────────────────────────────────────
    private void HandleMana()
    {
        CurrentMana += manaRegenPerSec * Time.deltaTime;

        if (CurrentMana >= maxMana)
        {
            CurrentMana = 0f;
            UseSkill();
        }

        manaBar?.SetFill(CurrentMana, maxMana);
    }

    private void UseSkill()
    {
        switch (skillType)
        {
            case SkillType.SingleStrike:
                UseSkillSingleStrike();
                break;
            case SkillType.AreaBlast:
                UseSkillAreaBlast();
                break;
        }
    }

    // 가장 가까운 적 한 마리에게 강력한 피해를 입힌다.
    private void UseSkillSingleStrike()
    {
        EnemyHealth target = FindNearestEnemy(attackRange);
        if (target == null) return;

        target.TakeDamage(skillDamage);

        // 씬 뷰에서 스킬 발동을 노란 선으로 확인한다.
        Debug.DrawLine(transform.position, target.transform.position, Color.yellow, 0.5f);
        Debug.Log($"[Unit] '{unitName}' 스킬 — 강타 {skillDamage} 피해!");
    }

    // 스킬 사거리 내 모든 적에게 피해를 입힌다.
    private void UseSkillAreaBlast()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, skillRange);
        int hitCount = 0;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null) continue;

            enemy.TakeDamage(skillDamage);
            Debug.DrawLine(transform.position, hit.transform.position, Color.cyan, 0.5f);
            hitCount++;
        }

        Debug.Log($"[Unit] '{unitName}' 스킬 — 범위 폭발 {skillDamage} 피해 x{hitCount}마리!");
    }

    // ─────────────────────────────────────────────
    // 지정 범위 내 가장 가까운 적을 탐색한다.
    // range 파라미터를 받아 기본 공격/스킬 탐색에 공통으로 사용한다.
    // ─────────────────────────────────────────────
    private EnemyHealth FindNearestEnemy(float range)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        EnemyHealth nearest     = null;
        float       nearestDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest     = enemy;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 스킬 범위는 파란색으로 표시 (AreaBlast 전용)
        if (skillType == SkillType.AreaBlast)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, skillRange);
        }
    }
}
