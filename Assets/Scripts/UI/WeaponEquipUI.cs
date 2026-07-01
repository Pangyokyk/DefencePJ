// WeaponEquipUI.cs
// 무기 장착 패널을 관리한다.
// 유닛 행(WeaponEquipRow)은 에디터에서 직접 만들고, 이 스크립트가 데이터를 채운다.
// 무기 선택 팝업 내부 버튼만 보유 무기에 따라 동적으로 생성된다.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEquipUI : MonoBehaviour
{
    [Header("유닛 행 목록")]
    [SerializeField] private WeaponEquipRow[] rows;

    [Header("무기 선택 팝업")]
    [SerializeField] private GameObject    weaponSelectPopup;  // 팝업 루트 (기본 비활성화)
    [SerializeField] private TMP_Text      popupTitleText;     // "○○에게 장착할 무기"
    [SerializeField] private Transform     popupButtonContent; // 버튼들이 들어갈 컨테이너 (VerticalLayoutGroup 권장)

    [Header("폰트 (동적 생성 텍스트에 적용)")]
    [SerializeField] private TMP_FontAsset font; // Inspector에서 Maplestory Bold SDF 연결

    private string selectedUnitName; // 현재 장착 변경 중인 유닛 이름

    private void Start()
    {
        weaponSelectPopup.SetActive(false);

        foreach (var row in rows)
        {
            var captured = row;
            row.changeButton.onClick.AddListener(() => OpenWeaponSelect(captured));
            RefreshRow(row);
        }
    }

    // ─────────────────────────────────────────────
    // 무기 선택 팝업 열기
    // ─────────────────────────────────────────────
    private void OpenWeaponSelect(WeaponEquipRow row)
    {
        selectedUnitName     = row.unitData.unitName;
        popupTitleText.text  = $"{selectedUnitName}에게 장착할 무기를 선택하세요";

        // 이전에 생성된 버튼 제거 후 새로 생성
        foreach (Transform child in popupButtonContent)
            Destroy(child.gameObject);

        // 보유 무기 버튼 생성
        var ownedWeapons = PlayerDataManager.Instance.GetOwnedWeapons();
        if (ownedWeapons.Count == 0)
        {
            MakeLabel(popupButtonContent, "보유한 무기가 없습니다.");
        }
        else
        {
            foreach (string weaponName in ownedWeapons)
            {
                WeaponData weapon = WeaponDatabase.Instance.GetByName(weaponName);
                if (weapon == null) continue;

                bool isEquipped = PlayerDataManager.Instance.GetEquippedWeaponName(selectedUnitName) == weaponName;
                MakeWeaponButton(weapon, isEquipped);
            }
        }

        // 장착 해제 버튼 (현재 무기가 있을 때만)
        if (!string.IsNullOrEmpty(PlayerDataManager.Instance.GetEquippedWeaponName(selectedUnitName)))
            MakeUnequipButton();

        weaponSelectPopup.SetActive(true);
    }

    // ─────────────────────────────────────────────
    // 팝업 내부 버튼 생성 헬퍼
    // ─────────────────────────────────────────────
    private void MakeWeaponButton(WeaponData weapon, bool isEquipped)
    {
        var obj = new GameObject(weapon.weaponName + "_Btn");
        obj.transform.SetParent(popupButtonContent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300f, 65f);

        var img = obj.AddComponent<Image>();
        img.color = isEquipped ? new Color(0.2f, 0.7f, 0.3f) : new Color(0.25f, 0.45f, 0.7f);

        var btn = obj.AddComponent<Button>();
        var captured = weapon;
        btn.onClick.AddListener(() => OnWeaponSelected(captured.weaponName));

        // 버튼 텍스트 (무기 이름 + 스탯 요약)
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(obj.transform, false);
        var lrt = labelObj.AddComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = new Vector2(8f, 0f);
        lrt.offsetMax = Vector2.zero;
        lrt.localScale = Vector3.one;
        var t = labelObj.AddComponent<TextMeshProUGUI>();
        t.fontSize  = 15f;
        t.color     = Color.white;
        t.alignment = TextAlignmentOptions.MidlineLeft;
        if (font != null) t.font = font;

        string equippedMark = isEquipped ? " [장착 중]" : "";
        t.text = $"<b>{weapon.weaponName}</b>{equippedMark}\n" +
                 StatSummary(weapon);
    }

    private void MakeUnequipButton()
    {
        var obj = new GameObject("UnequipBtn");
        obj.transform.SetParent(popupButtonContent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300f, 50f);

        var img = obj.AddComponent<Image>();
        img.color = new Color(0.6f, 0.2f, 0.2f);

        var btn = obj.AddComponent<Button>();
        btn.onClick.AddListener(() => OnWeaponSelected(""));

        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(obj.transform, false);
        var lrt = labelObj.AddComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;
        lrt.localScale = Vector3.one;
        var t = labelObj.AddComponent<TextMeshProUGUI>();
        t.fontSize  = 15f;
        t.color     = Color.white;
        t.alignment = TextAlignmentOptions.Center;
        if (font != null) t.font = font;
        t.text      = "장착 해제";
    }

    private void MakeLabel(Transform parent, string text)
    {
        var obj = new GameObject("Label");
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300f, 50f);
        var t = obj.AddComponent<TextMeshProUGUI>();
        t.fontSize  = 16f;
        t.color     = Color.gray;
        t.alignment = TextAlignmentOptions.Center;
        if (font != null) t.font = font;
        t.text      = text;
    }

    // ─────────────────────────────────────────────
    // 무기 선택 / 해제 처리
    // ─────────────────────────────────────────────
    private void OnWeaponSelected(string weaponName)
    {
        PlayerDataManager.Instance.EquipWeapon(selectedUnitName, weaponName);
        RefreshAllRows();
        weaponSelectPopup.SetActive(false);
    }

    // 팝업 닫기 버튼 OnClick에 연결
    public void OnClosePopupClicked()
    {
        weaponSelectPopup.SetActive(false);
    }

    // ─────────────────────────────────────────────
    // 행 갱신
    // ─────────────────────────────────────────────
    private void RefreshRow(WeaponEquipRow row)
    {
        string equipped = PlayerDataManager.Instance.GetEquippedWeaponName(row.unitData.unitName);
        string weaponDisplay = string.IsNullOrEmpty(equipped) ? "없음" : equipped;

        WeaponData weaponData = string.IsNullOrEmpty(equipped) ? null : WeaponDatabase.Instance?.GetByName(equipped);
        string statDisplay = weaponData != null ? $"  ({StatSummary(weaponData)})" : "";

        row.infoText.text = $"<b>{row.unitData.unitName}</b>\n장착 무기: {weaponDisplay}{statDisplay}";
    }

    private void RefreshAllRows()
    {
        foreach (var row in rows) RefreshRow(row);
    }

    // 스탯 보너스 요약 문자열 생성
    private string StatSummary(WeaponData w)
    {
        var parts = new System.Collections.Generic.List<string>();
        if (w.attackBonus      != 0)  parts.Add($"공격+{w.attackBonus}");
        if (w.attackRangeBonus != 0f) parts.Add($"사거리+{w.attackRangeBonus}");
        if (w.attackSpeedBonus != 0f) parts.Add($"속도+{w.attackSpeedBonus}");
        if (w.manaRegenBonus   != 0f) parts.Add($"마나+{w.manaRegenBonus}/s");
        return parts.Count > 0 ? string.Join("  ", parts) : "보너스 없음";
    }
}
