
/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarController : MonoBehaviour
{
    public enum Axel
    {
        Front,
        Rear
    }

    //[Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public float maxAcceleration = 150.0f;
    public float brakeAcceleration = 50.0f;

    public float turnSensitivy = 1.0f;
    public float maxSteerAngle = 30.0f;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody carRb;

    public void Start()
    {
        carRb = GetComponent<Rigidbody>();
        Debug.Log("ci arrivo");
        Debug.Log(carRb.velocity.magnitude);

    }

    public void Update()
    {
        getInputs();
    }

    void LateUpdate()
    {

        Move();
        Steer();
        Brake();
    }

    void getInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    void Move()
    {
        foreach (var wheel in wheels)
        {
            Debug.Log("moveInput=" + moveInput + "maxAcceleration=" + maxAcceleration);

            wheel.wheelCollider.motorTorque = moveInput * 1000 * maxAcceleration * Time.deltaTime;
        }


    }

    void Brake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            foreach(var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }


    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput *turnSensitivy* maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle,_steerAngle,0.6f);
            }
        }
    }
} 
*/
   