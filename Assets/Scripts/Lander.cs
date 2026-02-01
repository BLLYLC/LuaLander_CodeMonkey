using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    private Rigidbody2D rb;

    private float force=700f;
    private float turnSpeed=100f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (Keyboard.current.upArrowKey.isPressed)
        {
            
            rb.AddForce(force * transform.up * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.T))
        {
            rb.AddForce(transform.up * Time.deltaTime);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            rb.AddTorque(turnSpeed * Time.deltaTime);
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            rb.AddTorque(-turnSpeed * Time.deltaTime);
        }
    }
}
