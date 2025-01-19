using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private event EventHandler UploadedSongChange;
    public event EventHandler OnUploadedSongChange
    {
        add
        {
            if (UploadedSongChange == null || !UploadedSongChange.GetInvocationList().Contains(value))
                UploadedSongChange += value;
        }
        remove { UploadedSongChange -= value; }
    }
    public SongUploadItem SelectedUploadedSong
    {
        get => uploadedSongSelected;
        set
        {
            uploadedSongSelected = value;
            UploadedSongChange?.Invoke(this, EventArgs.Empty);
        }
    }

    //  ======================

    [SerializeField] private APIController apiController;
    [SerializeField] private PitchVisualizer pitchVisualizer;
    [SerializeField] private NoteSpawner noteSpawner;

    [Space]
    [SerializeField] private GameObject exerciseListObj;

    [Space]
    [SerializeField] private AudioSource demoSource;
    [SerializeField] private Button playDemoBtn;

    [Space]
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private AudioSource gameSource;

    [Space]
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private TextMeshProUGUI scoreTMP;

    [Space]
    [SerializeField] private GameObject historyObj;
    [SerializeField] private TextMeshProUGUI historyTMP;

    [Header("DEBUGGER")]
    public NoteData noteSelected;
    [SerializeField] private SongUploadItem uploadedSongSelected;

    private void Update()
    {
        if (demoSource.isPlaying)
            playDemoBtn.interactable = false;
        else
            playDemoBtn.interactable = true;
    }

    public void PlayDemo()
    {
        demoSource.clip = noteSelected.DemoClip;
        demoSource.Play();
    }

    public void StartGame()
    {
        demoSource.Stop();
        noteSpawner.score = 0;
        gameSource.clip = noteSelected.GameplayClip;
        gameSource.Play();
        controlPanel.SetActive(false);
    }

    public void GoBack()
    {
        noteSelected = null;
        demoSource.Stop();
        controlPanel.SetActive(false);
        exerciseListObj.SetActive(true);
    }

    public void SelectGame(NoteData data)
    {
        loadingObj.SetActive(true);

        noteSelected = data;
        StartCoroutine(apiController.GetRequest($"/score/getscore?song={data.SongName}", "", false, (response) =>
        {
            if (response.ToString() != "")
                noteSelected.Score = Convert.ToInt32(float.Parse(response.ToString()));
            else
                noteSelected.Score = 0;

            scoreTMP.text = $"Highest Score: {noteSelected.Score}";
            controlPanel.SetActive(true);
            exerciseListObj.SetActive(false);
            pitchVisualizer.StartInvoke();
            loadingObj.SetActive(false);
        }, () =>
        {
            noteSelected.Score = 0;
            scoreTMP.text = $"Highest Score: {noteSelected.Score}";
            controlPanel.SetActive(true);
            exerciseListObj.SetActive(false);
            pitchVisualizer.StartInvoke();
            loadingObj.SetActive(false);
        }));
    }

    public void GetScoreHistory()
    {
        loadingObj.SetActive(true);

        StartCoroutine(apiController.GetRequest($"/score/getscorehistory", "", false, (response) =>
        {
            if (response != null)
            {
                List<ScoreHistory> scoreHistoryData = JsonConvert.DeserializeObject<List<ScoreHistory>>(response.ToString());

                if (scoreHistoryData.Count <= 0)
                {
                    historyTMP.text = "No Data yet";
                }
                else
                {
                    for (int a = 0; a < scoreHistoryData.Count; a++)
                    {
                        historyTMP.text += $"Score: {scoreHistoryData[a].amount:n0}  Date: {scoreHistoryData[a].createdAt}\n\n";
                    }
                }
            }
            else
            {
                historyTMP.text = "No Data yet";
            }

            historyObj.SetActive(true);
        }, () =>
        {
            historyTMP.text = "No Data yet";
            historyObj.SetActive(true);
            loadingObj.SetActive(false);
        }));
    }
}

[System.Serializable]
public class ScoreHistory
{
    public string amount;
    public string createdAt;
}