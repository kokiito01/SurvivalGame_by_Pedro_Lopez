using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipSystem : MonoBehaviour
{
    public static EquipSystem Instance { get; set; }

    // -- UI -- //
    public GameObject quickSlotsPanel;

    public List<GameObject> quickSlotsList = new List<GameObject>();


    public GameObject numbersHolder;

    public int selectedNumber = -1;
    public GameObject selectedItem;

    public GameObject toolHolder;

    public GameObject selectedItemModel;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    private void Start()
    {
        PopulateSlotList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectQuickSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectQuickSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectQuickSlot(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectQuickSlot(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectQuickSlot(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectQuickSlot(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectQuickSlot(7);
        }
    }

    void SelectQuickSlot(int number)
    {
        if (checkIfSlotsFull(number) == true)
        {

            if (selectedNumber != number)
            {
                selectedNumber = number;

                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                }

                selectedItem = GetSelectedItem(number);
                selectedItem.GetComponent<InventoryItem>().isSelected = true;

                SetEquippedModel(selectedItem);

                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = Color.gray;
                }

                TextMeshProUGUI toBeChanged = numbersHolder.transform.Find("number" + number).transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                toBeChanged.color = Color.white;
            }
            else
            {
                selectedNumber = -1;

                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                    selectedItem = null;
                }

                if (selectedItemModel != null)
                {
                    DestroyImmediate(selectedItemModel.gameObject);
                    selectedItemModel = null;
                }

                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = Color.gray;
                }
            }
        }
    }

    private void SetEquippedModel(GameObject selectedItem)
    {

        if (selectedItemModel != null)
        {
            DestroyImmediate(selectedItemModel.gameObject);
            selectedItemModel = null;
        }

        string selectedItemName = selectedItem.name.Replace("(Clone)", "");
        selectedItemModel = Instantiate(Resources.Load<GameObject>(selectedItemName + "_Model"),
            new Vector3(0.7f, 0f, 1f), Quaternion.Euler(0f, -100f, 0f));
        selectedItemModel.transform.SetParent(toolHolder.transform, false);
    }


    GameObject GetSelectedItem(int slotNumber)
    {
        return quickSlotsList[slotNumber - 1].transform.GetChild(0).gameObject;
    }

    bool checkIfSlotsFull(int slotNumber)
    {
        if (quickSlotsList[slotNumber - 1].transform.childCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private void PopulateSlotList()
    {
        foreach (Transform child in quickSlotsPanel.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                quickSlotsList.Add(child.gameObject);
            }
        }
    }

    public void AddToQuickSlots(GameObject itemToEquip)
    {
        // Find next free slot
        GameObject availableSlot = FindNextEmptySlot();
        // Set transform of our object
        itemToEquip.transform.SetParent(availableSlot.transform, false);
        // Getting clean name
        string cleanName = itemToEquip.name.Replace("(Clone)", "");


        InventorySystem.Instance.ReCalculateList();

    }


    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckSlotsAvailable()
    {

        int counter = 0;

        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        if (counter == 7)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal bool IsHoldingWeapon()
    {
        if (selectedItem != null)
        {
            if (selectedItem.GetComponent<Weapon>() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }

    internal int GetWeaponDamage()
    {
        if (selectedItem != null)
        {
            return selectedItem.GetComponent<Weapon>().weaponDamage;
        }
        else
        {
            return 0;
        }
    }

    internal bool IsThereASwingLock()
    {
        if (selectedItemModel && selectedItemModel.GetComponent<EquipableItem>())
        {
            return selectedItemModel.GetComponent<EquipableItem>().swingWait;
        }
        else
        {
            return false;
        }
    }
}
