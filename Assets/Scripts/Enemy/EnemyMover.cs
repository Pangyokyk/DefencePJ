// EnemyMover.cs
// 몬스터 오브젝트에 붙이는 이동 컴포넌트.
// Initialize(WaypointPath, EnemyData) 를 호출하면 웨이포인트 경로를 따라 이동한다.

using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float arrivalThreshold = 0.1f; // 웨이포인트 도달 판정 거리

    private WaypointPath path;
    private float        speed;
    private int          currentWaypointIndex;

    /// <summary>
    /// WaveManager가 스폰 직후 호출한다.
    /// </summary>
    public void Initialize(WaypointPath waypointPath, EnemyData data)
    {
        path  = waypointPath;
        speed = data.moveSpeed;
        currentWaypointIndex = 0;
        transform.position = path.GetWaypoint(0).position;
    }

    private void Update()
    {
        if (path == null) return;
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        Vector3 targetPosition = path.GetWaypoint(currentWaypointIndex).position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude > 0.001f)
            transform.forward = direction.normalized;

        if (Vector3.Distance(transform.position, targetPosition) <= arrivalThreshold)
            OnReachWaypoint();
    }

    private void OnReachWaypoint()
    {
        if (currentWaypointIndex >= path.Count - 1)
        {
            OnReachEnd();
            return;
        }
        currentWaypointIndex++;
    }

    private void OnReachEnd()
    {
        LevelManager.Instance.LoseLife();
        WaveManager.Instance.OnEnemyRemoved();
        Destroy(gameObject);
    }
}
