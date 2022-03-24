using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldNameScript : MonoBehaviour
{
    public PlayerStats _realStats;
    public TextMeshProUGUI _worldName;
    GameObject _clientPlayer;

    // Start is called before the first frame update
    void Start()
    {
        //find the client player
        PlayerStats[] players = GameObject.FindObjectsOfType<PlayerStats>();

        foreach (PlayerStats player in players)
        {
            if (player.isLocalPlayer)
            {
                _clientPlayer = player.gameObject;
                break;
            }
        }

        if (_realStats == null)
        {
            _realStats = GetComponentInParent<PlayerStats>();
        }

        if (_worldName == null)
        {
            _worldName = GetComponent<TextMeshProUGUI>();
        }

        if (_realStats != null && _realStats.isLocalPlayer){
            _worldName.enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_clientPlayer == null || !_worldName.enabled) return;

        if (_realStats != null && _worldName != null)
        {
            _worldName.text = _realStats.username;
        }

        //rotate towards local player
        transform.LookAt(_clientPlayer.transform);
    }
}
