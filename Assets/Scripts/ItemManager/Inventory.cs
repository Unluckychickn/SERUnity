using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory {

    private List<Item> itemList;
    
    public Inventory(){
        itemList = new List<Item>();


    }

    public void AddItem(Item newItem)
    {
        Item existingItem = itemList.Find(item => item.itemType == newItem.itemType);
        if (existingItem != null)
        {
            existingItem.amount += newItem.amount;
        }
        else
        {
             itemList.Add(new Item(newItem.itemType, newItem.amount, newItem.isSelected, newItem.firstSelected));
        }
        
    }

    public void RemoveItem(Item itemToRemove)
    {
        Item existingItem = itemList.Find(item => item.itemType == itemToRemove.itemType);
        if (existingItem != null)
        {
            existingItem.amount--;
            if (existingItem.amount <= 0)
            {
                itemList.Remove(existingItem);
            }
        }
       
    }

    public List<Item> GetItemList(){

        return itemList;
    }
}
