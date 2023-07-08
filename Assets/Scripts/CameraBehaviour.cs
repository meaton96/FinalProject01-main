using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.GraphicsBuffer;

public class CameraBehaviour : MonoBehaviour {

    public Transform player;
    public Vector3 cameraOffset;
    public float cameraSpeed = 0.1f;

    void Start() {
        transform.position = player.position + cameraOffset;
    }

    //https://www.sebastianhutteri.com/blog/how-to-get-that-silky-smooth-camera-movement-in-unity
    //camera movement follows player
    void FixedUpdate() {
        Vector3 finalPosition = player.position + cameraOffset;
        Vector3 lerpPosition = Vector3.Lerp(transform.position, finalPosition, cameraSpeed);
        transform.position = lerpPosition;
    }
}
