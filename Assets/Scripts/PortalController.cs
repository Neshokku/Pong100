using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private GrowAndDissapear shrinkAnim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<BallController>(out BallController ballController))
        {
            ballController.SetPosition(new Vector2(-(ballController.transform.position.x), ballController.transform.position.y));
            shrinkAnim.Animate();
        }
    }
}
