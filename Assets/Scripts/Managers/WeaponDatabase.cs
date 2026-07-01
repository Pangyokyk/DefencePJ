// WeaponDatabase.cs
// 게임에 존재하는 모든 WeaponData를 보관하는 싱글턴.
// 무기 이름으로 WeaponData를 검색하는 기능을 제공한다.
// StageSelect 씬의 오브젝트에 붙이고 DontDestroyOnLoad로 유지한다.

using System;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    public static WeaponDatabase Instance { get; private set; }

    [Header("게임에 존재하는 모든 무기 에셋을 여기에 등록한다")]
    [SerializeField] private WeaponData[] allWeapons;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 무기 이름으로 WeaponData를 찾아 반환한다. 없으면 null.
    /// </summary>
    public WeaponData GetByName(string weaponName)
        => Array.Find(allWeapons, w => w.weaponName == weaponName);

    /// <summary>
    /// weight 기반 랜덤으로 무기 하나를 뽑아 반환한다.
    /// </summary>
    public WeaponData GetWeightedRandom()
    {
        int total = 0;
        foreach (var w in allWeapons) total += w.weight;

        int roll       = UnityEngine.Random.Range(0, total);
        int cumulative = 0;
        foreach (var w in allWeapons)
        {
            cumulative += w.weight;
            if (roll < cumulative) return w;
        }
        return allWeapons[allWeapons.Length - 1];
    }
}
