using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PowerToggleButton : MonoBehaviour
{
    private bool powerEnabled = true;

    [Header("References")]
    [SerializeField] Image powerImage;

    [Header("Config")]
    [SerializeField] Power power;

    private void Awake()
    {
        ImportSettings();
    }

    private void ImportSettings()
    {
        if (PlayerPrefs.HasKey(GetPrefName()))
        {
            SetEnabled(PlayerPrefs.GetInt(GetPrefName()) != 0); 
        } 
        else
        {
            SetEnabled(true);
        }
    }

    private string GetPrefName()
    {
        return power.ToString() + "Enabled";
    }

    private void SetEnabled(bool enabled)
    {
        powerEnabled = enabled;
        PlayerPrefs.SetInt(GetPrefName(), powerEnabled ? 1 : 0);
        powerImage.color = powerEnabled ? Color.white : Color.gray3;
    }

    public void Toggle()
    {
        SetEnabled(!powerEnabled);
    }
}
