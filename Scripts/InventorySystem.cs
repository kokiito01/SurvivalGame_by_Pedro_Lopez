using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public GameObject ItemInfoUi;
    public GameObject inventoryScreenUI;
    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();
    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;
    public bool isOpen;
    public GameObject pickupAlert;
    public TextMeshProUGUI pickupName;
    public Image pickupImage;


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
        isOpen = false;
        PopulateSlotList();
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            OpenUI();
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            CloseUI();
        }
    }

    public void OpenUI()
    {
        inventoryScreenUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        isOpen = true;
    }


    public void CloseUI()
    {
        inventoryScreenUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        if (!CraftingSystem.Instance.isOpen && !CampfireUIManager.Instance.isUiOPen)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        isOpen = false;
    }

    IEnumerator ShowPickupNotification()
    {
        // Muestra la notificación
        pickupAlert.SetActive(true);


        // Espera 1 segundo
        yield return new WaitForSeconds(1f);

        // Oculta la notificación después de 1 segundo
        pickupAlert.SetActive(false);
    }

    public void AddToInventory(string itemName)
    {
        whatSlotToEquip = FindNextEmptySlot();
        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);
        itemList.Add(itemName);

        StartCoroutine(ShowPickupNotification());

        TriggerPickupPopUp(itemName, itemToAdd.GetComponent<Image>().sprite);
        ReCalculateList();
    }

    void TriggerPickupPopUp(string itemName, Sprite itemSprite)
    {
        pickupAlert.SetActive(true);
        pickupName.text = itemName;
        pickupImage.sprite = itemSprite;
    }

    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public int CountItem(string itemName)
    {
        int count = 0;

        foreach (string item in itemList)
        {
            if (item == itemName)
            {
                count++;
            }
        }

        return count;
    }

    public bool CheckSlotAvaiable()
    {
        return slotList.Count >= itemList.Count;
    }

    public void RemoveItem(string nameToRemove, int amountToRemove)
    {
        int counter = amountToRemove;

        for (int i = slotList.Count - 1; i >= 0; i--)
        {
            if (slotList[i].transform.childCount > 0)
            {
                if (slotList[i].transform.GetChild(0).name == nameToRemove + "(Clone)" && counter != 0)
                {
                    Destroy(slotList[i].transform.GetChild(0).gameObject);
                    counter -= 1;
                }
            }
        }

        ReCalculateList();
    }

    public void ExchangeItems(string itemNameToRemove1, int amountToRemove1, string itemNameToRemove2, int amountToRemove2, string itemNameToAdd)
    {
        int counter1 = amountToRemove1;
        int counter2 = amountToRemove2;

        for (int i = slotList.Count - 1; i >= 0; i--)
        {
            if (slotList[i].transform.childCount > 0)
            {
                string itemName = slotList[i].transform.GetChild(0).name;

                if (itemName.Contains(itemNameToRemove1) && counter1 > 0)
                {
                    Destroy(slotList[i].transform.GetChild(0).gameObject);
                    counter1--;
                }

                if (itemName.Contains(itemNameToRemove2) && counter2 > 0)
                {
                    Destroy(slotList[i].transform.GetChild(0).gameObject);
                    counter2--;
                }
            }
        }

        // Añadir el nuevo objeto al inventario
        AddToInventory(itemNameToAdd);

        // Actualizar la UI
        ReCalculateList();
    }

    public void ReCalculateList()
    {
        itemList.Clear();

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string result = name.Replace(str2, "");
                itemList.Add(result);
            }
        }
    }
}