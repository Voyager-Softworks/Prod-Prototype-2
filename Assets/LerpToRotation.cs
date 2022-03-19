using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpToRotation : MonoBehaviour
{
    public Transform target;
    public float speed = 1.0f;
    

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, speed * Time.deltaTime);
        transform.position = target.position;
    }
}
