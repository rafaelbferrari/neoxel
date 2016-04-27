using UnityEngine;
using System.Collections;

/*
** Attach to camera and you will be able to drag the mouse on scene and rotate the camera
*/

public class RotateCameraDragging : MonoBehaviour
{

    [Header("Set speed of mouse dragging")]
    public float speed = 1f; 

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * speed;

        }
    }
}
