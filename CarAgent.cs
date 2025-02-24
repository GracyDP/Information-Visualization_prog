using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Random.Range;
using Unity.VisualScripting;
using System.Linq;

public class carAgent : Agent
{

    //private int count = Random.Next(0, 2);//per far spostare l'addestramento
    public Transform parcheggio;
    public Light segnaleStop;

    //vettori WayPoint
    private List<Vector3> premiPosizioni = new List<Vector3>();
    private List<GameObject> premiList = new List<GameObject>();
    private List<Vector3> ingressoPosizione = new List<Vector3>();
    private List<GameObject> ingressoList = new List<GameObject>();
    private List<Vector3> errorePosizione = new List<Vector3>();
    private List<GameObject> erroreList = new List<GameObject>();


    private int premiRaccolti;
    private int premiMassimi=10;
    private float premio;


    private GameObject svolta;
    private float finalReword = 0;

    public float speed = 5f;
    public Rigidbody carRigidbody;

    //Incrocio
    private GameObject swapObject;
    private Dictionary<GameObject, string> tagOriginali = new Dictionary<GameObject, string>();
    private GameObject currentSwap; // Riferimento all'oggetto swap attuale
    private bool ingressoPrioritario = false; // Quando diventa true, evita i premi

    private void Update(){

        float distanzaIngresso = ingressoList.Min(i => Vector3.Distance(transform.position, i.transform.position));

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
        }

    if (ingressoPrioritario && distanzaIngresso >= 6f)
        {
            addRewordWrapped(-5f);
        }
    }


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

    public override void OnEpisodeBegin()
    {
        // Resetta la posizione e la velocità della macchina
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        
        Vector3 spawnPosition= new Vector3(9.68f, -32.3f, -44.45f);
        Quaternion spawnRotation= new Quaternion.Euler(0, 0, 0);

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
        // Il Ray Perception Sensor 3D aggiunger� automaticamente le osservazioni dei raggi
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
            Debug.Log($"Step massimo raggiunto: {StepCount}. Episodio terminato.");
            addRewordWrapped(-100f);
            terminaConRewardFinale();
        }

        // Penalità costante per evitare il reward hacking e incentivare la velocità
        addRewordWrapped(-0.001f);

        // Reward positivo per andare avanti
        if (forwardAmount > 0)
        {
            AddRewardWrapped(0.05f);
        }
        // Penalità per andare indietro
        else if (forwardAmount < 0)
        {
            AddReward(-0.3f);
        }
    }


    //funzione per penalizzarlo/premiarlo e gestire i rallentamenti/linea STOP
    private void Riparto(){
        if(speed>=8 && speed <=10){
            Debug.Log("Sto ripartendo a velocità: "+ speed+"\n");
            AddReward(1f);
        }    
        if(speed<=0){
            AddReward(-1f);
            EndEpisode();
        }
    }

    //metodo per farla aspettare allo stop
    private IEnumerator WaitforRestart()
{
    Debug.Log("Aspetto 5 secondi...");
    yield return new WaitForSeconds(5f); // Aspetta 2 secondi
    EndEpisode();
    //Per ora aspetta la faccio ripartire poi togliamo l'end dopo che avrà imparato
}


    private void OnCollisionEnter(Collision collision){
        if(collision.CompareTag("car")){
            addRewordWrapped(-100f);
            terminaConRewardFinale();
        }    
    }

    private void OnTriggerEnter(Collider other)
    {
        // Reward positivo per aver toccato l'oggetto con tag "Reward"
        if (other.CompareTag("premio"))
        {
            AddReward(1f);
            EndEpisode();  // Fine episodio dopo aver toccato l'oggetto
        }
        // Penalità per aver toccato un muro
        else if (other.CompareTag("muro"))
        {
            if (premiRaccolti >= premiMassimi)
            {
                addRewordWrapped(-50f);
                terminaConRewardFinale();
            }
            else
            {
                addRewordWrapped(-30f);
                terminaConRewardFinale();
            }
        }

        //se prende il cubo prima dello stop
        if(other.CompareTag("rallenta")){
            if(carRigidbody.velocity.magnitude < speed){ //controllo se la macchina rallenta
                AddReward(1f);
                Debug.Log("sto rallentando\n");
            }    
            else AddReward(-0.3f);
        }

//premio per aver raggionto il parcheggio
        if (other.CompareTag("parcheggio"))
        {
            Debug.Log("Entrato nel parcheggio!");
            addRewordWrapped(300f);
            Debug.Log("Parcheggio riuscito!");
            terminaConRewardFinale();
            
        }

//per quando incrocerà il gatto
        if(other.CompareTag("gatto")){
            if(carRigidbody.Velocity.magnitude!=0)
                AddReward(-1f);//se non si ferma finisce l'episodio
                Debug.Log("Oh no! non potevo investire un gatto\n");
            else {
                    carRigidbody.velocity.magnitude ==0;
                    WaitforRestart();
                    AddReward(1f); 
            }   
            Riparto();
        }
//*************************************************************************************************************

    private void StopLuce(Light segnaleStop)
    {
        // Accende la luce specificata e spegne l'altra
        segnaleStop.enabled = true;
    }    

//prova con spotLight al semaforo
       if(other.CommpareTag("stopLine")){
            if(carRigidbody.velocity.magnitude == 0f){
                Debug.Log("mi sono fermato\n");
                AddReward(1f);
                StarCouroutine(WaitforRestart())//aspetta prima di ripartire
                //aggiungere funzione wait
                }else {
                    AddReward(-1f);
                    EndEpisode();
                }
            if(other.ComparTag("Car"))
                Riparto();
        }

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

        if(other.CompareTag("RedLight")){
            if(carRigidbody.velocity.magnitude!=0){
                Debug.Log("ho attraversato con il semaforo rosso\n");
                AddReward(-1f);}}
        if(other.CompareTag("YellowLight")){
                    if(carRigidbody.velocity.magnitude<speed)AddReward(0.5f);
                    else AddReward(-0.5f);
                    Debug.Log("sto accellerando per non passare con il rosso\n");
        }    
        if(other.CompareTag("GreenLight")){
            AddReward(1f);
            Debug.Log("Ho attraversato con il VERDE");
            }
    } 

     private void terminaConRewardFinale ()
    {
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


