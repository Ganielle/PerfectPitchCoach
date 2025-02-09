using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalAssessment : MonoBehaviour
{
    [SerializeField] private UserData userData;
    [SerializeField] private GameObject noBgLoading;
    [SerializeField] private APIController apiController;
    [SerializeField] private NotificationController notificationController;

    [Space]
    [SerializeField] private Button assessmentBtn;
    [SerializeField] private GameObject assessmentPanel;
    [SerializeField] private TextMeshProUGUI assessmentContentTMP;

    private void OnEnable()
    {
        assessmentBtn.interactable = userData.FinalAssessment == 1;
    }

    public void GetFinalAssessment()
    {
        noBgLoading.SetActive(true);

        StartCoroutine(apiController.GetRequest("", "", false, (tempdata) => 
        {
            if (tempdata.ToString() != "")
            {
                assessmentContentTMP.text = tempdata.ToString();

                assessmentPanel.SetActive(true);
                noBgLoading.SetActive(false);
            }
            else
            {
                Debug.Log($"There's a problem getting the final assessment. Error: {tempdata.ToString()}");
                notificationController.ShowError("There's a problem with the server, Please try again later.", null);
                assessmentPanel.SetActive(true);
                noBgLoading.SetActive(false);
            }
        }, () =>
        {
            Debug.Log($"There's a problem getting the final assessment.");
            notificationController.ShowError("There's a problem with the server, Please try again later.", null);
            assessmentPanel.SetActive(true);
            noBgLoading.SetActive(false);
        }));
    }
}
