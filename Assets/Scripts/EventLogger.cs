using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EventLogger : NetworkBehaviour
{
    public ClientEventLog _clientEventLog;

    public List<string> events = new List<string>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_clientEventLog == null)
        {
            _clientEventLog = GameObject.FindObjectOfType<ClientEventLog>();
        }
    }

    [ClientRpc]
    private void RpcLogEvent(string eventText)
    {
        LogEvent(eventText);
    }

    [Command(requiresAuthority = false)]
    public void CmdLogEvent(string eventText)
    {
        RpcLogEvent(eventText);

        if (isServerOnly) LogEvent(eventText);
    }

    private void LogEvent(string eventText)
    {
        events.Add(eventText);

        if (_clientEventLog != null)
        {
            _clientEventLog.AddEvent(eventText);
        }
    }
}
