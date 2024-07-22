using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;
using Unity.Mathematics;

public class GenerateGround : MonoBehaviour
{   
    [SerializeField] private float minHeightPoint;
    [SerializeField] private float maxHeightPoint;
    [SerializeField] private float minGap;
    [SerializeField] private float maxGap;
    [SerializeField] private ObjectPooler GroundPooler;
    [SerializeField] private ObjectPooler VisualPooler;
    [SerializeField] private ObjectPooler CoinPooler;
    
    [SerializeField] private Transform StartingPoint;
    [SerializeField] private float SpeedIncreaseFactor = 1.25f;
    [SerializeField] private Transform SpawnControllPoint;
   
    [SerializeField] private Transform OldGround;
    [SerializeField] private float SpeedMileStone = 500f;
    [SerializeField] private float MileStoneCounter;
    [SerializeField] private float CurrentMS = -4.0f;
    [SerializeField] private int maxItemSpawnMileStone;
    [SerializeField] private int minItemSpawnMileStone;
    [SerializeField] private ObjectPooler spawnItems;
    [SerializeField] private List<string> spawnableItems;

    

    private Vector3 pos;
    private int CoinCounter;
    private List<IGroundMovementSpeed> GroundMSObjects;
    private Score Score;
    private int rndMileStone;
    private int tempRnd;
    private bool spawnItem = false;


    
    void Start()
    {   
        tempRnd = UnityEngine.Random.Range(minItemSpawnMileStone, maxItemSpawnMileStone);
        rndMileStone = tempRnd;
        Score = FindAnyObjectByType<Score>();
        OldGround.position = StartingPoint.position;
       
        MileStoneCounter = SpeedMileStone;

        GenerateFirstGround();
    }

    private List<IGroundMovementSpeed> FindAllGroundMSObjects() 
    {
        IEnumerable<IGroundMovementSpeed> GroundMSObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IGroundMovementSpeed>();

        return new List<IGroundMovementSpeed>(GroundMSObjects);
    }

 

    private void GenerateFirstGround()
    {
        CreateGround();
    }

    void Update()
    {   
      
      
        if ( Score.ScorePoints > MileStoneCounter)
        {  
            MileStoneCounter += SpeedMileStone;
            CurrentMS *= SpeedIncreaseFactor;
            LoadNewMS(CurrentMS);
        }

        if(Score.ScorePoints > rndMileStone && OldGround.position.x < SpawnControllPoint.position.x)
        {   tempRnd = UnityEngine.Random.Range(minItemSpawnMileStone*2, maxItemSpawnMileStone*2);
            rndMileStone += tempRnd;
            spawnItem = true;
            CreateGround();
            
            spawnItem = false;
            return;
        }
       
        if (OldGround.position.x < SpawnControllPoint.position.x)
        {
            CreateGround();           
        }

        
    }

    public void LoadNewMS(float MS)
    {
        this.GroundMSObjects = FindAllGroundMSObjects();
        if (this.GroundMSObjects != null)
        { 
            foreach (IGroundMovementSpeed DataWantingObj in GroundMSObjects)
            {
                DataWantingObj.SetMovementSpeed(MS);
                
            }
        }
    }

    private void CreateGround()
    {
        string[] GroundTags = { "SmallGround", "MediumGround", "LargeGround" };
        string RandomGroundTag = GroundTags[UnityEngine.Random.Range(0, GroundTags.Length)];
        pos.x = UnityEngine.Random.Range(minGap, maxGap);
        pos.y = UnityEngine.Random.Range(minHeightPoint, maxHeightPoint);

        GameObject Ground = GroundPooler.GetPooledObject(RandomGroundTag);
        if(Ground == null)
        {
            Debug.LogError($"No GroundObject found for tag {RandomGroundTag}");
        }
        Ground.transform.position = StartingPoint.position + new Vector3(pos.x, pos.y, 0);
       try{ Ground.GetComponent<GroundMovement>().SetMovementSpeed(CurrentMS);
       }catch(Exception e){
        Debug.LogError($"No GroundMovement found on {Ground.tag}!:{e}");
       }  
        AttachVisuals(Ground, RandomGroundTag);
        

        SetOldGroundParent(Ground);

        
    }

    private void SetOldGroundParent(GameObject Ground){
            OldGround.SetParent(Ground.transform, true);
            BoxCollider2D GroundCollider = Ground.GetComponentInChildren<BoxCollider2D>();
            if(GroundCollider != null){
            OldGround.position = new Vector3(Ground.transform.position.x + GroundCollider.size.x, Ground.transform.position.y, Ground.transform.position.z);
            }else{
                Debug.LogError($"No BoxCollider2D fround in children of {Ground.name}");
            }
    }
    private void AttachVisuals(GameObject ground, string GroundTag)
    {  
        switch(GroundTag){
                        
           
            case "LargeGround":
                CoinCounter = 6;

                // Attach a random medium visual
                GameObject mediumVisual = VisualPooler.GetPooledObject("MediumVisual");
                if(mediumVisual != null && !ground.transform.Find("MediumVisual") )
                {
                mediumVisual.transform.SetParent(ground.transform);
                mediumVisual.transform.position = ground.transform.position;
                }else{
                    Debug.LogError($"No {tag} found!");
                }
                
                
               // Attach a random small visual
                GameObject smallVisual = VisualPooler.GetPooledObject("SmallVisual");
                if(smallVisual != null && !ground.transform.Find("MediumVisual") ){
                smallVisual.transform.SetParent(ground.transform);
                smallVisual.transform.position = ground.transform.position + new Vector3(4, 0, 0);
                }else{
                    Debug.LogError($"No {tag} found!");
                }
                if(spawnItem)
                {
                    AttachItem(ground);
                }else{
                AttachCoins(ground, CoinCounter);
                }
                break;
            case "MediumGround":
                
                if(!ground.transform.Find("MediumVisual"))
                {
                AttachVisualFromPool(ground, "MediumVisual");
                }

                CoinCounter = 4;
                 if(spawnItem)
                {
                    AttachItem(ground);
                }else{
                AttachCoins(ground, CoinCounter);
                }
                break;

            case "SmallGround":

              if(!ground.transform.Find("SmallVisual")) 
                {
                    AttachVisualFromPool(ground, "SmallVisual");  
                }
                
                CoinCounter = 3;
                 if(spawnItem)
                {
                    AttachItem(ground);
                }else{
                AttachCoins(ground, CoinCounter);
                }
                break;

                default:
                 Debug.LogError($"Unsupported ground tag: {GroundTag}");
                 break;
        }
            
       
    }

    private void AttachItem(GameObject ground)

    {   pos.x = UnityEngine.Random.Range(-2, 2);
        pos.y = UnityEngine.Random.Range(1, 4);
        int iIndex = UnityEngine.Random.Range(0,spawnableItems.Count);
        string itemtag = spawnableItems[iIndex];
        GameObject item = spawnItems.GetPooledObject(itemtag);
        item.transform.SetParent(ground.transform);
        item.transform.position = ground.transform.position + new Vector3(pos.x, pos.y,0);
        

    }

    private void AttachVisualFromPool(GameObject ground, string visualTag)
    {   
    GameObject visual = VisualPooler.GetPooledObject(visualTag);
      if(visual != null)
        { 
        visual.transform.SetParent(ground.transform);
        visual.transform.position = ground.transform.position;
        }else
        {
             print($"No {visualTag} found!");
        }
    
    }

    private void AttachCoins(GameObject ground, int coinCounter)
    {   
        int maxCoins = UnityEngine.Random.Range(0, coinCounter);
        for (int i = 0; i < maxCoins; i++)
        {
            GameObject newCoin = CoinPooler.GetPooledObject("Coin");
            newCoin.transform.SetParent(ground.transform);
            newCoin.transform.position = ground.transform.position + new Vector3(i, 0.5f, 0);
        }
    }

}
