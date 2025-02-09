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

    [Space]
    [SerializeField] private TextMeshProUGUI songUploadScore;

    [Space]
    [SerializeField] private GameObject historyObj;
    [SerializeField] private TextMeshProUGUI historyTMP;

    [Header("DEBUGGER")]
    [SerializeField] private float currentUploadedSongScore;

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
            ShowCurrentScore();
        }
    }

    public void PlayDemo()
    {
        loadingObj.SetActive(true);
       StartCoroutine(GetAudioDemo());
    }

    IEnumerator GetAudioDemo()
    {
        string url = $"{apiController.url}/{gameManager.SelectedUploadedSong.songlink}";
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            var handler = (DownloadHandlerAudioClip)www.downloadHandler;
            handler.compressed = false; // Ensure uncompressed handling
            handler.streamAudio = true;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = handler.audioClip;
                if (clip == null)
                {
                    Debug.LogError("Failed to decode WAV file properly.");
                }
                else
                {
                    demoSource.clip = clip;
                    demoSource.Play();
                }
            }

            loadingObj.SetActive(false);
        }
    }


    public void StartGame()
    {
        loadingObj.SetActive(true);
        StartCoroutine(GetAudioStartGame());
    }

    IEnumerator GetAudioStartGame()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"{apiController.url}/{gameManager.SelectedUploadedSong.songlink}", AudioType.WAV))
        {
            yield return www.SendWebRequest();

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

        StartCoroutine(apiController.GetRequest($"/ai/getrecommendation", $"?score={currentUploadedSongScore}&maxscore=max score not measured&songname={gameManager.SelectedUploadedSong.songname}", false, (response) =>
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

    public void ShowCurrentScore()
    {
        loadingObj.SetActive(true);

        StartCoroutine(apiController.GetRequest($"/score/getuploadedscore", $"?songid={gameManager.SelectedUploadedSong.songid}", false, (response) =>
        {
            if (response.ToString() != "")
            {
                songUploadScore.text = $"Current Score: {float.Parse(response.ToString()).ToString("n0")}";
                currentUploadedSongScore = float.Parse(response.ToString());
            }

            uploadedControlPanel.SetActive(true);
            exerciseListObj.SetActive(false);
            pitchVisualizer.StartInvoke();
            loadingObj.SetActive(false);
        }, null));
    }

    public void GetScoreHistory()
    {
        loadingObj.SetActive(true);

        StartCoroutine(apiController.GetRequest($"/score/getuploadedscorehistory", $"?songid={gameManager.SelectedUploadedSong.songid}", false, (response) =>
        {
            if (response != null)
            {
                List<ScoreHistory> scoreHistoryData = JsonConvert.DeserializeObject<List<ScoreHistory>>(response.ToString());

                if (scoreHistoryData.Count <= 0)
                {
                    historyTMP.text = "No Data yet";
                }
                else
                {
                    for (int a = 0; a < scoreHistoryData.Count; a++)
                    {
                        historyTMP.text += $"Score: {scoreHistoryData[a].amount:n0}  Date: {scoreHistoryData[a].createdAt}\n\n";
                    }
                }
            }
            else
            {
                historyTMP.text = "No Data yet";
            }

            historyObj.SetActive(true);
        }, () =>
        {
            historyTMP.text = "No Data yet";
            historyObj.SetActive(true);
            loadingObj.SetActive(false);
        }));
    }
}
