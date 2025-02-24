using System.Collections;
using UnityEngine;

public class Semafori : MonoBehaviour
{
    //sono spotLight
    public Light redLight;
    public Light yellowLight;
    public Light greenLight;

    public float greenTime = 8f; 
    public float yellowTime = 2f;
    public float redTime = 5f;

    private void Start()
    {
        StartCoroutine(TrafficCycle()); //avvio il semaforo allo start del progetto
    }

    private IEnumerator TrafficCycle()
    {
        while (true)
        {
            // Verde acceso
            SetLights(true, false, false);
            Debug.Log("semaforo  verde");
            yield return new WaitForSeconds(greenTime);

            // Giallo acceso
            SetLights(false, true, false);
            Debug.Log("semaforo  giallo");
            yield return new WaitForSeconds(yellowTime);

            // Rosso acceso
            SetLights(false, false, true);
            Debug.Log("semaforo  rosso");
            yield return new WaitForSeconds(redTime);
        }
    }

    private void SetLights(bool green, bool yellow, bool red)
    {
        greenLight.enabled = green;
        yellowLight.enabled = yellow;
        redLight.enabled = red;
    }
}
