using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.CompilerServices;




public class GameOverScreen : MonoBehaviour , IPlayerData
{

    [SerializeField] private Button RestartButton;
    [SerializeField] private Button BackButton;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text HighScoreText;
    [SerializeField] private TMP_Text OwnedCoinsTxt;
    [SerializeField] private TMP_Text EarnedCoinsTxt;
    [SerializeField] private TMP_Text bonusCoinsTxt;

    private float ScorePoints;
    private float HighScore;
    private int OwnedCoins;
    private int EarnedCoins;
    private int boni;
   
    
    
   
    // Start is called before the first frame update
    void Start()
    {   
        StartCoroutine(ButtonActivation());
        HighScoreText.text = $"Highscore: {HighScore:f0}";
        ScoreText.text = $"Score: {ScorePoints:f0}";
        EarnedCoinsTxt.text = "Earned: " + EarnedCoins.ToString();
        OwnedCoinsTxt.text = "Owned: " + OwnedCoins.ToString();
        bonusCoinsTxt.text = "Bonus Coins: " + boni.ToString();
    }


    private IEnumerator ButtonActivation()
    {   Debug.Log("Start Waiting: " + Time.time);
        yield return new WaitForSeconds(2.2f);
        BackButton.interactable = true;
        RestartButton.interactable = true;
        
        StopCoroutine(ButtonActivation());
        Debug.Log("Ended Waiting: " + Time.time + " and stopped Corountine.");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Endless Runner");
    }

    public void Back()
    {
        SceneManager.LoadScene("LevelSelect");
    }
    
     public void LoadData(PlayerSaveData data)
    {
       this.HighScore = data.HighScore;
        this.ScorePoints = data.ScorePoints;
        this.OwnedCoins = data.CurrentCoins;
        this.EarnedCoins = data.EarnedCoins;
        this.boni = data.BonusCoins;
    }
    public void SaveData(PlayerSaveData data)
    {          
    }
    public void LoadInventoryData(PlayerSaveData data){}
   public void SaveInventoryData(PlayerSaveData data){}
}
