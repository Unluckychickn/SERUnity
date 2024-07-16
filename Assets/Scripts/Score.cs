using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D.IK;


public class Score : MonoBehaviour, IPlayerData
{
 
    private TextMeshPro ScoreBoard;
    [SerializeField] private int CoinsValue;

    private float HighScore;
    private float scoreMultiplayer = 0.2f;
    public float ScorePoints;
    private int foundCoins;
    private bool coinsDoubled = false;
    private bool playerAlive;

    

    private void Start()
    {   playerAlive = true;
        ScoreBoard = GetComponent<TextMeshPro>();
        ScorePoints = 0;
        foundCoins = 0;
        ItemAssets.Instance.OnDoubleCoinsTrigger += DoubleCoinsValue;
        ItemAssets.Instance.OnPlayerDeath += PlayerDied;
    }
    void Update()
    {

        if(playerAlive){
            ScorePoints += scoreMultiplayer * Time.timeSinceLevelLoad;
        }
        ScoreBoard.text = $"Score: {ScorePoints:f0}";
        UpdateHighScore();

    }

    private void PlayerDied(object sender, EventArgs e){
        playerAlive = false;
    }

    private void DoubleCoinsValue(object sender, EventArgs e)
    {   Item item = new Item(Item.ItemType.DoubleCoins,1,true,false);
        coinsDoubled = true;
           Invoke(nameof(ResetCoinBool), item.duration);
    }
    private void ResetCoinBool(){
            coinsDoubled = false;
    }
    private void UpdateHighScore(){
        
        if(ScorePoints > HighScore)
        {
            HighScore = ScorePoints;
        }
    }


     public void LoadData(PlayerSaveData data)
    { 
        this.HighScore = data.HighScore;  
        Debug.Log("Load: Set Highscore to: " + this.HighScore + " and Score to: " + this.ScorePoints);      
    }
    public void SaveData(PlayerSaveData data)
    {   int boni = (int)this.ScorePoints/2000;
        data.HighScore = this.HighScore;
        data.ScorePoints = this.ScorePoints;
        data.BonusCoins = boni;
        data.EarnedCoins = foundCoins ;
        data.CurrentCoins += foundCoins + boni;
        Debug.Log("save: Set Highscore to: " + data.HighScore + " and Score to: " + data.ScorePoints);      
    }

   public void UpdateScore()
    {
        if(coinsDoubled)
        {
            ScorePoints += CoinsValue*2;
            foundCoins+=2;        
        }else{
        ScorePoints += CoinsValue; 
        foundCoins++;      
        } 
        
    }
   public void LoadInventoryData(PlayerSaveData data){}
   public void SaveInventoryData(PlayerSaveData data){}

   private void OnDestroy(){
    
    ItemAssets.Instance.OnDoubleCoinsTrigger -= DoubleCoinsValue;
    ItemAssets.Instance.OnPlayerDeath -= PlayerDied;
    coinsDoubled = false;
   }
}
