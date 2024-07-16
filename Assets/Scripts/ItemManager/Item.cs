using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public enum ItemType
    {
        SuperMagnet,
        DoubleCoins,
        FlyTool,
        SlowTime,
    }

    public ItemType itemType;
    public int amount;
    public bool isSelected;
    public int coolDown;
    public float duration;
    public bool firstSelected;

    public Item(ItemType itemType, int amount, bool isSelected, bool firstSelected)
    {
        this.itemType = itemType;
        this.amount = amount;
        this.isSelected = isSelected;
        this.firstSelected = firstSelected;
        this.coolDown = CalculateCoolDown(itemType);
        this.duration = CalculateDuration(itemType);
    }

    private int CalculateCoolDown(ItemType type)
    {
        switch (type)
        {
            case ItemType.SuperMagnet:
                return 50;
            case ItemType.DoubleCoins:
                return 90;
            case ItemType.FlyTool:
            return 80;
            case ItemType.SlowTime:
            return 120;
            default:
                return 250;
        }
    }

    private float CalculateDuration(ItemType type)
    {
        switch (type)
        {
            case ItemType.SuperMagnet:
                return 20;
            case ItemType.FlyTool:
                return 25;
            case ItemType.DoubleCoins:
            return 25;
            case ItemType.SlowTime:
            return 20;
            default:
                return 0;
        }
    }

    public Sprite GetSprite()
    {   
        switch (itemType)
        {
            default: return ItemAssets.Instance.emptyField;
            case ItemType.SuperMagnet: return amount > 0 ? ItemAssets.Instance.superMagnetSprite : ItemAssets.Instance.emptyField;
            case ItemType.FlyTool: return amount > 0 ? ItemAssets.Instance.flyToolSprite : ItemAssets.Instance.emptyField;
            case ItemType.DoubleCoins: return amount > 0 ? ItemAssets.Instance.doubleCoinsSprite : ItemAssets.Instance.emptyField;
            
            
        }
        
    }
}
