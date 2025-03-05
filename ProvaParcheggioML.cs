using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
public class carAgent : Agent
{
    public float speed = 5f;
    public Rigidbody carRigidbody;
    public Transform parcheggio;
    public Transform frontSensor;  // Posizione del sensore frontale
    private List<Vector3> premiPosizioni = new List<Vector3>();
    private List<GameObject> premiList = new List<GameObject>();
    private List<Vector3> ingressoPosizione = new List<Vector3>();
    private List<GameObject> ingressoList = new List<GameObject>();
    private List<Vector3> errorePosizione = new List<Vector3>();
    private List<GameObject> erroreList = new List<GameObject>();
    private List<Vector3> swapPosizione = new List<Vector3>();
    private List<GameObject> swapList = new List<GameObject>();
    private int premiRaccolti;
    private int premiMassimi=10;
    private float premio;
    GameObject svolta;
    private float finalReword = 0;
    private bool passanteVisibile = false;
    GameObject swapObject;
    private Dictionary<GameObject, string> tagOriginali = new Dictionary<GameObject, string>();
    private GameObject currentSwap; // Riferimento all'oggetto swap attuale
    private bool ingressoPrioritario = false; // Quando diventa true, evita i premi


    private void Start()
    {
        Application.targetFrameRate = 60;
        carRigidbody = GetComponent<Rigidbody>();
        // Trova tutti gli oggetti con il tag "premio" e salva le loro posizioni
        GameObject[] premi = GameObject.FindGameObjectsWithTag("premio");
        GameObject[] rewardErrori = GameObject.FindGameObjectsWithTag("rewardErrore");
        GameObject[] ingressi = GameObject.FindGameObjectsWithTag("ingresso");
        GameObject[] swaps = GameObject.FindGameObjectsWithTag("swap");

        svolta = GameObject.FindGameObjectWithTag("Svolta");
        swapObject = GameObject.FindGameObjectWithTag("swap");
        foreach (GameObject premio in premi)
        {
            premiList.Add(premio);
            premiPosizioni.Add(premio.transform.position); // Salva la posizione originale
            tagOriginali[premio] = "premio"; // Salva il tag originale
        }
        foreach (GameObject rewardErrore in rewardErrori)
        {
            erroreList.Add(rewardErrore);
            errorePosizione.Add(rewardErrore.transform.position); //salva la posizione originale
            tagOriginali[rewardErrore] = "rewardErrore"; // Salva il tag originale
        }
        foreach (GameObject ingresso in ingressi)
        {
            ingressoList.Add(ingresso);
            ingressoPosizione.Add(ingresso.transform.position); // Salva la posizione originale
            tagOriginali[ingresso] = "ingresso"; // Salva il tag originale
        }

        foreach (GameObject swap in swaps)
        {
            swapList.Add(swap);
            swapPosizione.Add(swap.transform.position); // Salva la posizione originale
            tagOriginali[swap] = "swap"; // Salva il tag originale
        }

        //Debug.Log($"Premi salvati: {premiList.Count}");
    }

    private void Update()
    {
        if (Vector3.Dot(transform.up, Vector3.up) < 0.3f) // Controlla se l'auto è capovolta
        {
            Debug.Log("Macchina ribaltata! Episodio terminato.");

            addRewordWrapped(-10f); // Penalità per il ribaltamento
            terminaConRewardFinale();
        }


        // Controlla se vede il passante
        passanteVisibile = VedePassante();

        if (passanteVisibile)
        {
            carRigidbody.velocity = Vector3.zero;  // Ferma la macchina
            Debug.Log("Passante rilevato! L'auto si ferma.");
        }
    }


        /*float distanzaIngresso = ingressoList.Min(i => Vector3.Distance(transform.position, i.transform.position));

        if (distanzaIngresso < 6f)
        {
            ingressoPrioritario = true; // Attiva la priorità agli ingressi


            if (distanzaIngresso > 2f && StepCount>3000)
            {
                addRewordWrapped(-0.05f);
            }
            else
            {
                addRewordWrapped(2f - (distanzaIngresso * 0.15f));
            }
            //Debug.Log("sono vicina");
        }*/

        /*if (ingressoPrioritario && distanzaIngresso >= 6f)
        {
            addRewordWrapped(-5f);
          //  Debug.Log("Mi allontano");// Penalità se si allontana dopo essersi avvicinato
        }

        //Debug.Log($"Distanza dall'ingresso più vicino: {distanzaIngresso}");
    }
        */
    public override void OnEpisodeBegin()
    {
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;

        // Due possibili posizioni di spawn
        Vector3[] spawnPositions = new Vector3[]
        {
        new Vector3(-8.62f, -19.4f, 36.42f), // Posizione 1
        new Vector3(21.9f, -19.4f, 12.6f)    // Posizione 2
        };

        Quaternion[] spawnRotations = new Quaternion[]
        {
        Quaternion.Euler(0, 90f, 0), // Rotazione per posizione 1
        Quaternion.Euler(0, -90f, 0)  // Rotazione per posizione 2
        };

        // Seleziona casualmente una posizione
        int randomIndex = UnityEngine.Random.Range(0, spawnPositions.Length);
        transform.SetPositionAndRotation(spawnPositions[randomIndex], spawnRotations[randomIndex]);

        finalReword = 0;
        premiRaccolti = 0;
        premio = 2;
        ingressoPrioritario = false;
        // **Ripristina i tag originali**
        foreach (var obj in tagOriginali)
        {
            obj.Key.tag = obj.Value;
        }
        RespawnAllPremi();

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(carRigidbody.velocity.magnitude);
        if (!ingressoPrioritario)
        {
            Vector3 distanzaPremio = premiList.FirstOrDefault(p => p.activeSelf)?.transform.position - transform.position ?? Vector3.zero;
            sensor.AddObservation(distanzaPremio.normalized);
            sensor.AddObservation(distanzaPremio.magnitude);
        }
        else
        {
            Vector3 distanzaIngresso = ingressoList.FirstOrDefault(i => i.activeSelf)?.transform.position - transform.position ?? Vector3.zero;
            sensor.AddObservation(distanzaIngresso.normalized);
            sensor.AddObservation(distanzaIngresso.magnitude);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        if (passanteVisibile)
        {

            carRigidbody.velocity = Vector3.zero;
            Debug.Log("L'auto si ferma per il passante.");
            return; // Esce senza eseguire il movimento
        }



        float forwardAmount = actions.ContinuousActions[0];
        float turnAmount = actions.ContinuousActions[1];

        // Applica forza progressiva per accelerare
        Vector3 force = transform.forward * forwardAmount * speed;
        carRigidbody.AddForce(force, ForceMode.Acceleration);
       // Debug.Log(carRigidbody.velocity.magnitude);
        transform.Rotate(0, turnAmount * 50f * Time.deltaTime, 0);

        if (StepCount >= 5000)
        {
            if (premiRaccolti < premiMassimi)
            {

                Debug.Log($"Step massimo raggiunto: {StepCount}. Episodio terminato.");
                addRewordWrapped(-50f);
                terminaConRewardFinale(); 
            }else
            {
                Debug.Log($"Step massimo raggiunto: {StepCount}. Episodio terminato.");
                addRewordWrapped(-150f);
                terminaConRewardFinale();
            }
        }

        // Penalità costante per evitare il reward hacking e incentivare la velocità
        addRewordWrapped(-0.001f);

        
        if (forwardAmount > 0)
        {
            addRewordWrapped(0.05f);
        }
        else if (forwardAmount < 0)
        {
            addRewordWrapped(-0.1f);
        }

        CheckForEntrance();
        // Calcola la distanza dall'ingresso più vicino
        /* float minDistanzaIngresso = float.MaxValue;

        foreach (GameObject ingresso in ingressoList)
        {
            float distanza = Vector3.Distance(transform.position, ingresso.transform.position);
            if (distanza < minDistanzaIngresso)
            {
                minDistanzaIngresso = distanza;
            }
        }
        float rewardVicino = Mathf.Clamp(10f - minDistanzaIngresso, 0f, 10f);
        if (Math.Abs(minDistanzaIngresso) < 5)
        {
            addRewordWrapped(rewardVicino * 0.01f);
        }*/
        //Debug.Log($"Distanza ingresso: {minDistanzaIngresso}, Reward progressiva: {rewardVicino}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("premio"))
        {
            if (!ingressoPrioritario)
            {
                //Debug.Log("Ho preso il premio");
                addRewordWrapped(premio);
                premiRaccolti++;
                /*if (premiRaccolti>=6)
                    premio = premio * 0.9f;
                */
                other.gameObject.SetActive(false);
            }
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("parcheggio"))
        {
            Debug.Log("Entrato nel parcheggio!");
            addRewordWrapped(300f);
            Debug.Log("Parcheggio riuscito!");
            terminaConRewardFinale();
            
        }

        //if (other.CompareTag("greenLight")) Debug.Log("luceVerde");
        //if (other.CompareTag("redLight")) Debug.Log("luceRossa");


        if (other.CompareTag("rewardErrore"))
        {
            if (premiRaccolti > 0)
            {
                if (premiRaccolti >= premiMassimi)
                {
                    Debug.Log("Reward negativo!");
                    addRewordWrapped(-100f);
                    terminaConRewardFinale();
                }
                else
                {
                    Debug.Log("Reward negativo!");
                    addRewordWrapped(-50f);
                    terminaConRewardFinale();
                }
            }
            else
            {
                Debug.Log("Reward negativo!");
                addRewordWrapped(-30f);
                terminaConRewardFinale();
            }
        }
        if (other.CompareTag("Svolta"))
        {
            Vector3 curvaDirezione = other.transform.forward;  // Direzione della curva
            other.gameObject.SetActive(false);
            float angolo = Vector3.Angle(transform.forward, curvaDirezione);

            if (angolo < 30f)  // Se l’angolo è inferiore a 30°, la svolta è buona
            {
                addRewordWrapped(50f);
                Debug.Log("Curva presa bene!");
            }
            else
            {
                addRewordWrapped(-60f);  // Penalità se l'angolo è sbagliato
                Debug.Log("Curva presa male!");
                terminaConRewardFinale();
            }
        }

        if (other.CompareTag("ingresso"))
        {
            addRewordWrapped(100f);
            ingressoPrioritario = true; // Ora cerca solo ingressi
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("swap"))
        {
            //other.gameObject.SetActive(false);
            Debug.Log("Attivato swap dei tag!");
            other.gameObject.SetActive(false);
            SwapTags();
        }

        if (other.CompareTag("muro"))
        {
            if (premiRaccolti >= premiMassimi)
            {
                addRewordWrapped(-100f);
                terminaConRewardFinale();
            }
            else
            {
                addRewordWrapped(-50f);
                terminaConRewardFinale();
            }
        }


    }

    private void terminaConRewardFinale ()
    {
        // Incentivo progressivo per avvicinarsi al parcheggio
        /*float distanza = Vector3.Distance(transform.position, parcheggio.position);
        //Debug.Log("Distanza:" + distanza);
        float reward = Mathf.Clamp(59f - (distanza), -60f, 60f)*0.5f;
        Debug.Log("Reward per distanza dal parcheggio:" + reward);
        addRewordWrapped(reward);*/
        float distanzaIngresso = ingressoList.Min(i => Vector3.Distance(transform.position, i.transform.position));
        Debug.Log("Distanza ingresso:" + distanzaIngresso);
        Debug.Log("Final reword:" + finalReword);
        EndEpisode();

    }

    private void addRewordWrapped (float value)
    {
        finalReword += value;
        AddReward(value);
    }

    

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("car"))
        {
            addRewordWrapped(-100f);
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
        for (int i = 0; i < erroreList.Count; i++)
        {
            if (erroreList[i] != null)
            {
                erroreList[i].SetActive(true);
                erroreList[i].transform.position = errorePosizione[i]; // Ripristina la posizione originale
            }
        }
        for (int i = 0; i < ingressoList.Count; i++)
        {
            if (ingressoList[i] != null)
            {
                ingressoList[i].SetActive(true);
                ingressoList[i].transform.position = ingressoPosizione[i]; // Ripristina la posizione originale
            }
        }

        for (int i = 0; i < swapList.Count; i++)
        {
            if (swapList[i] != null)
            {
                swapList[i].SetActive(true);
                swapList[i].transform.position = swapPosizione[i]; // Ripristina la posizione originale
            }
        }
        //svolta.SetActive(true);
        //swapObject.SetActive(true);
    }
    // Metodo per verificare se il passante è davanti all'agente
    private bool VedePassante()
    {
        RaycastHit hit;
        Vector3 avanti = transform.forward;

        // Raycast davanti alla macchina per vedere se c'è il passante
        if (Physics.Raycast(transform.position, avanti, out hit, 4f))
        {
            //Debug.Log($"Oggetto rilevato: {hit.collider.gameObject.name} con tag {hit.collider.tag}");
            if (hit.collider.CompareTag("passante"))
            {
              //  Debug.Log("Passante visto! L'auto si ferma.");
                return true;
            }
        }
       // Debug.Log("Nessun passante in vista, l'auto può continuare.");
        return false;
    }

    private void SwapTags()
    {
        // Trova tutti gli oggetti con il tag "premio" e salva una copia in lista
        GameObject[] premi = GameObject.FindGameObjectsWithTag("premio");
        List<GameObject> premiList = new List<GameObject>(premi);

        // Trova tutti gli oggetti con il tag "rewardErrore" e salva una copia in lista
        GameObject[] rewardErrori = GameObject.FindGameObjectsWithTag("rewardErrore");
        List<GameObject> rewardErroriList = new List<GameObject>(rewardErrori);

        // Cambia i tag usando le liste
        foreach (GameObject premio in premiList)
        {
            premio.tag = "rewardErrore";
        }

        foreach (GameObject rewardErrore in rewardErroriList)
        {
            rewardErrore.tag = "premio";
        }
    }


    private void CheckForEntrance()
    {
        float angle = 15f; // Angolo per i raggi laterali

        // Direzioni dei raggi
        Vector3 forward = transform.forward;
        Vector3 right = Quaternion.Euler(0, angle, 0) * forward; // Raggio destro
        Vector3 left = Quaternion.Euler(0, -angle, 0) * forward; // Raggio sinistro

        // Controlla con tre raggi
        if (CheckRay(forward) || CheckRay(right) || CheckRay(left))
        {
            Debug.Log("Ingresso rilevato!");
        }
    }

    private bool CheckRay(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(frontSensor.position, direction, out hit, 6f))
        {
            if (hit.collider.CompareTag("ingresso"))
            {
                float distance = hit.distance;
                float reward = Mathf.Lerp(1.0f, 0.1f, distance / 6f);
                addRewordWrapped(reward);
                Debug.DrawRay(frontSensor.position, direction * distance, Color.green, 0.5f);
                return true;
            }
        }
        return false;
    }





}
