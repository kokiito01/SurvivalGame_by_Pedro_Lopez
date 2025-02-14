using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{

    public static SelectionManager Instance { get; set; }

    public GameObject interaction_info_UI;
    TextMeshProUGUI interaction_text;


    public bool onTarjet;

    //Para que no se recojan todas los objetos a la vez
    public GameObject selectedObject;

    public Image centerDotImage;
    public Image handIcon;

    public bool handIsVisible;

    public GameObject selectedTree;
    public GameObject chopHolder;

    public GameObject selectedStorageBox;
    public GameObject selectedCampfire;
    public GameObject respawnUno;
    public GameObject player;


    private void Start()
    {
        interaction_text = interaction_info_UI.GetComponent<TextMeshProUGUI>();
        onTarjet = false;
    }

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

    void DisableSelection()
    {
        if (selectedTree != null)
        {
            selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
            selectedTree = null;
            chopHolder.gameObject.SetActive(false);
        }
    }

    void Update()
    {

         
        if (Input.GetKeyDown(KeyCode.P))
        {
            player.transform.position = respawnUno.transform.position;
        }


        centerDotImage.gameObject.SetActive(true);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

           

            ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();

            if (choppableTree && choppableTree.playerInRange)
            {
                choppableTree.canBeChopped = true;
                selectedTree = choppableTree.gameObject;
                chopHolder.gameObject.SetActive(true);
            }
            else
            {
                if (selectedTree != null)
                {
                    selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
                    selectedTree = null;
                    chopHolder.gameObject.SetActive(false);
                }
            }


            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

            if (interactable && interactable.playerInRange)
            {

                onTarjet = true;
                selectedObject = interactable.gameObject;
                interaction_text.text = interactable.GetItemName();
                interaction_info_UI.SetActive(true);

                ///centerDotImage.gameObject.SetActive(false);
                handIcon.gameObject.SetActive(true);

                handIsVisible = true;
            }

            StorageBox storageBox = selectionTransform.GetComponent<StorageBox>();

            if (storageBox && storageBox.playerInRange && PlacementSystem.Instance.inPlacementMode == false)
            {
                interaction_text.text = "Open";
                interaction_info_UI.SetActive(true);

                selectedStorageBox = storageBox.gameObject;

                if (Input.GetMouseButtonDown(0))
                {
                    StorageManager.Instance.OpenBox(storageBox);
                }
            }
            else
            {
                if (selectedStorageBox != null)
                {
                    selectedStorageBox = null;
                }
            }

            Campfire campfire = selectionTransform.GetComponent<Campfire>();

            if (campfire && campfire.playerInRange && PlacementSystem.Instance.inPlacementMode == false)
            {
                interaction_text.text = "Interact";
                interaction_info_UI.SetActive(true);

                selectedCampfire = campfire.gameObject;

                if (Input.GetMouseButtonDown(0) && campfire.isCooking == false)
                {
                    campfire.OpenUI();
                }
            }
            else
            {
                if (selectedCampfire != null)
                {
                    selectedCampfire = null;
                }
            }

            Animal animal = selectionTransform.GetComponent<Animal>();

            if (animal && animal.playerInRange)
            {

                if (animal.isDead)
                {
                    interaction_text.text = "Loot";
                    interaction_info_UI.SetActive(true);

                    centerDotImage.gameObject.SetActive(false);
                    handIcon.gameObject.SetActive(false);

                    handIsVisible = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        Lootable lootable = animal.GetComponent<Lootable>();
                        Loot(lootable);
                    }
                }
                else
                {
                    interaction_text.text = animal.animalName;
                    interaction_info_UI.SetActive(true);

                    centerDotImage.gameObject.SetActive(true);
                    handIcon.gameObject.SetActive(false);

                    handIsVisible = false;

                    if (Input.GetMouseButtonDown(0) && EquipSystem.Instance.IsHoldingWeapon()&& EquipSystem.Instance.IsThereASwingLock() == false)
                    {
                        StartCoroutine(DealDamageTo(animal, 0.3f, EquipSystem.Instance.GetWeaponDamage()));
                    }
                }
            }

            if (!interactable && !animal)
            {
                onTarjet = false;
                handIsVisible = false;

                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
            }
            
            if (InventorySystem.Instance.isOpen || CraftingSystem.Instance.isOpen)
            {
                centerDotImage.gameObject.SetActive(false);
                handIcon.gameObject.SetActive(false);
                return;
            }
            if (!interactable && !animal && !choppableTree && !storageBox && !campfire)
            {
                interaction_text.text = "";
                interaction_info_UI.SetActive(false);
            }



        }
    }

    private void Loot(Lootable lootable)
    {
        if (lootable.wasLootCalculated == false)
        {
            List<LootRecieved> recievedLoot = new List<LootRecieved>();

            foreach (LootPosibility loot in lootable.posibleLoot)
            {
                var lootAmount = UnityEngine.Random.Range(loot.amountMin, loot.amountMax +1);
                if(lootAmount != 0)
                {
                    LootRecieved lt = new LootRecieved();
                    lt.item = loot.item;
                    lt.amount = lootAmount;

                    recievedLoot.Add(lt);
                }
            }

            lootable.finalLoot = recievedLoot;
            lootable.wasLootCalculated = true;
        }

        Vector3 lootSpawnPosition = lootable.gameObject.transform.position;

        foreach (LootRecieved lootRecieved in lootable.finalLoot)
        {
            for (int i = 0; i < lootRecieved.amount; i++)
            {
                string resourceName = lootRecieved.item.name + "_Model";
                GameObject lootPrefab = Resources.Load<GameObject>(resourceName);

                if (lootPrefab == null)
                {
                    Debug.LogError($"No se pudo cargar el recurso: {resourceName}");
                }
                else
                {
                    GameObject lootSpawn = Instantiate(lootPrefab, new Vector3(lootSpawnPosition.x, lootSpawnPosition.y + 0.2f, lootSpawnPosition.z), Quaternion.Euler(0, 0, 0));
                }
            }
        }

        //if (lootable.GetComponent<Animal>())
        //{
        //    lootable.GetComponent<Animal>().bloodPuddle.transform.SetParent(lootable.transform.parent);
        //}

        Destroy(lootable.gameObject);
    }

    IEnumerator DealDamageTo(Animal animal, float delay, int damage)
    {
        yield return new WaitForSeconds(delay);
        animal.TakeDamage(damage);
    }
}