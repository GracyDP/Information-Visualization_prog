using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public Light LuceVerde; 
    public Light LuceRosso; 
    public int countParcheggio = 10; // count di macchine per semaforo rosso

    private int numeroMacchine = 0; // Numero di macchine nel parcheggio

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se è una macchina che entra nel parcheggio
        if (other.CompareTag("Car"))
        {
            numeroMacchine++;
            ControllaSemaforo();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Controlla se è una macchina che esce dal parcheggio
        if (other.CompareTag("Car"))
        {
            numeroMacchine--;
            ControllaSemaforo();
        }
    }

    private void ControllaSemaforo()
    {

        if (numeroMacchine >= countParcheggio)
        {
            CambiaLuci(semaforoRosso, LuceVerde); // Accende la luce rossa e spegne quella verde
        }
        else
        {
            CambiaLuci(semaforoVerde, LuceRosso); // Accende la luce verde e spegne quella rossa
        }
    }

    private void CambiaLuci(Light luceAccesa, Light luceSpenta)
    {
        // Accende la luce specificata e spegne l'altra
        luceAccesa.enabled = true;
        luceSpenta.enabled = false;
    }
}
