using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class UploadSongController : MonoBehaviour
{
    [SerializeField] private UserData userData;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private NotificationController notificationController;
    [SerializeField] private APIController apiController;

    [Space]
    [SerializeField] private string[] fileExtentions;
    [SerializeField] private GameObject loadingObj;

    [Space]
    [SerializeField] private GameObject uploadSongObj;

    [Space]
    [SerializeField] private TMP_InputField songNameTMP;
    [SerializeField] private TextMeshProUGUI songUploadPath;
    [SerializeField] private TMP_InputField songSpeedTMP;

    [Space]
    [SerializeField] private GameObject notesPrefab;
    [SerializeField] private Transform notesTF;

    [Space]
    [SerializeField] private GameObject exerciseListObj;

    [Space]
    [SerializeField] private Transform uploadedSongParent;
    [SerializeField] private GameObject noSongsYetUpload;
    [SerializeField] private GameObject songUploadedListObj;
    [SerializeField] private GameObject songUploadedItem;

    [Header("DEBUGGER")]
    [SerializeField] private NativeFilePicker.Permission permission;
    [SerializeField] private string songfilepath;
    [SerializeField] private List<UploadSongNotes> uploadSongSettings;

    private async void AskPermission()
    {
        NativeFilePicker.Permission permissionResult = await NativeFilePicker.RequestPermissionAsync(false);
        permission = permissionResult;

        if (permission == NativeFilePicker.Permission.Granted)
            AndroidPickCameraRoll();
    }

    public void OpenUploadSong()
    {
        loadingObj.SetActive(true);

        songNameTMP.text = "";
        songUploadPath.text = "";
        DeleteNotes();
    }

    public void CloseUploadSong()
    {
        songfilepath = "";
        uploadSongObj.SetActive(false);
    }

    async void DeleteNotes()
    {
        uploadSongSettings.Clear();

        while (notesTF.childCount > 0)
        {
            for (int a = 0; a < notesTF.childCount; a++)
            {
                Destroy(notesTF.GetChild(a).gameObject);
                await Task.Yield();
            }

            await Task.Yield();
        }

        uploadSongObj.SetActive(true);
        loadingObj.SetActive(false);
    }

    public void AddNoteSettings()
    {
        GameObject tempsettings = Instantiate(notesPrefab, notesTF);

        uploadSongSettings.Add(tempsettings.GetComponent<UploadSongNotes>());
    }

    public void StartUploadSong()
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanelWithFilters("Show all sound files (.wav)", "", fileExtentions);

        if (path != "")
        {
            songfilepath = path;
            songUploadPath.text = songfilepath;
        }
#else
            AndroidPickCameraRoll();
#endif
    }

    private void AndroidPickCameraRoll()
    {
        if (permission == NativeFilePicker.Permission.Granted)
        {
            NativeFilePicker.PickFile((path) =>
            {
                if (path == null) return;

                songfilepath = path;
                songUploadPath.text = songfilepath;

            }, "audio/*");
        }
        else
        {
            AskPermission();
        }
    }

    public void UploadYourSong()
    {
        StartCoroutine(UploadSongFinish());
    }

    IEnumerator UploadSongFinish()
    {
        loadingObj.SetActive(true);

        if (songNameTMP.text == "")
        {
            notificationController.ShowError("Please enter song name first.", null);
            loadingObj.SetActive(false);
            yield break;
        }
        else if (songSpeedTMP.text == "")
        {
            notificationController.ShowError("Please enter song speed first.", null);
            loadingObj.SetActive(false);
            yield break;
        }
        else if (songfilepath == "")
        {
            notificationController.ShowError("Please select a song first.", null);
            loadingObj.SetActive(false);
            yield break;
        }
        else if (uploadSongSettings.Count <= 0)
        {
            notificationController.ShowError("Please enter a song settings first.", null);
            loadingObj.SetActive(false);
            yield break;
        }
        else if (uploadSongSettings.Count <= 1)
        {
            notificationController.ShowError("Please enter two or more song settings first.", null);
            loadingObj.SetActive(false);
            yield break;
        }

        for (int a = 0; a < uploadSongSettings.Count; a++)
        {
            if (uploadSongSettings[a].GetNotesValue() == "")
            {
                notificationController.ShowError("Please select a song notes first.", null);
                loadingObj.SetActive(false);
                yield break;
            }
            else if (uploadSongSettings[a].GetSpawnTimeValue() == "")
            {
                notificationController.ShowError("Please select a song spawn time first.", null);
                loadingObj.SetActive(false);
                yield break;
            }
            yield return null;
        }

        byte[] wavBytes = File.ReadAllBytes(songfilepath);
        WWWForm form = new WWWForm();
        form.AddField("songname", songNameTMP.text);
        form.AddBinaryData("song", wavBytes, "audio.wav", "audio/wav");
        form.AddField("speed", songSpeedTMP.text);

        for (int a = 0; a < uploadSongSettings.Count; a++)
        {
            form.AddField("notes", uploadSongSettings[a].GetSpawnTimeValue());
            form.AddField("noteletter", uploadSongSettings[a].GetNotesValue());
            yield return null;
        }

        UnityWebRequest apiRquest = UnityWebRequest.Post($"{apiController.url}/song/uploadsong", form);
        apiRquest.SetRequestHeader("Authorization", "Bearer " + userData.UserToken);

       yield return apiRquest.SendWebRequest();

        if (apiRquest.result == UnityWebRequest.Result.Success)
        {
            string response = apiRquest.downloadHandler.text;

            if (response[0] == '{' && response[response.Length - 1] == '}')
            {
                Dictionary<string, object> apiresponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

                if (!apiresponse.ContainsKey("message"))
                {
                    //  ERROR PANEL HERE
                    Debug.Log("Error API CALL! Error Code: " + response);
                    notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                    loadingObj.SetActive(false);
                    yield break;
                }

                if (apiresponse["message"].ToString() != "success")
                {
                    //  ERROR PANEL HERE
                    Debug.Log(apiresponse["data"].ToString());
                    loadingObj.SetActive(false);
                    yield break;
                }
                Debug.Log("SUCCESS API CALL");

                while (notesTF.childCount > 0)
                {
                    for (int a = 0; a < notesTF.childCount; a++)
                    {
                        Destroy(notesTF.GetChild(a).gameObject);

                        yield return null;
                    }

                    yield return null;
                }

                StartCoroutine(apiController.GetRequest("/song/getuploadedsongs", "", false, (tempdatasong) =>
                {
                    Debug.Log("ENTERED GET UPLOADED SONGS API");

                    if (tempdatasong != null)
                    {
                        userData.UploadSongs = JsonConvert.DeserializeObject<List<UploadedSong>>(tempdatasong.ToString());

                        if (userData.UploadSongs.Count > 0)
                        {
                            InitializeUploadedSongs();

                            songUploadedListObj.SetActive(true);
                            noSongsYetUpload.SetActive(false);
                        }
                        else
                        {
                            songUploadedListObj.SetActive(false);
                            noSongsYetUpload.SetActive(true);

                            exerciseListObj.SetActive(true);
                            loadingObj.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.Log("Failed to get uploaded songs" + "  tempdatasong: " + tempdatasong);
                        notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                        loadingObj.SetActive(false);
                    }
                }, () =>
                {
                    Debug.Log("Failed to get songs on error");
                    notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                    loadingObj.SetActive(false);
                }));
            }
            else
            {
                //  ERROR PANEL HERE
                Debug.Log("Error API CALL! Error Code: " + response);
                notificationController.ShowError("There's a problem with the server! Please try again later.", null);
                loadingObj.SetActive(false);
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
                        break;
                    case 300:
                        notificationController.ShowError($"{apiresponse["data"]}", null);
                        break;
                    case 301:
                        notificationController.ShowError($"{apiresponse["data"]}", null);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Error API CALL! Error Code: " + apiRquest.result + ", " + apiRquest.downloadHandler.text);
                notificationController.ShowError("There's a problem with your internet connection! Please check your connection and try again.", null);
                loadingObj.SetActive(false);
            }
        }
    }

    private async void InitializeUploadedSongs()
    {
        for (int a = 0; a < uploadedSongParent.childCount; a++) 
        {
            Destroy(uploadedSongParent.GetChild(a).gameObject);
            await Task.Yield();
        }

        for (int a = 0; a < userData.UploadSongs.Count; a++)
        {
            GameObject tempuploadsongs = Instantiate(songUploadedItem, uploadedSongParent);

            tempuploadsongs.GetComponent<SongUploadItem>().InitializeSong(userData.UploadSongs[a]._id, userData.UploadSongs[a].songname, userData.UploadSongs[a].songfile, a, gameManager);

            await Task.Yield();
        }

        exerciseListObj.SetActive(true);
        loadingObj.SetActive(false);
    }
}
