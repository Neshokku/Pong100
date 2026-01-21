using System;
using UnityEngine;
using UnityEngine.UI;

public class PaddleInput : MonoBehaviour
{

    [Header("Controls")]
    [SerializeField] private KeyCode keyUp = KeyCode.UpArrow;
    [SerializeField] private Button upButton;
    [SerializeField] private KeyCode keyDown = KeyCode.DownArrow;
    [SerializeField] private Button downButton;
    [SerializeField] private KeyCode keyPower = KeyCode.Space;


    // Variables
    public Vector2 input { get; private set; } = Vector2.zero;
    public event Action powerKeyPressed;

    void Update()
    {
        input = Vector2.zero;

        if (Input.GetKey(keyUp))
        {
            input = new Vector2(input.x, input.y + 1);
        }

        if (Input.GetKey(keyDown))
        {
            input = new Vector2(input.x, input.y - 1);
        }

        if (Input.GetKeyDown(keyPower))
        {
            powerKeyPressed.Invoke();
        }
    }
}
