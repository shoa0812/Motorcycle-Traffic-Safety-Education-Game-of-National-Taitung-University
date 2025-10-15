using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizToDemo : MonoBehaviour
{
    [SerializeField] string fromScene = "quiz";        // �ӷ�����
    [SerializeField] string toScene = "DemoScene";   // �ؼг���
    [SerializeField] KeyCode hotkey = KeyCode.B;     // Ĳ�o��

    void Awake()
    {
        // �T�{�ؼг����w�[�J Build Settings
        if (!Application.CanStreamedLevelBeLoaded(toScene))
            Debug.LogError($"Scene '{toScene}' ���b Build Settings �� Scenes In Build �̡C");
    }

    void Update()
    {
        // �u�b quiz �����ͮġA�קK��L�����~Ĳ
        if (SceneManager.GetActiveScene().name != fromScene) return;

        if (Input.GetKeyDown(hotkey))
            LoadDemo();
    }

    // �� UI Button �j�w�Ρ]OnClick -> QuizToDemo.LoadDemo�^
    public void LoadDemo()
    {
        SceneManager.LoadScene(toScene);
        // �Y�Q�ί��ޡ]�A�{�b DemoScene �O 0�^�A�i�אּ�G
        // SceneManager.LoadScene(0);
    }
}
