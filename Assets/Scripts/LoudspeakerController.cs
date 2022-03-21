using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LoudspeakerController : NetworkBehaviour
{
    public 
    List<Loudspeaker> loudspeakers = new List<Loudspeaker>();

    AudioClip currentClip;

    public AudioClip[] killAnnouncements, deathAnnouncements, gameStartAnnouncements, gameEndAnnouncements;

    public void RegisterLoudspeaker(Loudspeaker loudspeaker)
    {
        loudspeakers.Add(loudspeaker);
    }

    public enum AnnouncementType
    {
        KILL,
        DEATH,
        GAMESTART,
        GAMEEND
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    public void Cmd_Play(AnnouncementType type)
    {
        Play(type);
    }

    [Command]
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
            case AnnouncementType.DEATH:
                currentClip = deathAnnouncements[Random.Range(0, deathAnnouncements.Length)];
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
