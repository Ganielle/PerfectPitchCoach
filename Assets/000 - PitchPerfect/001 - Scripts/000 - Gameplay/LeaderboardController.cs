using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private NotificationController notificationController;
    [SerializeField] private APIController apiController;
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private GameObject leaderboardObj;
    [SerializeField] private List<LeaderboardItem> leaderboardItems;

    public void ShowLeaderboard()
    {
        loadingObj.SetActive(true);

        StartCoroutine(apiController.GetRequest($"/score/getleaderboard?song={gameManager.noteSelected.SongName}", "", true, (response) =>
        {
            SetLeaderboardData(response.ToString());
        }, () =>
        {
            loadingObj.SetActive(false);
        }));
    }

    private async Task SetLeaderboardData(string response)
    {
        try
        {
            List<LeaderboardData> tempdata = JsonConvert.DeserializeObject<List<LeaderboardData>>(response);

            for (int a = 0; a < leaderboardItems.Count; a++)
            {
                if (a < tempdata.Count)
                {
                    leaderboardItems[a].SetData(tempdata[a].username, tempdata[a].amount.ToString("n0"));
                }
                else
                {
                    leaderboardItems[a].SetData("", "");
                }
                await Task.Delay(100);
            }

            leaderboardObj.SetActive(true);
            loadingObj.SetActive(false);
        }
        catch(Exception ex)
        {
            notificationController.ShowError($"There's an error with the server. Error: {ex.Message}", null);

            loadingObj.SetActive(false);
        }
    }
}

[System.Serializable]
public class LeaderboardData
{
    public string _id;
    public string username;
    public float amount;
}