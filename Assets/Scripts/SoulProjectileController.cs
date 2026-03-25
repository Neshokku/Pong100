using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SoulProjectileController : ProjectileController
{
    private PlayerPowerController ownerPowerController;
    private SpriteRenderer spriteRenderer;

    [SerializeField] GameObject soulBoxPrefab;

    public event Action<Power, Vector3> soulGotPower;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        Invoke(nameof(DestroyByLifetime), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PowerContainer>(out PowerContainer powerContainer))
        {
            if (!powerContainer.hasPower || powerContainer.lockPower) return;
            canMove = false;
            StartCoroutine(PossessRoutine(powerContainer));
        }
    }

    private void DestroyByLifetime()
    {

        if (gameObject != null && ownerPowerController != null)
        {
            Destroy(gameObject);
            ownerPowerController.soulDeployed = false;
        }
    }

    public void SetOwnerController(PlayerPowerController controller)
    {
        ownerPowerController = controller;
    }

    private IEnumerator PossessRoutine(PowerContainer powerContainer)
    {
        powerContainer.lockPower = true;

        Vector2 initialPos = transform.position;
        Vector2 targetPos = powerContainer.transform.position;
        Color initialColor = spriteRenderer.color;
        Color targetColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.0f);

        float duration = .5f;
        float t = 0.0f;

        AnimationCurve easeCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        while (t < duration)
        {
            t += Time.deltaTime;

            targetPos = powerContainer.transform.position;

            Vector3 rotateDir = ((Vector3)targetPos - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, rotateDir);
            transform.rotation = targetRotation * Quaternion.Euler(0, 0, 90);

            transform.position = Vector2.Lerp(initialPos, targetPos, easeCurve.Evaluate(t / duration));
            spriteRenderer.color = Color.Lerp(initialColor, targetColor, easeCurve.Evaluate(t / duration));

            yield return null;
        }

        transform.position = targetPos;
        spriteRenderer.color = targetColor;

        if (powerContainer.gameObject == null || !powerContainer.hasPower)
        {
            Destroy(gameObject);
            powerContainer.lockPower = false;
            yield break;
        }
        soulGotPower.Invoke(powerContainer.power, transform.position);
        powerContainer.ResetPower();
        powerContainer.lockPower = false;
        PowerBoxController powerBoxController = powerContainer.GetComponent<PowerBoxController>();
        if (powerBoxController != null) powerBoxController.Dissapear();
        Destroy(gameObject);
    }
}
