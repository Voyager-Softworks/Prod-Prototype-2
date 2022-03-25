using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamScript : MonoBehaviour
{
    Camera cam;
    AudioListener listener;
    public GameObject Canvas;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (listener == null) listener = GetComponent<AudioListener>();
    }

    public void Enable()
    {
        cam.enabled = true;
        listener.enabled = true;
        Canvas.SetActive(true);

    }

    public void Disable()
    {
        cam.enabled = false;
        listener.enabled = false;
        Canvas.SetActive(false);
    }
}
