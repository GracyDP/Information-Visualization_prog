using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
public class carAgent : Agent
{
    public float speed = 10f;
    public Rigidbody carRigidbody;
    public Transform parcheggio;
    private List<Vector3> premiPosizioni = new List<Vector3>();
    private List<GameObject> premiList = new List<GameObject>();
    private float finalReword = 0;

    private void Start()
    {
        Application.targetFrameRate = 60;
        carRigidbody = GetComponent<Rigidbody>();
        // Trova tutti gli oggetti con il tag "premio" e salva le loro posizioni
        GameObject[] premi = GameObject.FindGameObjectsWithTag("premio");

        foreach (GameObject premio in premi)
        {
            premiList.Add(premio);
            premiPosizioni.Add(premio.transform.position); // Salva la posizione originale
        }

        //Debug.Log($"Premi salvati: {premiList.Count}");
    }

    public override void OnEpisodeBegin()
    {
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        Vector3 spawnPosition = new Vector3(-8.62f, -19.4f, 36.42f);
        Quaternion spawnRotation = Quaternion.Euler(0, 90f, 0);
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        finalReword = 0;
        RespawnAllPremi();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(carRigidbody.velocity.magnitude);
        Vector3 distanzaParcheggio = parcheggio.position - transform.position;
        sensor.AddObservation(distanzaParcheggio.normalized);
        sensor.AddObservation(distanzaParcheggio.magnitude);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = actions.ContinuousActions[0];
        float turnAmount = actions.ContinuousActions[1];

        Vector3 move = transform.forward * forwardAmount * speed * Time.deltaTime;
        carRigidbody.MovePosition(transform.position + move);
        transform.Rotate(0, turnAmount * 100f * Time.deltaTime, 0);

        /*if (StepCount >= 8000)
        {
            Debug.Log($"Step massimo raggiunto: {StepCount}. Episodio terminato.");
            AddReward(-1f);
            EndEpisode();
        }*/

        // Penalità costante per evitare il reward hacking e incentivare la velocità
        addRewordWrapped(-0.001f);


        if (forwardAmount > 0)
        {
            addRewordWrapped(0.05f);
        }
        else if (forwardAmount < 0)
        {
            addRewordWrapped(-0.05f);
        }

        if (Vector3.Distance(transform.position, parcheggio.position) < 2f && carRigidbody.velocity.magnitude < 0.1f)
        {
            addRewordWrapped(50f);
            Debug.Log("Parcheggio completato!");
            terminaConRewardFinale();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("premio"))
        {
            addRewordWrapped(15f);
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("parcheggio"))
        {
            Debug.Log("Entrato nel parcheggio!");
            if (carRigidbody.velocity.magnitude < 0.1f)
            {
                addRewordWrapped(50f);
                Debug.Log("Parcheggio riuscito!");
                terminaConRewardFinale();
            }
        }

        if (other.CompareTag("rewardErrore"))
        {
            Debug.Log("Reward negativo!");
            addRewordWrapped(-10f);
            terminaConRewardFinale();
        }
    }

    private void terminaConRewardFinale()
    {
        // Incentivo progressivo per avvicinarsi al parcheggio
        float distanza = Vector3.Distance(transform.position, parcheggio.position);
        //Debug.Log("Distanza:" + distanza);
        float reward = Mathf.Clamp(59f - (distanza), -60f, 60f);
        Debug.Log("Reward per distanza dal parcheggio:" + reward);
        addRewordWrapped(reward);
        Debug.Log("Final reword:" + finalReword);
        EndEpisode();

    }

    private void addRewordWrapped(float value)
    {
        finalReword += value;
        AddReward(value);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("parcheggio"))
        {
            Debug.Log("Uscito dal parcheggio!");
            addRewordWrapped(-20f);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("muro"))
        {
            addRewordWrapped(-20f);
            terminaConRewardFinale();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);
        continuousActions[1] = Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f);
    }


    private void RespawnAllPremi()
    {
        // Debug.Log($"Riattivando {premiList.Count} premi...");

        for (int i = 0; i < premiList.Count; i++)
        {
            if (premiList[i] != null)
            {
                premiList[i].SetActive(true);
                premiList[i].transform.position = premiPosizioni[i]; // Ripristina la posizione originale

                //Debug.Log($"Premio {premiList[i].name} riposizionato a: {premiPosizioni[i]}");
            }
        }
    }
}