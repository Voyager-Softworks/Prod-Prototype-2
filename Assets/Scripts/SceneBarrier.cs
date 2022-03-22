using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBarrier : MonoBehaviour
{
    public enum BarrierType
    {
        GROUP_A,
        GROUP_B,
    }

    public BarrierType barrierType;
    public GameObject barrier;

    MeshRenderer meshRenderer;
    BoxCollider barrierCollider;

    public void SwitchBarriers(BarrierType type)
    {
        if(type == barrierType)
        {
            //Enable Barrier
        }
        else
        {
            //Disable Barrier
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = barrier.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
