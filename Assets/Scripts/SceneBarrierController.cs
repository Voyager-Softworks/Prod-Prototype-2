using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SceneBarrierController : NetworkBehaviour
{
    public bool testSwitch = false;
    public bool testSwitch2 = false;
    List<SceneBarrier> barriers = new List<SceneBarrier>();

    public void RegisterBarrier(SceneBarrier barrier)
    {
        barriers.Add(barrier);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(testSwitch)
        {
            testSwitch = false;
            foreach(SceneBarrier barrier in barriers)
            {
                barrier.SwitchBarriers(SceneBarrier.BarrierType.GROUP_A);
            }
        }
        if(testSwitch2)
        {
            testSwitch2 = false;
            foreach(SceneBarrier barrier in barriers)
            {
                barrier.SwitchBarriers(SceneBarrier.BarrierType.GROUP_B);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSwitchBarriers(SceneBarrier.BarrierType type)
    {
        SwitchBarriers(type);
    }

    [ClientRpc]
    void SwitchBarriers(SceneBarrier.BarrierType type)
    {
        foreach(SceneBarrier barrier in barriers)
        {
            barrier.SwitchBarriers(type);
        }
    }
}
