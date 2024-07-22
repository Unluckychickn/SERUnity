using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EndlessRun : MonoBehaviour, IPlayerData
{
    [SerializeField] private GameObject magnetVisual;
    [SerializeField] private GameObject flyngTool;
    [SerializeField] private GameObject itemPreviewTemplate_1;
    [SerializeField] private Image itemPreviewSprite_1;
    [SerializeField] private GameObject itemPreviewTemplate_2;
    [SerializeField] private Image itemPreviewSprite_2;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput playerInput;

    
    private List<Item> selectedItems = new List<Item>();
    private Inventory inventory;
    
    private Dictionary<Item, bool> itemCooldowns = new Dictionary<Item, bool>();

    public void SetInventory(Inventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory is null.");
            return;
        }

        this.inventory = inventory;
        
    }

    public void LoadData(PlayerSaveData data)
    {
        LoadInventoryData(data);
    }

    public void SaveData(PlayerSaveData data)
    {
        SaveInventoryData(data);
    }

    public void LoadInventoryData(PlayerSaveData data)
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory is not assigned.");
            return;
        }

        selectedItems.Clear();
        inventory.GetItemList().Clear();

      
        // Load inventory items
        foreach (var itemData in data.InventoryItems)
        {
            var item = new Item
            (
                itemData.ItemType,
                itemData.Amount,
                itemData.IsSelected,
                itemData.FirstSelected
            );
            inventory.AddItem(item);
        }
       
        
        RefreshDisplay();
       
    }

    public void SaveInventoryData(PlayerSaveData data)
    {
        data.InventoryItems.Clear();
       

        // Save inventory items
        foreach (var item in inventory.GetItemList())
        {
            var itemData = new ItemData(item.itemType, item.amount, item.isSelected, item.firstSelected);
            data.InventoryItems.Add(itemData);
        }

    }

    public void SetSelectedItems()
    {   selectedItems.Clear();

        selectedItems = inventory.GetItemList().FindAll(i => i.firstSelected);
        
           foreach(Item item in inventory.GetItemList())
            {
                if(!selectedItems.Contains(item) && item.isSelected)
                {
                    selectedItems.Add(item);
                }

            }
        
       /* else if(selectedItems.Count < 3 && selectedItems.Count > 1)
        {   foreach(Item item in inventory.GetItemList())
            {
                if(!selectedItems.Contains(item) && item.isSelected)
                {
                    selectedItems.Add(item);
                }

            }

        }*/
    }

    private void RefreshDisplay()
    {   
        SetSelectedItems();
       // Clear previous display
        
        itemPreviewSprite_1.sprite = ItemAssets.Instance.emptyField;
        itemPreviewSprite_2.sprite = ItemAssets.Instance.emptyField;
        itemPreviewTemplate_1.GetComponentInChildren<TMP_Text>().text = "";
        itemPreviewTemplate_2.GetComponentInChildren<TMP_Text>().text = "";    
        
        
         for (int i = 0; i < selectedItems.Count; i++)
           { Item currentItem = selectedItems[i];
            Image previewSprite = i == 0 ? itemPreviewSprite_1 : itemPreviewSprite_2;
            RectTransform previewTemplate = i == 0 ? itemPreviewTemplate_1.GetComponent<RectTransform>() : itemPreviewTemplate_2.GetComponent<RectTransform>();

            previewSprite.sprite = currentItem.GetSprite();

            TMP_Text tempText = previewTemplate.GetComponentInChildren<TMP_Text>();
            if (tempText != null)
            {
                tempText.text = currentItem.amount <= 1 ? "" : currentItem.amount.ToString();
            }
            Button button = previewTemplate.GetComponentInChildren<Button>();
           
            if(button != null)
            {button.onClick.RemoveAllListeners(); // Clear existing listeners
            button.onClick.AddListener(() => ActivateBonus(currentItem, previewTemplate)); // Capture item in lambda
            
            /*RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
            buttonRectTransform.anchorMin = Vector2.zero;
            buttonRectTransform.anchorMax = Vector2.one;
            buttonRectTransform.sizeDelta = Vector2.zero;
            buttonRectTransform.anchoredPosition = Vector2.zero;*/}
            else{
                print($"Cant find Button at {previewTemplate}");
            }
        }
    }

    private void ActivateBonus(Item item, RectTransform previewTemplate)
    {   print($"{item} button pressed!");
        if (!itemCooldowns.ContainsKey(item) || !itemCooldowns[item])
        {   print($"{item} button actuvated!");
            Debug.Log($"Activated bonus for {item.itemType}!");
            StartCoroutine(ButtonPressed(item, previewTemplate));
        }
    }

    private IEnumerator ButtonPressed(Item item, RectTransform previewTemplate)
    {
        Button button = previewTemplate.GetComponentInChildren<Button>();
        button.interactable = false;
        itemCooldowns[item] = true;
        float clickedTime = item.coolDown;
        Debug.Log($"Start in coroutine with {item.itemType}");

        inventory.RemoveItem(item);
        RefreshDisplay();
        ActivatePower(item, previewTemplate);

        TMP_Text coolDownTxt = previewTemplate.Find("CoolDownTxt").GetComponent<TMP_Text>();
        while (clickedTime > 0)
        {
            clickedTime -= Time.unscaledDeltaTime;
            coolDownTxt.text = $"{(int)clickedTime}";
            yield return null;
        }

        Debug.Log($"In ButtonPressed {item.coolDown} for {item.itemType}");

        coolDownTxt.text = $"";
        button.interactable = true;
        itemCooldowns[item] = false;
    }

    private void ActivatePower(Item item, RectTransform previewTemplate)
    {   switch(item.itemType)
       { 
        case Item.ItemType.SuperMagnet: StartCoroutine(StartMagnetPower(item, previewTemplate));
        break;
        case Item.ItemType.FlyTool: StartCoroutine(StartFlyTool(item, previewTemplate));
        break;
        }
    }

    private IEnumerator StartFlyTool(Item item, RectTransform previewTemplate)
    {   flyngTool.SetActive(true);
        float clickedTime = item.duration;
        playerInput.MaxJump = 500;
        rb.gravityScale = 0.2f;
        rb.mass = 0.5f;
        rb.drag = 1.5f;
        TMP_Text durationTxt = previewTemplate.Find("DurationTxt").GetComponent<TMP_Text>();
        while (clickedTime > 0)
        {
            clickedTime -= Time.unscaledDeltaTime;
            durationTxt.text = $"D: {clickedTime:f2}s";
            yield return null;
        }
        rb.gravityScale = 1.8f; 
        rb.mass = 3.5f;
        rb.drag = 0;
        durationTxt.text = "";
        playerInput.MaxJump = 1;
        playerInput.CheckDoubleJump = 0;
        flyngTool.SetActive(false);
    }

    private IEnumerator StartMagnetPower(Item item, RectTransform previewTemplate)
    {
        magnetVisual.SetActive(true);
        float clickedTime = item.duration;
        TMP_Text durationTxt = previewTemplate.Find("DurationTxt").GetComponent<TMP_Text>();
        while (clickedTime > 0)
        {
            clickedTime -= Time.unscaledDeltaTime;
            durationTxt.text = $"D: {clickedTime:f2}s";
            yield return null;
        }
        Debug.Log($"In StartMagnetPower {item.duration} for {item.itemType}");
        durationTxt.text = "";
        magnetVisual.gameObject.SetActive(false);
        
    }
}
