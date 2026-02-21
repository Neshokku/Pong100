using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance { get; private set; }

    public ControlType p1ControlType { get; private set; }
    public ControlType p2ControlType { get; private set; }

    [Header("References")]
    [SerializeField] AudioMixer audioMixer;

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
            PlayerPrefs.SetInt("P1ControlType", (int)p1ControlType);
        }

        if (PlayerPrefs.HasKey("P2ControlType"))
        {
            p2ControlType = (ControlType)PlayerPrefs.GetInt("P2ControlType");
        } else
        {
            p2ControlType = ControlType.PowerCenter;
            PlayerPrefs.SetInt("P2ControlType", (int)p2ControlType);
        }

        if (PlayerPrefs.HasKey("SFXVol"))
        {
            float importedVolume = PlayerPrefs.GetFloat("SFXVol");
            Debug.Log("Imported SFX volume: " + importedVolume);
            audioMixer.SetFloat("SFXVol", importedVolume);
        }
        else
        {
            audioMixer.SetFloat("SFXVol", 0f);
            PlayerPrefs.SetFloat("SFXVol", 0f);
        }

        if (PlayerPrefs.HasKey("BGMVol"))
        {
            float importedVolume = PlayerPrefs.GetFloat("BGMVol");
            Debug.Log("Imported BGM volume: " + importedVolume);
            audioMixer.SetFloat("BGMVol", importedVolume);
        }
        else
        {
            audioMixer.SetFloat("BGMVol", 0f);
            PlayerPrefs.SetFloat("BGMVol", 0f);
        }

    }

    public void SetControlTypeForPlayer(PlayerID playerId, ControlType controlType)
    {
        if (playerId == PlayerID.Player1)
        {
            SetP1ControlType(controlType);
        } else if (playerId == PlayerID.Player2)
        {
            SetP2ControlType(controlType);
        }
    }

    public ControlType GetPlayerControlType(PlayerID playerID)
    {
        if (playerID == PlayerID.Player1) return p1ControlType;
        if (playerID == PlayerID.Player2) return p2ControlType;
        return ControlType.PowerCenter;
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


    public float GetAudioGroupVolume(string audioGroup)
    {
        if (audioMixer.GetFloat(audioGroup, out float volume))
        {
            Debug.Log("AudioGroup " + audioGroup + " found, volume: " + volume);
            return volume;
        }
        else
        {
            Debug.Log("AudioGroup " + audioGroup + " not found");
            return 0f;
        }
        
    }

    public void SetAudioGroupVolume(string audioGroup, float decibels)
    {
        audioMixer.SetFloat(audioGroup, decibels);
        PlayerPrefs.SetFloat(audioGroup, decibels);

        Debug.Log("Audio Group " + audioGroup + " set in PlayerPrefs as " + decibels);

        PlayerPrefs.Save();
    }
}
