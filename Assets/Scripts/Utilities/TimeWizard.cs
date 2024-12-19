using UnityEngine;

public class TimeWizard : MonoBehaviour
{
    public void StopTime() {
        Time.timeScale = 0;
    }

    public void UnStopTime() {
        Time.timeScale = 1;
    }

    public void SetTimeScale(float newTimeScale) {
        Time.timeScale = newTimeScale;
    }
}