using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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
        noteSelected = data;
        controlPanel.SetActive(true);
        exerciseListObj.SetActive(false);
        pitchVisualizer.StartInvoke();
    }
}
