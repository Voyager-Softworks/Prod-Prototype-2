using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KillTracker : NetworkBehaviour
{
    public StatsManager _statsManager;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            CmdDoConnectStuff();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    private void CmdDoConnectStuff(){
        Debug.Log("DoConnectStuff");

        if (_statsManager == null)
        {
            _statsManager = FindObjectOfType<StatsManager>();
        }

        if (_statsManager)
        {
            _statsManager.RpcAddPlayer(GetComponent<NetworkIdentity>());
        }
    }
}
