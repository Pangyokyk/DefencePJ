// PlayerDataManager.cs
// 씬이 바뀌어도 사라지지 않는 영구 데이터 매니저.
// 스테이지 클리어 골드와 유닛 업그레이드 레벨을 PlayerPrefs에 저장한다.
//
// [DontDestroyOnLoad란?]
//   일반 오브젝트는 씬이 전환될 때 파괴된다.
//   DontDestroyOnLoad를 호출하면 해당 오브젝트는 씬이 바뀌어도 유지된다.
//
// [PlayerPrefs란?]
//   Unity에서 제공하는 간단한 키-값 저장소.
//   게임을 종료해도 데이터가 남아있어 세션 간 저장에 적합하다.

using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    private const string GOLD_KEY     = "PersistentGold";
    private const string LEVEL_KEY    = "UnitLevel_";    // + unitName
    private const string EQUIPPED_KEY = "Equipped_";     // + unitName → 장착 무기 이름
    private const string WEAPONS_KEY  = "OwnedWeapons";  // 쉼표 구분 보유 무기 목록

    // 로비에서 사용하는 지속 골드. 전투 골드(GoldManager)와 별개다.
    public int PersistentGold { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않음
        Load();
    }

    // ─────────────────────────────────────────────
    // 골드
    // ─────────────────────────────────────────────

    public void AddPersistentGold(int amount)
    {
        PersistentGold += amount;
        Save();
    }

    /// <summary>
    /// 골드를 소비한다. 잔액이 부족하면 false를 반환한다.
    /// </summary>
    public bool SpendPersistentGold(int amount)
    {
        if (PersistentGold < amount) return false;
        PersistentGold -= amount;
        Save();
        return true;
    }

    // ─────────────────────────────────────────────
    // 유닛 업그레이드
    // ─────────────────────────────────────────────

    /// <summary>
    /// 해당 유닛의 현재 업그레이드 레벨을 반환한다. 업그레이드 전은 0.
    /// 둘은 모두 같은 뜻 여기서 => 는 프로퍼티가 아닌 메서드이다.
    /// public int GetUnitLevel(string unitName)
    /// {
    ///     return PlayerPrefs.GetInt(LEVEL_KEY + unitName, 0);
    /// }
    /// </summary>
    public int GetUnitLevel(string unitName)
        => PlayerPrefs.GetInt(LEVEL_KEY + unitName, 0);

    /// <summary>
    /// 업그레이드 비용을 지불하고 레벨을 올린다.
    /// 골드가 부족하면 false를 반환한다.
    /// </summary>
    public bool TryUpgradeUnit(UnitData data)
    {
        int level = GetUnitLevel(data.unitName);
        int cost  = data.upgradeBaseCost * (level + 1); // 레벨이 오를수록 비용 증가

        if (!SpendPersistentGold(cost)) return false;

        PlayerPrefs.SetInt(LEVEL_KEY + data.unitName, level + 1);
        PlayerPrefs.Save();
        Debug.Log($"[PlayerDataManager] '{data.unitName}' Lv.{level} → Lv.{level + 1} 업그레이드!");
        return true;
    }

    // ─────────────────────────────────────────────
    // 무기 보유 / 장착
    // ─────────────────────────────────────────────

    /// <summary>
    /// 보유 무기 목록을 반환한다. 없으면 빈 리스트.
    /// </summary>
    public List<string> GetOwnedWeapons()
    {
        string raw = PlayerPrefs.GetString(WEAPONS_KEY, "");
        if (string.IsNullOrEmpty(raw)) return new List<string>();

        // 쉼표로 구분해 저장된 무기 이름을 분리한다.
        return new List<string>(raw.Split(','));
    }

    /// <summary>
    /// 무기를 보유 목록에 추가한다. 이미 있으면 중복 추가하지 않는다.
    /// </summary>
    public void AddWeapon(string weaponName)
    {
        var owned = GetOwnedWeapons();
        if (!owned.Contains(weaponName))
        {
            owned.Add(weaponName);
            PlayerPrefs.SetString(WEAPONS_KEY, string.Join(",", owned));
            PlayerPrefs.Save();
            Debug.Log($"[PlayerDataManager] 무기 획득: {weaponName}");
        }
    }

    /// <summary>
    /// 해당 유닛에 현재 장착된 무기 이름을 반환한다. 없으면 빈 문자열.
    /// </summary>
    public string GetEquippedWeaponName(string unitName)
        => PlayerPrefs.GetString(EQUIPPED_KEY + unitName, "");

    /// <summary>
    /// 유닛에 무기를 장착한다. weaponName이 빈 문자열이면 장착 해제.
    /// </summary>
    public void EquipWeapon(string unitName, string weaponName)
    {
        PlayerPrefs.SetString(EQUIPPED_KEY + unitName, weaponName);
        PlayerPrefs.Save();
        string msg = string.IsNullOrEmpty(weaponName) ? "장착 해제" : $"'{weaponName}' 장착";
        Debug.Log($"[PlayerDataManager] {unitName} — {msg}");
    }

    // ─────────────────────────────────────────────
    // 저장 / 불러오기
    // ─────────────────────────────────────────────

    private void Save()
    {
        PlayerPrefs.SetInt(GOLD_KEY, PersistentGold);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        PersistentGold = PlayerPrefs.GetInt(GOLD_KEY, 0);
    }

    // ─────────────────────────────────────────────
    // 디버그 전용 — 테스트가 끝나면 삭제해도 된다.
    // Inspector에서 이 컴포넌트 우클릭 → 아래 메뉴가 나타난다.
    // ─────────────────────────────────────────────

    [ContextMenu("디버그: 골드 500 추가")]
    private void Debug_AddGold()
    {
        AddPersistentGold(500);
        Debug.Log($"[PlayerDataManager] 디버그 골드 추가 → 현재: {PersistentGold}G");
    }

    [ContextMenu("디버그: 테스트 무기 2개 추가")]
    private void Debug_AddWeapons()
    {
        AddWeapon("롱소드");
        AddWeapon("마법 지팡이");
        Debug.Log("[PlayerDataManager] 테스트 무기 추가 완료");
    }

    [ContextMenu("디버그: 데이터 전체 초기화")]
    private void Debug_ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PersistentGold = 0;
        Debug.Log("[PlayerDataManager] 모든 데이터 초기화 완료");
    }
}
