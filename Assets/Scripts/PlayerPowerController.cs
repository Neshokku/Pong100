using UnityEngine;

public class PlayerPowerController : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private KeyCode keyPower = KeyCode.Space;

    [Header("Components")]
    [SerializeField] SpriteRenderer powerSprite;
    [SerializeField] PowerSpritesData powerSpritesData;

    [Header("PortalConfig")]
    [SerializeField] private GameObject portal;
    [SerializeField] private float portalSpawnDistance = 1.0f;

    [Header("ShockConfig")]
    [SerializeField] private GameObject shockProjectile;

    [Header("VFX")]
    [SerializeField] private GameObject inversionVFX;
    public bool hasPower { get; private set; } = false;
    public Power power { get; private set; }

    void Update()
    {
        if (hasPower && Input.GetKeyDown(keyPower))
        {
            UsePower();
        }
    }

    private bool IsOnLeftSide()
    {
        return transform.position.x < 0;
    }

    public void SetPower(Power power)
    {
        hasPower = true;
        this.power = power;
        powerSprite.sprite = powerSpritesData.GetSprite(power);
    }

    public void UsePower()
    {
        if (power == Power.Inversion)
        {
            BallController ballController = FindFirstObjectByType<BallController>();

            if (ballController != null)
            {
                Instantiate(inversionVFX, ballController.gameObject.transform.position, Quaternion.identity);
                ballController.SetDirection(new Vector2(ballController.direction.x, -(ballController.direction.y)));
            }
        }

        if (power == Power.Portal)
        {
            Instantiate(portal, new Vector2(transform.position.x + (IsOnLeftSide() ? portalSpawnDistance : -portalSpawnDistance), transform.position.y), Quaternion.identity);
        }

        if (power == Power.Shock)
        {
            GameObject newProjectile = Instantiate(shockProjectile, (Vector2)transform.position + (IsOnLeftSide() ? Vector2.right : Vector2.left), Quaternion.identity);
            
            if (newProjectile != null && newProjectile.TryGetComponent<ShockProjectileController>(out ShockProjectileController projController))
            {
                projController.direction = IsOnLeftSide() ? Vector2.right : Vector2.left;
            }
        }

        hasPower = false;
        powerSprite.sprite = null;

    }
}
