// EnemyMover.cs
// 몬스터 오브젝트에 붙이는 이동 컴포넌트.
// EnemySpawner가 몬스터를 생성한 뒤 Initialize()를 호출하면
// 웨이포인트 경로를 따라 이동하고, 마지막 지점 도달 시 사라지며 목숨을 감소시킨다.

using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float speed = 3f;              // 이동 속도 (단위: m/s)
    [SerializeField] private float arrivalThreshold = 0.1f; // 이 거리 이하면 웨이포인트 도달로 판정

    // 이 몬스터가 따라갈 WaypointPath. Initialize()로 외부에서 주입받는다.
    private WaypointPath path;

    // 현재 목표로 삼고 있는 웨이포인트의 배열 인덱스
    private int currentWaypointIndex;

    // ─────────────────────────────────────────────
    // 공개 메서드: EnemySpawner가 생성 직후 호출해 경로를 전달한다.
    // Awake/Start 대신 이 방식을 쓰면 생성 시점과 초기화 시점을 명확히 분리할 수 있다.
    // ─────────────────────────────────────────────
    public void Initialize(WaypointPath waypointPath)
    {
        path = waypointPath;
        currentWaypointIndex = 0;

        // 스폰 직후 첫 번째 웨이포인트 위치에 딱 맞게 배치한다.
        // (스포너가 임의 위치에 생성했을 경우를 대비)
        transform.position = path.GetWaypoint(0).position;
    }

    // Update는 매 프레임 호출된다. 여기서 실제 이동 로직을 실행한다.
    private void Update()
    {
        // 경로가 아직 할당되지 않았으면 아무것도 하지 않는다.
        if (path == null) return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        // 현재 목표 웨이포인트의 월드 좌표를 가져온다.
        Vector3 targetPosition = path.GetWaypoint(currentWaypointIndex).position;

        // Vector3.MoveTowards:
        //   현재 위치(from)에서 목표 위치(to)를 향해 최대 maxDelta만큼 이동한 위치를 반환한다.
        //   목표를 넘어서 이동하는 '오버슈팅'이 없어서 웨이포인트 이동에 안전하다.
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime  // Time.deltaTime: 이전 프레임과의 시간 차 → 프레임레이트 독립 이동
        );

        // 진행 방향을 바라보도록 회전시킨다.
        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude > 0.001f) // 거의 도달해 방향 벡터가 0에 가까울 때 튀는 회전 방지
        {
            // transform.forward를 목표 방향으로 맞추면 오브젝트가 이동 방향을 바라본다.
            transform.forward = direction.normalized;
        }

        // 목표 웨이포인트까지의 거리를 계산하여 도달 여부를 판정한다.
        if (Vector3.Distance(transform.position, targetPosition) <= arrivalThreshold)
        {
            OnReachWaypoint();
        }
    }

    private void OnReachWaypoint()
    {
        // 마지막 웨이포인트(도착 지점)에 도달했는지 확인한다.
        // path.Count - 1 이 마지막 인덱스이므로, 현재 인덱스와 비교한다.
        if (currentWaypointIndex >= path.Count - 1)
        {
            OnReachEnd();
            return;
        }

        // 아직 중간 경유 지점이라면 다음 웨이포인트로 넘어간다.
        currentWaypointIndex++;
    }

    private void OnReachEnd()
    {
        LevelManager.Instance.LoseLife();

        // 처치된 경우와 동일하게 WaveManager에도 제거를 알린다.
        // (도착 지점에 닿은 몬스터도 씬에서 사라지므로 카운트를 차감해야 한다)
        WaveManager.Instance.OnEnemyRemoved();

        Destroy(gameObject);
    }
}
