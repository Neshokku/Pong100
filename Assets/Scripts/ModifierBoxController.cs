using UnityEngine;

public enum ModifierType
{
    Portal,
    Inversion,
    Stun
}

public class ModifierBoxController : MonoBehaviour
{
    [SerializeField] private ModifierType modifierType;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
