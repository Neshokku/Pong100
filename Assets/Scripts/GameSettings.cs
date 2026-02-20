using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance { get; private set; }

    public ControlType p1ControlType { get; private set; }
    public ControlType p2ControlType { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

        ImportSettings();
    }

    private void ImportSettings()
    {
        if (PlayerPrefs.HasKey("P1ControlType"))
        {
            p1ControlType = (ControlType)PlayerPrefs.GetInt("P1ControlType");
        } else
        {
            p1ControlType = ControlType.PowerCenter;
        }

        if (PlayerPrefs.HasKey("P2ControlType"))
        {
            p2ControlType = (ControlType)PlayerPrefs.GetInt("P2ControlType");
        }
        else
        {
            p2ControlType = ControlType.PowerCenter;
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("P1ControlType", (int)p1ControlType);
        PlayerPrefs.SetInt("P2ControlType", (int)p2ControlType);
    }

    public void SetP1ControlType(ControlType controlType)
    {
        p1ControlType = controlType;
        PlayerPrefs.SetInt("P1ControlType", (int)p1ControlType);
    }

    public void SetP2ControlType(ControlType controlType)
    {
        p2ControlType = controlType;
        PlayerPrefs.SetInt("P2ControlType", (int)p2ControlType);
    }
}
