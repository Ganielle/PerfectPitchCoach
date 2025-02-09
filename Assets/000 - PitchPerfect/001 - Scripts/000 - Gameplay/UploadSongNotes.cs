using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UploadSongNotes : MonoBehaviour
{
    [SerializeField] private TMP_InputField spawnTime;
    [SerializeField] private TMP_Dropdown notes;

    public string GetSpawnTimeValue() => spawnTime.text;
    public string GetNotesValue() => notes.options[notes.value].text;
}
