using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    public static Lander Instance { get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public int score;
    }

    private Rigidbody2D rb;
    private float fuelAmount;
    private float fuelAmountMax = 10f;

    private float turnSpeed=100f;
    private float softLandingVelocityMagnitude = 4f;
    private void Awake()
    {
        Instance = this;
        fuelAmount = fuelAmountMax;
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {   
        OnBeforeForce?.Invoke(this,EventArgs.Empty);
        if (fuelAmount <= 0f)
        {
            return;
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            float force = 700f;
            rb.AddForce(force * transform.up * Time.deltaTime);
            ConsumeFuel();
            OnUpForce?.Invoke(this,EventArgs.Empty); 
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            rb.AddTorque(turnSpeed * Time.deltaTime);
            ConsumeFuel();
            OnLeftForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            rb.AddTorque(-turnSpeed * Time.deltaTime);
            ConsumeFuel();
            OnRightForce?.Invoke(this, EventArgs.Empty);
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
        int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());
        print("Score: "+ score);
        OnLanded?.Invoke(this,new OnLandedEventArgs { score = score });
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
     if(collision.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
            float addFuelAmount = 10f;
            fuelAmount += addFuelAmount;
            if (fuelAmount>fuelAmountMax)
            {
                fuelAmount = fuelAmountMax;
            }
            fuelPickup.DestroySelf();  
        }
        if (collision.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {   
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }
    private void ConsumeFuel()
    {
        float fuelConsumptionAmount = 1f;
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }
    public float GetSpeedX()
    {
        return rb.linearVelocityX;
    }
    public float GetSpeedY()
    {
        return rb.linearVelocityY;
    }
    public float GetFuel()
    {
        return fuelAmount;
    }
    public float GetFuelAmountNormalized()
    {
        return fuelAmount / fuelAmountMax;
    }
}
