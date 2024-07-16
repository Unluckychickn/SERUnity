using System.Collections.Generic;
using UnityEngine;
using System.Collections;



[System.Serializable]
public class PlayerSaveData
{
    public int CurrentCoins;
    public float HighScore;
    public float ScorePoints;
    public int EarnedCoins;
    public string LastScene;
    public int BonusCoins;

  
     public List<ItemData> InventoryItems;
   
    public PlayerSaveData(){
        CurrentCoins = 0;
        HighScore = 0;
        ScorePoints = 0;
        LastScene = "";
        BonusCoins = 0;
        InventoryItems = new List<ItemData>();

        
    }
}

[System.Serializable]
public class ItemData
{
    public Item.ItemType ItemType;
    public int Amount;
    public bool IsSelected;
    public int CoolDown;
    public float Duration;
    public bool FirstSelected;
    
    public ItemData(Item.ItemType type, int amount, bool selected, bool firstSelected)
    {
        ItemType = type;
        Amount = amount;
        IsSelected = selected;
        FirstSelected = firstSelected;
    }
}

[System.Serializable]
public class ItemTypeData
{
    public Item.ItemType ItemType;
}
/*
[System.Serializable]
public class RectTransformData
{
    public float[] Position;
    public float[] Rotation;
    public float[] Scale;
}

[System.Serializable]
public class ItemUIData
{
    public ItemTypeData ItemType;
    public RectTransformData RectTransform;
}


public static class RectTransformExtensions
{
    public static RectTransformData ToRectTransformData(this RectTransform rectTransform)
    {
        return new RectTransformData
        {
            Position = new float[] { rectTransform.localPosition.x, rectTransform.localPosition.y, rectTransform.localPosition.z },
            Rotation = new float[] { rectTransform.localRotation.eulerAngles.x, rectTransform.localRotation.eulerAngles.y, rectTransform.localRotation.eulerAngles.z },
            Scale = new float[] { rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.z }
        };
    }

    public static void FromRectTransformData(this RectTransform rectTransform, RectTransformData data)
    {
        rectTransform.localPosition = new Vector3(data.Position[0], data.Position[1], data.Position[2]);
        rectTransform.localRotation = Quaternion.Euler(data.Rotation[0], data.Rotation[1], data.Rotation[2]);
        rectTransform.localScale = new Vector3(data.Scale[0], data.Scale[1], data.Scale[2]);
    }
}
*/
