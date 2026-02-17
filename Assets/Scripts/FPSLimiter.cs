using UnityEngine;

public class FPSLimiter
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void SetFPS()
    {
        Debug.Log("Limiting FPS to 60");
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
}
