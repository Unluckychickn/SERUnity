using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour, IPlayerData
{
    [SerializeField] private GameObject itemPreviewTemplate_1;
    [SerializeField] private Image itemPreviewSprite_1;
    [SerializeField] private GameObject itemPreviewTemplate_2;
    [SerializeField] private Image itemPreviewSprite_2;
    [SerializeField] private RectTransform itemInventoryTemplate_1;
    [SerializeField] private RectTransform border;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject rowContainerPrefab;

    private Inventory inventory;
    private List<Item> selectedItems = new List<Item>();
    private List<RectTransform> rowContainers = new List<RectTransform>();

   public void SetInventory(Inventory sendInventory)
    {
        if (sendInventory == null)
        {
            Debug.LogError("sendInventory is null.");
            return;
        }

        inventory = sendInventory;
        RefreshInventoryItems();
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

    foreach (var itemData in data.InventoryItems)
    {
        var item = new Item(itemData.ItemType, itemData.Amount, itemData.IsSelected, itemData.FirstSelected);
        inventory.AddItem(item);
    }

    // Add the firstSelected item(s) first
    selectedItems = inventory.GetItemList().Where(i => i.firstSelected).ToList();

    // Add other selected items
    var otherSelectedItems = inventory.GetItemList().Where(i => i.isSelected && !i.firstSelected).ToList();
    selectedItems.AddRange(otherSelectedItems);

    RefreshInventoryItems();
}


    public void SaveInventoryData(PlayerSaveData data)
    {
        data.InventoryItems.Clear();

        foreach (var item in inventory.GetItemList())
        {
            var itemData = new ItemData(item.itemType, item.amount, item.isSelected, item.firstSelected);
            data.InventoryItems.Add(itemData);
        }
    }

    private void RefreshInventoryItems()
    {
        ClearExistingRows();

        RectTransform currentRow = CreateNewRow();
        foreach (var item in inventory.GetItemList())
        {
            if (currentRow.childCount >= 3)
            {
                currentRow = CreateNewRow();
            }

            RectTransform newItem = Instantiate(itemInventoryTemplate_1);
            newItem.SetParent(currentRow, false);

            SetItemImage(newItem, item);
            SetItemText(newItem, item);
            SetSelectionIndicator(newItem, item);

            AddButtonListener(newItem, item);
        }

        
        RefreshPreviewItems();
        
    }

    private void ClearExistingRows()
    {
        foreach (var row in rowContainers)
        {
            Destroy(row.gameObject);
        }
        rowContainers.Clear();
    }

    private RectTransform CreateNewRow()
    {
        RectTransform newRow = Instantiate(rowContainerPrefab, border).GetComponent<RectTransform>();
        rowContainers.Add(newRow);
        return newRow;
    }

    private void SetItemImage(RectTransform item, Item inventoryItem)
    {
        Image image = item.Find("ItemImage").GetComponent<Image>();
        image.sprite = inventoryItem.GetSprite();
    }

    private void SetItemText(RectTransform item, Item inventoryItem)
    {
        TMP_Text tempText = item.GetComponentInChildren<TMP_Text>();
        if (tempText != null)
        {
            tempText.text = inventoryItem.amount <= 1 ? "" : inventoryItem.amount.ToString();
        }
    }

    private void SetSelectionIndicator(RectTransform item, Item inventoryItem)
    {
        Image selectionIndicator = item.Find("SelectionIndicator").GetComponent<Image>();
        selectionIndicator.gameObject.SetActive(inventoryItem.isSelected);
    }

    private void AddButtonListener(RectTransform item, Item inventoryItem)
    {
        Button button = item.gameObject.AddComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ToggleSelectItem(inventoryItem, item.Find("SelectionIndicator").GetComponent<Image>()));
    }

   

    private void ClearPreviewItems()
    {
        itemPreviewSprite_1.sprite = ItemAssets.Instance.emptyField;
        itemPreviewSprite_2.sprite = ItemAssets.Instance.emptyField;
        itemPreviewTemplate_1.GetComponentInChildren<TMP_Text>().text = "";
        itemPreviewTemplate_2.GetComponentInChildren<TMP_Text>().text = "";
    }

    private void RefreshPreviewItems()
    {   
        if (selectedItems.Count == 0){
            ClearPreviewItems();
            return;
        }
        
        ClearPreviewItems();
        for (int i = 0; i < selectedItems.Count; i++)
        {   
            Item item = selectedItems[i];
            Image previewSprite = i == 0 ? itemPreviewSprite_1 : itemPreviewSprite_2;
            RectTransform previewTemplate = i == 0 ? itemPreviewTemplate_1.GetComponent<RectTransform>() : itemPreviewTemplate_2.GetComponent<RectTransform>();

            previewSprite.sprite = item.GetSprite();

            TMP_Text tempText = previewTemplate.GetComponentInChildren<TMP_Text>();
            if (tempText != null)
            {
                tempText.text = item.amount <= 1 ? "" : item.amount.ToString();
            }
        }
    }

    public void ToggleSelectItem(Item selectedItem, Image selectionIndicator)
    {   
           // Handle deselection
        if (selectedItem.isSelected)
        {
            selectedItem.isSelected = false;
            selectedItem.firstSelected = false;
            selectedItems.Remove(selectedItem);
        }
        else
        {
            // Handle selection
            if (selectedItems.Count >= 2)
            {
                var firstSelected = selectedItems[0];
                firstSelected.isSelected = false;
                firstSelected.firstSelected = false;
                selectedItems.RemoveAt(0);
            }

            selectedItem.isSelected = true;
            selectedItems.Add(selectedItem);

            // Ensure the first selected item is correctly marked
            if (selectedItems.Count == 1)
            {
                selectedItem.firstSelected = true;
            }
            else
            {
                selectedItems[0].firstSelected = true;
                selectedItem.firstSelected = false;
            }
        }

        selectionIndicator.gameObject.SetActive(selectedItem.isSelected);
        RefreshPreviewItems();
    }

   public void ShowUI()
    {
        RefreshPreviewItems();
        inventoryUI.SetActive(true);
        StartCoroutine(ForceRebuildLayout());
    }

    private IEnumerator ForceRebuildLayout()
    {
        // Wait for the end of frame to ensure layout calculations are done
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(border);
    }

    public void HideUI()
    {
        inventoryUI.SetActive(false);
        PlayerDataManager.Instance.SaveGame();
    }
}
