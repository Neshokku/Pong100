using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerID
{
    Player1,
    Player2
}

public class ControlSelector : MonoBehaviour
{
    private ControlType currentControlType;

    [Header("Configuration")]
    [SerializeField] PlayerID playerID;

    [Header("Elements")]
    [SerializeField] Button leftArrow;
    [SerializeField] Button rightArrow;
    [SerializeField] TextMeshProUGUI text;

    [Header("References")]
    [SerializeField] ControlSet controlSet;

    private void Start()
    {
        currentControlType = GameSettings.instance.GetPlayerControlType(playerID);
        UpdateControlSet();
        UpdateText();
        UpdateButtonInteractability();
    }

    private void SetControlType(ControlType controlType)
    {
        GameSettings.instance.SetControlTypeForPlayer(playerID, controlType);
        currentControlType = controlType;
    }

    private void UpdateControlSet()
    {
        controlSet.SetActiveControls(currentControlType);
    }

    private void UpdateButtonInteractability()
    {
        leftArrow.interactable = !((int)currentControlType <= 0);
        rightArrow.interactable = !((int)currentControlType >= System.Enum.GetValues(typeof(ControlType)).Length - 1);
    }

    private void UpdateText()
    {
        text.text = ((int)currentControlType + 1).ToString();
    }

    public void CycleLeft()
    {
        if ((int)currentControlType <= 0) return;
        SetControlType((ControlType)((int)currentControlType - 1));
        UpdateAll();
    }
    public void CycleRight()
    {
        if ((int)currentControlType >= System.Enum.GetValues(typeof(ControlType)).Length - 1) return;
        SetControlType((ControlType)((int)currentControlType + 1));
        UpdateAll();
    }

    private void UpdateAll()
    {
        UpdateControlSet();
        UpdateButtonInteractability();
        UpdateText();
    }

}
