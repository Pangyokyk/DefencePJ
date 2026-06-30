// StageSelectManager.cs
// 스테이지 선택 씬(StageSelect)의 버튼 이벤트를 처리한다.
//
// [사용 방법]
//   - StageSelect 씬에 빈 GameObject를 만들고 이 컴포넌트를 붙인다.
//   - 각 스테이지 버튼의 OnClick → OnStage1Clicked() 를 연결한다.

using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [Header("씬 이름")]
    // 전투 씬의 이름. Build Settings에 등록한 씬 이름과 정확히 일치해야 한다.
    [SerializeField] private string battleSceneName = "SampleScene";

    // ─────────────────────────────────────────────
    // 스테이지 1 버튼 OnClick 에 연결한다.
    // 나중에 스테이지가 늘어나면 씬 이름 파라미터만 바꿔서 복사하면 된다.
    // ─────────────────────────────────────────────
    public void OnStage1Clicked()
    {
        SceneManager.LoadScene(battleSceneName);
    }
}
