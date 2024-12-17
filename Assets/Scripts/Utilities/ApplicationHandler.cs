using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationHandler : MonoBehaviour
{
    private static ApplicationHandler Instance;

    private void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void ChangeScene(string sceneName) {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }

    public static void ChangeSceneDelayed(string delayScene) {
        string[] split = delayScene.Split(":");
        float delay = float.Parse(split[0]);
        ChangeSceneDelayed(split[1], delay);
    }

    public static void ChangeSceneDelayed(string sceneName, float delay) {
        Instance.StartCoroutine(DelayedSceneChange(sceneName, delay));
    }

    private static IEnumerator DelayedSceneChange(string sceneName, float delay) {
        yield return new WaitForSecondsRealtime(delay);
        ChangeScene(sceneName);
    }

    public static void QuitGame() {
        Application.Quit();
    }
}