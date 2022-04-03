using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    private Transform cameraTransform;

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    /// <summary>
    /// Update phase in the native player loop
    /// </summary>
    private void Update()
    {
        // make sure health bar is facing the camera
        transform.rotation = cameraTransform.rotation;
    }
}
