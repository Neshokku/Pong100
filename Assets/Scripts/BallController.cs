using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // Base
    private float originalBallAlpha;
    private float originalGreenAuraAlpha;

    private List<float> doubleSpeedStacks = new List<float>();
    private float invisibleTime = 0.0f;

    [SerializeField] private float initialSpeed = 1.0f;
    private float speed;
    [SerializeField] private float onCollisionSpeedAdd = 0.2f;
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float intangibilityTimeOnHit = 0.8f;

    [Header("Limits")]
    [SerializeField] private float right = 9.0f;
    [SerializeField] private float left = -9.0f;
    [SerializeField] private float up = 5.0f;
    [SerializeField] private float down = -5.0f;

    // Non Assignables
    public Vector2 direction { get; private set; }
    public bool isOutside { get; private set; } = false;
    private int intangibleStacks = 0;

    [Header("References")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject greenAura;

    // Component References
    private SpriteRenderer greenAuraRenderer;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private GameManagerController gm;
    public GameObject lastPlayerHit { get; private set; }
    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pointSound;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip invisibilitySound;

    void Start()
    {
        originalBallAlpha = spriteRenderer.color.a;
        greenAuraRenderer = greenAura.GetComponent<SpriteRenderer>();
        originalGreenAuraAlpha = greenAuraRenderer.color.a;

        rb = GetComponent<Rigidbody2D>();
        RandomizeDirection();
        speed = initialSpeed;
        isOutside = false;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        gm = FindFirstObjectByType<GameManagerController>();
    }

    // randomizes the direction of the ball
    private void RandomizeDirection()
    {
        float horizontalDir = Random.value < 0.5f ? -1f : 1f;

        float angle = Random.Range(-45f, 45f);

        float rad = angle * Mathf.Deg2Rad;
        direction = new Vector2(
            Mathf.Cos(rad) * horizontalDir,
            Mathf.Sin(rad)
        ).normalized;
    }

    private void Update()
    {
        if (transform.position.x > right && !isOutside)
        {
            isOutside = true;
            gm?.AddScoreToPlayer1(1);
            gm?.ResetDoublePoints();
            audioSource.PlayOneShot(pointSound, 2.0f);
            ResetInvisibility();
            Invoke(nameof(ResetBall), 1.2f);
        }

        if (transform.position.x < left && !isOutside)
        {
            isOutside = true;
            gm?.AddScoreToPlayer2(1);
            gm?.ResetDoublePoints();
            audioSource.PlayOneShot(pointSound, 2.0f);
            ResetInvisibility();
            Invoke(nameof(ResetBall), 1.2f);
        }

        if (transform.position.y + boxCollider.size.y / 2 > up && Mathf.Sign(direction.y) > 0)
        {
            direction = new Vector2(direction.x, direction.y * -1);
            audioSource.PlayOneShot(bounceSound);
        }

        if (transform.position.y - boxCollider.size.y / 2 < down && Mathf.Sign(direction.y) < 0)
        {
            direction = new Vector2(direction.x, direction.y * -1);
            audioSource.PlayOneShot(bounceSound);
        }

        HandleDoubleSpeed();
        HandleInvisibility();
    }

    // resets the ball position to the center of the screen
    private void ResetBall()
    {
        if (!gm.gameUp) return;

        transform.position = Vector2.zero;
        speed = initialSpeed;
        isOutside = false;
        spriteRenderer.color = Color.white;
        lastPlayerHit = null;
        ResetDoubleSpeed();
        RandomizeDirection();
    }

    void FixedUpdate()
    {
        rb.MovePosition((Vector2)transform.position + (direction.normalized * speed * (doubleSpeedStacks.Count > 0 ? 1.3f : 1.0f) * Time.fixedDeltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle") && !(IsIntangible() && collision.gameObject != lastPlayerHit))
        {
            float hitPoint = transform.position.y - collision.transform.position.y;
            float paddleHeight = collision.collider.bounds.size.y;
            float normalizedHitPoint = hitPoint / (paddleHeight / 2);

            direction = new Vector2(
                transform.position.x > 0 ? -1 : 1,
                normalizedHitPoint
            ).normalized;

            if (speed < maxSpeed)
                speed += onCollisionSpeedAdd;

            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Color color = playerController.GetColor();
                spriteRenderer.color = new(color.r, color.g, color.b, spriteRenderer.color.a);
            }

            lastPlayerHit = collision.gameObject;

            IntangibleForSeconds(intangibilityTimeOnHit);
            ResetInvisibility();
        }

        audioSource.PlayOneShot(bounceSound);
    }

    public void SetDirection(Vector2 dir)
    {
        this.direction = dir;
    }

    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
    }

    public void IntangibleForSeconds(float seconds)
    {
        StartCoroutine(IntangibleForSecondsRoutine(seconds));
    }

    public void RemoveIntangibility()
    {
        intangibleStacks = 0;
    }

    private bool IsIntangible() { return intangibleStacks < 0; }

    private IEnumerator IntangibleForSecondsRoutine(float seconds)
    {
        intangibleStacks++;
        yield return new WaitForSeconds(seconds);
        if (intangibleStacks > 0) intangibleStacks--;
    }

    public void DoubleSpeedForSeconds(float seconds)
    {
        doubleSpeedStacks.Add(seconds);
        greenAura.SetActive(true);
    }

    private void ResetDoubleSpeed()
    {
        doubleSpeedStacks.Clear();
        greenAura.SetActive(false);
    }

    private void HandleDoubleSpeed()
    {
        for (int i = 0; i < doubleSpeedStacks.Count; i++)
        {
            doubleSpeedStacks[i] -= Time.deltaTime;
            if (doubleSpeedStacks[i] <= 0.0f)
            {
                doubleSpeedStacks.RemoveAt(i);
                if (doubleSpeedStacks.Count <= 0)
                {
                    ResetDoubleSpeed();
                }
            }
        }
    }

    public void InvisibilityForSeconds(float seconds)
    {
        if (invisibleTime <= 0.0f) StartCoroutine(FadeOutRoutine());
        invisibleTime += seconds;
    }

    public void ResetInvisibility()
    {
        invisibleTime = 0.0f;
        StartCoroutine(FadeInRoutine());
    }

    private void HandleInvisibility()
    {
        if (invisibleTime > 0.0f)
        {
            invisibleTime -= Time.deltaTime;
            if (invisibleTime <= 0.0f)
            {
                ResetInvisibility();
            }
        }
    }

    private IEnumerator FadeInRoutine()
    {
        float fadeTime = 0.3f;

        float t = 0.0f;

        float initialAlpha = spriteRenderer.color.a;
        float initialGreenAuraAlpha = greenAuraRenderer.color.a;
        float endAlpha = originalBallAlpha;
        float endGreenAuraAlpha = originalGreenAuraAlpha;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            spriteRenderer.color = new(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(initialAlpha, endAlpha, t / fadeTime));
            greenAuraRenderer.color = new(greenAuraRenderer.color.r, greenAuraRenderer.color.g, greenAuraRenderer.color.b, Mathf.Lerp(initialGreenAuraAlpha, endGreenAuraAlpha, t / fadeTime));
            yield return null;
        }
    }

    private IEnumerator FadeOutRoutine()
    {
        float fadeTime = 0.3f;

        float t = 0.0f;

        float initialAlpha = spriteRenderer.color.a;
        float initialGreenAuraAlpha = greenAuraRenderer.color.a;
        float endAlpha = 0.0f;
        float endGreenAuraAlpha = 0.0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            spriteRenderer.color = new(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(initialAlpha, endAlpha, t / fadeTime));
            greenAuraRenderer.color = new(greenAuraRenderer.color.r, greenAuraRenderer.color.g, greenAuraRenderer.color.b, Mathf.Lerp(initialGreenAuraAlpha, endGreenAuraAlpha, t / fadeTime));
            yield return null;
        }
    }

    public void ApplyRatioFix() 
    {
        float ratioMultiplier = 11.0f / 14.0f;

        initialSpeed *= ratioMultiplier;
        maxSpeed *= ratioMultiplier;
        onCollisionSpeedAdd *= ratioMultiplier;
    }
}
