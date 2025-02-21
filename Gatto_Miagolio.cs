using UnityEngine;
using System.Collections;
public class StopMusicTrigger : MonoBehaviour
{
    public AudioSource miagolio; // Riferimento al miagolio del gatto



    private GameObject gatto;

    private void Start(){
        Vector3 spawn1;
        Quaternion rotazione2;

        spawn= new Vector3(19.63,-19.11,29.3);
        rorazione.Euler(0,0,0);


    }

    private void OnTriggerEnter(Collider other)
    {
        // Se il player raggiunge il traguardo, ferma la musica
        if (other.CompareTag("gatto"))
        {
            Debug.Log("C'è un gatto che sta passando!!");
            if (ambiente.isPlaying)
            {   
                //ci sarà la frenata soudn che parte dallo script sound.cs
                ambiente.Stop();
                miagolio.Play();
            }
        }
        StartCoroutine(ResumeMusicAfterDelay()); // Avvia la coroutine per riprendere la musica
       
    }
        private IEnumerator ResumeMusicAfterDelay()
    {
        //aggiustiamo i secondi se non risulta parallelo alla partenza
        yield return new WaitForSeconds(10f); //quando riparte la macchina riparte la musica
        if (!ambiente.isPlaying) // Se la musica è ferma, riprende
        {
            ambiente.Play();
            Destroy(gatto);//lo facciamo scomparire
            //riappare nell'altro incrocio e lo facciamo sedere...devo vedere i comandi quindi per ora spwana solo dobbiamo aprire il pacchetto in unity
            
            spawn= new Vector3(19.63 -19.11 46.6);
            rorazione.Euler(0,0,0);

        }
    }

}