using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject slotPref, inventoryPanel, chestPanel, inventoryContent, chestContent;
    public ItemData[] items;
    public List<GameObject> inventorySlots = new List<GameObject>();
    public List<GameObject> currentChestSlots = new List<GameObject>();

    private void Start()
    {
        inventoryPanel.SetActive(false);
        chestPanel.SetActive(false);
    }

    public void CreateItem(int itemId, List<ItemData> items) 
    {
        ItemData item = new ItemData(items[itemId].name, items[itemId].description, items[itemId].id, items[itemId].count, items[itemId].isUniq);
        // if ( )
        // {

        // } else if ()
        // {

        // }
    }

}
