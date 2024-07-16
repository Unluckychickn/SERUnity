using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ShopManager : MonoBehaviour, IPlayerData
{
    public TMP_Text ownedCoins;
    public ShopItemSO[] shopItems;
    public ShopTemplate[] shopPanels;
    public GameObject[] shopPanelsGO;
    public Button[] purchaseButton;
    private int currentCoins;
    private int currentAmount;
    private Inventory inventory;
    public void SetInventory(Inventory sendInventory)
    { 
        if (sendInventory == null)
        {
            Debug.LogError("sendInventory is null.");
            return;
        }
       
        this.inventory = sendInventory;
    }

    private void Start()
    {   
        LoadPanels();

        for (int i = 0; i < shopItems.Length; i++)
        {
            shopPanelsGO[i].SetActive(true);
        }
        UpdateCoinsUI();
        //CheckPurchaseable();
    }

    public void LoadInventoryData(PlayerSaveData inventoryData){
         if (inventory == null)
        {
            Debug.LogError("Inventory is not assigned.");
            return;
        }
        inventory.GetItemList().Clear();

        foreach (var itemData in inventoryData.InventoryItems)
        {
            var item = new Item(itemData.ItemType, itemData.Amount, itemData.IsSelected, itemData.FirstSelected);
            inventory.AddItem(item);
        }
       
    }
   public void SaveInventoryData(PlayerSaveData inventoryData){
        inventoryData.InventoryItems.Clear();

       
          foreach (var item in inventory.GetItemList())
        {
            var itemData = new ItemData(item.itemType, item.amount, item.isSelected,item.firstSelected);
            inventoryData.InventoryItems.Add(itemData);
        }

   }
     public void LoadData(PlayerSaveData playerData)
    { 
        this.currentCoins = playerData.CurrentCoins;  
          
    }
    public void SaveData(PlayerSaveData playerData)
    {
        playerData.CurrentCoins = this.currentCoins;         
    }

 /*   public void CheckPurchaseable()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            bool isOwned = playerData.ownedSkins.Contains(shopItems[i].title);
            bool hasEnoughCoins = playerData.currentCoins >= shopItems[i].baseCost;

            if (!isOwned && hasEnoughCoins)
            {
                purchaseButton[i].interactable = true;
                purchaseButton[i].GetComponentInChildren<TMP_Text>().text = "Buy";
            }
            else if (isOwned && playerData.selectedPlayerIndex != i)
            {
                purchaseButton[i].interactable = true;
                purchaseButton[i].GetComponentInChildren<TMP_Text>().text = "Select";
            }
            else if (isOwned && playerData.selectedPlayerIndex == i)
            {
                purchaseButton[i].interactable = false;
                purchaseButton[i].GetComponentInChildren<TMP_Text>().text = "Selected";
            }
            else
            {
                purchaseButton[i].interactable = false;
                purchaseButton[i].GetComponentInChildren<TMP_Text>().text = "Buy";
            }
        }
    }   */

    public void PurchaseItem(int btnNo)
    {
        bool hasEnoughCoins = currentCoins >= shopItems[btnNo].baseCost;
         

        if (hasEnoughCoins)
        {
            // Deduct coins and add the skin to ownedSkins
            currentCoins -= shopItems[btnNo].baseCost;
           switch(btnNo){
            case 0: inventory.AddItem(new Item(Item.ItemType.SuperMagnet,1,true, false));
            break;
            case 1: inventory.AddItem(new Item(Item.ItemType.FlyTool, 1, true, false));
            break;
            
           }
            
         

            // Save the updated player configuration
            

    
           UpdateUI(btnNo);
            UpdateCoinsUI();
           
        }
        else if (!hasEnoughCoins)
        {   //Popup with not enough coins and coinsleft amount
        print("not enough coins");
        Button btn = shopItems[btnNo].GetComponentInChildren<Button>();
        btn.GetComponentInChildren<TMP_Text>().text = "Not Enough Coins!";
        }
    }

    private void UpdateUI(int index){
        Item item = inventory.GetItemList().Find(i => i.itemType == shopItems[index].itemType);
        shopPanels[index].currentAmountText.text = "Current Amount: " + item.amount.ToString();
    }

    public void LoadPanels()
    {
        for (int i = 0; i < shopItems.Length; i++)
        { 
            shopPanels[i].currentItemImage.sprite = shopItems[i].image;
            shopPanels[i].titleText.text = shopItems[i].title;
            shopPanels[i].descriptionText.text = shopItems[i].description;
            shopPanels[i].costText.text = "Coins: " + shopItems[i].baseCost.ToString();
            if(inventory.GetItemList().Count > 0)
            {Item item = inventory.GetItemList().Find(newi => newi.itemType == shopItems[i].itemType);
            if(item != null && item.amount > 0){
                shopPanels[i].currentAmountText.text = "Current Amount: " + item.amount.ToString();
                }
                else{
                shopPanels[i].currentAmountText.text = "Current Amount: 0";
            }
            }
            else{
                shopPanels[i].currentAmountText.text = "Current Amount: 0";
            }
        }
    }

    private void UpdateCoinsUI()
    {
        ownedCoins.text = "Coins: " + currentCoins.ToString();
    }

     public void Back(){
        SceneManager.LoadScene("LevelSelect");
    }

    /*
    private int GetIndex(){
        for(int i = 0; i < allPrefabs.playerPrefabs.Count; i++)
        {
            if(allPrefabs.playerPrefabs.skinName == shopItems[btnNo].title)
            {
                return allPrefabs.playerPrefabs[];
            }
        }

    }
    */
}