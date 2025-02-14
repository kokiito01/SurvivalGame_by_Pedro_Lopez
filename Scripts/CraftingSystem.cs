using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; set; }

    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI;
    public GameObject survivalScreenUI;
    public GameObject campfireScreenUI;

    public List<string> inventoryItemList = new List<string>();

    Button toolsBTN;
    Button survivalBTN;
    Button craftAxeBTN;
    Button craftStorageBoxBTN;
    Button craftCampfireBTN;
    TextMeshProUGUI CampfireReq1, CampfireReq2;
    TextMeshProUGUI AxeReq1, AxeReq2, StorageBoxReq1;

    public bool isOpen;
    bool canCraft = false;

    public Blueprint AxeBLP = new Blueprint("Axe", 1, "Stone", 3, "Stick", 3);
    public Blueprint StorageBoxBLP = new Blueprint("StorageBox", 1, "Stone", 2, "Stick", 0);
    public Blueprint CampfireBLP = new Blueprint("Campfire", 1, "Stone", 5, "Stick", 5);


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

    public IEnumerator calculate()
    {
        yield return 0;
        InventorySystem.Instance.ReCalculateList();
        RefreshNeededItems();
    }

    IEnumerator craftedDelayForSound(Blueprint blueprintToCraft)
    {
        yield return new WaitForSeconds(1f);
        for (var i = 0; i < blueprintToCraft.numberOfItemsToProduce; i++)
        {
            InventorySystem.Instance.AddToInventory(blueprintToCraft.itemName);
        }
    }


    private void Start()
    {



        isOpen = false;
        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(OpenToolsCategory);

        survivalBTN = craftingScreenUI.transform.Find("SurvivalButton").GetComponent<Button>();
        survivalBTN.onClick.AddListener(OpenSurvivalCategory);

        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<TextMeshProUGUI>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<TextMeshProUGUI>();

        // Configuración del botón de crafteo del hacha
        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(() => CraftAnyItem(AxeBLP));

        // Configuración del botón de crafteo del cofre
        StorageBoxReq1 = survivalScreenUI.transform.Find("StorageBox").transform.Find("req1").GetComponent<TextMeshProUGUI>();
        craftStorageBoxBTN = survivalScreenUI.transform.Find("StorageBox").transform.Find("Button").GetComponent<Button>();
        craftStorageBoxBTN.onClick.AddListener(delegate { CraftAnyItem(StorageBoxBLP);});

        // Configuración del botón de crafteo de la hoguera

        CampfireReq1 = survivalScreenUI.transform.Find("Campfire").transform.Find("req1").GetComponent<TextMeshProUGUI>();
        CampfireReq2 = survivalScreenUI.transform.Find("Campfire").transform.Find("req2").GetComponent<TextMeshProUGUI>();
        craftCampfireBTN = survivalScreenUI.transform.Find("Campfire").transform.Find("Button").GetComponent<Button>();
        craftCampfireBTN.onClick.AddListener(delegate { CraftAnyItem(CampfireBLP); });

        //CampfireReq1 = survivalScreenUI.transform.Find("Campfire").transform.Find("req1").GetComponent<TextMeshProUGUI>();
        //CampfireReq2 = survivalScreenUI.transform.Find("Campfire").transform.Find("req2").GetComponent<TextMeshProUGUI>();

        //craftCampfireBTN = survivalScreenUI.transform.Find("Campfire").transform.Find("Button").GetComponent<Button>();
        //craftCampfireBTN.onClick.AddListener(() => CraftAnyItem(CampfireBLP));


    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.C) && !isOpen)
        {
            Debug.Log("En");
            craftingScreenUI.SetActive(true);
            toolsScreenUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            survivalScreenUI.SetActive(false);

            if (!InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            isOpen = false;
        }

        if (craftingScreenUI.gameObject.activeSelf)
        {
            RefreshNeededItems();
        }
    }

    private void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
        survivalScreenUI.SetActive(false);
    }

    private void OpenSurvivalCategory()
    {
        craftingScreenUI.SetActive(false);
        survivalScreenUI.SetActive(true);
        toolsScreenUI.SetActive(false);
    }

    private void CraftAnyItem(Blueprint blueprintToCraft)
    {
        if (canCraft) return; // Prevenir acciones de crafteo múltiples
        canCraft = true;

        CheckCraftingRequirements(blueprintToCraft);

        if (canCraft)
        {
            // Verificar si hay suficientes recursos en el inventario
            if (InventorySystem.Instance.CountItem(blueprintToCraft.Req1) >= blueprintToCraft.Req1amount &&
                InventorySystem.Instance.CountItem(blueprintToCraft.Req2) >= blueprintToCraft.Req2amount)
            {
                // Eliminar los recursos del inventario
                InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
                InventorySystem.Instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2amount);

                // Añadir el objeto al inventario
                InventorySystem.Instance.AddToInventory(blueprintToCraft.itemName);

                // Actualizar la UI después de añadir el objeto al inventario
                RefreshNeededItems();
            }
        }
        canCraft = false; // Restablecer la bandera de crafteo
    }

    private void CheckCraftingRequirements(Blueprint blueprintToCraft)
    {
        Debug.Log(AxeReq1, AxeReq2);

        if (blueprintToCraft == null)
        {
            Debug.LogError("Error: El blueprintToCraft es nulo.");
            canCraft = false;
        }

        InventorySystem inventorySystem = InventorySystem.Instance;

        if (inventorySystem == null)
        {
            Debug.LogError("Error: El objeto InventorySystem.Instance es nulo.");
            canCraft = false;
        }

        string req1 = blueprintToCraft.Req1;
        string req2 = blueprintToCraft.Req2;

        if (string.IsNullOrEmpty(req1) || string.IsNullOrEmpty(req2))
        {
            Debug.LogError("Error: El nombre del requisito es nulo o vacío.");
            canCraft = false;
        }

        int countReq1 = inventorySystem.CountItem(req1);
        int countReq2 = inventorySystem.CountItem(req2);

        bool meetsRequirements = countReq1 >= blueprintToCraft.Req1amount && countReq2 >= blueprintToCraft.Req2amount;

        if (!meetsRequirements)
        {
            Debug.Log($"Error: No se cumplen los requisitos para craftear {blueprintToCraft.itemName}");
        }

        canCraft = true;
    }

    public void RefreshNeededItems()
    {
        int stoneCount = InventorySystem.Instance.CountItem("Stone");
        int stickCount = InventorySystem.Instance.CountItem("Stick");

        // Forzar una actualización de la lista después de modificar el inventario
        InventorySystem.Instance.ReCalculateList();

        AxeReq1.text = "3 Stone [" + stoneCount + "]";
        AxeReq2.text = "3 Stick [" + stickCount + "]";

        if (stoneCount >= 3 && stickCount >= 3 && InventorySystem.Instance.CheckSlotAvaiable())
        {
            craftAxeBTN.interactable = true;
            craftAxeBTN.gameObject.SetActive(true);
        }
        else
        {
            craftAxeBTN.interactable = false;
            craftAxeBTN.gameObject.SetActive(false);
        }

        //Debug.Log("PIEDRAS" + stoneCount);
        //Debug.Log("PALOS" + stickCount);
        //Debug.Log("INVENTARIO" + InventorySystem.Instance.CheckSlotAvaiable());

        StorageBoxReq1.text = $"2 Stone [{stoneCount}]";

        if (stoneCount >= 2 && InventorySystem.Instance.CheckSlotAvaiable())
        {
            craftAxeBTN.interactable = true;
            craftStorageBoxBTN.gameObject.SetActive(true);

        }
        else
        {
            craftStorageBoxBTN.gameObject.SetActive(false);
        }

        CampfireReq1.text = "5 Stone [" + stoneCount + "]";
        CampfireReq2.text = "5 Stick [" + stickCount + "]";

        if (stoneCount >= 5 && stickCount >= 5 && InventorySystem.Instance.CheckSlotAvaiable())
        {
            craftAxeBTN.interactable = true;
            craftCampfireBTN.gameObject.SetActive(true);

        }
        else
        {
            craftCampfireBTN.gameObject.SetActive(false);
        }
    }

    public void CloseUI()
    {

        toolsScreenUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
    }

}