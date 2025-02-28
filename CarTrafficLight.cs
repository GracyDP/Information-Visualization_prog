using UnityEngine;

public class CarAgent : MonoBehaviour {
    public float velocità = 10f;
    public float distanzaRaggio = 15f; // Distanza a cui controllare il semaforo
    public int penalità = 0;

    private bool deveFermarsi = false;
    private bool staRallentando = false;

    void Update() {
        if (deveFermarsi) {
            return; // Se l'auto deve fermarsi, non si muove
        }

        if (staRallentando) {
            transform.Translate(Vector3.forward * (velocità * 0.5f) * Time.deltaTime); // Rallenta
        } else {
            transform.Translate(Vector3.forward * velocità * Time.deltaTime); // Procede normalmente
        }

        ControllaSemaforo();
    }

    void ControllaSemaforo() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanzaRaggio)) {
            if (hit.collider.CompareTag("Semaforo")) {
                SemaforoController semaforo = hit.collider.GetComponent<SemaforoController>();

                if (semaforo != null) {
                    switch (semaforo.stato) {
                        case SemaforoController.StatoSemaforo.Rosso:
                            deveFermarsi = true;
                            staRallentando = false;
                            Debug.Log("Fermata al semaforo rosso!");
                            break;
                        case SemaforoController.StatoSemaforo.Giallo:
                            staRallentando = true;
                            Debug.Log("Rallentamento per il giallo!");
                            break;
                        case SemaforoController.StatoSemaforo.Verde:
                            staRallentando = false;
                            deveFermarsi = false;
                            break;
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("SemaforoTrigger")) {
            SemaforoController semaforo = other.GetComponentInParent<SemaforoController>();

            if (semaforo != null && semaforo.stato == SemaforoController.StatoSemaforo.Rosso) {
                penalità += 1;
                Debug.Log("🚨 Penalità assegnata! Totale penalità: " + penalità);
            }
        }
    }
}
