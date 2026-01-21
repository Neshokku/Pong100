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

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        input = playerInput.actions[movementActionName].ReadValue<Vector2>();
        Debug.Log("Read input from " + movementActionName + " - the value is " + input);
        playerInput.actions[usePowerActionName].performed += OnPowerUsed;
    }

    public void OnPowerUsed(InputAction.CallbackContext callbackContext)
    {
        powerKeyPressed.Invoke();
    }
}
