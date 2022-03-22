using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SceneBarrierController : NetworkBehaviour
{
    public bool testSwitch = false;
    public bool testSwitch2 = false;

    public float cooldown = 1f;
    float cooldownTimer = 0f;

    //Get cooldown timer formatted into minutes and seconds
    public string GetCooldownTimer()
    {
        string minutes = Mathf.Floor(cooldownTimer / 60).ToString("00");
        string seconds = (cooldownTimer % 60).ToString("00");
        return minutes + ":" + seconds;
    }

    public SceneBarrier.BarrierType currentBarrierType = SceneBarrier.BarrierType.GROUP_A;
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
        cooldownTimer -= Time.deltaTime;
        if(cooldownTimer < 0) cooldownTimer = 0;
        if(testSwitch)
        {
            testSwitch = false;
            foreach(SceneBarrier barrier in barriers)
            {
                barrier.SwitchBarriers(SceneBarrier.BarrierType.GROUP_A);
                currentBarrierType = SceneBarrier.BarrierType.GROUP_A;
            }
        }
        if(testSwitch2)
        {
            testSwitch2 = false;
            foreach(SceneBarrier barrier in barriers)
            {
                barrier.SwitchBarriers(SceneBarrier.BarrierType.GROUP_B);
                currentBarrierType = SceneBarrier.BarrierType.GROUP_B;
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSwitchBarriers()
    {
        if(cooldownTimer > 0)
        {
            return;
        }
        else
        {
            cooldownTimer = cooldown;
        }
        if(currentBarrierType == SceneBarrier.BarrierType.GROUP_A)
        {
            SwitchBarriers(SceneBarrier.BarrierType.GROUP_B);
            currentBarrierType = SceneBarrier.BarrierType.GROUP_B;
        }
        else
        {
            SwitchBarriers(SceneBarrier.BarrierType.GROUP_A);
            currentBarrierType = SceneBarrier.BarrierType.GROUP_A;
        }
        

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
