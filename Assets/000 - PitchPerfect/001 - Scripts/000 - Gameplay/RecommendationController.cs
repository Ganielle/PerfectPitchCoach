using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecommendationController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private APIController apiController;

    [Space]
    [SerializeField] private GameObject loadingObj;

    [Space]
    [SerializeField] private TextMeshProUGUI recommendationTMP;
    [SerializeField] private GameObject recommendationObj;

    [Space]
    [SerializeField] private TMP_InputField preAssessmentQuestionTMP;
    [SerializeField] private TextMeshProUGUI preAssessmentAnswerTMP;

    public void ShowRecommendation()
    {
        loadingObj.SetActive(true);

        StartCoroutine(apiController.GetRequest($"/ai/getrecommendation", $"?score={gameManager.noteSelected.Score}&maxscore={gameManager.noteSelected.MaxScore}&songname={gameManager.noteSelected.SongName}", false, (response) =>
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

        StartCoroutine(apiController.GetRequest($"/ai/preassessment", $"?question={preAssessmentQuestionTMP.text}&songname={gameManager.noteSelected.SongName}", false, (response) =>
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

[Serializable]
public class Recommendation
{
    public string content;
}
