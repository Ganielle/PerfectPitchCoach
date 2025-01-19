using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UploadedSongControlPanel : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private APIController apiController;
    [SerializeField] private NotificationController notificationController;
    [SerializeField] private PitchVisualizer pitchVisualizer;
    [SerializeField] private UserData userData;
    [SerializeField] private NoteSpawner noteSpawner;

    [Space]
    [SerializeField] private GameObject exerciseListObj;
    [SerializeField] private GameObject uploadedControlPanel;

    [Space]
    [SerializeField] private GameObject loadingObj;

    [Space]
    [SerializeField] private AudioSource demoSource;
    [SerializeField] private AudioSource gameSource;

    [Space]
    [SerializeField] private TextMeshProUGUI recommendationTMP;
    [SerializeField] private GameObject recommendationObj;

    [Space]
    [SerializeField] private TMP_InputField preAssessmentQuestionTMP;
    [SerializeField] private TextMeshProUGUI preAssessmentAnswerTMP;

    private void OnEnable()
    {
        gameManager.OnUploadedSongChange += SelectedSongChange;
    }

    private void OnDisable()
    {
        gameManager.OnUploadedSongChange -= SelectedSongChange;
    }

    private void SelectedSongChange(object sender, EventArgs e)
    {
        if (gameManager.SelectedUploadedSong == null)
        {
            exerciseListObj.SetActive(true);
            uploadedControlPanel.SetActive(false);
        }
        else
        {
            uploadedControlPanel.SetActive(true);
            exerciseListObj.SetActive(false);
            pitchVisualizer.StartInvoke();
        }
    }

    public void PlayDemo()
    {
        loadingObj.SetActive(true);
        GetAudioDemo();
    }

    private async void GetAudioDemo()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"{apiController.url}/{gameManager.SelectedUploadedSong.songlink}", AudioType.WAV))
        {
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                notificationController.ShowError("Audio file not found on server! Please contact customer support for more details", null);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                demoSource.clip = clip;
                demoSource.Play();
            }

            loadingObj.SetActive(false);
        }
    }

    public void StartGame()
    {
        loadingObj.SetActive(true);
        GetAudioStartGame();
    }

    private async void GetAudioStartGame()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"{apiController.url}/{gameManager.SelectedUploadedSong.songlink}", AudioType.WAV))
        {
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                notificationController.ShowError("Audio file not found on server! Please contact customer support for more details", null);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                demoSource.Stop();
                noteSpawner.score = 0;
                gameSource.clip = clip;
                gameSource.Play();
                uploadedControlPanel.SetActive(false);
            }

            loadingObj.SetActive(false);
        }
    }

    public void GoBack()
    {
        gameManager.SelectedUploadedSong = null;
        demoSource.Stop();
    }

    public void ShowRecommendation()
    {
        loadingObj.SetActive(true);

        StartCoroutine(apiController.GetRequest($"/ai/getrecommendation", $"?score={gameManager.noteSelected.Score}&maxscore={gameManager.noteSelected.MaxScore}&songname={gameManager.SelectedUploadedSong.songname}", false, (response) =>
        {
            if (response.ToString() != "")
            {
                Recommendation recommendation = JsonConvert.DeserializeObject<Recommendation>(response.ToString());

                if (recommendation != null)
                {
                    recommendationTMP.text = recommendation.content;
                    recommendationObj.SetActive(true);
                }
                else
                {
                    recommendationTMP.text = "No data yet!";
                }
            }
        }, null));
    }

    public void ShowPreAssessment()
    {
        loadingObj.SetActive(true);

        StartCoroutine(apiController.GetRequest($"/ai/preassessment", $"?question={preAssessmentQuestionTMP.text}&songname={gameManager.SelectedUploadedSong.songname}", false, (response) =>
        {
            if (response.ToString() != "")
            {
                Recommendation recommendation = JsonConvert.DeserializeObject<Recommendation>(response.ToString());

                if (recommendation != null)
                {
                    preAssessmentAnswerTMP.text = recommendation.content;
                }
                else
                {
                    preAssessmentAnswerTMP.text = "No data yet!";
                }
            }
        }, null));
    }
}
