using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] Transform target;
    [SerializeField] float cameraSpeed;
    [SerializeField] Vector2 offset;
    [SerializeField] float mouseFollowMagnitude;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerController.instance.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 targetPosition = (Vector2)target.transform.position + offset;

        var mouse = Mouse.current;
        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());

        targetPosition += new Vector2(Mathf.Sqrt(mousePosition.x), Mathf.Sqrt(mousePosition.y)) * mouseFollowMagnitude;
        // transform.position = Vector2.Lerp(transform.position, targetPosition, cameraSpeed);
    }
}
