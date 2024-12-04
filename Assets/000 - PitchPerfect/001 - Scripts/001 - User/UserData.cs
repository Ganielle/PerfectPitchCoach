using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "PerfectPitchCoach/User/UserData")]
public class UserData : ScriptableObject
{
    [field: SerializeField] public string Username { get; set; }
    [field: SerializeField] public string UserToken { get; set; }
}
