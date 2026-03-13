
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBoxGenerator : MonoBehaviour
{
    // Singleton
    public static PowerBoxGenerator Instance;

    [Header("Prefab")]
    [SerializeField] private GameObject powerBoxPrefab;

    [Header("Config")]
    [SerializeField] private float generatorHeight = 3.0f;
    [SerializeField] private float generatorWidth = 3.0f;
    [SerializeField] private int maxBoxes = 2;
    [SerializeField] private float timeBetweenSpawns = 4.0f;
    [SerializeField] private float timeUntilFirstSpawn = 6.0f;


    // Variables
    private List<GameObject> boxList = new List<GameObject>();
    private List<Power> availablePowers = new List<Power>();


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        GenerateAvailableList();
        if (availablePowers.Count > 0) StartCoroutine(nameof(StartSpawning));
    }

    private void GenerateAvailableList()
    {
        Power[] allPowers = (Power[])System.Enum.GetValues(typeof(Power));

        foreach (Power power in allPowers)
        {
            if (PlayerPrefs.HasKey(GetPrefNameFromPower(power)))
            {
                if (PlayerPrefs.GetInt(GetPrefNameFromPower(power)) != 0)
                {
                    availablePowers.Add(power);
                }
            } else
            {
                PlayerPrefs.SetInt(GetPrefNameFromPower(power), 1);
            }
        }
    }

    private string GetPrefNameFromPower(Power power)
    {
        return power.ToString() + "Enabled";
    }

    private Power GetRandomPowerFromAvailable()
    {
        int index = Random.Range(0, availablePowers.Count);
        return availablePowers[index];
    }

    IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(timeUntilFirstSpawn);

        while (true) {

            if (boxList.Count < maxBoxes)
            {
                SpawnBox();
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void SpawnBox()
    {
        float posX = transform.position.x + Random.Range(-generatorWidth / 2, generatorWidth / 2);
        float posY = transform.position.y + Random.Range(-generatorHeight / 2, generatorHeight / 2);

        GameObject newBox = Instantiate(powerBoxPrefab, new Vector3(posX, posY, transform.position.z), Quaternion.identity);
        PowerBoxController boxController = newBox.GetComponent<PowerBoxController>();
        if (boxController != null)
        {
            boxController.container.SetPower(GetRandomPowerFromAvailable());
            boxController.SetBoxListReference(boxList);
        }
        boxList.Add(newBox);
    }
}
