// EnemyData.cs
// 적 타입 하나의 스탯을 담는 ScriptableObject.
// Project 창에서 우클릭 → Create → Defence → Enemy Data 로 생성한다.
// 타입마다 에셋을 하나씩 만들어 WaveManager Inspector에 연결하면 된다.

using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "DefencePJ/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName = "기본";   // 적 이름 (디버그·UI 표시용)

    [Header("스탯")]
    public int   maxHp      = 100;      // 최대 체력
    public float moveSpeed  = 3f;       // 이동 속도 (m/s)
    public int   goldReward = 10;       // 처치 시 지급 골드

    [Header("임시 색상 (3D 에셋 교체 전까지 타입 구분용)")]
    public Color bodyColor  = Color.red;
}
