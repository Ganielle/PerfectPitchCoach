using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LoginController : MonoBehaviour
{
    [SerializeField] private UserData userData;
    [SerializeField] private APIController apiController;
    [SerializeField] private NotificationController notificationController;

    [Space]
    [SerializeField] private GameObject noBgLoader;
    
    [Space]
    [SerializeField] private TMP_InputField usernameTMP;
    [SerializeField] private TMP_InputField passwordTMP;

    [Space]
    [SerializeField] private GameObject exerciseListObj;
    [SerializeField] private GameObject loginObj;

    [Header("REGISTER")]
    [SerializeField] private TMP_InputField usernameRegisterTMP;
    [SerializeField] private TMP_InputField passwordRegisterTMP;

    //public void 

    public void Login()
    {
        noBgLoader.SetActive(true);
        if (usernameTMP.text == "")
        {
            notificationController.ShowError("Please input username first!", null);
            noBgLoader.SetActive(false);
            return;
        }

        if (passwordTMP.text == "")
        {
            notificationController.ShowError("Please input password first!", null);
            noBgLoader.SetActive(false);
            return;
        }
        Debug.Log("starting login api");
        StartCoroutine(LoginAPI());
    }

    IEnumerator LoginAPI()
    {
        Debug.Log("start login");
        UnityWebRequest apiRquest = UnityWebRequest.Get($"{apiController.url}/auth/login?username={usernameTMP.text}&password={passwordTMP.text}");
        apiRquest.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("start login");
        yield return apiRquest.SendWebRequest();

        Debug.Log(apiRquest.downloadHandler.text);

        if (apiRquest.result == UnityWebRequest.Result.Success)
        {
            string response = apiRquest.downloadHandler.text;


            if (response[0] == '{' && response[response.Length - 1] == '}')
            {
                try
                {
                    Dictionary<string, object> apiresponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

                    if (!apiresponse.ContainsKey("message"))
                    {
                        //ERROR PANEL HERE
                        Debug.Log("Error API CALL! Error Code: " + response);
                        notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                        noBgLoader.SetActive(false);
                        yield break;
                    }

                    if (apiresponse["message"].ToString() != "success")
                    {
                        //ERROR PANEL HERE
                        if (apiresponse.ContainsKey("data"))
                        {
                            Debug.Log(apiresponse["data"].ToString());
                            notificationController.ShowError(apiresponse["data"].ToString(), null);
                            noBgLoader.SetActive(false);

                            yield break;
                        }
                        notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                        noBgLoader.SetActive(false);
                        yield break;
                    }

                    if (!apiresponse.ContainsKey("data"))
                    {
                        notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                        noBgLoader.SetActive(false);
                        yield break;
                    }

                    Dictionary<string, object> dataresponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(apiresponse["data"].ToString());

                    userData.UserToken = dataresponse["token"].ToString();
                    userData.Username = usernameTMP.text;


                    Debug.Log("starting getting songs");
                    StartCoroutine(apiController.GetRequest("/users/getsongs", "", false, (tempdata) =>
                    {
                        Debug.Log("entered song api");
                        if (tempdata != null)
                        {
                            Debug.Log("song response api not null");
                            userData.SongUnlocked = JsonConvert.DeserializeObject<Dictionary<string, SongUnlock>>(tempdata.ToString());

                            Debug.Log("done deserialize");

                            StartCoroutine(apiController.GetRequest("/song/getuploadedsongs", "", false, (tempdatasong) =>
                            {
                                Debug.Log("ENTERED GET UPLOADED SONGS API");

                                if (tempdatasong != null)
                                {
                                    userData.UploadSongs = JsonConvert.DeserializeObject<List<UploadedSong>>(tempdatasong.ToString());

                                    exerciseListObj.SetActive(true);
                                    loginObj.SetActive(false);
                                }
                                else
                                {
                                    Debug.Log("Failed to get uploaded songs" + "  tempdatasong: " + tempdata);
                                    notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                                }
                            }, () =>
                            {
                                Debug.Log("Failed to get songs on error");
                                notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                            }));
                        }
                        else
                        {
                            Debug.Log("Failed to get songs" + "  tempdata: " + tempdata);
                            notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                        }
                    }, () =>
                    {
                        Debug.Log("Failed to get songs on error");
                        notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                    }));

                }
                catch (Exception ex)
                {
                    Debug.Log("Error API CALL! Error Code: " + response);
                    notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                    noBgLoader.SetActive(false);
                }
            }
        }
        else
        {
            try
            {
                Dictionary<string, object> apiresponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(apiRquest.downloadHandler.text);

                switch (apiRquest.responseCode)
                {
                    case 400:
                        notificationController.ShowError($"{apiresponse["data"]}", null);
                        noBgLoader.SetActive(false);
                        break;
                    case 300:
                        notificationController.ShowError($"{apiresponse["data"]}", null);
                        noBgLoader.SetActive(false);
                        break;
                    case 301:
                        notificationController.ShowError($"{apiresponse["data"]}", null);
                        noBgLoader.SetActive(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Error API CALL! Error Code: " + apiRquest.result + ", " + apiRquest.downloadHandler.text);
                notificationController.ShowError("There's a problem with your internet connection! Please check your connection and try again.", null);
                noBgLoader.SetActive(false);
            }
        }
    }

    public void Register()
    {
        noBgLoader.SetActive(true);
        if (usernameRegisterTMP.text == "")
        {
            notificationController.ShowError("Please input username first!", null);
            noBgLoader.SetActive(false);
            return;
        }

        if (passwordRegisterTMP.text == "")
        {
            notificationController.ShowError("Please input password first!", null);
            noBgLoader.SetActive(false);
            return;
        }
        Debug.Log("helloooo");
        StartCoroutine(apiController.PostRequest("/users/createuser", "", new Dictionary<string, object>
        {
            { "username", usernameRegisterTMP.text },
            { "password", passwordRegisterTMP.text }
        }, false, (response) =>
        {
            usernameRegisterTMP.text = "";
            passwordRegisterTMP.text = "";

            notificationController.ShowError("You have successfully registered your account!", null);
        }, null, false));
    }
}
