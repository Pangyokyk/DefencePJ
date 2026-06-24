// WaypointPath.cs
// 몬스터가 따라갈 경로(웨이포인트 목록)를 보관하고 씬 뷰에 시각화하는 컴포넌트.
// 씬에 빈 GameObject를 만들고 이 스크립트를 붙인 뒤,
// Inspector에서 waypoints 배열에 경유 지점 Transform들을 순서대로 넣어준다.

using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    // [SerializeField] : private을 유지하면서 Inspector에서 값을 편집할 수 있게 해준다.
    // Inspector에 보이는 순서가 곧 몬스터가 이동하는 순서(0 → 1 → 2 → ...)다.
    [SerializeField] private Transform[] waypoints;

    /// <summary>
    /// 웨이포인트의 총 개수. EnemyMover가 마지막 지점 도달 여부를 판단하는 데 사용한다.
    /// </summary>
    public int Count => waypoints.Length;

    /// <summary>
    /// 인덱스에 해당하는 웨이포인트 Transform을 반환한다.
    /// </summary>
    /// <param name="index">0부터 시작하는 순서 번호</param>
    public Transform GetWaypoint(int index)
    {
        return waypoints[index];
    }

    // ─────────────────────────────────────────────
    // 에디터 전용: 씬 뷰에 경로를 시각적으로 그린다.
    // 빌드에는 포함되지 않으므로 성능 걱정 없이 사용해도 된다.
    // ─────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] == null || waypoints[i + 1] == null) continue;

            // 연속된 웨이포인트 사이에 선을 그려 경로를 표시한다.
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);

            // 각 경유 지점에 작은 구를 그린다.
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
        }

        // 마지막 웨이포인트(도착 지점)는 빨간 구로 강조한다.
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.3f);
    }
}
