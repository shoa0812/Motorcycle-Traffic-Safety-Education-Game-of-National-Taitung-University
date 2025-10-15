//test1
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public AudioClip[] audioClips; //設定button音效
    public AudioSource audioSource;  //設定AudioSource

    // Reference to the fade screen component
    public FadeScreen fadeScreen;

    // Function to transition to the specified scene
    public void GoToScene(int sceneIndex)
    {
        // Start the scene transition coroutine
        StartCoroutine(GoToSceneRoutine(sceneIndex));
    }

    // Coroutine for scene transition
    IEnumerator GoToSceneRoutine(int sceneIndex)
    {
        // If there's a fade screen component attached
        
        if (fadeScreen != null)
        {
            // Perform fade out animation
            //fadeScreen.FadeOut();
            // Wait for the fade duration
            yield return new WaitForSeconds(fadeScreen.fadeDuration);
        }
        // Load the new scene
        SceneManager.LoadScene(sceneIndex);
        DynamicGI.UpdateEnvironment(); // 更新全局光照
    }


    public void GoToSceneAsync(int sceneIndex)
    {
        audioSource.PlayOneShot(audioClips[0]);
        // Start the scene transition coroutine
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    }

    // Coroutine for scene transition
    IEnumerator GoToSceneAsyncRoutine(int sceneIndex)
    {   
        fadeScreen.FadeOut();
        // Load the new scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float timer = 0;
        while(timer <= fadeScreen.fadeDuration && !operation.isDone){
          timer += Time.deltaTime;
          yield return null;  
        }

        operation.allowSceneActivation = true;
    }
}


