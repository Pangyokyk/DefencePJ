// PlacementManager.cs
// 유닛 배치를 총괄하는 싱글턴 매니저.
//
// 동작 흐름:
//   1. 매 프레임 마우스 위치에서 레이(광선)를 쏜다.
//   2. 레이가 MapTile에 맞으면 호버 색상을 표시한다.
//   3. 마우스 왼쪽 버튼을 클릭하면 해당 타일에 유닛을 배치한다.
//
// [새 Input System 사용]
//   이 프로젝트는 activeInputHandler=1(New Only)이므로
//   Input.GetMouseButtonDown() 대신 UnityEngine.InputSystem.Mouse 를 사용한다.

using UnityEngine;
using UnityEngine.InputSystem; // 새 Input System 네임스페이스

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    [Header("배치 설정")]
    // unitPrefab은 GachaSystem.PendingUnit.prefab으로 대체됐으므로 제거됨.

    [Header("레이캐스트 설정")]
    [SerializeField] private Camera mainCamera;       // 비워두면 Camera.main을 자동 사용
    // LayerMask: 레이캐스트가 감지할 레이어를 선택한다.
    // 나중에 "Tile" 레이어를 만들고 여기에 지정하면 불필요한 충돌 감지를 줄일 수 있다.
    [SerializeField] private LayerMask tileLayerMask = ~0; // ~0 = 모든 레이어

    // 현재 마우스가 올라가 있는 타일을 추적한다.
    private MapTile hoveredTile;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // mainCamera를 Inspector에서 지정하지 않으면 씬의 Main Camera를 자동으로 찾는다.
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateHover();
        HandleClickInput();
    }

    // ─────────────────────────────────────────────
    // 호버 처리: 마우스 아래 타일을 감지하고 색상을 갱신한다.
    // ─────────────────────────────────────────────
    private void UpdateHover()
    {
        MapTile newHovered = GetTileUnderMouse();

        // 이전 프레임과 같은 타일이면 아무것도 안 해도 된다.
        if (newHovered == hoveredTile) return;

        // 이전 타일의 호버 강조 해제
        if (hoveredTile != null)
            hoveredTile.SetHovered(false);

        hoveredTile = newHovered;

        // 새 타일 호버 강조 적용
        if (hoveredTile != null)
            hoveredTile.SetHovered(true);
    }

    // ─────────────────────────────────────────────
    // 클릭 처리: 새 Input System으로 마우스 좌클릭을 감지한다.
    // ─────────────────────────────────────────────
    private void HandleClickInput()
    {
        // Mouse.current: 현재 연결된 마우스 장치. 마우스가 없으면 null이다.
        // wasPressedThisFrame: 이 프레임에 처음 눌린 경우에만 true → 누르는 동안 계속 발동 안 함
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlaceUnit();
        }
    }

    private void TryPlaceUnit()
    {
        if (hoveredTile == null) return;

        // 가챠로 뽑은 유닛이 없으면 배치할 수 없다.
        UnitData pending = GachaSystem.Instance.PendingUnit;
        if (pending == null)
        {
            Debug.Log("[PlacementManager] 먼저 뽑기 버튼으로 유닛을 뽑아주세요.");
            return;
        }

        bool placed = hoveredTile.TryPlaceUnit(pending);
        if (placed)
        {
            // 배치 성공 시 손패를 비운다.
            GachaSystem.Instance.ClearPending();
        }
    }

    // ─────────────────────────────────────────────
    // 레이캐스트: 마우스 화면 좌표 → 3D 월드의 타일 감지
    // ─────────────────────────────────────────────
    private MapTile GetTileUnderMouse()
    {
        if (mainCamera == null || Mouse.current == null) return null;

        // Mouse.current.position.ReadValue(): 현재 마우스의 화면 픽셀 좌표 (Vector2)
        Vector2 screenPos = Mouse.current.position.ReadValue();

        // ScreenPointToRay: 화면 좌표 → 카메라에서 나가는 3D 광선(Ray)으로 변환
        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        // Physics.Raycast: 광선이 Collider에 맞으면 true, 충돌 정보는 hit에 담긴다.
        // 100f: 최대 감지 거리, tileLayerMask: 감지할 레이어 필터
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayerMask))
        {
            // 충돌한 오브젝트에 MapTile 컴포넌트가 있으면 반환한다.
            return hit.collider.GetComponent<MapTile>();
        }

        return null;
    }
}
