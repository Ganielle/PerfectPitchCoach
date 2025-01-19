using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "PerfectPitchCoach/User/UserData")]
public class UserData : ScriptableObject
{
    [field: SerializeField] public string Username { get; set; }
    [field: SerializeField] public string UserToken { get; set; }

    //  =======================

    public Dictionary<string, SongUnlock> SongUnlocked;

    [field: SerializeField] public List<UploadedSong> UploadSongs { get; set; }

    //  =======================

    private void OnEnable()
    {
        Username = "";
        UserToken = "";
        SongUnlocked = new Dictionary<string, SongUnlock>();
        UploadSongs = new List<UploadedSong>();
    }
}

[System.Serializable]
public class SongUnlock
{
    public string _id;
    public string owner;
    public int locked;
}

[System.Serializable]
public class UploadedSong
{
    public string _id;
    public string owner;
    public string songname;
    public string songfile;
    public float speed;
    public List<UploadedSongNotes> notes;
    public List<UploadedSongNoteLetters> noteletter;
}

[System.Serializable]
public class UploadedSongNotes
{
    public string notevalue;
}

[System.Serializable]
public class UploadedSongNoteLetters
{
    public string notelettervalue;
}