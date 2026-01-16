using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShockProjectileController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float speed = 10.0f;
    public Vector2 direction = Vector2.right;
    [SerializeField] private float stunDuration = 1.5f;
    [SerializeField] private float lifeTime = 4.0f;
    [SerializeField] private GameObject electricityVFX;

    // Components
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        rb.MovePosition((Vector2)transform.position + (direction * speed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.DisableMovementForSeconds(stunDuration);
            GameObject vfx = Instantiate(electricityVFX, playerController.transform.position, Quaternion.identity);
            Destroy(vfx, stunDuration);
            Destroy(gameObject);
        }
    }
}
