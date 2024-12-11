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
    public int nextNoteIndex = 0;

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
        if (audioSource.isPlaying && nextNoteIndex < gameManager.noteSelected.SpawnTime.Count)
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
        // Calculate time difference between the current note and the next
        float timeDifference = nextNoteIndex >= gameManager.noteSelected.SpawnTime.Count - 1
            ? -1 // Default value when at the last note
            : gameManager.noteSelected.SpawnTime[nextNoteIndex + 1] - gameManager.noteSelected.SpawnTime[nextNoteIndex];

        // If the time difference exceeds a threshold, do not spawn a connecting note
        float maxAllowedGap = 0.5f; // Adjust this value based on the allowed gap duration
        if (timeDifference > maxAllowedGap && nextNoteIndex < gameManager.noteSelected.SpawnTime.Count - 1)
        {
            nextNoteIndex++; // Skip the note and move to the next one
            return;
        }

        // Instantiate the note at a predefined position
        GameObject newNote = Instantiate(notePrefab, new Vector3(0, -5, 0), Quaternion.identity);

        Note note = newNote.GetComponent<Note>();

        // Set note properties
        note.SetData(
            timeDifference,
            notePositions[gameManager.noteSelected.Notes[nextNoteIndex]],
            gameManager.noteSelected.Speed,
            this
        );
    }


    public float CalculatePercentage(float value)
    {
        float percentage = (value / gameManager.noteSelected.Length.Count) * 100f;
        return percentage;
    }
}

