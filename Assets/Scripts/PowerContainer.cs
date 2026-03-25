using UnityEngine;

public class PowerContainer : MonoBehaviour
{
    public bool hasPower { get; private set; } = false;
    public Power power { get; private set; }

    [SerializeField] SpriteRenderer powerSprite;

    public bool lockPower = false;

    public void SetPower(Power power)
    {
        hasPower = true;
        this.power = power;
        powerSprite.sprite = GameDataManager.instance.GetPowerSpritesData().GetSprite(power);
    }

    public void ResetPower()
    {
        hasPower = false;
        powerSprite.sprite = null;  
    }

    public void ResetSpriteOnly()
    {
        powerSprite.sprite = null;
    }
}
