using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioClip btnClip;

    [Header("GameObject")]
    [SerializeField] public GameObject errorPanelObj;
    [SerializeField] public GameObject confirmationPanelObj;
    [SerializeField] private TextMeshProUGUI errorTMP;
    [SerializeField] private TextMeshProUGUI confirmationTMP;

    [Header("CONGRATULATIONS")]
    [SerializeField] private GameObject congratsOkObj;
    [SerializeField] private GameObject congratsOkCancelObj;
    [SerializeField] private TextMeshProUGUI congratsOkTMP;
    [SerializeField] private TextMeshProUGUI congratsOkCancelTMP;

    public Action currentAction, closeAction;

    #region CONFIRMATION

    public void ShowConfirmation(string statusText, Action currentConfirmationAction, Action closeConfirmationAction)
    {
        confirmationTMP.text = statusText;
        confirmationPanelObj.SetActive(true);
        currentAction = currentConfirmationAction;
        closeAction = closeConfirmationAction;
    }

    public void ConfirmedAction()
    {

        confirmationPanelObj.SetActive(false);
        confirmationTMP.text = "";

        if (currentAction != null)
        {
            currentAction();
            currentAction = null;
        }

        if (closeAction != null)
        {
            closeAction = null;
        }
    }

    public void CloseConfirmationAction()
    {

        //statesList.canChange = true;
        confirmationPanelObj.SetActive(false);
        confirmationTMP.text = "";

        if (closeAction != null)
        {
            closeAction();
            closeAction = null;

        }

        if (currentAction != null)
        {
            currentAction = null;
        }
    }

    #endregion

    #region ERROR

    public void ShowError(string statusText, Action closeConfirmationAction)
    {
        errorTMP.text = statusText;
        errorPanelObj.SetActive(true);
        closeAction = closeConfirmationAction;
    }

    public void CloseErrorAction()
    {

        errorPanelObj.SetActive(false);
        errorTMP.text = "";

        if (closeAction != null)
        {
            closeAction();
            closeAction = null;
        }
    }


    #endregion

    #region CONGRATS OK

    public void ShowCongratsOk(string statusText, Action closeConfirmationAction)
    {
        congratsOkTMP.text = statusText;
        congratsOkObj.SetActive(true);
        closeAction = closeConfirmationAction;
    }

    public void CloseCongratsOkAction()
    {

        congratsOkObj.SetActive(false);
        congratsOkTMP.text = "";

        if (closeAction != null)
        {
            closeAction();
            closeAction = null;
        }
    }

    #endregion

    #region CONGRATS OK CANCEL

    public void ShowCongratsOkCancel(string statusText, Action currentConfirmationAction, Action closeConfirmationAction)
    {
        congratsOkCancelTMP.text = statusText;
        congratsOkCancelObj.SetActive(true);
        currentAction = currentConfirmationAction;
        closeAction = closeConfirmationAction;
    }

    public void CongratsOkCancelConfirmedAction()
    {
        congratsOkCancelObj.SetActive(false);
        congratsOkCancelTMP.text = "";

        if (currentAction != null)
        {
            currentAction();
            currentAction = null;
        }

        if (closeAction != null)
        {
            closeAction = null;
        }
    }

    public void CloseCongratsOkCancelAction()
    {

        congratsOkCancelObj.SetActive(false);
        congratsOkCancelTMP.text = "";

        if (closeAction != null)
        {
            closeAction();
            closeAction = null;

        }

        if (currentAction != null)
        {
            currentAction = null;
        }
    }

    #endregion
}
