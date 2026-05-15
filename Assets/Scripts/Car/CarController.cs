using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Wheels Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Wheels Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;


    [Header("Car Settings")]
    //Fuerza Motor
    public float motorForce = 1500f;
    //Fuerza de Frenado
    public float brakeForce = 1500f;
    //Ángulo Máximo de Giro
    public float maxSteerAngle = 30f;

    public Rigidbody carRigidbody;

    //multiplicador defrenado 
    public float brakeVelocityMultiplier = 0.94f;

    //velocidad minima para frenado extre
    public float minSpeedForExtraBrake = 0.5f;


    [Header("Freno al soltar acelerador")]
    public float autoBrakeForce = 800f;
    //multiplicador de velocidad de frenado al soltar acelerador
    public float autoBrakeVelocityMultiplier = 0.985f;
    //zona muerta acelerador
    public float deadZoneAcceleration = 0.05f;

    //Image Stop
    public GameObject imageStop;

    private PlayerInput playerInput;

    // X Cursores
    private InputAction moveAction;

    //freno accion Jump space
    private InputAction brakeAction;

    //input y
    private  float accelerationInput;

    private  float steeringInput;

    //pulsado freno
    private bool isBraking;

    public bool arrancado = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       playerInput = GetComponent<PlayerInput>();

       moveAction = playerInput.actions["Move"];
       brakeAction = playerInput.actions["Jump"];

       if (imageStop != null)
           imageStop.SetActive(false);
       
    }

    // Update is called once per frame
    void Update()
    {
        if(!arrancado)
        {
            accelerationInput = 0f;
            steeringInput = 0f;
            isBraking = false;
            MostrarImagenStop();
            UpdateWheelMeshes();
            return;
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        steeringInput = moveInput.x;
        accelerationInput = moveInput.y;
        isBraking = brakeAction.IsPressed();
        MostrarImagenStop();
        UpdateWheelMeshes();
    }

    private void FixedUpdate()
    {
        if(!WheelsAsigned())
        {
            return;
        }
        if(!arrancado)
        {
            StopMotorForce();
            ApplyBrakeTorque(0f);
            return;
        }

        //motor
        HandleMotor();
        //dirección
        HandleSteering();
        //frenado
        HandleBraking();
    }

    private void MostrarImagenStop()
    {
        if (imageStop == null)
            return;
        
        imageStop.SetActive(isBraking);
        
    }

    private bool WheelsAsigned()
    {
        if (frontLeftWheel == null || frontRightWheel == null || rearLeftWheel == null || rearRightWheel == null)
        {
            Debug.LogError("Asigna todos los WheelColliders en el inspector.");
            return false;
        }
        return true;
    }

    //Acelera coche
    private void HandleMotor()
    {
        if(isBraking)
        {
            StopMotorForce();
            return;
        }
        if(Mathf.Abs(accelerationInput) < deadZoneAcceleration)
        {
            StopMotorForce();
            return;
        }

        float motorTorque = accelerationInput * motorForce;
        rearLeftWheel.motorTorque = motorTorque;
        rearRightWheel.motorTorque = motorTorque;
    }

    private void StopMotorForce()
    {
        rearLeftWheel.motorTorque = 0f;
        rearRightWheel.motorTorque = 0f;
        frontLeftWheel.motorTorque = 0f;
        frontRightWheel.motorTorque = 0f;
    }

    private void HandleSteering()
    {
        float steerAngle = steeringInput * maxSteerAngle;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;
    } 

    private void HandleBraking()
    {
        float currentBrakeForce = 0f;

        if(isBraking)
        {
            currentBrakeForce = brakeForce;
        }
        else if (Mathf.Abs(accelerationInput) < deadZoneAcceleration)
        {
            currentBrakeForce = autoBrakeForce;
        }

        ApplyBrakeTorque(currentBrakeForce);

        if(isBraking)
        {
            ApplyExtraBrake();
        }
        else if (Mathf.Abs(accelerationInput) < deadZoneAcceleration)
        {
            ApplyAutoBrake();
        }
    }

    private void ApplyBrakeTorque(float currentBrakeTorque)
    {
        rearLeftWheel.brakeTorque = currentBrakeTorque;
        rearRightWheel.brakeTorque = currentBrakeTorque;
        frontLeftWheel.brakeTorque = currentBrakeTorque;
        frontRightWheel.brakeTorque = currentBrakeTorque;
    }

    private void ApplyExtraBrake()
    {
        if(carRigidbody == null)
            return;

        float speed = carRigidbody.linearVelocity.magnitude;

        if(speed < minSpeedForExtraBrake)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
            return;
        }
        carRigidbody.linearVelocity *= brakeVelocityMultiplier;
    }

    private void ApplyAutoBrake()
    {
        if(carRigidbody == null)
            return;

        float speed = carRigidbody.linearVelocity.magnitude;

        if(speed < minSpeedForExtraBrake)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
            return;
        }

        carRigidbody.linearVelocity *= autoBrakeVelocityMultiplier;
    }

    private void UpdateWheelMeshes()
    {
        UpdateSingleWheel(frontLeftWheel, frontLeftMesh);
        UpdateSingleWheel(frontRightWheel, frontRightMesh);
        UpdateSingleWheel(rearLeftWheel, rearLeftMesh);
        UpdateSingleWheel(rearRightWheel, rearRightMesh);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelMesh)
    {
        if (wheelCollider == null || wheelMesh == null)
            return;

        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelMesh.position = position;
        wheelMesh.rotation = rotation;
    }
}
