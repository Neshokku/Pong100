using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private GrowAndDissapear shrinkAnim;
    [SerializeField] private float lifeTime = 3.0f;

    [SerializeField] private AudioClip transportSound;
    [SerializeField] private AudioClip spawnSound;

    private AudioSource audioSource;
    private Collider2D portalCollider;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        portalCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        audioSource.PlayOneShot(spawnSound);
        Invoke(nameof(Dissapear), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<BallController>(out BallController ballController))
        {
            ballController.SetPosition(new Vector2(-(ballController.transform.position.x), ballController.transform.position.y));
            ballController.SetDirection(new Vector2(transform.position.x < 0 ? 1 : -1, ballController.direction.y));
            audioSource.PlayOneShot(transportSound);
            shrinkAnim.Animate();
            portalCollider.enabled = false;
        }
    }   

    private void Dissapear()
    {
        shrinkAnim.Animate();
    }
}
