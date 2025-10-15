using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScreen : MonoBehaviour
{
    public bool fadeOnStart = true;
    public float fadeDuration = 2f;
    public Color fadeColor = Color.white;

    private Renderer rend;
    private Material mat;
    private string colorProp = "_Color";
    private Coroutine running;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material; // 實例化，避免影響共用材質

        // 避免把場景投成陰影/變暗
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rend.receiveShadows = false;

        // 自動偵測顏色屬性
        if (mat.HasProperty("_BaseColor")) colorProp = "_BaseColor";
        else if (mat.HasProperty("_Color")) colorProp = "_Color";

        // 對齊 RGB（alpha 交給程式）
        var c = mat.GetColor(colorProp);
        c.r = fadeColor.r; c.g = fadeColor.g; c.b = fadeColor.b;
        mat.SetColor(colorProp, c);
    }

    void OnEnable()
    {
        // 新場景載入完成後自動淡入，避免切場景時卡在白色
        SceneManager.sceneLoaded += OnSceneLoadedFadeIn;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedFadeIn;
        if (running != null) StopCoroutine(running);
        running = null;
    }

    void Start()
    {
        if (fadeOnStart) FadeIn();
    }

    private void OnSceneLoadedFadeIn(Scene s, LoadSceneMode m)
    {
        FadeIn();
    }

    public void FadeIn() { Fade(1f, 0f); }
    public void FadeOut() { Fade(0f, 1f); }

    public void Fade(float alphaIn, float alphaOut)
    {
        if (running != null) StopCoroutine(running); // 防重入
        running = StartCoroutine(FadeRoutine(alphaIn, alphaOut));
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut)
    {
        float norm = 0f;
        Color c = mat.GetColor(colorProp);
        c.r = fadeColor.r; c.g = fadeColor.g; c.b = fadeColor.b;

        // 起點
        c.a = alphaIn;
        mat.SetColor(colorProp, c);

        while (norm < 1f)
        {
            // 只在此處使用不受 timeScale 影響的時間，並未改變全局時間軸
            float step = (fadeDuration > 0f) ? (Time.unscaledDeltaTime / fadeDuration) : 1f;
            norm += step;
            c.a = Mathf.Lerp(alphaIn, alphaOut, Mathf.Clamp01(norm));
            mat.SetColor(colorProp, c);
            yield return null;
        }

        // 壓到終點
        c.a = alphaOut;
        mat.SetColor(colorProp, c);
        running = null;
    }
}
