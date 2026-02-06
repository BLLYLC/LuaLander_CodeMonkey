using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    private const float GRAVITY_NORMAL = 0.7f;
    public static Lander Instance { get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {   
        public LandingType landingType;
        public int score;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier;
    }
    public enum LandingType
    {
        Success, WrongLandingArea, TooSteepAngle, TooFastLanding,
    }
    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver,
    }
    private Rigidbody2D rb;
    private float fuelAmount;
    private float fuelAmountMax = 10f;
    private State state;
    private float turnSpeed=100f;
    private float softLandingVelocityMagnitude = 4f;
    private void Awake()
    {
        Instance = this;
        fuelAmount = fuelAmountMax;
        rb = GetComponent<Rigidbody2D>();
        state = State.WaitingToStart;
        rb.gravityScale = 0f;
    }
    private void FixedUpdate()
    {   
        OnBeforeForce?.Invoke(this,EventArgs.Empty);
        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (GameInput.instance.IsUpActionPressed() ||
                    GameInput.instance.IsRightActionPressed() ||
                    GameInput.instance.IsLeftActionPressed() || GameInput.instance.GetMovementInputVector2() != Vector2.zero)
                {
                    rb.gravityScale = GRAVITY_NORMAL;
                    SetState(State.Normal);
                }
                break;
            case State.Normal:
                if (fuelAmount <= 0f)
                {
                    return;
                }
                float gamePadDeadzone = .4f; 
                if (GameInput.instance.IsUpActionPressed()||GameInput.instance.GetMovementInputVector2().y>gamePadDeadzone)
                {
                    float force = 700f;
                    rb.AddForce(force * transform.up * Time.deltaTime);
                    ConsumeFuel();
                    OnUpForce?.Invoke(this, EventArgs.Empty);
                }
                if (GameInput.instance.IsLeftActionPressed()|| GameInput.instance.GetMovementInputVector2().x < -gamePadDeadzone)
                {
                    rb.AddTorque(turnSpeed * Time.deltaTime);
                    ConsumeFuel();
                    OnLeftForce?.Invoke(this, EventArgs.Empty);
                }
                if (GameInput.instance.IsRightActionPressed()|| GameInput.instance.GetMovementInputVector2().x > gamePadDeadzone)
                {
                    rb.AddTorque(-turnSpeed * Time.deltaTime);
                    ConsumeFuel();
                    OnRightForce?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Crashed");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.WrongLandingArea,
                dotVector = 0f,
                landingSpeed = 0f,
                scoreMultiplier = 0 ,
                score = 0
            });
            SetState(State.GameOver);
            return; 
        }
        float relativeVelocityMagnitude = collision.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            Debug.Log("Landed too hard");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooFastLanding,
                dotVector = 0f,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = landingPad.GetScoreMultiplier(),
                score = 0
            });
            SetState(State.GameOver);
            return;
        }
        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .9f;
        if(dotVector < minDotVector)
        {
            Debug.Log("Landed too Steep");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooSteepAngle,
                dotVector = dotVector,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0,
            });
            SetState(State.GameOver);
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
        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            landingType = LandingType.Success,
            dotVector = dotVector,
            landingSpeed = relativeVelocityMagnitude,
            scoreMultiplier = landingPad.GetScoreMultiplier(),
            score = score
        });
        SetState(State.GameOver);
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
    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state,
        });
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
