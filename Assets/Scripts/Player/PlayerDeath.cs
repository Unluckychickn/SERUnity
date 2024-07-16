using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
   
    
    [SerializeField] private Score Score;
    

    private void Start()
    {
        //Score = GetComponent<Score>();

    }

    private void OnTriggerEnter2D(Collider2D Collider)
    {
        
        if (Collider.tag == "Player")
        {  print("Entered Deathtrigger");
            
           
           ItemAssets.Instance.InvokeOnPlayerDeath();
            
          //#Right now in ItemAssets: SceneManager.LoadScene("GameOver");

        }else if(Collider.tag == "SuperMagnet"){return;}
    }
}
