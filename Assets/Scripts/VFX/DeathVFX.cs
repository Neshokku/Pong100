using UnityEngine;

public class DeathVFX : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] GameObject particleObject;

    [Header("Config")]
    [SerializeField] int particleCount;
    [SerializeField] float particleSpeed;

    void Start()
    {
        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = Instantiate(particleObject, transform);
            particle.transform.parent = null;
            Rigidbody2D particleRb = particle.GetComponent<Rigidbody2D>();
            particleRb.AddForce(Random.insideUnitCircle.normalized * particleSpeed);
        }

        // Destroys Itself at the end
        Destroy(gameObject);
    }
}
