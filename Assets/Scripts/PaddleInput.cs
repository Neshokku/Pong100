using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PaddleInput : MonoBehaviour
{

    private PlayerInput playerInput;

    [Header("Controls")]
    [SerializeField] private string movementActionName;
    [SerializeField] private string usePowerActionName;

    // Variables
    public Vector2 input { get; private set; } = Vector2.zero;
    public event Action powerKeyPressed;

    private void OnEnable()
    {
        GameManagerController gm = FindFirstObjectByType<GameManagerController>();
        playerInput = gm?.GetPlayerInput();
        playerInput.actions[usePowerActionName].performed += OnPowerUsed;
    }

    private void OnDisable()
    {
        if (playerInput != null)
            playerInput.actions[usePowerActionName].performed -= OnPowerUsed;
    }

    void Update()
    {
        input = playerInput.actions[movementActionName].ReadValue<Vector2>();
    }

    public void OnPowerUsed(InputAction.CallbackContext callbackContext) => powerKeyPressed?.Invoke();
}
