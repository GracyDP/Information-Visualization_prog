
    private void CheckForEntrance()
    {
        float angle = 15f; // Angolo per i raggi laterali

        // Direzioni dei raggi
        Vector3 forward = transform.forward;
        Vector3 right = Quaternion.Euler(0, angle, 0) * forward; // Raggio destro
        Vector3 left = Quaternion.Euler(0, -angle, 0) * forward; // Raggio sinistro

        // Controlla con tre raggi
        if (CheckRay(forward) || CheckRay(right) || CheckRay(left))
        {
            Debug.Log("Ingresso rilevato!");
        }
    }

    private bool CheckRay(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(frontSensor.position, direction, out hit, 6f))
        {
            if (hit.collider.CompareTag("ingresso"))
            {
                float distance = hit.distance;
                float reward = Mathf.Lerp(1.0f, 0.1f, distance / 6f);
                addRewordWrapped(reward);
                Debug.DrawRay(frontSensor.position, direction * distance, Color.green, 0.5f);
                return true;
            }
        }
        return false;
    }
