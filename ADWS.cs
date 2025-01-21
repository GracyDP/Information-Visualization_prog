using UnityEngine;

public class ControlloMacchina : MonoBehaviour
{
    //per lo stop

    // WheelCollider delle ruote
    public WheelCollider ruotaAnterioreSinistra;
    public WheelCollider ruotaAnterioreDestra;
    public WheelCollider ruotaPosterioreSinistra;
    public WheelCollider ruotaPosterioreDestra;

    // Oggetti grafici delle ruote
    public Transform trasformRuotaAnterioreSinistra;
    public Transform trasformRuotaAnterioreDestra;
    public Transform trasformRuotaPosterioreSinistra;
    public Transform trasformRuotaPosterioreDestra;

    // Forza motrice e angolo di sterzata
    public float forzaMotore = 1000f;/*ricorda che ho impostato a 1500 la massa quindi ci vuole forza per farla camminare NON MODIFICARE */
    public float angoloSterzata = 30f;
    public float time=5f;
    void FixedUpdate()
    {
        // Controllo input per accelerazione e sterzata
        float inputMotore = Input.GetAxis("Vertical"); // Input per accelerare/frenare
        float inputSterzata = Input.GetAxis("Horizontal"); // Input per girare

        // Applicazione della forza motrice alle ruote motrici
        ruotaAnterioreSinistra.motorTorque = inputMotore * forzaMotore;
        ruotaAnterioreDestra.motorTorque = inputMotore * forzaMotore;

        // Sterzata
        ruotaAnterioreSinistra.steerAngle = inputSterzata * angoloSterzata;
        ruotaAnterioreDestra.steerAngle = inputSterzata * angoloSterzata;

        // Aggiorna la posizione e rotazione grafica delle ruote
        AggiornaPosizioneRuota(ruotaAnterioreSinistra, trasformRuotaAnterioreSinistra);
        AggiornaPosizioneRuota(ruotaAnterioreDestra, trasformRuotaAnterioreDestra);
        AggiornaPosizioneRuota(ruotaPosterioreSinistra, trasformRuotaPosterioreSinistra);
        AggiornaPosizioneRuota(ruotaPosterioreDestra, trasformRuotaPosterioreDestra);
    }

    void AggiornaPosizioneRuota(WheelCollider collider, Transform trasform)
    {
        // Calcola la posizione e la rotazione della ruota
        Vector3 posizione;
        Quaternion rotazione;
        collider.GetWorldPose(out posizione, out rotazione);

        // Aggiorna la posizione e la rotazione dell'oggetto grafico
        trasform.position = posizione;
        trasform.rotation = rotazione;
    }

}

