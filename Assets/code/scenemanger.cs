using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizToDemo : MonoBehaviour
{
    [SerializeField] string fromScene = "quiz";        // 來源場景
    [SerializeField] string toScene = "DemoScene";   // 目標場景
    [SerializeField] KeyCode hotkey = KeyCode.B;     // 觸發鍵

    void Awake()
    {
        // 確認目標場景已加入 Build Settings
        if (!Application.CanStreamedLevelBeLoaded(toScene))
            Debug.LogError($"Scene '{toScene}' 不在 Build Settings 的 Scenes In Build 裡。");
    }

    void Update()
    {
        // 只在 quiz 場景生效，避免其他場景誤觸
        if (SceneManager.GetActiveScene().name != fromScene) return;

        if (Input.GetKeyDown(hotkey))
            LoadDemo();
    }

    // 給 UI Button 綁定用（OnClick -> QuizToDemo.LoadDemo）
    public void LoadDemo()
    {
        SceneManager.LoadScene(toScene);
        // 若想用索引（你現在 DemoScene 是 0），可改為：
        // SceneManager.LoadScene(0);
    }
}
