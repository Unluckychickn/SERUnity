using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopMenu", menuName = "Scriptable Objects/New Shop Item", order = 1)]
public class ShopItemSO : ScriptableObject
    {
        public string title;
        public string description;
        public int baseCost;
        public Sprite image;
        public Item.ItemType itemType;

    }

