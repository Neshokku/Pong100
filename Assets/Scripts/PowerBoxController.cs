using System.Collections.Generic;
using UnityEngine;

public class PowerBoxController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Power power;

    [Header("Components")]
    [SerializeField] SpriteRenderer powerSpriteRenderer;

    [Header("Data")]
    [SerializeField] PowerSpritesData powerSpriteData;


    private List<GameObject> boxListReference;

    void Start()
    {
        SetSpriteToPower();
    }

    private void SetSpriteToPower()
    {
        powerSpriteRenderer.sprite = powerSpriteData.GetSprite(power);
    }

    public void SetRandomPower()
    {
        Power[] values = (Power[])System.Enum.GetValues(typeof(Power));
        power = values[Random.Range(0, values.Length)];

        SetSpriteToPower();
    }

    public void SetBoxListReference(List<GameObject> list)
    {
        boxListReference = list;
    }

    public void Dissapear()
    {
        if (boxListReference != null) boxListReference.Remove(gameObject);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<BallController>(out BallController ballController))
        {
            PlayerPowerController playerPowerController = ballController.lastPlayerHit.GetComponent<PlayerPowerController>();
            if (playerPowerController != null && !playerPowerController.hasPower)
            {
                playerPowerController.SetPower(power);
            }
            Dissapear();
        }
    }
}
