using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBehaviour : MonoBehaviour
{
    private void OnMouseDown() {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
