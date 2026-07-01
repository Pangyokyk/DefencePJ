// WeaponEquipRow.cs
// 무기 장착 패널에서 유닛 한 행을 담당하는 컴포넌트.
// Inspector에서 유닛 데이터와 UI 요소를 연결한다.
// 폰트·크기·위치는 Unity 에디터에서 자유롭게 조정할 수 있다.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEquipRow : MonoBehaviour
{
    [Header("이 행이 나타낼 유닛")]
    public UnitData unitData;

    [Header("UI 연결")]
    public TMP_Text infoText;     // 유닛 이름 + 현재 장착 무기 표시
    public Button   changeButton; // "장착 변경" 버튼
}
