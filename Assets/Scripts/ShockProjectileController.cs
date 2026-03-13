using System.Collections;
using UnityEngine;

public class ShockProjectileController : ProjectileController
{
    [SerializeField] private float stunDuration = 1.5f;
    [SerializeField] private GameObject electricityVFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.DisableMovementForSeconds(stunDuration);
            GameObject vfx = Instantiate(electricityVFX, playerController.transform.position, Quaternion.identity);
            vfx.transform.parent = playerController.transform;
            if (vfx != null) Destroy(vfx, stunDuration);
            Destroy(gameObject);
        }
    }
}
