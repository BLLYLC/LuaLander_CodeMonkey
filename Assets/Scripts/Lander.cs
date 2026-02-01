using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    private Rigidbody2D rb;

    private float force=700f;
    private float turnSpeed=100f;
    private float softLandingVelocityMagnitude = 4f;
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Crashed");
            return; 
        }
        float relativeVelocityMagnitude = collision.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            Debug.Log("Landed too hard");
            return;
        }
        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .9f;
        if(dotVector < minDotVector)
        {
            Debug.Log("Landed too Steep");
            return;
        }

        Debug.Log("Succesful Landing");
        float maxScoreAmountLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreAmountLandingAngle-Mathf.Abs(dotVector - 1f)* scoreDotVectorMultiplier* maxScoreAmountLandingAngle;

        float maxScoreAmountLandingSpeed=100;
        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

        print("landing angle score: "+landingAngleScore);
        print("landing speed score: " + landingSpeedScore);
    }
}
