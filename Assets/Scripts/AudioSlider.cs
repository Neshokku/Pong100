using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    private Slider slider;
    private bool initialized = false;


    [Header("Config")]
    [SerializeField] private string audioGroup;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI textMeshPro;


    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        slider.value = GetPercentageFromDecibels(PlayerPrefs.GetFloat(audioGroup)) / 100.0f;
        initialized = true;
        ChangeAudioGroupValue(slider.value);
    }

    public void ChangeAudioGroupValue(float value)
    {
        if (!initialized) return;

        float percentage = value * 100.0f;

        GameSettings.instance.SetAudioGroupVolume(audioGroup, GetDecibelsFromPercentage(percentage));

        textMeshPro.text = percentage.ToString("0") + "%";
    }

    private float GetPercentageFromDecibels(float decibels)
    {
        return 100f * Mathf.Pow(10.0f, decibels / 20.0f);
    }

    private float GetDecibelsFromPercentage(float percentage)
    {
        if (percentage <= 0f) return -80f;

        return 20f * Mathf.Log10(percentage / 100f);
    }
}
