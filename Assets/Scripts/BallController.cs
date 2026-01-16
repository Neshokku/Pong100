using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{

    [SerializeField] private float initialSpeed = 1.0f;
    private float speed;
    [SerializeField] private float onCollisionSpeedAdd = 0.2f;
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float intangibilityTimeOnHit = 0.3f;

    [Header("Limits")]
    [SerializeField] private float right = 9.0f;
    [SerializeField] private float left = -9.0f;

    // Non Assignables
    public Vector2 direction { get; private set; }
    private bool isOutside = false;
    private int intangibleStacks = 0;

    // Component References
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public GameObject lastPlayerHit { get; private set; }
    [Header("Sounds")]
    [SerializeField] private AudioSource pointSound;
    [SerializeField] private AudioSource bounceSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RandomizeDirection();
        speed = initialSpeed;
        isOutside = false;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

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
            GameManagerController.Instance.AddScoreToPlayer1(1);
            pointSound.Play();
            Invoke(nameof(ResetBall), 1.2f);
        }

        if (transform.position.x < left && !isOutside)
        {
            isOutside = true;
            GameManagerController.Instance.AddScoreToPlayer2(1);
            pointSound.Play();
            Invoke(nameof(ResetBall), 1.2f);
        }
    }

    private void ResetBall()
    {
        transform.position = Vector2.zero;
        speed = initialSpeed;
        isOutside = false;
        spriteRenderer.color = Color.white;
        lastPlayerHit = null;
        RandomizeDirection();
    }

    void FixedUpdate()
    {
        rb.MovePosition((Vector2)transform.position + (direction.normalized * speed * Time.fixedDeltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle") && !IsIntangible())
        {
            float hitPoint = transform.position.y - collision.transform.position.y;
            float paddleHeight = collision.collider.bounds.size.y;
            float normalizedHitPoint = hitPoint / (paddleHeight / 2);

            direction = new Vector2(
                direction.x > 0 ? -1 : 1,
                normalizedHitPoint
            ).normalized;

            if (speed < maxSpeed)
                speed += onCollisionSpeedAdd;

            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                spriteRenderer.color = playerController.GetColor();
            }

            lastPlayerHit = collision.gameObject;

            IntangibleForSeconds(intangibilityTimeOnHit);
        }
        else
        {
            direction = Vector2.Reflect(direction, collision.GetContact(0).normal);
        }

        bounceSound.Play();
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

    private bool IsIntangible() { return intangibleStacks < 0; }

    private IEnumerator IntangibleForSecondsRoutine(float seconds)
    {
        intangibleStacks++;
        yield return new WaitForSeconds(seconds);
        intangibleStacks--;
    }
}
