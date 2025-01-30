using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{

// sono state cambiate le luci e non sono più light ma materiali emissivi

    public Material redLight;
    public Material yellowLight;
    public Material greenLight;

/*creiamo l'oggetto rend e settiamo inizialmente il colore rosso*/
    private Renderer rend;

    void Start()
    {
        //assegno il render prima di usarlo
        rend = GetComponent<Renderer>();
        StartCoroutine(lightSwitch());
    }

    public void SetLight(Material lightMaterial)
    {
        rend.material = lightMaterial;
    }

    IEnumerator LightSwitch()
    {
        while (true)
        {
            SetLight(redLight);
            yield return new WaitForSeconds(10);

            SetLight(yellowLight);
            yield return new WaitForSeconds(5);

            SetLight(greenLight);
            yield return new WaitForSeconds(30);
        }
    }
}
