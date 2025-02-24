using UnityEngine;

public class StopMusicTrigger : MonoBehaviour
{
    public AudioSource ambiente; // Riferimento alla musica di ambiente
    public AudioSource frenata; // Riferimento al suono di frenata

    private void OnTriggerEnter(Collider other)
    {
        // Se il player raggiunge il traguardo, ferma la musica
        if (other.CompareTag("traguardo"))
        {
            if (ambiente.isPlaying)
            {
                ambiente.Stop();
            }
        }

        // Se il player passa sulla stopLine, ferma la musica e inizia la frenata
        if (other.CompareTag("stopLine"))
        {
            if (ambiente.isPlaying)
            {
                ambiente.Stop(); // Ferma la musica
                frenata.Play();  // Suona il suono di frenata
            }

            StartCoroutine(ResumeMusicAfterDelay()); // Avvia la coroutine per riprendere la musica dopo un ritardo
        }


         if (other.CompareTag("gatto"))
        {
            if (ambiente.isPlaying)
            {
                ambiente.Stop(); // Ferma la musica
                frenata.Play();  // Suona il suono di frenata
            }
            //parte il miagolio dal suo script
            StartCoroutine(ResumeMusicAfterDelay()); // Avvia la coroutine per riprendere la musica dopo un ritardo
        }
    }

    private IEnumerator ResumeMusicAfterDelay()
    {
        //aggiustiamo i secondi se non risulta parallelo alla partenza
        yield return new WaitForSeconds(4f); //dopo lo stop aspetta e poi quando riparte la macchina riparte la musica

        if (!ambiente.isPlaying) // Se la musica è ferma, riprende
        {
            ambiente.Play();
        }
    }
}
