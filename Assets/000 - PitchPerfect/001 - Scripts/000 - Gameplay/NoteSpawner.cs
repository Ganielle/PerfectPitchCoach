using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public AudioRecorder recorder;
    public PitchVisualizer pitchVisualizer;
    public GameManager gameManager;
    public GameObject notePrefab;
    public GameObject scoreObj;
    public AudioSource audioSource;
    public TextMeshProUGUI scoreTMP;
    private int nextNoteIndex = 0;

    public List<Transform> notes;
    public int score;
    private bool scoreShown = true;

    [Space]
    [SerializeField] private APIController apiController;
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private TextMeshProUGUI controlPanelScoreTMP;

    string[] names = {
            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
    };

    private Dictionary<string, float> notePositions = new Dictionary<string, float>();

    private void Start()
    {
        StartCoroutine(InitializeNotes());
    }

    private void Update()
    {
        SpawnNotes();
    }

    IEnumerator InitializeNotes()
    {
        for (int a = 0; a < names.Length; a++)
        {
            notePositions.Add(names[a], notes[a].position.x);
            yield return null;
        }
    }

    private void SpawnNotes()
    {
        // Check if the audio is playing and there are notes to spawn
        if (audioSource.isPlaying && nextNoteIndex < gameManager.noteSelected.Length.Count)
        {
            scoreShown = false;
            // If the current time of the audio source is greater than or equal to the next note time
            if (audioSource.time >= gameManager.noteSelected.SpawnTime[nextNoteIndex])
            {
                SpawnNote();
                nextNoteIndex++; // Move to the next note
            }
        }
        else if (!audioSource.isPlaying)
        {
            if (!scoreShown)
            {
                scoreShown = true;
                recorder.StopAudioRecorder();
                nextNoteIndex = 0;
                pitchVisualizer.StopInvoke();

                StartCoroutine(apiController.PostRequest("/score/savescore", "", new Dictionary<string, object>
                {
                    { "song",  gameManager.noteSelected.SongName },
                    { "score", CalculatePercentage(score) }
                }, false, (response) =>
                {
                    gameManager.noteSelected.Score = (int)CalculatePercentage(score);
                    scoreTMP.text = CalculatePercentage(score).ToString("n0");
                    scoreObj.SetActive(true);
                    controlPanelScoreTMP.text = $"Highest Score: {(int)CalculatePercentage(score)}";
                }, () =>
                {
                    gameManager.noteSelected.Score = (int)CalculatePercentage(score);
                    scoreTMP.text = CalculatePercentage(score).ToString("n0");
                    scoreObj.SetActive(true);
                    controlPanelScoreTMP.text = $"Highest Score: {(int)CalculatePercentage(score)}";
                }));
            }
        }
    }

    private void SpawnNote()
    {
        // Instantiate the note at a predefined position
        GameObject newNote = Instantiate(notePrefab, new Vector3(0, -5, 0), Quaternion.identity);

        Note note = newNote.GetComponent<Note>();
        // Set additional properties on the note if needed
        note.SetData(gameManager.noteSelected.Length[nextNoteIndex], notePositions[gameManager.noteSelected.Notes[nextNoteIndex]], gameManager.noteSelected.Speed, this);
    }

    public float CalculatePercentage(float value)
    {
        float percentage = (value / gameManager.noteSelected.Length.Count) * 100f;
        return percentage;
    }
}

