using UnityEngine;

public enum ControlType
{
    PowerCenter,
    PowerSideRight,
    PowerSideLeft
}

public class ControlSet : MonoBehaviour
{
    [SerializeField] private GameObject powerCenterSet;
    [SerializeField] private GameObject powerRightSet;
    [SerializeField] private GameObject powerLeftSet;

    public void SetActiveControls(ControlType controlType)
    {
        powerCenterSet.SetActive(controlType == ControlType.PowerCenter);
        powerRightSet.SetActive(controlType == ControlType.PowerSideRight);
        powerLeftSet.SetActive(controlType == ControlType.PowerSideLeft);
    }
}
