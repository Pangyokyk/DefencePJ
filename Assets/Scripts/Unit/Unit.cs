// Unit.cs
// 배치된 유닛의 스탯과 자동 공격 로직을 담당하는 컴포넌트.
//
// 공격 흐름:
//   1. 매 프레임 attackTimer를 누적한다.
//   2. 1/attackSpeed 초가 지나면 사거리 내 몬스터를 탐색한다.
//   3. 가장 가까운 몬스터의 EnemyHealth.TakeDamage()를 호출한다.

using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("기본 스탯")]
    [SerializeField] private string unitName    = "기본 유닛";
    [SerializeField] private int    attack      = 10;  // 공격력
    [SerializeField] private float  attackRange = 3f;  // 사거리 (단위: m)
    [SerializeField] private float  attackSpeed = 1f;  // 공격 횟수/초

    public string UnitName    => unitName;
    public int    Attack      => attack;
    public float  AttackRange => attackRange;
    public float  AttackSpeed => attackSpeed;

    // 다음 공격까지 남은 시간을 추적하는 타이머
    private float attackTimer;

    /// <summary>
    /// GachaSystem이 뽑은 UnitData로 스탯을 덮어쓴다.
    /// MapTile이 유닛을 생성한 직후 호출한다.
    /// </summary>
    public void Initialize(UnitData data)
    {
        unitName    = data.unitName;
        attack      = data.attack;
        attackRange = data.attackRange;
        attackSpeed = data.attackSpeed;
    }

    private void Start()
    {
        // 배치 즉시 공격할 수 있도록 타이머를 쿨타임 값으로 초기화한다.
        attackTimer = 1f / attackSpeed;
        Debug.Log($"[Unit] '{unitName}' 배치 — 공격력:{attack}, 사거리:{attackRange}, 공격속도:{attackSpeed}/s");
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        // 쿨타임(1/attackSpeed 초)이 지나면 공격 시도
        if (attackTimer >= 1f / attackSpeed)
        {
            attackTimer = 0f;
            TryAttack();
        }
    }

    private void TryAttack()
    {
        EnemyHealth target = FindNearestEnemy();
        if (target == null) return;

        target.TakeDamage(attack);

        // 씬 뷰에서 공격 방향을 선으로 확인할 수 있다 (빌드에는 포함되지 않음).
        Debug.DrawLine(transform.position, target.transform.position, Color.red, 0.1f);
    }

    // ─────────────────────────────────────────────
    // 사거리 내 가장 가까운 몬스터를 탐색한다.
    // Physics.OverlapSphere: 지정한 구 범위 안의 모든 Collider를 배열로 반환한다.
    // ─────────────────────────────────────────────
    private EnemyHealth FindNearestEnemy()
    {
        // attackRange 반경 내의 모든 Collider를 가져온다.
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        EnemyHealth nearest     = null;
        float       nearestDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            // "Enemy" 태그가 붙은 오브젝트만 처리한다.
            // (Unity 에디터에서 Enemy 프리팹에 "Enemy" 태그를 반드시 지정해야 한다.)
            if (!hit.CompareTag("Enemy")) continue;

            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest     = enemy;
            }
        }

        return nearest;
    }

    // ─────────────────────────────────────────────
    // 씬 뷰에서 사거리를 흰 원으로 시각화한다.
    // ─────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        // OnDrawGizmosSelected: 이 오브젝트가 씬 뷰에서 선택됐을 때만 그린다.
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
