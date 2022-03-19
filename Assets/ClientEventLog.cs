using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClientEventLog : MonoBehaviour
{
    public NetworkIdentity _networkIdentity;

    public GameObject contentBox;
    public List<GameObject> entires = new List<GameObject>();

    public EventLogger _eventLogger;

    public GameObject eventLogEntryPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _networkIdentity = GetComponentInParent<NetworkIdentity>();

        if (_eventLogger == null)
        {
            _eventLogger = GameObject.FindObjectOfType<EventLogger>();
        }

        if (_eventLogger != null && _networkIdentity && _networkIdentity.isLocalPlayer)
        {
            //_eventLogger.CmdLogEvent(GetComponent<PlayerStats>().username + " has joined the game.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEvent(string eventText)
    {
        GameObject newEntry = Instantiate(eventLogEntryPrefab, contentBox.transform);
        newEntry.GetComponent<TextMeshProUGUI>().SetText(eventText);
        entires.Add(newEntry);
    }
}
