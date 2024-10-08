using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchVisualizer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioPitchEstimator estimator;
    public LineRenderer lineSRH;
    public LineRenderer lineFrequency;
    public Transform marker;
    public TextMesh textFrequency;
    public TextMesh textMin;
    public TextMesh textMax;

    public float estimateRate = 30;
    public List<Transform> notes;

    string[] names = {
            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
    };

    private Dictionary<string, float> notePositions = new Dictionary<string, float>();

    void Start()
    {
        StartCoroutine(InitializeNotes());
    }

    public void StartInvoke()
    {
        InvokeRepeating(nameof(UpdateVisualizer), 0, 1.0f / estimateRate);
    }

    public void StopInvoke()
    {
        CancelInvoke();
    }

    IEnumerator InitializeNotes()
    {
        for (int a = 0; a < names.Length; a++)
        {
            notePositions.Add(names[a], notes[a].position.x);
            yield return null;
        }
    }

    void UpdateVisualizer()
    {
        var frequency = estimator.Estimate(audioSource);

        //var srh = estimator.SRH;
        //var numPoints = srh.Count;
        //var positions = new Vector3[numPoints];
        //for (int i = 0; i < numPoints; i++)
        //{
        //    var position = (float)i / numPoints - 0.5f;
        //    var value = srh[i] * 0.005f;
        //    positions[i].Set(position, value, 0);
        //}
        //lineSRH.positionCount = numPoints;
        //lineSRH.SetPositions(positions);

        if (float.IsNaN(frequency))
        {
            lineFrequency.positionCount = 0;
        }
        else
        {
            var min = estimator.frequencyMin;
            var max = estimator.frequencyMax;
            //var position = (frequency - min) / (max - min) - 0.5f;

            //  new positions
            var position = notePositions[GetNameFromFrequency(frequency)];

            lineFrequency.positionCount = 2;
            lineFrequency.SetPosition(0, new Vector3(position, +1, 0));
            lineFrequency.SetPosition(1, new Vector3(position, -1, 0));

            marker.position = new Vector3(position, 0, 0);
            textFrequency.text = string.Format("{0}\n{1:0.0} Hz", GetNameFromFrequency(frequency), frequency);
        }

        textMin.text = string.Format("{0} Hz", estimator.frequencyMin);
        textMax.text = string.Format("{0} Hz", estimator.frequencyMax);
    }
    string GetNameFromFrequency(float frequency)
    {
        var noteNumber = Mathf.RoundToInt(12 * Mathf.Log(frequency / 440) / Mathf.Log(2) + 69);
        return names[noteNumber % 12];
    }


}
