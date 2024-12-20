using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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

    [Header("DEBUGGER")]
    public NoteData noteSelected;

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
}
