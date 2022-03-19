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
        float speedMulti = Vector3.Dot(transform.forward, target.forward);
        speedMulti += 1.0f;
        speedMulti /= 2.0f;
        speedMulti = 1.0f - speedMulti;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, Mathf.Pow((speed * speedMulti), 2.0f) * Time.deltaTime );
        
        transform.position = target.position;
    }
}
