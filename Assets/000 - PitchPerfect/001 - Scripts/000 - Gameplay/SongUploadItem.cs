using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SongUploadItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI songName;

    [Header("DEBUGGER")]
    public string songname;
    public string songlink;
    public int index;

    //  ==================

    [Space]
    public GameManager gameManager;

    //  ==================

    public void InitializeSong(string songname, string link, int index, GameManager gameManager)
    {
        songName.text = songname;
        this.songname = songname;
        songlink = link;
        this.index = index;
        this.gameManager = gameManager;
    }

    public void SelectSong()
    {
        gameManager.SelectedUploadedSong = this;
    }
}
