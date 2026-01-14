using UnityEngine;

public class BallController : MonoBehaviour
{

    [SerializeField] private float initialSpeed = 1.0f;
    private float speed;
    [SerializeField] private Vector2 initialDir = Vector2.zero;
    [SerializeField] private float onCollisionSpeedAdd = 0.2f;
    [SerializeField] private float maxSpeed = 10.0f;

    [Header("Limits")]
    [SerializeField] private float right = 9.0f;
    [SerializeField] private float left = -9.0f;

    // Non Assignables
    private Vector2 direction;
    private bool isOutside = false;

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
        direction = initialDir;
        speed = initialSpeed;
        isOutside = false;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (transform.position.x > right && !isOutside)
        {
            isOutside = true;
            GameManagerController.Instance.AddScoreToPlayer1(1);
            pointSound.Play();
            Invoke(nameof(ResetBall), 0.8f);
        }

        if (transform.position.x < left && !isOutside)
        {
            isOutside = true;
            GameManagerController.Instance.AddScoreToPlayer2(1);
            pointSound.Play();
            Invoke(nameof(ResetBall), 0.8f);
        }
    }

    private void ResetBall()
    {
        transform.position = Vector2.zero;
        speed = initialSpeed;
        isOutside = false;
        spriteRenderer.color = Color.white;
    }

    void FixedUpdate()
    {
        rb.MovePosition((Vector2)transform.position + (direction.normalized * speed * Time.fixedDeltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
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
                
        }
        else
        {
            direction = Vector2.Reflect(direction, collision.GetContact(0).normal);
        }

        bounceSound.Play();
    }
}
