using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] protected float speed = 10.0f;
    [SerializeField] protected float lifeTime = 4.0f;
    public Vector2 direction = Vector2.right;
    protected bool canMove = true;

    // Components
    private Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (!canMove) return;
        rb.MovePosition((Vector2)transform.position + (direction * speed * Time.fixedDeltaTime));
    }
}
