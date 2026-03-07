using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera;

    public float panSpeed;
    public float zoomSpeed;

    public float smallestSize;
    public float largestSize;

    public Vector3 lastMousePosition;

    public void Start()
    {
        lastMousePosition = Input.mousePosition;
    }

    public void Update()
    {

        Vector2 mouseScroll = Input.mouseScrollDelta;

        float cameraSizeDelta = -1 * mouseScroll.y * zoomSpeed * Time.deltaTime;
        camera.orthographicSize += cameraSizeDelta;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, smallestSize, largestSize);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 mousePosDelta = (Input.mousePosition - lastMousePosition) / (20 - camera.orthographicSize);
            mousePosDelta.x *= -1f;
            camera.transform.position += -1 * mousePosDelta * panSpeed * Time.deltaTime;
        }

        lastMousePosition = Input.mousePosition;
    }
}
