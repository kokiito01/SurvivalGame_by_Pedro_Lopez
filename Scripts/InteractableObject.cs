using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{

    public bool playerInRange;

    public string ItemName;

    public string GetItemName()
    {
        return ItemName;
    }


    //Para recoger un objeto del suelo y que desaparezca
    void Update()
    {//Soloo si, Se hace click con el raton, estamos en el rango de recogida, si estamos en el objetivo apuntando , y solo si estamos recogiendo el objeto que queremos recoger                
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && SelectionManager.Instance.onTarjet && SelectionManager.Instance.selectedObject == gameObject)
        {
            if (InventorySystem.Instance.CheckSlotAvaiable())
            {
                InventorySystem.Instance.AddToInventory(ItemName);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("nO Añadido");
                Destroy(gameObject);
            }

        }
    }

    // Entrada y salida del area de recogida del objeto
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}