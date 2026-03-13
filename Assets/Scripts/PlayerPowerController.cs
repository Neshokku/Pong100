using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PaddleInput))]
[RequireComponent(typeof(PowerContainer))]
public class PlayerPowerController : MonoBehaviour
{
    public PowerContainer powerContainer { get; private set; }

    [SerializeField] private PlayerID playerID;

    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioClip failPowerSound;

    [Header("Components")]
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

    [Header("InvisibilityConfig")]
    [SerializeField] private float invisibilityTime = 1.0f;
    [SerializeField] private AudioClip invisibilitySound;

    [Header("TelekinesisConfig")]
    [SerializeField] private AudioClip telekinesisSound;

    [Header("SoulConfig")]
    [SerializeField] private AudioClip soulSound;


    [Header("VFX")]
    [SerializeField] private GameObject inversionVFX;
    [SerializeField] private GameObject invisibilityVFX;
    [SerializeField] private GameObject telekinesisVFX;
    [SerializeField] private GameObject telekinesisSignalVFX;
    [SerializeField] private GameObject movingSoulVFX;
    [SerializeField] private GameObject soulBoxVFX;

    public bool soulDeployed = false;


    private void Awake()
    {
        paddleInput = GetComponent<PaddleInput>();
        powerContainer = GetComponent<PowerContainer>();
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


    IEnumerator DoublePowerRoutine()
    {
        yield return new WaitForSeconds(doubleTime);
    }

    private void OnSoulCollision(Power power, Vector3 position)
    {
        StartCoroutine(OnSoulCollisionRoutine(power, position));
    }

    private IEnumerator OnSoulCollisionRoutine(Power power, Vector3 position)
    {
        GameObject newSoulBox = Instantiate(soulBoxVFX, position, Quaternion.identity);
        PowerContainer soulBoxContainer = newSoulBox.GetComponent<PowerContainer>();
        soulBoxContainer.SetPower(power);

        Vector3 startPos = position;
        Vector3 endPos = transform.position;

        AnimationCurve easeCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        float duration = 1.0f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            endPos = transform.position;
            newSoulBox.transform.position = Vector3.Lerp(startPos, endPos, easeCurve.Evaluate(t / duration));
            yield return null;
        }

        Destroy(newSoulBox);
        powerContainer.SetPower(power);
        soulDeployed = false;

    }

    public void UsePower()
    {
        if (!powerContainer.hasPower) return;

        if (soulDeployed) return;
        if (powerContainer.lockPower) return;

        bool cancelPowerLoss = false;
        bool resetPower = true;

        if (powerContainer.power == Power.Inversion)
        {
            BallController ballController = FindFirstObjectByType<BallController>();

            if (ballController == null || ballController.isOutside)
            {
                cancelPowerLoss = true;
            }
            else
            {
                Instantiate(inversionVFX, ballController.gameObject.transform.position, Quaternion.identity);
                ballController.SetDirection(new Vector2(ballController.direction.x, -(ballController.direction.y)));
            }
        }

        if (powerContainer.power == Power.Portal)
        {
            Instantiate(portal, new Vector2(transform.position.x + (IsOnLeftSide() ? portalSpawnDistance : -portalSpawnDistance), transform.position.y), Quaternion.identity);
        }

        if (powerContainer.power == Power.Shock)
        {
            GameObject newProjectile = Instantiate(shockProjectile, (Vector2)transform.position + (IsOnLeftSide() ? Vector2.right : Vector2.left), Quaternion.identity);

            if (newProjectile != null && newProjectile.TryGetComponent<ShockProjectileController>(out ShockProjectileController projController))
            {
                projController.direction = IsOnLeftSide() ? Vector2.right : Vector2.left;
                mainSource.PlayOneShot(shootSound);
            }
        }

        if (powerContainer.power == Power.Double)
        {
            GameManagerController gm = FindFirstObjectByType<GameManagerController>();
            BallController ballController = FindFirstObjectByType<BallController>();

            if (ballController == null || ballController.isOutside)
            {
                cancelPowerLoss = true;
            }
            else
            {
                mainSource.PlayOneShot(doubleSound);
                gm?.ApplyDoublePointsForSeconds(doubleTime);
                ballController?.DoubleSpeedForSeconds(doubleTime);
            }
        }

        if (powerContainer.power == Power.Invisibility)
        {
            BallController ballController = FindFirstObjectByType<BallController>();

            if (ballController == null || ballController.isOutside)
            {
                cancelPowerLoss = true;
            }
            else
            {
                ballController?.InvisibilityForSeconds(invisibilityTime, gameObject);
                Instantiate(invisibilityVFX, ballController.gameObject.transform.position, Quaternion.identity);
                mainSource.PlayOneShot(invisibilitySound);
            }
        }

        if (powerContainer.power == Power.Telekinesis)
        {
            BallController ballController = FindFirstObjectByType<BallController>();

            if (ballController == null || ballController.isOutside || ballController.isOnTelekinesis)
            {
                cancelPowerLoss = true;
            }
            else
            {
                GameObject vfx = Instantiate(telekinesisVFX, ballController.transform);
                GameObject signal = Instantiate(telekinesisSignalVFX, transform);
                signal.transform.localPosition = new Vector2(-Mathf.Sign(transform.position.x) * 1.0f, 0.0f);
                signal.transform.localScale = new Vector3(-Mathf.Sign(transform.position.x), 1.0f, 1.0f);
                TelekinesisOverlay vfxComponent = vfx.GetComponent<TelekinesisOverlay>();
                ballController.TelekinesisBind(transform, vfxComponent, signal);
                mainSource.PlayOneShot(telekinesisSound);
            }
        }

        if (powerContainer.power == Power.Soul)
        {

            GameObject newProjectile = Instantiate(movingSoulVFX, (Vector2)transform.position + (IsOnLeftSide() ? Vector2.right : Vector2.left), Quaternion.identity);

            if (newProjectile != null && newProjectile.TryGetComponent<SoulProjectileController>(out SoulProjectileController projController))
            {
                projController.direction = IsOnLeftSide() ? Vector2.right : Vector2.left;
                projController.transform.localScale = new Vector2(IsOnLeftSide() ? 1.0f : -1.0f, projController.transform.localScale.y);
                projController.soulGotPower += OnSoulCollision;
                mainSource.PlayOneShot(soulSound);
                projController.SetOwnerController(this);
                soulDeployed = true;
            }
        }

        if (cancelPowerLoss)
        {
            mainSource.PlayOneShot(failPowerSound);
            return;
        }

        if (resetPower) powerContainer.ResetPower();
    }
}
