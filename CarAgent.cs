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

    //private int count = Random.Next(0, 2);//per far spostare l'addestramento
    public Transform parcheggio;
    private Vector3 pausa;

    //vettori WayPoint
    private List<Vector3> premiPosizioni = new List<Vector3>();
    private List<GameObject> premiList = new List<GameObject>();
    private List<Vector3> ingressoPosizione = new List<Vector3>();
    private List<GameObject> ingressoList = new List<GameObject>();
    private List<Vector3> errorePosizione = new List<Vector3>();
    private List<GameObject> erroreList = new List<GameObject>();

    public Transform frontSensor;


    private float collisionTime = 0f;  // Tempo trascorso in collisione
    private bool isInCollision = false;  // Verifica se è in collisione

    private bool ferma=false;


    private int premiRaccolti;
    private int premiMassimi=10;


    private GameObject svolta;
    private float finalReword = 0;
    private float premio;

    public float speed = 10f;
    public Rigidbody carRigidbody;

    //Incrocio
    private GameObject swapObject;
    private Dictionary<GameObject, string> tagOriginali = new Dictionary<GameObject, string>();
    private GameObject currentSwap; // Riferimento all'oggetto swap attuale
    private bool ingressoPrioritario = false; // Quando diventa true, evita i premi


     private void Start()
    {
        Application.targetFrameRate = 60;
        carRigidbody = GetComponent<Rigidbody>();

        // Trova tutti gli oggetti con il tag "premio",... e salva le loro posizioni
        GameObject[] premi = GameObject.FindGameObjectsWithTag("premio");
        GameObject[] rewardErrori = GameObject.FindGameObjectsWithTag("rewardErrore");
        GameObject[] ingressi = GameObject.FindGameObjectsWithTag("ingresso");

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

    }

    private float altezza;

   private void Update(){
    
    if( Vector3.Dot(transform.up, Vector3.up) < 0.3f){
        Debug.Log("non voiglio accappottarmiìiiiii");
        AddReward(-30f);
        EndEpisode();
    }

   }



    public override void OnEpisodeBegin()
    {
        // Resetta la posizione e la velocità della macchina
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        
        Vector3 spawnPosition= new Vector3(22.99f, -19.07f, 29.93f);
        Quaternion spawnRotation= Quaternion.Euler(0, 180, 0);

        // Imposta la posizione e rotazione della macchina
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
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

        private void addRewordWrapped (float value)
    {
        finalReword += value;
        AddReward(value);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Osservazioni manuali: velocit� della macchina
        sensor.AddObservation(carRigidbody.velocity.magnitude);  // Velocit�
       
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
        // Il Ray Perception Sensor 3D aggiungemautomaticamente le osservazioni dei raggi
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = actions.ContinuousActions[0];  // Azione per andare avanti/indietro
        float turnAmount = actions.ContinuousActions[1];     // Azione per girare a destra/sinistra

        // Applica forza progressiva per accelerare
        Vector3 force = transform.forward * forwardAmount * speed;
        carRigidbody.AddForce(force, ForceMode.Acceleration);
        transform.Rotate(0, turnAmount * 50f * Time.deltaTime, 0);


       /*if (StepCount >= 5000)
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
            Debug.Log($"Step massimo raggiunto: {StepCount}. Episodio terminato.");
            addRewordWrapped(-100f);
            terminaConRewardFinale();
        }*/

        // Penalità costante per evitare il reward hacking e incentivare la velocità
        addRewordWrapped(-0.001f);

        // Reward positivo per andare avanti
        if (forwardAmount > 0)
        {
            addRewordWrapped(0.5f);
        }
        // Penalità per andare indietro
        else if (forwardAmount < 0)
        {
            AddReward(-1f);
        }

        CheckForEntrance();
    }


    private void StopLuce(Light segnaleStop)
    {
        // Accende la luce specificata e spegne l'altra
        segnaleStop.enabled = true;
    }  

    //funzione per penalizzarlo/premiarlo e gestire i rallentamenti/linea STOP
    private void Riparto(){  
        if(carRigidbody.velocity.magnitude>0){
            AddReward(80f);
            Debug.Log("sto ripartendo");
        }
    }

    //metodo per farla aspettare allo stop
    private IEnumerator WaitforRestart()
{
    Debug.Log("Aspetto 5 secondi...");
    yield return new WaitForSeconds(20f); // Aspetta 5 secondi
    AddReward(50f);
    //Per ora aspetta la faccio ripartire poi togliamo l'end dopo che avrà imparato
}


    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("car")){
            addRewordWrapped(-300f);
            terminaConRewardFinale();
        }    
    }
    
    

    private void OnTriggerEnter(Collider other)
    {
        // Reward positivo per aver toccato l'oggetto con tag "Reward"
        if (other.CompareTag("premio"))
        {
            AddReward(4f);
            other.gameObject.SetActive(false);
            //Debug.Log("sto andando FORTE");
        }
        // Penalità per aver toccato un muro
        else if (other.CompareTag("muro"))
        {
            if (premiRaccolti >= premiMassimi)
            {
                addRewordWrapped(-100f);
                terminaConRewardFinale();
            }
            else
            {
                addRewordWrapped(-150f);
                terminaConRewardFinale();
            }
        }

        //se prende il cubo prima dello stop

        if(other.CompareTag("redLight")){
            Debug.Log("ho captato la luce rossa");


        }

//premio per aver raggionto il parcheggio
        if (other.CompareTag("parcheggio"))
        {
            Debug.Log("Entrato nel parcheggio!");
            addRewordWrapped(300f);
            Debug.Log("Parcheggio riuscito!");
            //WaitForSeconds();
            terminaConRewardFinale();
            
        }

//per quando incrocerà il gatto
        if(other.CompareTag("passante")){
            if(carRigidbody.velocity.magnitude==0){
                WaitforRestart();
                AddReward(50f);

            } else{
                AddReward(-150f);//se non si ferma finisce l'episodio
                Debug.Log("Oh no! non potevo investire un gatto\n");
                EndEpisode();
            }
        }

//*************************************************************************************************************  


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




//Gestione "assistita" della curva        
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

//incrocio e possibili vie 
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
            SwapTags();
        }

        
        //Gestione Luci semaforo

        if(other.CompareTag("redLight")){
            if(carRigidbody.velocity.magnitude!=0){
                Debug.Log("ho attraversato con il semaforo rosso\n");
                AddReward(-10f);
            }
        }
        if(other.CompareTag("yellowLight")){
                    if(carRigidbody.velocity.magnitude<speed)AddReward(0.5f);
                    else AddReward(-0.5f);
                    Debug.Log("sto accellerando per non passare con il rosso\n");
        }    
        if(other.CompareTag("greenLight")){
            AddReward(1f);
            Debug.Log("Ho attraversato con il VERDE");
            }


        if(other.CompareTag("car")){
            addRewordWrapped(-150f);
            EndEpisode();
        }




        if(other.CompareTag("stopLine")){
            WaitforRestart();
            if(carRigidbody.velocity.magnitude==0){
                AddReward(50f);
                Riparto();
                other.gameObject.SetActive(false);
                Debug.Log("oggetto disattivato");
            }else {AddReward(-100f); 
                    //EndEpisode();
                    Debug.Log("NON MI SONO FERMATO");
        }
    }
}
//Gestione luce semafori
/*
RaycastHit hit;
if (Physics.Raycast(transform.position, transform.forward, out hit, distanzaRaggio)) {
    if (hit.collider.CompareTag("Semaforo")) {
        SemaforoController semaforo = hit.collider.GetComponent<SemaforoController>();
        if (semaforo.stato == StatoSemaforo.Rosso) {
            StopCar();
        }
    }
}
*/


//Gestione STOP
private void CheckForStop()
    {
        float angle = 30f; // Angolo per i raggi laterali

        // Direzioni dei raggi
        Vector3 forward = transform.forward;
        Vector3 right = Quaternion.Euler(0, angle, 0) * forward; // Raggio destro
        Vector3 left = Quaternion.Euler(0, -angle, 0) * forward; // Raggio sinistro

        // Controlla con tre raggi
        if (CheckRay(forward) || CheckRay(right) || CheckRay(left))
        {
            Debug.Log("Ho preso lo stop");
        }
    }


//Segnale di stop
/*private void OnCollisionStay(Collision coll){
    if(coll.gameObject.CompareTag("stopLine")){
        Debug.Log("sono vicino lo stop");
        if (!isInCollision)  // Se non è già in collisione, inizia il timer
        {
            isInCollision = true;
            collisionTime = 0f;  // Inizia il conteggio
        }

        // Aggiungi ricompensa per ogni secondo che rimane in collisione
        collisionTime += Time.deltaTime;

        if (collisionTime >= 1f)  // Ogni secondo che passa
        {
            AddReward(10f);  // Aggiungi ricompensa
            collisionTime = 0f;  // Reset del timer per il prossimo secondo
        }

        if (collisionTime >= 3f)  // Dopo 3 secondi
        {
            Debug.Log("3 secondi in collisione. Aggiunto ricompensa finale.");
            isInCollision = false;  // Ferma la collisione
            //coll.gameObject.SetActive(false);
        }
    }
    else
    {
        // Se l'agente non è più in collisione, resetta il timer
        isInCollision = false;
        collisionTime = 0f;
        if(carRigidbody.velocity.magnitude==0) AddReward(-80f);
    }

    if(isInCollision){
        AddReward(-40f);
        Debug.Log("sono passato senza fermarmi");
        
    }
        
}*/




//PARCHEGGIO-GESTIONE
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
            if (hit.collider.CompareTag("stopSignal"))
            {
                float distance = hit.distance;
                if(distance<=10f && distance>2){
                    addRewordWrapped(20f);
                    return true;}
                    else addRewordWrapped(-30f);
            }
        }
        return false;
    }



     private void terminaConRewardFinale ()
    {
        float distanzaIngresso = ingressoList.Min(i => Vector3.Distance(transform.position, i.transform.position));
        Debug.Log("Distanza ingresso:" + distanzaIngresso);
        Debug.Log("Final reword:" + finalReword);
        EndEpisode();

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);  // Avanti/indietro
        continuousActions[1] = Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f);  // Destra/sinistra
    }

    private void RespawnAllPremi()
    {
        //controllo sui Reward del vettore
        for (int i = 0; i < premiList.Count; i++)
        {
            if (premiList[i] != null)
            {
                premiList[i].SetActive(true);
                premiList[i].transform.position = premiPosizioni[i]; // Ripristina la posizione originale
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


}
