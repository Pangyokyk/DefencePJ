// LevelManager.cs
// 게임 상태(목숨 수, 게임오버)를 관리하는 싱글턴 컴포넌트.
//
// [싱글턴 패턴이란?]
//   씬에 딱 하나만 존재하는 매니저 오브젝트를 전역 정적 변수 Instance로 노출해,
//   어떤 스크립트에서든 LevelManager.Instance.메서드() 형태로 접근하게 하는 디자인 패턴이다.

using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // static: 인스턴스가 아닌 클래스 자체에 속하는 변수.
    //         씬 전체에서 하나의 값만 유지하므로 싱글턴 전역 참조에 적합하다.
    // { get; private set; }: 외부에서 읽기는 가능하지만 대입은 이 클래스 안에서만 허용.
    public static LevelManager Instance { get; private set; }

    [SerializeField] private int startingLives = 3; // Inspector에서 시작 목숨 수를 조정할 수 있다.

    // 현재 남은 목숨. 외부(UI 등)에서 읽을 수 있게 public 프로퍼티로 노출한다.
    public int CurrentLives { get; private set; }

    // ─────────────────────────────────────────────
    // Awake: 오브젝트가 활성화될 때 가장 먼저 호출된다 (Start보다 이전).
    //        싱글턴 초기화는 다른 스크립트의 Start보다 먼저 완료돼야 하므로 Awake를 사용한다.
    // ─────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // 이미 LevelManager가 존재하는데 새 오브젝트가 생성되면 새 것을 파괴한다.
            // 씬 전환 등으로 중복 생성되는 상황을 방지한다.
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentLives = startingLives;
    }

    /// <summary>
    /// 몬스터가 도착 지점에 닿았을 때 EnemyMover에서 호출한다.
    /// 목숨을 1 감소시키고 0 이하가 되면 게임오버를 처리한다.
    /// </summary>
    public void LoseLife()
    {
        CurrentLives--;
        Debug.Log($"[LevelManager] 목숨 감소! 남은 목숨: {CurrentLives}");

        if (CurrentLives <= 0)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("[LevelManager] 게임 오버!");
        // TODO (2단계 이후): 게임 오버 UI 표시, 씬 재로드 등 구현 예정.
        // Time.timeScale = 0f;  ← 게임을 완전히 멈추려면 이 줄을 활성화한다.
    }
}
