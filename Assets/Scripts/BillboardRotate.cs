using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardRotate : MonoBehaviour
{
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(target.transform.position, Vector3.up, 10 * Time.deltaTime);
    }
}
