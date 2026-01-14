using UnityEngine;

public class PlayerPowerController : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private KeyCode keyPower = KeyCode.Space;

    [SerializeField] SpriteRenderer powerSprite;
    [SerializeField] PowerSpritesData powerSpritesData;

    public bool hasPower { get; private set; } = false;
    public Power power { get; private set; }

    void Update()
    {
        if (hasPower && Input.GetKeyDown(keyPower))
        {
            UsePower();
        }
    }

    public void SetPower(Power power)
    {
        hasPower = true;
        this.power = power;
        powerSprite.sprite = powerSpritesData.GetSprite(power);
    }

    public void UsePower()
    {
        hasPower = false;
        powerSprite.sprite = null;
    }
}
