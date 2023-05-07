using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = .2f;

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y += 6f;
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed); //stops the jittering of the camera
    }
}