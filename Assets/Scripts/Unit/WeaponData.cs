// WeaponData.cs
// 무기 한 종류의 스탯을 담는 ScriptableObject.
// Project 창 우클릭 → Create → DefencePJ → Weapon Data 로 생성한다.

using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "DefencePJ/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("기본 정보")]
    public string weaponName  = "무기";
    public string description = "";     // UI에 표시할 간단한 설명

    [Header("유닛 스탯 보너스 (장착 시 합산)")]
    public int   attackBonus       = 0;   // 추가 공격력
    public float attackRangeBonus  = 0f;  // 추가 사거리
    public float attackSpeedBonus  = 0f;  // 추가 공격속도 (회/초)
    public float manaRegenBonus    = 0f;  // 추가 마나 회복 (/초)
}
