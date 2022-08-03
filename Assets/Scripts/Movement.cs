using UnityEngine;

public class Movement : MonoBehaviour
{
    Vector3 previous;
    public Animator anim;
    void Update()
    {
        float h = Input.GetAxis("Horizontal") * 3 * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * 3 * Time.deltaTime;
        
        transform.position = new Vector3(transform.position.x + h, 0, transform.position.z + v);
        if (h != 0 || v != 0)
        {
            transform.GetChild(0).transform.rotation = Quaternion.LookRotation(new Vector3(h, 0, v));
        }           

        anim.SetFloat("MoveSpeed", ((transform.position - previous) / Time.deltaTime).magnitude);

        previous = transform.position;
    }
}
