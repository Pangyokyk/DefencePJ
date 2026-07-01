// UnitData.cs
// 유닛 한 종류의 데이터를 담는 ScriptableObject.
// Project 창 우클릭 → Create → DefencePJ → Unit Data 로 생성한다.

using UnityEngine;

// 유닛이 사용할 수 있는 스킬 종류.
// Inspector 드롭다운으로 선택할 수 있도록 public enum으로 선언한다.
public enum SkillType
{
    SingleStrike, // 단일 강타 — 가장 가까운 적 한 마리에게 강력한 피해
    AreaBlast,    // 범위 폭발 — 스킬 사거리 내 모든 적에게 피해
}

[CreateAssetMenu(fileName = "NewUnitData", menuName = "DefencePJ/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("기본 정보")]
    public string     unitName = "유닛";
    public GameObject prefab;

    [Header("기본 공격 스탯")]
    public int   attack      = 10;
    public float attackRange = 3f;
    public float attackSpeed = 1f;  // 공격 횟수/초

    [Header("마나 / 스킬")]
    public int       maxMana         = 100;  // 마나 최대치 (이 값에 도달하면 스킬 발동)
    public float     manaRegenPerSec = 10f;  // 초당 마나 회복량
    public SkillType skillType       = SkillType.SingleStrike;
    public int       skillDamage     = 50;   // 스킬 피해량
    public float     skillRange      = 5f;   // AreaBlast 전용 범위 (SingleStrike는 무시)

    [Header("업그레이드")]
    public int attackPerLevel  = 5;   // 레벨당 증가하는 공격력
    public int upgradeBaseCost = 100; // 레벨 1 업그레이드 비용. 이후 레벨 * baseCost로 증가

    [Header("뽑기 설정")]
    [Range(1, 100)]
    public int weight = 50;
}
