using UnityEngine;

public class Movement : MonoBehaviour
{
    void Update()
    {
        float h = Input.GetAxis("Horizontal") * 3 * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * 3 * Time.deltaTime;

        transform.position = new Vector3(transform.position.x + h, 0, transform.position.z + v);
    }
}
