using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    [SyncVar]
    public string username = "";

    public EventLogger _eventLogger;

    private NetworkManager _networkManager;

    // Start is called before the first frame update
    void Start()
    {
        if (_networkManager == null)
        {
            GameObject _networkManagerObject = GameObject.Find("NetworkManager");
            if (_networkManagerObject != null) _networkManager = _networkManagerObject.GetComponent<NetworkManager>();
        }

        if (_eventLogger == null)
        {
            _eventLogger = GameObject.FindObjectOfType<EventLogger>();
        }

        if (isLocalPlayer){
            if (_networkManager) CmdChangeUsername(_networkManager.username);

            if (_eventLogger != null) _eventLogger.CmdLogEvent(_networkManager.username + " has joined the game.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ClientRpc]
    private void RpcChangeUsername(string newUsername)
    {
        username = newUsername;
    }

    [Command]
    public void CmdChangeUsername(string newUsername)
    {
        RpcChangeUsername(newUsername);

        if (isServerOnly) username = newUsername;
    }
}
