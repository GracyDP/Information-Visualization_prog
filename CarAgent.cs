using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Random.Range;


public class carAgent : Agent
{

    //private int count = Random.Next(0, 2);//per far spostare l'addestramento

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
        
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        //if(count%2=0){
            spawnPosition = new Vector3(9.68f, -32.3f, -44.45f);  // Cambia con la tua posizione desiderata
            spawnRotation = Quaternion.Euler(0, 0, 0);        // Rotazione di 90 gradi sull'asse Y
        /*} else{ spawnPosition = new Vector3(24.5f, -19.5f, 37f);
                spawnRotation = Quaternion.Euler(0, 0, 0);}*/
        // Imposta la posizione e rotazione della macchina
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);


        // Posizione di partenza
        // Debug.Log("ci arrivo2");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Osservazioni manuali: velocit� della macchina
        sensor.AddObservation(carRigidbody.velocity.magnitude);  // Velocit�
        //Debug.Log("ci arrivo3");

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

        if (StepCount>=1000)
        {
            Debug.Log($"Step massimo raggiunto: {StepCount}. Episodio terminato.");
            AddReward(-0.2f);  // Penalit� per non aver completato il compito entro il limite di step
            EndEpisode();       // Riavvia l'episodio
        }

        // Reward positivo per andare avanti
        if (forwardAmount > 0)
        {
            AddReward(0.5f);
        }
        // Penalità per andare indietro
        else if (forwardAmount < 0)
        {
            AddReward(-0.5f);
        }//HO AUMENTATO IL REWARD ****************************************************
    }


    //funzione per penalizzarlo/premiarlo e gestire i rallentamenti/linea STOP

        private void Riparto(){ //
        if(speed>=8 && speed <=10){
            Debug.Log("Sto ripartendo a velocità: "+speed);
            AddReward(1f);
        }    
        if(speed<=0){
            AddReward(-1f);//kill
            EndEpisode();}
    }

    //metodo per farla aspettare allo stop
    private IEnumerator WaitforRestart()
{
    Debug.Log("Aspetto 5 secondi...");
    yield return new WaitForSeconds(5f); // Aspetta 2 secondi
    EndEpisode();
    //Per ora aspetta la faccio ripartire poi togliamo l'end dopo che avrà imparato
}


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("ci arrivo5");
        // Reward positivo per aver toccato l'oggetto con tag "Reward"
        if (other.CompareTag("premio"))
        {
            AddReward(1f);
            EndEpisode();  // Fine episodio dopo aver toccato l'oggetto
        }
        // Penalit� per aver toccato un muro
        else if (other.CompareTag("muro"))
        {
            AddReward(-0.5f);
            EndEpisode();
        }

        //se prende il cubo prima dello stop
        if(other.CompareTag("rallenta")){
            if(carRigidbody.velocity.magnitude < speed){ //controllo se la macchina rallenta
                AddReward(1f);
                Debug.Log("sto rallentando\n");
            }    
            else AddReward(-0.3f);
        }

        //per quando incrocerà la palla/animale che rotola
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


        //Gestione Luci semaforo

        if(other.CompareTag("RedLight")){
            if(carRigidbody.velocity.magnitude!=0){
                Debug.Log("ho attraversato con il semaforo rosso\n");
                AddReward(-1f);
            }else if(other.CompareTag("YellowLight")){
                    if(carRigidbody.velocity.magnitude>speed)AddReward(0.5f);
                    else AddReward(-0.5f);
                    Debug.Log("sto accellerando per non passare con il rosso\n");
            }else AddReward(1f);
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



