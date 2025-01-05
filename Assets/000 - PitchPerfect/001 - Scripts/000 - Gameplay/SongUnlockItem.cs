using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongUnlockItem : MonoBehaviour
{
    [SerializeField] private string songname;
    [SerializeField] private UserData userData;
    [SerializeField] private Button songBtn;

    private void OnEnable()
    {
        if (userData.SongUnlocked.ContainsKey(songname))
        {
            if (userData.SongUnlocked[songname].locked == 0)
                songBtn.interactable = true;
            else
                songBtn.interactable = false;
        }
        else
            songBtn.interactable = false;
    }
}
