using UnityEngine;

[RequireComponent(typeof(PowerContainer))]
public class SoulBox : MonoBehaviour
{
    public PowerContainer powerContainer { get; private set; }

    private void Awake()
    {
        powerContainer = GetComponent<PowerContainer>();
    }
}
