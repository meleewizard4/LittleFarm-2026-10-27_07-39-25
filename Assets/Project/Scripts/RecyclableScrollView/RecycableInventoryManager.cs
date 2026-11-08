using PolyAndCode.UI;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RecycableInventoryManager : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;

    [SerializeField]
    private int _datalength;

    public GameObject inventoryGameObject;

    private List<InventoryItems> _InventoryItems = new List<InventoryItems>();

    private void Awake()
    {
        _recyclableScrollRect.DataSource = this;
    }

    public int GetItemCount()
    {
        return _InventoryItems.Count;
    }    

    public void SetCell(ICell cell, int index)
    {
        var item = cell as CellItemData;
        item.ConfigureCell(_InventoryItems[index], index);
    }

    public void Start()
    {
        List<InventoryItems> listItem = new List<InventoryItems>();
        for (int i = 0; i < 50; i++)
        {
            InventoryItems inventoryItem = new InventoryItems();
            inventoryItem.Name = "Name_" + i.ToString();
            inventoryItem.Description = "Des_" + i.ToString();

            listItem.Add(inventoryItem);
        }
        SetListItem(listItem);
        _recyclableScrollRect.ReloadData();
        ToggleInventory();
    }

    public void SetListItem(List<InventoryItems> list)
    {
        _InventoryItems = list;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) //bag
        {
            ToggleInventory();
        }
    }

    public void AddInventoryItem(InventoryItems item)
    {
        _InventoryItems.Add(item);
        _recyclableScrollRect.ReloadData();
    }

    void ToggleInventory()
    {
        Vector3 crrPosInven = inventoryGameObject.GetComponent<RectTransform>().anchoredPosition;
        inventoryGameObject.GetComponent<RectTransform>().anchoredPosition = crrPosInven.y == 1000 ? Vector3.zero : new Vector3(0, 1000, 0);
    }
}
