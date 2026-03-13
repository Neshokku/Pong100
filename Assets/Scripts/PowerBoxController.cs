using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PowerContainer))]
public class PowerBoxController : MonoBehaviour
{
    public PowerContainer container { get; private set; }

    private List<GameObject> boxListReference;

    private void Awake()
    {
        container = GetComponent<PowerContainer>();
    }

    public void SetBoxListReference(List<GameObject> list)
    {
        boxListReference = list;
    }

    public void Dissapear()
    {
        if (boxListReference != null) boxListReference.Remove(gameObject);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<BallController>(out BallController ballController))
        {
            if (ballController.lastPlayerHit == null || container.lockPower) return;

            PlayerPowerController playerPowerController = ballController.lastPlayerHit.GetComponent<PlayerPowerController>();

            if (playerPowerController != null && !playerPowerController.powerContainer.hasPower && !playerPowerController.soulDeployed)
            {
                playerPowerController.powerContainer.SetPower(container.power);
                Dissapear();
            }
        }
    }
}
