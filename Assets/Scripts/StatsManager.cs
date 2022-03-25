using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatsManager : NetworkBehaviour
{
    [Serializable]
    public class Player
    {
        public NetworkIdentity _player;
        public string _username;
        public int kills;
        public int deaths;
    }

    DateTime _startTime;

    public NetworkManager _networkManager;

    public List<Player> players = new List<Player>();

    public PlayerCanvas _clientPlayerCanvas;

    // Start is called before the first frame update
    void Start()
    {
        _startTime = DateTime.Now;

        if (_networkManager == null)
        {
            GameObject _networkManagerObject = GameObject.Find("NetworkManager");
            if (_networkManagerObject != null) _networkManager = _networkManagerObject.GetComponent<NetworkManager>();
        }

        FindPlayerCanvas();
    }

    private void FindPlayerCanvas()
    {
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
        if (_clientPlayerCanvas != null)
        {
            _clientPlayerCanvas.time.text = (DateTime.Now - _startTime).ToString("mm\\:ss");

            //if cursor locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                _clientPlayerCanvas.scoreboardBG.gameObject.SetActive(true);
                _clientPlayerCanvas.scorebaordText.text = "SCORES\n";

                //sort players by kills
                players.Sort((x, y) => y.kills.CompareTo(x.kills));

                foreach (Player player in players)
                {
                    if (player._player == null) continue;
                    _clientPlayerCanvas.scorebaordText.text += player._username + " [" + player.kills + " kills]  [" + player.deaths + " deaths]\n";

                    if (player._player.isLocalPlayer)
                    {
                        _clientPlayerCanvas.killsText.text = "Kills: " + player.kills;
                        _clientPlayerCanvas.deathsText.text = "Deaths: " + player.deaths;
                    }
                }
            }
            else{
                _clientPlayerCanvas.scoreboardBG.gameObject.SetActive(false);
            }

            foreach (Player player in players)
            {
                if (player._player == null) continue;

                if (player._player.isLocalPlayer)
                {
                    _clientPlayerCanvas.killsText.text = "Kills: " + player.kills;
                    _clientPlayerCanvas.deathsText.text = "Deaths: " + player.deaths;
                }
            }
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

    [Command(requiresAuthority = false)]
    public void CmdUpdateStats(){
        RpcUpdateStats();

        if (isServerOnly) UpdateStats();
    }

    [ClientRpc]
    private void RpcUpdateStats(){
        UpdateStats();
    }

    private void UpdateStats(){
        foreach (Player player in players)
        {
            if (player == null) continue;
            if (player._player == null) continue;
            if (player._player.GetComponent<PlayerStats>() == null) continue;

            player.kills = player._player.GetComponent<PlayerStats>()._kills.Count;
            player.deaths = player._player.GetComponent<PlayerStats>()._deaths.Count;
        }
    }

    [ClientRpc]
    private void RpcUpdateList(List<Player> _players)
    {
        Debug.Log("UpdateList");

        players = _players;
    }


    [ClientRpc]
    public void RpcAddPlayer(NetworkIdentity player)
    {
        Player p = new Player();
        p.kills = 0;
        p.deaths = 0;
        p._player = player;
        p._username = (player.GetComponent<PlayerStats>() ? player.GetComponent<PlayerStats>().username : "Unknown");
        players.Add(p);

        if (isServer)
        {
            RpcUpdateList(players);
        }

        FindPlayerCanvas();
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
}
