using UnityEngine;
using UnityEngine.Rendering;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance { get; private set; }

    [SerializeField] private PowerSpritesData powerSpritesData;

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
    }

    public PowerSpritesData GetPowerSpritesData()
    {
        return powerSpritesData;
    }
}
