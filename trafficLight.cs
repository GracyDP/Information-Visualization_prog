using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public Light LuceVerde;
    public Light LuceRosso;
    public int countParcheggio = 10; // Numero massimo di macchine prima che il semaforo diventi rosso
    public Parcheggio parcheggio;  // Riferimento al parcheggio
    public carAgent carAgent;
    public GameObject Verde;
    public GameObject Rosso;
    public GameObject ingresso1;
    public GameObject ingresso2;
    public GameObject ingresso3;
    public GameObject ingresso4;
    public GameObject ingresso5;
    public GameObject ingresso6;
    public GameObject ingresso7;
    public GameObject parcheggioObject1;
    public GameObject parcheggioObject2;

    private void Update()
    {
        ControllaSemaforo();
        parcheggioDisponibile();
    }

    private void ControllaSemaforo()
    {
        int numeroMacchine = parcheggio.GetNumeroMacchine();  // Ottiene il numero di macchine
        //Debug.Log("numero macchine:"+numeroMacchine);
        if (numeroMacchine >= countParcheggio)
        {
            CambiaLuci(LuceRosso, LuceVerde); // Accende il rosso, spegne il verde
            Verde.SetActive(false);
            Rosso.SetActive(true);
            Debug.Log("Semaforo ROSSO");
        }
        else
        {
            CambiaLuci(LuceVerde, LuceRosso); // Accende il verde, spegne il rosso
            Debug.Log("Semaforo VERDE");
            Rosso.SetActive(false);
            Verde.SetActive(true);
        }
    }

    private void CambiaLuci(Light luceAccesa, Light luceSpenta)
    {

        luceAccesa.enabled = true;
        luceSpenta.enabled = false;
    }
    public void SetCountParcheggio(int newCount)
    {
        countParcheggio = newCount;
        //Debug.Log(gameObject.name + " - Nuovo limite parcheggio: " + countParcheggio);
    }


    private void parcheggioDisponibile()
    {
        int numeroParcheggio = carAgent.getRandomInt();
        if (numeroParcheggio == 1) {
            ingresso1.SetActive(false);
            ingresso2.SetActive(false);
            ingresso3.SetActive(false);
            ingresso4.SetActive(false);
            ingresso5.SetActive(true);
            ingresso6.SetActive(true);
            ingresso7.SetActive(true);
            parcheggioObject1.SetActive(false);
            parcheggioObject2.SetActive(true);
        }
        if (numeroParcheggio == 0)
        {
            ingresso1.SetActive(true);
            ingresso2.SetActive(true);
            ingresso3.SetActive(true);
            ingresso4.SetActive(true);
            ingresso5.SetActive(false);
            ingresso6.SetActive(false);
            ingresso7.SetActive(false);
            parcheggioObject1.SetActive(true);
            parcheggioObject2.SetActive(false);
        }
    }
}
