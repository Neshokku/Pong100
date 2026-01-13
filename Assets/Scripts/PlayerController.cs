using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private KeyCode keyUp = KeyCode.UpArrow;
    [SerializeField] private KeyCode keyDown = KeyCode.DownArrow;

    [Header("Stats")]
    [SerializeField] private float speed = 0.1f;

    [Header("Limits")]
    [SerializeField] private float maxY = 5.0f;
    [SerializeField] private float minY = -5.0f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(keyUp) && GetUpperY() < maxY)
        {
            rb.MovePosition((Vector2)transform.position + (Vector2.up * speed * Time.fixedDeltaTime));
        }

        if (Input.GetKey(keyDown) && GetLowerY() > minY)
        {
            rb.MovePosition((Vector2)transform.position + (Vector2.down * speed * Time.fixedDeltaTime));
        }
    }

    private float GetUpperY()
    {
        return transform.position.y + (boxCollider.size.y / 2);
    }

    private float GetLowerY()
    {
        return transform.position.y - (boxCollider.size.y / 2);
    }
}
