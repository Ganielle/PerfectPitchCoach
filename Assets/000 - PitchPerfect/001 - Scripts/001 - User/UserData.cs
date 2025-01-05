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

    //  =======================

    private void OnEnable()
    {
        Username = "";
        UserToken = "";
        SongUnlocked = new Dictionary<string, SongUnlock>();
    }
}

[System.Serializable]
public class SongUnlock
{
    public string _id;
    public string owner;
    public int locked;
}