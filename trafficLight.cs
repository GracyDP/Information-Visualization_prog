using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public GameObject parcheggio;
    public Light LuceVerde;
    public Light LuceRosso;
    public int countParcheggio = 6; // count di macchine per semaforo rosso

    private int numeroMacchine = 0; // Numero di macchine nel parcheggio

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se � una macchina che entra nel parcheggio
        if (other.CompareTag("car"))
        {
            numeroMacchine++;
            ControllaSemaforo();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Controlla se � una macchina che esce dal parcheggio
        if (other.CompareTag("car"))
        {
            Debug.Log("macchina uscita");
            numeroMacchine--;
            ControllaSemaforo();
        }
    }

    private void ControllaSemaforo()
    {

        if (numeroMacchine >= countParcheggio)
        {
            CambiaLuci(LuceRosso, LuceVerde); // Accende la luce rossa e spegne quella verde
            Debug.Log("luce semaforo Rossa");
        }
        else
        {
            CambiaLuci(LuceVerde, LuceRosso); // Accende la luce verde e spegne quella rossa
            Debug.Log("luce semaforo Rossa");
        }
    }

    private void CambiaLuci(Light luceAccesa, Light luceSpenta)
    {
        // Accende la luce specificata e spegne l'altra
        luceAccesa.enabled = true;
        luceSpenta.enabled = false;
    }
}