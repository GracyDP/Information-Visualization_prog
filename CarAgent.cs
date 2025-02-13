using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;

public class carAgent : Agent
{
    public float speed = 10f;
    public Rigidbody carRigidbody;
    private void Start()
    {
       // Debug.Log("ci arrivo1");
        carRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Resetta la posizione e la velocità della macchina
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        Vector3 spawnPosition = new Vector3(9.68f, -32.3f, -44.45f);  // Cambia con la tua posizione desiderata
        Quaternion spawnRotation = Quaternion.Euler(0, 0, 0);        // Rotazione di 90 gradi sull'asse Y

        // Imposta la posizione e rotazione della macchina
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);


        // Posizione di partenza
        // Debug.Log("ci arrivo2");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Osservazioni manuali: velocità della macchina
        sensor.AddObservation(carRigidbody.velocity.magnitude);  // Velocità
        Debug.Log("ci arrivo3");

        // Il Ray Perception Sensor 3D aggiungerà automaticamente le osservazioni dei raggi
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log($"Azioni ricevute: {actions.ContinuousActions.Length}");
        float forwardAmount = actions.ContinuousActions[0];  // Azione per andare avanti/indietro
        float turnAmount = actions.ContinuousActions[1];     // Azione per girare a destra/sinistra

        // Movimento della macchina
        Vector3 move = transform.forward * forwardAmount * speed * Time.deltaTime;
        carRigidbody.MovePosition(transform.position + move);

        // Rotazione della macchina
        transform.Rotate(0, turnAmount * 100f * Time.deltaTime, 0);

        if (StepCount>=1000)
        {
            Debug.Log($"Step massimo raggiunto: {StepCount}. Episodio terminato.");
            AddReward(-0.2f);  // Penalità per non aver completato il compito entro il limite di step
            EndEpisode();       // Riavvia l'episodio
        }

        // Reward positivo per andare avanti
        if (forwardAmount > 0)
        {
            AddReward(0.01f);
        }
        // Penalità per andare indietro
        else if (forwardAmount < 0)
        {
            AddReward(-0.01f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ci arrivo5");
        // Reward positivo per aver toccato l'oggetto con tag "Reward"
        if (other.CompareTag("reward"))
        {
            AddReward(1f);
            EndEpisode();  // Fine episodio dopo aver toccato l'oggetto
        }
        // Penalità per aver toccato un muro
        else if (other.CompareTag("marciapiede"))
        {
            AddReward(-0.5f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

       // Debug.Log("ci arrivo6");
        // Controlli manuali per il test (WASD)
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);  // Avanti/indietro
        continuousActions[1] = Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f);  // Destra/sinistra
    }
}



