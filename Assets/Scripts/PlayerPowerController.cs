using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PaddleInput))]
public class PlayerPowerController : MonoBehaviour
{
    [SerializeField] private AudioSource mainSource;

    [Header("Components")]
    [SerializeField] SpriteRenderer powerSprite;
    [SerializeField] PowerSpritesData powerSpritesData;
    private PaddleInput paddleInput;

    [Header("PortalConfig")]
    [SerializeField] private GameObject portal;
    [SerializeField] private float portalSpawnDistance = 1.0f;

    [Header("ShockConfig")]
    [SerializeField] private GameObject shockProjectile;
    [SerializeField] private AudioClip shootSound;

    [Header("DoubleConfig")]
    [SerializeField] private float doubleTime = 3.0f;
    [SerializeField] private AudioClip doubleSound;

    [Header("VFX")]
    [SerializeField] private GameObject inversionVFX;
    public bool hasPower { get; private set; } = false;
    public Power power { get; private set; }
    

    private void Awake()
    {
        paddleInput = GetComponent<PaddleInput>();
    }

    private void Start()
    {
        paddleInput.powerKeyPressed += UsePower;
    }

    private void OnDestroy()
    {
        paddleInput.powerKeyPressed -= UsePower;
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

    IEnumerator DoublePowerRoutine()
    {
        yield return new WaitForSeconds(doubleTime);
    }

    public void UsePower()
    {
        if (!hasPower) return;

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
                mainSource.PlayOneShot(shootSound);
            }
        }

        if (power == Power.Double)
        {
            GameManagerController.Instance.ApplyDoublePointsForSeconds(doubleTime);
            mainSource.PlayOneShot(doubleSound);
        }

        hasPower = false;
        powerSprite.sprite = null;

    }
}
