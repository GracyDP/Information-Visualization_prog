using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public GameObject redLight;
    public GameObject yellowLight;
    public GameObject greenLight;

    void Start()
    {
        StartCoroutine(lightSwitch());
    }

    IEnumerator lightSwitch()
    {
        while (true)
        {
            redLight.SetActive(true);
            yellowLight.SetActive(false);
            greenLight.SetActive(false);
            yield return new WaitForSeconds(10);

            //Luca...rosso poi prima il giallo...

            redLight.SetActive(false);
            yellowLight.SetActive(true);
            greenLight.SetActive(false);
            yield return new WaitForSeconds(5);

            redLight.SetActive(false);
            yellowLight.SetActive(false);
            greenLight.SetActive(true);
            yield return new WaitForSeconds(30);
        }
    }
}
