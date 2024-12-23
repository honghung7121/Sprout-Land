﻿using Assets._Scripts.DataPersistence.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets._Scripts.Models
{
    public class InventoryManager : MonoBehaviour
    {
        public InventorySlot[] inventorySlots;
        public GameObject inventoryItemPrefab;
        private static InventoryManager instance;
        public InventoryData inventoryData;
        int selectedSlot = -1;
        public GameObjectData selectedItem;

        public static InventoryManager Instance { get => instance; set => instance = value; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("Found more than one Inventory Manager in the scene.");
                Destroy(gameObject);
            }
            instance = this;
            ChangeSelectedSlot(0);
        }

        private void Start()
        {
            this.inventoryData = GlobalControl.Instance.inventoryData;
            RefreshInventory();
        }
        public void SaveInventory()
        {
            GlobalControl.Instance.inventoryData = this.inventoryData;
        }

        private void Update()
        {
            if(Input.inputString != null)
            {
                bool isNumber = int.TryParse(Input.inputString, out int number);
                if(isNumber && number > 0 && number  <= 9)
                {
                    ChangeSelectedSlot(number - 1);
                }
            }
        }

        void ChangeSelectedSlot(int newValue)
        {
            if(selectedSlot >= 0)
            {
                inventorySlots[selectedSlot].DeSelected();
            }
            inventorySlots[newValue].Selected();
            DragableItem inventoryItem = inventorySlots[newValue].GetComponentInChildren<DragableItem>();
            if (inventoryItem != null)
            {
                selectedItem = inventoryItem.item;
            } else
            {
                selectedItem = null;
            }
            selectedSlot = newValue;
        }

        public void AddItem(GameObjectData itemData)
        {
            GameObjectData gameObjectData = Instantiate(itemData);
            GameObject globalObject = GameObject.Find("GlobalObject");
            gameObjectData.transform.SetParent(globalObject.transform);
            gameObjectData.transform.localPosition = new Vector3(0f, 100f, 0f);
            foreach (var slot in inventorySlots)
            {
                DragableItem inventoryItem = slot.GetComponentInChildren<DragableItem>();
                if (inventoryItem != null && (inventoryItem.item.item.ID == gameObjectData.item.ID) && (inventoryItem.item.item.isStackable == true) && (inventoryItem.item.amount < inventoryItem.item.item.maxStackNumber))
                {
                    inventoryItem.count ++;
                    inventoryItem.RefreshCount();
                    inventoryItem.item.amount ++;
                    GameObjectData item = inventoryData.items.FirstOrDefault(item => item.item.ID == inventoryItem.item.item.ID);
                    Destroy(gameObjectData.gameObject);
                    return;
                }
            }

            foreach (var slot in inventorySlots)
            {
                DragableItem inventoryItem = slot.GetComponentInChildren<DragableItem>();
                if (inventoryItem == null)
                {
                    SpawnItem(gameObjectData, slot);
                    return;
                }
            }
            RefreshInventory();
        }

        public void SpawnItem(GameObjectData item, InventorySlot slot)
        {
            GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
            DragableItem inventoryItem = newItem.GetComponent<DragableItem>();
            inventoryItem.InitialiseItem(item);
            inventoryData.items.Add(item);
        }

        public List<GameObjectData> GetItems()
        {
            return inventoryData.items;
        }

        public void RemoveItem(GameObjectData targetItem, int quantity)
        {
            GameObjectData itemToRemove = inventoryData.items.FirstOrDefault(item => item.item.ID == targetItem.item.ID & item.amount == targetItem.amount);
            if (targetItem.amount == quantity)
            {
                inventoryData.items.Remove(itemToRemove);
                Destroy(targetItem.gameObject);
            }
            else
            {
                if (itemToRemove != null)
                {
                    itemToRemove.amount -= quantity;
                }
            }
            RefreshInventory();
        }

        public void RefreshInventory()
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.GetComponentInChildren<DragableItem>() != null)
                    DestroyImmediate(slot.GetComponentInChildren<DragableItem>().gameObject);
            }

            foreach (var item in inventoryData.items)
            {
                foreach (var slot in inventorySlots)
                {
                    if (slot.GetComponentInChildren<DragableItem>() == null)
                    {
                        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
                        DragableItem inventoryItem = newItem.GetComponent<DragableItem>();
                        inventoryItem.count = item.amount;
                        inventoryItem.InitialiseItem(item);
                        break;
                    }
                }
            }
        }
    }
}

