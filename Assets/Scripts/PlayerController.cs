using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private KeyCode keyUp = KeyCode.UpArrow;
    [SerializeField] private KeyCode keyDown = KeyCode.DownArrow;
    [SerializeField] private KeyCode keyPower = KeyCode.Space;

    [Header("Stats")]
    [SerializeField] private float speed = 0.1f;

    [Header("Limits")]
    [SerializeField] private float maxY = 5.0f;
    [SerializeField] private float minY = -5.0f;

    [Header("Components")]
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    [SerializeField] SpriteRenderer mainSprite;
    [SerializeField] SpriteRenderer powerSprite;
    [SerializeField] PowerSpritesData powerSpritesData;

    public bool hasPower { get; private set; } = false;
    public Power power { get; private set; }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (hasPower && Input.GetKeyDown(keyPower))
        {
            UsePower();
        }
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

    public Color GetColor()
    {
        if (mainSprite != null) return mainSprite.color;
        return Color.white;
    }

    public void SetPower(Power power)
    {
        hasPower = true;
        this.power = power;
        powerSprite.sprite = powerSpritesData.GetSprite(power);
    }

    public void UsePower()
    {
        hasPower = false;
        powerSprite.sprite = null;
    }
}
