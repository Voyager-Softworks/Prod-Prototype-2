using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class LoudspeakerController : NetworkBehaviour
{
    public 
    List<Loudspeaker> loudspeakers = new List<Loudspeaker>();

    AudioClip currentClip;

    public InputAction testAction;

    public AudioClip[] killAnnouncements, gameStartAnnouncements, gameEndAnnouncements;

    public void RegisterLoudspeaker(Loudspeaker loudspeaker)
    {
        loudspeakers.Add(loudspeaker);
    }

    public enum AnnouncementType
    {
        KILL,
        GAMESTART,
        GAMEEND
    }

    // Start is called before the first frame update
    void Start()
    {
        testAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if(testAction.triggered)
        {
            Cmd_Play(AnnouncementType.KILL);
        }
    }

    [Command(requiresAuthority = false)]
    public void Cmd_Play(AnnouncementType type)
    {
        Play(type);
    }

    [Command(requiresAuthority = false)]
    public void Cmd_Stop()
    {
        Stop();
    }

    [ClientRpc]
    void Play(AnnouncementType type)
    {
        switch (type)
        {
            case AnnouncementType.KILL:
                currentClip = killAnnouncements[Random.Range(0, killAnnouncements.Length)];
                break;
            case AnnouncementType.GAMESTART:
                currentClip = gameStartAnnouncements[Random.Range(0, gameStartAnnouncements.Length)];
                break;
            case AnnouncementType.GAMEEND:
                currentClip = gameEndAnnouncements[Random.Range(0, gameEndAnnouncements.Length)];
                break;
        }
        foreach(Loudspeaker loudspeaker in loudspeakers)
        {
            loudspeaker.Play(currentClip);
        }
    }

    [ClientRpc]
    void Stop() 
    {
        foreach(Loudspeaker loudspeaker in loudspeakers)
        {
            loudspeaker.Stop();
        }
    }
}
