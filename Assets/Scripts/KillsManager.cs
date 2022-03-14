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

    public List<Player> players = new List<Player>();

    // Start is called before the first frame update
    void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayer(NetworkIdentity player)
    {
        Player p = new Player();
        p.name = player.name;
        p.kills = 0;
        p.deaths = 0;
        p.score = 0;
        p._player = player;
        players.Add(p);
    }

    public void RemovePlayer(NetworkIdentity player)
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
