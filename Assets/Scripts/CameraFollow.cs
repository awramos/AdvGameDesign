using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float cameraMoveSpeed = 5f;
    public Vector3 cameraOffset = new Vector3(0f, 6f, 0f);

    private void LateUpdate()
    {
        Vector3 endPosition = player.position + cameraOffset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, endPosition, Time.deltaTime * cameraMoveSpeed);

        transform.position = smoothPosition;
    }
}
