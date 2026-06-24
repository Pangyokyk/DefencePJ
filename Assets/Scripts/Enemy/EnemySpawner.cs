// EnemySpawner.cs
// 일정 간격으로 몬스터 프리팹을 생성(스폰)하는 컴포넌트.
// 첫 번째 웨이포인트 위치에 몬스터를 소환하고 EnemyMover에 경로를 전달한다.
//
// 이 스크립트는 나중에 "4단계: 웨이브" 시스템으로 발전시킬 예정이다.
// 지금은 테스트 목적으로 spawnInterval마다 몬스터를 반복 생성한다.

using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;   // 소환할 몬스터 프리팹
    [SerializeField] private WaypointPath path;        // 몬스터가 따라갈 경로
    [SerializeField] private float spawnInterval = 2f; // 몬스터 생성 간격 (초)

    // Invoke는 메서드 이름 문자열로 지연 호출하므로 리팩터링 시 오타 위험이 있다.
    // 여기서는 InvokeRepeating 대신 코루틴이나 타이머 필드를 쓰는 방식을 선택했다.
    private float timer;

    private void Start()
    {
        // 게임 시작과 동시에 첫 번째 몬스터를 즉시 소환한다.
        SpawnEnemy();

        // timer를 0으로 초기화해 첫 인터벌 후 두 번째 몬스터가 소환되게 한다.
        timer = 0f;
    }

    private void Update()
    {
        // 누적 시간을 더하고, 인터벌이 지나면 소환 후 타이머를 초기화한다.
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        // 필수 참조가 없으면 오류 메시지를 남기고 중단한다.
        if (enemyPrefab == null || path == null)
        {
            Debug.LogError("[EnemySpawner] enemyPrefab 또는 path가 할당되지 않았습니다!");
            return;
        }

        // Instantiate: 프리팹을 씬에 복제해 새 오브젝트를 만든다.
        // 위치는 첫 번째 웨이포인트로 지정하고, 회전은 기본값(Quaternion.identity)을 사용한다.
        GameObject spawnedEnemy = Instantiate(
            enemyPrefab,
            path.GetWaypoint(0).position,
            Quaternion.identity
        );

        // 생성된 오브젝트에서 EnemyMover 컴포넌트를 가져와 경로를 전달한다.
        // GetComponent<T>(): 같은 오브젝트에 붙어 있는 T 타입 컴포넌트를 반환한다.
        EnemyMover mover = spawnedEnemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.Initialize(path);
        }
        else
        {
            Debug.LogError("[EnemySpawner] 프리팹에 EnemyMover 컴포넌트가 없습니다!");
        }
    }
}
