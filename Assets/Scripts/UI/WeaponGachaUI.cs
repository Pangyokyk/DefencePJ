// WeaponGachaUI.cs
// 무기 뽑기(가챠) 패널을 담당한다.
//
// [애니메이션 확장 포인트]
//   나중에 카드 연출을 추가하려면 ShowResultCoroutine() 안의
//   "애니메이션 삽입 위치" 주석 부분에 코루틴을 추가하면 된다.
//   나머지 로직은 건드리지 않아도 된다.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponGachaUI : MonoBehaviour
{
    [Header("비용")]
    [SerializeField] private int singleCost = 100; // 1회 뽑기 비용
    [SerializeField] private int multiCost  = 900; // 10회 뽑기 비용 (10% 할인)

    [Header("UI 연결")]
    [SerializeField] private TMP_Text  goldText;         // 보유 골드 표시
    [SerializeField] private Button    singleDrawButton; // 1회 뽑기 버튼
    [SerializeField] private Button    multiDrawButton;  // 10회 뽑기 버튼
    [SerializeField] private TMP_Text  singleCostText;   // "1회 100G"
    [SerializeField] private TMP_Text  multiCostText;    // "10회 900G"

    [Header("결과 패널")]
    [SerializeField] private GameObject resultPanel;   // 결과 표시 패널 (기본 비활성화)
    [SerializeField] private Transform  resultContent; // 결과 카드들이 쌓이는 컨테이너
    [SerializeField] private Button     resultCloseButton; // "확인" 버튼

    [Header("폰트 (동적 생성 텍스트에 적용)")]
    [SerializeField] private TMP_FontAsset font;

    private void Start()
    {
        resultPanel.SetActive(false);

        singleDrawButton.onClick.AddListener(() => OnDrawClicked(1));
        multiDrawButton.onClick.AddListener(()  => OnDrawClicked(10));
        resultCloseButton.onClick.AddListener(CloseResult);

        RefreshUI();
    }

    // ─────────────────────────────────────────────
    // 뽑기 버튼 클릭
    // ─────────────────────────────────────────────
    private void OnDrawClicked(int count)
    {
        int cost = count == 1 ? singleCost : multiCost;

        if (!PlayerDataManager.Instance.SpendPersistentGold(cost))
        {
            Debug.Log("[WeaponGachaUI] 골드가 부족합니다.");
            return;
        }

        // 뽑기 실행
        var results = Roll(count);

        // 결과를 인벤토리에 추가
        foreach (var weapon in results)
            PlayerDataManager.Instance.AddWeapon(weapon.weaponName);

        // 결과 연출 시작
        StartCoroutine(ShowResultCoroutine(results));
    }

    // ─────────────────────────────────────────────
    // 결과 표시 코루틴 — 애니메이션 확장 포인트
    // ─────────────────────────────────────────────
    private IEnumerator ShowResultCoroutine(List<WeaponData> results)
    {
        // 기존 결과 카드 제거
        foreach (Transform child in resultContent)
            Destroy(child.gameObject);

        resultPanel.SetActive(true);

        // ★ 애니메이션 삽입 위치 ★
        // 나중에 카드 등장 연출을 넣으려면 아래 주석을 해제하고 구현한다.
        // yield return StartCoroutine(PlayOpeningAnimation());

        // 결과 카드 순차 등장
        foreach (var weapon in results)
        {
            CreateResultCard(weapon);

            // ★ 카드 한 장씩 등장하는 연출을 넣으려면 아래 주석을 해제한다.
            // yield return new WaitForSeconds(0.3f);
        }

        yield return null; // 코루틴 형태 유지용 (애니메이션 없을 때 한 프레임 대기)

        RefreshUI();
    }

    private void CloseResult()
    {
        resultPanel.SetActive(false);
    }

    // ─────────────────────────────────────────────
    // 가중치 기반 랜덤 뽑기
    // ─────────────────────────────────────────────
    private List<WeaponData> Roll(int count)
    {
        var results = new List<WeaponData>();
        for (int i = 0; i < count; i++)
        {
            WeaponData weapon = WeaponDatabase.Instance.GetWeightedRandom();
            if (weapon != null) results.Add(weapon);
        }
        return results;
    }

    // ─────────────────────────────────────────────
    // 결과 카드 생성 (resultContent 안에 동적 생성)
    // ─────────────────────────────────────────────
    private void CreateResultCard(WeaponData weapon)
    {
        var card = new GameObject(weapon.weaponName + "_Card");
        card.transform.SetParent(resultContent, false);

        var rt = card.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200f, 120f);

        var img = card.AddComponent<Image>();
        img.color = new Color(0.15f, 0.25f, 0.45f);

        // 무기 정보 텍스트
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(card.transform, false);
        var trt = textObj.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = new Vector2(8f,  8f);
        trt.offsetMax = new Vector2(-8f, -8f);
        trt.localScale = Vector3.one;

        var t = textObj.AddComponent<TextMeshProUGUI>();
        t.fontSize  = 15f;
        t.color     = Color.white;
        t.alignment = TextAlignmentOptions.Center;
        if (font != null) t.font = font;

        string stats = BuildStatText(weapon);
        t.text = $"<b>{weapon.weaponName}</b>\n{stats}";
    }

    private string BuildStatText(WeaponData w)
    {
        var parts = new List<string>();
        if (w.attackBonus      != 0)  parts.Add($"공격 +{w.attackBonus}");
        if (w.attackRangeBonus != 0f) parts.Add($"사거리 +{w.attackRangeBonus}");
        if (w.attackSpeedBonus != 0f) parts.Add($"속도 +{w.attackSpeedBonus}");
        if (w.manaRegenBonus   != 0f) parts.Add($"마나 +{w.manaRegenBonus}/s");
        return string.Join("\n", parts);
    }

    // ─────────────────────────────────────────────
    // UI 갱신
    // ─────────────────────────────────────────────
    private void RefreshUI()
    {
        int gold = PlayerDataManager.Instance.PersistentGold;
        goldText.text       = $"보유 골드: {gold}G";
        singleCostText.text = $"1회  {singleCost}G";
        multiCostText.text  = $"10회  {multiCost}G";

        singleDrawButton.interactable = gold >= singleCost;
        multiDrawButton.interactable  = gold >= multiCost;
    }
}
