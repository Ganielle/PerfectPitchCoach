using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoteData 1", menuName = "PerfectPitch/Notes/NoteData")]
public class NoteData : ScriptableObject
{
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public List<string> Notes { get; private set; }
    [field: SerializeField] public List<float> Length { get; private set; }
    [field: SerializeField] public List<float> SpawnTime { get; private set; }
    [field: SerializeField] public AudioClip DemoClip { get; private set; }
    [field: SerializeField] public AudioClip GameplayClip { get; private set; }
}
