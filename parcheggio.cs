using UnityEngine;

public class Parcheggio : MonoBehaviour
{
    private int numeroMacchine = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("car"))
        {
            numeroMacchine++;
            SetNumeroMacchine(numeroMacchine);
            Debug.Log("Macchine nel parcheggio: " + numeroMacchine);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            numeroMacchine++;
            SetNumeroMacchine(numeroMacchine);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("car"))
        {
            numeroMacchine--;
            Debug.Log("Macchine nel parcheggio in uscita: " + numeroMacchine);
        }
    }


    public int GetNumeroMacchine()
    {
        return numeroMacchine;
    }

    public void SetNumeroMacchine(int numeroMacchine)
    {
        this.numeroMacchine = numeroMacchine;
    }
}
