using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CarAgent : Agent
{
    public Transform parcheggio;
    public float speed = 10f;
    public Rigidbody carRigidbody;
    private bool fermatoAlSemaforo = false;
    private bool fermatoAlloStop = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
        carRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(new Vector3(-2.11f, -19.07f, 36.42f), Quaternion.Euler(0, 90, 0));
        //boolean per evitare che progressivamente vengano tolti punti.
        fermatoAlSemaforo = false;
        fermatoAlloStop = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(carRigidbody.velocity.magnitude);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = actions.ContinuousActions[0];
        float turnAmount = actions.ContinuousActions[1];

        Vector3 force = transform.forward * forwardAmount * speed;
        carRigidbody.AddForce(force, ForceMode.Acceleration);
        transform.Rotate(0, turnAmount * 50f * Time.deltaTime, 0);
    }

    private IEnumerator WaitForStop()
    {
        yield return new WaitForSeconds(3f);
        if (carRigidbody.velocity.magnitude < 0.1f && !fermatoAlloStop)////controllo se non l'ho già penalizzato
        {
            AddReward(50f);
            fermatoAlloStop = true;
            Debug.Log("STOP rispettato!");
            StartCoroutine(GradualSpeedIncrease());  //la velocità viene gradualmente ripristinata
        }
        else
        {
            AddReward(-50f);
            Debug.Log("NON mi sono fermato allo stop!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("stopLine"))
        {
            if (carRigidbody.velocity.magnitude < 0.1f)
            {
                StartCoroutine(WaitForStop());
            }
        }
                //gestione semafori

        if (other.CompareTag("redLight"))
        {
            if (!fermatoAlSemaforo && carRigidbody.velocity.magnitude < 0.1f)
            {
                AddReward(10f);
                fermatoAlSemaforo = true;
                Debug.Log("Mi sono fermato al semaforo rosso.");
            }
            else if (carRigidbody.velocity.magnitude > 0.5f)
            {
                AddReward(-10f);
                Debug.Log("Sono passato con il rosso!");
                EndEpisode();
            }
        }

        if (other.CompareTag("greenLight"))
        {
            fermatoAlSemaforo = false;
            StartCoroutine(GradualSpeedIncrease());
        }
    }

    private IEnumerator GradualSpeedIncrease()
    {
        float targetSpeed = 10f;
        while (speed < targetSpeed)
        {
            speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * 2f);
            yield return null;
        }
    }
}



/*
private IEnumerator WaitForStop()
{
    yield return new WaitForSeconds(3f); // Aspetta 3 secondi
    if (carRigidbody.velocity.magnitude < 0.1f)
    {
        AddReward(50f);
        Debug.Log("STOP rispettato!");
        Riparto();
    }
    else
    {
        AddReward(-50f);
        Debug.Log("NON mi sono fermato allo stop!");
    }
}

private void OnTriggerStay(Collider other)
{
    if (other.CompareTag("stopLine"))
    {
        if (carRigidbody.velocity.magnitude < 0.1f)
        {
            StartCoroutine(WaitForStop());
        }
    }
}
*/

/*
private bool fermatoAlSemaforo = false;

private void OnTriggerStay(Collider other)
{
    if (other.CompareTag("redLight"))
    {
        if (!fermatoAlSemaforo && carRigidbody.velocity.magnitude < 0.1f)
        {
            AddReward(10f);
            fermatoAlSemaforo = true;
            Debug.Log("Mi sono fermato al semaforo rosso.");
        }
        else if (carRigidbody.velocity.magnitude > 0.5f)
        {
            AddReward(-10f);
            Debug.Log("Sono passato con il rosso!");
            EndEpisode();
        }
    }

    if (other.CompareTag("greenLight"))
    {
        fermatoAlSemaforo = false; // Reset del flag quando il semaforo diventa verde
        Riparto();
    }
}
*/

/*
private IEnumerator GradualSpeedIncrease()
{
    float targetSpeed = 10f;
    while (speed < targetSpeed)
    {
        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * 2f); 
        yield return null;
    }
}

private void Riparto()
{
    StartCoroutine(GradualSpeedIncrease());
}
*/
