using System;
using System.Diagnostics;
using Firebase.Database;
using UnityEngine;

public class GameInfo
{
    public string date, time;
    public int playerWin, playerLose;
    public string duration;

    private Stopwatch stopwatch;

    public GameInfo()
    {
        stopwatch = Stopwatch.StartNew();
    }

    public void SaveGameInformation(int winner, int loser)
    {
        date = DateTime.Now.ToShortDateString();
        time = DateTime.Now.ToShortTimeString();

        playerWin = winner;
        playerLose = loser;

        stopwatch.Stop();
        duration = stopwatch.Elapsed.TotalSeconds.ToString();

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        string json = JsonUtility.ToJson(this);
        reference.Child("Game History").Push().SetRawJsonValueAsync(json);
    }
}