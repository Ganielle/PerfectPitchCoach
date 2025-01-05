using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoteData 1", menuName = "PerfectPitch/Notes/NoteData")]
public class NoteData : ScriptableObject
{
    [field: SerializeField] public string SongName { get; private set; }
    [field: SerializeField] public bool Inverted { get; private set; }
    [field: SerializeField] public int Score { get; set; }
    [field: SerializeField] public int MaxScore { get; set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public List<string> Notes { get; private set; }
    [field: SerializeField] public List<float> Length { get; private set; }
    [field: SerializeField] public List<float> SpawnTime { get; private set; }
    [field: SerializeField] public AudioClip DemoClip { get; private set; }
    [field: SerializeField] public AudioClip GameplayClip { get; private set; }

    private void OnEnable()
    {
        Score = 0;
    }
}
