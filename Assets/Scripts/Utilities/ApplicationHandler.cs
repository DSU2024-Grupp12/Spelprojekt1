using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationHandler : MonoBehaviour
{
    public void ChangeScene(string sceneName) {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }

    public void ChangeSceneDelayed(string delayScene) {
        string[] split = delayScene.Split(":");
        float delay = float.Parse(split[0]);
        StartCoroutine(DelayedSceneChange(delay, split[1]));
    }

    private IEnumerator DelayedSceneChange(float delay, string sceneName) {
        yield return new WaitForSeconds(delay);
        ChangeScene(sceneName);
    }

    public void QuitGame() {
        Application.Quit();
    }
}