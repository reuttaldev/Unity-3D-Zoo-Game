using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int initialHealth = 100;
    [SerializeField]
    private float yOffset = 15; // how much to raise the y value when respawning
    private int currentHealth; // current player health
    public int CurrentHealth { get { return currentHealth; } }
    [SerializeField]
    private Slider healthBar;

    private void Start()
    {
        SpawnPlayer();
        // subscribe to on enemy collision enter action
        CustomEventSystem.Instance.onEnemyCollision += UpdateHealth;
    }
    private void SpawnPlayer()
    {
        // raise hight to trigger landing annimation, make it look as player is respawned 
        transform.position= new Vector3(transform.position.x,yOffset, transform.position.z);
        // reset health
        this.currentHealth = initialHealth;
        UpdateUI();

    }
    private void UpdateHealth(GameObject enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("PlayerHealth: Enemy passed from action is null");
            return;
        }
        int damage = enemy.GetComponent<EnemyController>().Damage;
        this.currentHealth += damage;
        if (this.currentHealth <= 0)
        {
            SpawnPlayer();
            return;
        }
        UpdateUI();
    }
    private void UpdateUI()
    {
        // make the bar show the current health
        healthBar.value =this.currentHealth;
    }
}
