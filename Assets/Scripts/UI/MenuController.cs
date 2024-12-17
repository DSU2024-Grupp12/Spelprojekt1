using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void LoadScene(string sceneName) {
        ApplicationHandler.ChangeScene(sceneName);
    }

    public void QuitGame() {
        ApplicationHandler.QuitGame();
    }
}