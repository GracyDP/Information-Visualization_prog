using UnityEngine;

//script per impostare il suono della scena

public class StopMusicTrigger : MonoBehaviour
{
    public AudioSource musicSource; // oggetto empty

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("traguardo")) //se la macchina parcheggia la musica si ferma
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop(); // Ferma la musica
            }
        }
    }
}
