using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UploadSongController : MonoBehaviour
{
    [SerializeField] private NativeFilePicker.Permission permission;

    [Space]
    [SerializeField] private GameObject uploadSongObj;

    [Space]
    [SerializeField] private TMP_InputField songNameTMP;

    [Space]
    [SerializeField] private GameObject notesPrefab;
    [SerializeField] private Transform notesTF;

    [Header("DEBUGGER")]
    [SerializeField] private string songfilepath;

    private async void AskPermission()
    {
        NativeFilePicker.Permission permissionResult = await NativeFilePicker.RequestPermissionAsync(false);
        permission = permissionResult;

        if (permission == NativeFilePicker.Permission.Granted)
            AndroidPickCameraRoll();
    }

    private void AndroidPickCameraRoll()
    {
        if (permission == NativeFilePicker.Permission.Granted)
        {
            NativeFilePicker.PickFile((path) =>
            {
                if (path == null) return;

                songfilepath = path;

            }, "wav/*");
        }
        else
        {
            AskPermission();
        }
    }


}
