// WaveManager.cs
// 웨이브(몬스터 등장 단위)를 순서대로 진행하는 매니저.
//
// 동작 흐름:
//   웨이브 시작 → 지정한 적 타입(EnemyData)을 spawnInterval 간격으로 enemyCount마리 생성
//   → 모든 몬스터 제거 대기 → timeBetweenWaves 초 후 다음 웨이브
//   → 모든 웨이브 완료 시 스테이지 클리어

using System.Collections;
using UnityEngine;

// ─────────────────────────────────────────────
// WaveData: 웨이브 한 개의 설정.
// Inspector에서 배열 원소로 편집할 수 있도록 [System.Serializable]을 붙인다.
// ─────────────────────────────────────────────
[System.Serializable]
public class WaveData
{
    public EnemyData enemyType;              // 이 웨이브에서 등장할 적 타입 (ScriptableObject)
    public int       enemyCount    = 5;      // 등장할 몬스터 수
    public float     spawnInterval = 1.5f;   // 몬스터 사이 등장 간격 (초)
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("웨이브 설정")]
    [SerializeField] private WaveData[] waves;
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("스폰 설정")]
    [SerializeField] private GameObject enemyPrefab; // 모든 타입 공용 프리팹
    [SerializeField] private WaypointPath path;

    private int enemiesAlive = 0;
    public int CurrentWave { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); return; 
        }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(RunAllWaves());
    }

    private IEnumerator RunAllWaves()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            CurrentWave = i + 1;
            Debug.Log($"[WaveManager] ▶ 웨이브 {CurrentWave} / {waves.Length} 시작!");

            yield return StartCoroutine(SpawnWave(waves[i]));

            // 스폰이 끝난 뒤 살아있는 몬스터가 모두 제거될 때까지 대기
            yield return new WaitUntil(() => enemiesAlive <= 0);

            if (i < waves.Length - 1)
            {
                Debug.Log($"[WaveManager] ✔ 웨이브 {CurrentWave} 클리어! {timeBetweenWaves}초 후 다음 웨이브");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        Debug.Log("[WaveManager] ★ 모든 웨이브 클리어! 스테이지 성공!");
        GameResultManager.Instance.ShowStageClear();
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave.enemyType);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private void SpawnEnemy(EnemyData data)
    {
        if (enemyPrefab == null || path == null || data == null)
        {
            Debug.LogError("[WaveManager] Enemy Prefab, Path, 또는 EnemyData가 할당되지 않았습니다!");
            return;
        }

        GameObject obj = Instantiate(enemyPrefab, path.GetWaypoint(0).position, Quaternion.identity);

        // EnemyData를 각 컴포넌트에 전달해 스탯을 적용한다.
        obj.GetComponent<EnemyHealth>()?.Initialize(data);
        obj.GetComponent<EnemyMover>()?.Initialize(path, data);

        enemiesAlive++;
    }

    public void OnEnemyRemoved()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }
}
