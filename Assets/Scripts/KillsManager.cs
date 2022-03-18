using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KillsManager : NetworkBehaviour
{
    [Serializable]
    public class Player
    {
        public NetworkIdentity _player;
        public string name;
        public int kills;
        public int deaths;
        public int score;
    }

    public NetworkManager _networkManager;

    [SyncVar(hook = "OnChangePlayers")]
    public List<Player> players = new List<Player>();

    public PlayerCanvas _clientPlayerCanvas;

    // Start is called before the first frame update
    void Start()
    {
        if (_networkManager == null)
        {
            GameObject _networkManagerObject = GameObject.Find("NetworkManager");
            if (_networkManagerObject != null) _networkManager = _networkManagerObject.GetComponent<NetworkManager>();
        }

        if (_clientPlayerCanvas == null)
        {
            _clientPlayerCanvas = GameObject.FindObjectOfType<PlayerCanvas>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    [ClientRpc]
    private void RpcUpdateUI()
    {
        Debug.Log("RpcUpdateUI");

        if (_clientPlayerCanvas != null)
        {

        }
    }

    [Command]
    private void CmdUpdateUI()
    {
        RpcUpdateUI();
    }

    public void UpdateUI(){
        if (isServer)
        {
            RpcUpdateUI();
        }
    }


    [ClientRpc]
    public void RpcAddPlayer(NetworkIdentity player)
    {
        Player p = new Player();
        p.name = player.name;
        p.kills = 0;
        p.deaths = 0;
        p.score = 0;
        p._player = player;
        players.Add(p);
    }

    [ClientRpc]
    public void RpcRemovePlayer(NetworkIdentity player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i]._player == player)
            {
                players.RemoveAt(i);
                return;
            }
        }
    }

    public void OnChangePlayers(List<Player> oldPlayers, List<Player> newPlayers)
    {
        players = newPlayers;
    }
}
