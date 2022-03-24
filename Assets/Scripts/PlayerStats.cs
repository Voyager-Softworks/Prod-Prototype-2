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

    public StatsManager _statsManager;

    LoudspeakerController announcer;

    [SyncVar]
    public List<PlayerHealth.Damage> _kills = new List<PlayerHealth.Damage>();

    [SyncVar]
    public List<PlayerHealth.Damage> _deaths = new List<PlayerHealth.Damage>();

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

            CmdHookStatsManager();
        }

        announcer = GameObject.FindObjectOfType<LoudspeakerController>();
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


    //Adding Kills
    [ClientRpc]
    private void RpcAddKill(PlayerHealth.Damage dmg)
    {
        AddKill(dmg);
    }

    [Command(requiresAuthority = false)]
    public void CmdAddKill(PlayerHealth.Damage dmg)
    {
        RpcAddKill(dmg);

        if (isServerOnly) AddKill(dmg);
    }

    private void AddKill(PlayerHealth.Damage dmg)
    {
        _kills.Add(dmg);
        GetComponent<ExpressionController>().SetExpression(Expression.ExpressionType.Happy);

        if (_statsManager)
        {
            _statsManager.CmdUpdateStats();
        }
    }


    //Adding Deaths
    [ClientRpc]
    private void RpcAddDeath(PlayerHealth.Damage dmg)
    {
        AddDeath(dmg);
    }

    [Command(requiresAuthority = false)]
    public void CmdAddDeath(PlayerHealth.Damage dmg)
    {
        RpcAddDeath(dmg);

        if (isServerOnly) AddDeath(dmg);
    }

    private void AddDeath(PlayerHealth.Damage dmg)
    {
        _deaths.Add(dmg);

        if (_statsManager)
        {
            _statsManager.CmdUpdateStats();
        }
    }

    [Command]
    private void CmdHookStatsManager(){
        Debug.Log("Hooked StatsManager");

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
