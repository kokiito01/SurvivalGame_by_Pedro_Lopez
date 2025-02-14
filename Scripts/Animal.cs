using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public string animalName;
    public bool playerInRange;

    private Animator animator;
    public bool isDead;

    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;

    enum AnimalType
    {
        Rabbit,
        Dog
    }

    [SerializeField] AnimalType thisAnimalType;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead == false)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                animator.SetTrigger("DIE");
                GetComponent<AI_Movement>().enabled = false;

                isDead = true;
            }
        }
    }

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
