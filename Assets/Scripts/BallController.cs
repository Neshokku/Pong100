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
        direction = Vector2.Reflect(direction, collision.GetContact(0).normal);

        if (collision.gameObject.CompareTag("Paddle"))
        {
            if (speed < maxSpeed) speed += onCollisionSpeedAdd;

            SpriteRenderer paddleRenderer = collision.gameObject.GetComponentInChildren<SpriteRenderer>();

            if (paddleRenderer != null) {
                spriteRenderer.color = paddleRenderer.color;
            }

        }

        bounceSound.Play();
    }
}
