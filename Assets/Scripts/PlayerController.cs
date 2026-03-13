using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PaddleInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed = 0.1f;

    [Header("Limits")]
    [SerializeField] private float maxY = 5.0f;
    [SerializeField] private float minY = -5.0f;

    [Header("Components")]
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private PaddleInput paddleInput;
    [SerializeField] SpriteRenderer mainSprite;

    [Header("VFX")]
    [SerializeField] GameObject deathVFX;
    private float movementRestrictedForSeconds = 0.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        paddleInput = GetComponent<PaddleInput>();
    }

    private void Update()
    {
        if (movementRestrictedForSeconds > 0.0f)
        {
            movementRestrictedForSeconds -= Time.deltaTime;
        } else if (movementRestrictedForSeconds < 0.0f)
        {
            movementRestrictedForSeconds = 0.0f;
        }
    }

    void FixedUpdate()
    {
        if (movementRestrictedForSeconds <= 0.0f)
        {
            if (paddleInput.input.y > 0 && GetUpperY() < maxY)
            {
                rb.MovePosition((Vector2)transform.position + (Vector2.up * speed * Time.fixedDeltaTime));
            }

            if (paddleInput.input.y < 0 && GetLowerY() > minY)
            {
                rb.MovePosition((Vector2)transform.position + (Vector2.down * speed * Time.fixedDeltaTime));
            }
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

    public Color GetColor()
    {
        if (mainSprite != null) return mainSprite.color;
        return Color.white;
    }

    public void DisableMovementForSeconds(float seconds)
    {
        movementRestrictedForSeconds += seconds;
    }

    public void Lose()
    {
        Instantiate(deathVFX, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        gameObject.SetActive(false);
    }
}
