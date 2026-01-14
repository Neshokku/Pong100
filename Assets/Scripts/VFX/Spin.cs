using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] float spinSpeed = 1.0f;

    void Update() 
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - spinSpeed * Time.deltaTime);

        if (transform.rotation.eulerAngles.z < -360.0f) transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 360.0f);
    }
}
