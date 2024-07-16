using UnityEngine;
using UnityEngine.UIElements;

public class ShopUIController : MonoBehaviour
{
    [SerializeField] private UIDocument shopUIDocument;
    [SerializeField] private Button playerButton;

    private VisualElement shopRoot;
    private VisualElement skinsContainer;
    private VisualElement consumablesContainer;
    private Button skinsTabButton;
    private Button consumablesTabButton;
    private Button closeButton;

    private void Start()
    {
        var rootVisualElement = shopUIDocument.rootVisualElement;
        shopRoot = rootVisualElement.Q<VisualElement>("ShopRoot");
        skinsContainer = rootVisualElement.Q<VisualElement>("SkinsContainer");
        consumablesContainer = rootVisualElement.Q<VisualElement>("ConsumablesContainer");
        skinsTabButton = rootVisualElement.Q<Button>("SkinsTabButton");
        consumablesTabButton = rootVisualElement.Q<Button>("ConsumablesTabButton");
        closeButton = rootVisualElement.Q<Button>("CloseButton");

        playerButton.clicked += ToggleShopUI;
        skinsTabButton.clicked += () => ShowSection("Skins");
        consumablesTabButton.clicked += () => ShowSection("Consumables");
        closeButton.clicked += ToggleShopUI;

        shopRoot.style.display = DisplayStyle.None;
       // ShowSection("Skins"); // Default section
    }

    public void ToggleShopUI()
    {
        shopRoot.style.display = shopRoot.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void ShowSection(string sectionName)
    {
        skinsContainer.style.display = sectionName == "Skins" ? DisplayStyle.Flex : DisplayStyle.None;
        consumablesContainer.style.display = sectionName == "Consumables" ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void BuyItem(int itemId, int price)
    {
        
            Debug.Log("Not enough coins.");
        
    }
}