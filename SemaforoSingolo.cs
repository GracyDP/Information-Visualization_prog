using System.Collections;
using UnityEngine;

public class SemaforoSingolo : MonoBehaviour {

    public Light luce; //spotLight
    public float tempoRosso = 8f;
    public float tempoGiallo = 5f;
    public float tempoVerde = 15f;

    public enum StatoSemaforo { Rosso, Giallo, Verde } //possibili valori che può assumere il semaforo
    public StatoSemaforo stato;

    void Start() {
        StartCoroutine(CicloSemaforo());
    } //all'inizio del game parte il semaforo

    //Gestione autonoma della luce
    IEnumerator CicloSemaforo() {
        while (true) {
            stato = StatoSemaforo.Rosso;
            luce.color = Color.red;  // Usare direttamente Color.red
            luce.tag = "redLight";
            yield return new WaitForSeconds(tempoRosso);
            
            stato = StatoSemaforo.Giallo;
            luce.color = Color.yellow;  // Usare direttamente Color.yellow
            luce.tag = "yellowLight";
            yield return new WaitForSeconds(tempoGiallo);

            stato = StatoSemaforo.Verde;
            luce.color = Color.green;  // Usare direttamente Color.green
            luce.tag = "greenLight";
            yield return new WaitForSeconds(tempoVerde);
        }
    }
}
