using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSection : MonoBehaviour
{
    public Vector2 maxPosition;
    public Vector2 minPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.maxPosition = maxPosition;
            cameraController.minPosition = minPosition;
        }
    }
}
