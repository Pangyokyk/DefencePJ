// WaveManager.cs
// 웨이브(몬스터 등장 단위)를 순서대로 진행하는 매니저.
// 기존 EnemySpawner를 대체한다 — 씬의 EnemySpawner 오브젝트는 삭제해도 된다.
//
// 동작 흐름:
//   웨이브 시작 → 몬스터를 spawnInterval 간격으로 enemyCount마리 생성
//   → 모든 몬스터가 처치되거나 도착 지점에 닿아 제거될 때까지 대기
//   → timeBetweenWaves 초 후 다음 웨이브 시작
//   → 모든 웨이브 완료 시 스테이지 클리어

using System.Collections;
using UnityEngine;

// ─────────────────────────────────────────────
// WaveData: 웨이브 한 개의 설정을 담는 데이터 클래스.
// [System.Serializable]: Inspector에서 배열 원소로 편집할 수 있게 해준다.
// ─────────────────────────────────────────────
[System.Serializable]
public class WaveData
{
    public int   enemyCount    = 5;    // 이 웨이브에서 등장할 몬스터 수
    public float spawnInterval = 1.5f; // 몬스터 한 마리씩 등장하는 간격 (초)
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("웨이브 설정")]
    // Inspector에서 웨이브를 원하는 만큼 추가하고 각 설정을 편집한다.
    [SerializeField] private WaveData[] waves;
    [SerializeField] private float timeBetweenWaves = 5f; // 웨이브 사이 대기 시간 (초)

    [Header("스폰 설정")]
    [SerializeField] private GameObject enemyPrefab;  // 생성할 몬스터 프리팹
    [SerializeField] private WaypointPath path;       // 몬스터가 따라갈 경로

    // 현재 씬에 살아있는(아직 제거되지 않은) 몬스터 수.
    // EnemyHealth.Die()와 EnemyMover.OnReachEnd() 양쪽에서 차감한다.
    private int enemiesAlive = 0;

    // 현재 진행 중인 웨이브 번호 (0부터 시작, 로그 표시용으로 +1)
    public int CurrentWave { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // 코루틴(Coroutine): 여러 프레임에 걸쳐 실행되는 비동기 함수.
        // yield return으로 중간에 일시 정지했다가 조건이 충족되면 재개한다.
        StartCoroutine(RunAllWaves());
    }

    // ─────────────────────────────────────────────
    // 모든 웨이브를 순서대로 실행하는 메인 코루틴
    // ─────────────────────────────────────────────
    private IEnumerator RunAllWaves()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            CurrentWave = i + 1;
            Debug.Log($"[WaveManager] ▶ 웨이브 {CurrentWave} / {waves.Length} 시작!");

            // 이번 웨이브의 몬스터를 모두 스폰할 때까지 대기
            yield return StartCoroutine(SpawnWave(waves[i]));

            // 스폰이 끝나도 몬스터가 살아있을 수 있으므로 모두 제거될 때까지 대기.
            // WaitUntil: 람다(조건 함수)가 true를 반환할 때까지 매 프레임 검사하며 대기한다.
            yield return new WaitUntil(() => enemiesAlive <= 0);

            if (i < waves.Length - 1)
            {
                Debug.Log($"[WaveManager] ✔ 웨이브 {CurrentWave} 클리어! {timeBetweenWaves}초 후 다음 웨이브");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        Debug.Log("[WaveManager] ★ 모든 웨이브 클리어! 스테이지 성공!");
        // TODO: 스테이지 클리어 UI 표시
    }

    // ─────────────────────────────────────────────
    // 한 웨이브 안의 몬스터를 간격을 두고 순서대로 스폰하는 코루틴
    // ─────────────────────────────────────────────
    private IEnumerator SpawnWave(WaveData wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy();
            // WaitForSeconds: 지정한 초 동안 코루틴 실행을 중단한다.
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || path == null)
        {
            Debug.LogError("[WaveManager] Enemy Prefab 또는 Path가 할당되지 않았습니다!");
            return;
        }

        GameObject obj = Instantiate(enemyPrefab, path.GetWaypoint(0).position, Quaternion.identity);
        obj.GetComponent<EnemyMover>()?.Initialize(path);

        // 스폰할 때마다 카운터를 1 증가 — 나중에 OnEnemyRemoved()에서 차감한다.
        enemiesAlive++;
    }

    /// <summary>
    /// 몬스터가 처치되거나 도착 지점에 도달해 씬에서 제거될 때
    /// EnemyHealth와 EnemyMover에서 호출한다.
    /// </summary>
    public void OnEnemyRemoved()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0; // 안전 장치
    }
}
