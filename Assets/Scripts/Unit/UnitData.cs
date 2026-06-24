// UnitData.cs
// 유닛 한 종류의 데이터를 담는 ScriptableObject.
//
// [ScriptableObject란?]
//   MonoBehaviour처럼 씬 오브젝트에 붙이는 게 아니라,
//   Project 창에 에셋(.asset)으로 저장되는 데이터 컨테이너다.
//   여러 씬·프리팹이 동일한 데이터 에셋을 참조하므로 데이터 중복을 없앨 수 있다.
//
// [CreateAssetMenu]: Project 창 우클릭 메뉴에 "DefencePJ/Unit Data" 항목을 추가한다.
//   이 메뉴로 UnitData 에셋을 원하는 만큼 만들 수 있다.

using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "DefencePJ/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("기본 정보")]
    public string     unitName    = "유닛";
    public GameObject prefab;           // 타일에 배치할 때 생성되는 프리팹

    [Header("스탯")]
    public int   attack      = 10;
    public float attackRange = 3f;
    public float attackSpeed = 1f;

    [Header("뽑기 설정")]
    // weight가 높을수록 더 자주 뽑힌다.
    // 예) A=70, B=30 → A가 70%, B가 30% 확률
    [Range(1, 100)]
    public int weight = 50;
}
