using UnityEngine;

public class EnableScriptOnActivate : MonoBehaviour
{
    public MonoBehaviour targetScript; // 要啟用的目標腳本

    public AudioClip[] soundEffects;  // 音效陣列
    public AudioSource audioSource;

    private void OnEnable()
    {
        // 當物件啟用時，啟用目標腳本
        if (targetScript != null)
        {
            targetScript.enabled = true;

            // 隨機選擇音效並播放
            if (soundEffects.Length > 0 && audioSource != null)
            {
                AudioClip randomClip = soundEffects[Random.Range(0, soundEffects.Length)];
                audioSource.clip = randomClip;
                audioSource.Play();
            }
        }
    }

    private void OnDisable()
    {
        // 當物件停用時，停用目標腳本
        if (targetScript != null)
        {
            targetScript.enabled = false;
        }
    }
}
