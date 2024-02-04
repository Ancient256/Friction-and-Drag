using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedCounter : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public Transform playerTransform;

    private Vector3 lastPosition;

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = transform; // Assume the playerTransform is the script's own transform
        }

        lastPosition = playerTransform.position;
    }

    void Update()
    {
        if (speedText != null)
        {
            float speed = CalculateSpeed();
            speedText.text = "Speed: " + speed.ToString("F2");
        }
    }

    private float CalculateSpeed()
    {
        // Calculate the distance traveled over time
        float distanceTraveled = Vector3.Distance(playerTransform.position, lastPosition);

        // Calculate speed (distance / time)
        float speed = distanceTraveled / Time.deltaTime;

        // Update lastPosition for the next frame
        lastPosition = playerTransform.position;

        return speed;
    }
}
