using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class DataStorage : MonoBehaviour
{

    private PlayerData player;

    string[] GameDataArray;

    Dictionary<string, string> GameData = new Dictionary<string, string>();

    void Start()
    {

        //SaveData();
        LoadData();
    }

    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public int playerScore;
        public int trophyCount;
        public bool hasWon;
    }

    public void StoreHighscore(int ScorePoints)
    {
        player = new PlayerData
        {
            playerScore = ScorePoints,
        };

        SaveData();
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(player, true);
  
        System.IO.File.WriteAllText("playerData.json", json);
    }

    public void LoadData()
    {
        string json = System.IO.File.ReadAllText("playerData.json");

        ConvertDataToTable(json);

        PlayerData loadedPlayer = JsonUtility.FromJson<PlayerData>(json);

        int playerScore = loadedPlayer.playerScore;

        Debug.Log($"JSON  Score {playerScore} ");

    }

    private void ConvertDataToTable(string Data)
    {
        GameDataArray = Data.Split(new char[] { '{', ',', ':', '}' });

        for (int i = 0; i < GameDataArray.Length; i++)
        {
            if (GameDataArray[i] != "")
            {
                //GameIndex
                if (i % 2 != 0)
                {
                    GameData.Add(GameDataArray[i], GameDataArray[i+1]);
                }

            }
        }

        foreach(KeyValuePair<string, string> kvp in GameData)
        {
            Debug.Log("JSON Table Index: " + kvp.Key + " | JSON Table Data: " + kvp.Value);
        }
    }
}
