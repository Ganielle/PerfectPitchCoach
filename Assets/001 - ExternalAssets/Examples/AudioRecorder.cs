using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    public AudioSource audioSource;
    public int duration = 8;


    public void StartAudioRecorder()
    {
        audioSource.clip = Microphone.Start(Microphone.devices[0], audioSource.loop, duration, AudioSettings.outputSampleRate);
        audioSource.Play();
    }

    public void StopAudioRecorder()
    {
        audioSource.Stop();
    }
}
