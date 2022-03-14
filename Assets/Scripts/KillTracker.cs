using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KillTracker : NetworkBehaviour
{
    public KillsManager _killsManager;

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

        if (_killsManager == null)
        {
            _killsManager = GameObject.Find("KillsManager").GetComponent<KillsManager>();
        }

        if (_killsManager)
        {
            _killsManager.AddPlayer(GetComponent<NetworkIdentity>());
        }
    }
}
