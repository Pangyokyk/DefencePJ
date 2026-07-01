// BarUI.cs
// HP바·마나바 등 채움(fill) 방식의 상태바를 표시하는 재사용 컴포넌트.
// World Space Canvas 아래에 배치하고, SetFill()로 값을 갱신한다.
//
// [World Space Canvas란?]
//   일반 Canvas는 화면(Screen)에 고정되지만,
//   World Space Canvas는 3D 공간에 오브젝트처럼 배치된다.
//   캐릭터 머리 위 HP바처럼 게임 오브젝트에 붙여 쓸 때 적합하다.

using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage; // 채워지는 이미지 (Image Type = Filled 로 설정)

    // LateUpdate: 모든 Update가 끝난 뒤 호출된다.
    // 카메라 이동이 Update에서 일어나므로, 바가 카메라를 향하는 처리는 LateUpdate에서 해야 한다.
    private void LateUpdate()
    {
        // 항상 메인 카메라를 향하도록 회전 (Billboard 효과)
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;
    }

    /// <summary>
    /// 바의 채움 비율을 갱신한다.
    /// EnemyHealth, Unit 등에서 값이 바뀔 때마다 호출한다.
    /// </summary>
    public void SetFill(float current, float max)
    {
        if (fillImage == null) return;
        // Mathf.Clamp01: 0~1 사이로 제한한다. HP가 음수가 되는 경우를 방지한다.
        fillImage.fillAmount = Mathf.Clamp01(current / max);
    }
}
