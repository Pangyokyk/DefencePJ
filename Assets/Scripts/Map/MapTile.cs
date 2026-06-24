// MapTile.cs
// 맵의 개별 칸(타일)을 나타내는 컴포넌트.
// 유닛 배치 여부를 관리하고, 마우스 호버/배치 상태에 따라 색상을 바꿔 시각적 피드백을 제공한다.
// 이 스크립트를 납작한 Cube(타일)에 붙여 사용한다.

using UnityEngine;

// [RequireComponent]: 이 스크립트가 붙은 오브젝트에 Renderer가 반드시 있어야 함을 강제한다.
// 없으면 Unity가 자동으로 추가해 준다.
[RequireComponent(typeof(Renderer))]
public class MapTile : MonoBehaviour
{
    // Inspector에서 각 상태별 색상을 자유롭게 조절할 수 있다.
    [SerializeField] private Color normalColor   = new Color(0.55f, 0.75f, 0.55f); // 기본 (초록빛 회색)
    [SerializeField] private Color hoverColor    = new Color(0.90f, 0.95f, 0.50f); // 호버 (노란색)
    [SerializeField] private Color occupiedColor = new Color(0.45f, 0.45f, 0.45f); // 배치 완료 (회색)

    private Renderer tileRenderer;

    // 이 타일에 유닛이 있는지 여부. 외부에서는 읽기만 가능.
    public bool IsOccupied { get; private set; }

    // 배치된 유닛 참조. 3단계(공격)에서 유닛에 접근할 때 사용한다.
    public GameObject PlacedUnit { get; private set; }

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
        // URP Lit 셰이더의 기본 색상 프로퍼티는 _BaseColor다.
        // renderer.material 을 쓰면 Unity가 자동으로 머티리얼 인스턴스를 만들어 이 타일만 색이 바뀐다.
        tileRenderer.material.color = normalColor;
    }

    // ─────────────────────────────────────────────
    // PlacementManager가 레이캐스트 결과에 따라 호출하는 메서드들
    // ─────────────────────────────────────────────

    /// <summary>
    /// 마우스가 타일 위에 있을 때(hovered=true) / 벗어났을 때(false) PlacementManager가 호출한다.
    /// </summary>
    public void SetHovered(bool hovered)
    {
        // 이미 유닛이 배치된 칸은 색상을 바꾸지 않는다.
        if (IsOccupied) return;
        tileRenderer.material.color = hovered ? hoverColor : normalColor;
    }

    /// <summary>
    /// 뽑기로 얻은 UnitData를 받아 유닛을 이 타일에 배치한다.
    /// 이미 점유됐으면 false를 반환한다.
    /// </summary>
    public bool TryPlaceUnit(UnitData unitData)
    {
        if (IsOccupied)
        {
            Debug.Log("[MapTile] 이미 유닛이 배치된 칸입니다.");
            return false;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 0.6f;
        PlacedUnit = Instantiate(unitData.prefab, spawnPos, Quaternion.identity);

        // 생성된 유닛에 UnitData 스탯을 주입한다.
        // Start()보다 먼저 호출되므로 Initialize()에서 설정한 값이 Start()의 로그에 반영된다.
        Unit unit = PlacedUnit.GetComponent<Unit>();
        if (unit != null) unit.Initialize(unitData);

        IsOccupied = true;
        tileRenderer.material.color = occupiedColor;

        Debug.Log($"[MapTile] '{unitData.unitName}' 배치 완료.");
        return true;
    }
}
