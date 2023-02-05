using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private FollowerManager followerManager;
    private int currentHealth;
    private Vector3 origin;


    private void OnEnable() {
        currentHealth = maxHealth;
        healthBar.Set(1);
        origin = transform.position;
    }

    public void Damage(int damage) {
        currentHealth -= damage;

        if (currentHealth <= 0) {
            currentHealth = maxHealth;
            healthBar.Set(1);
            transform.position = origin;
            followerManager.TeleportFollowers();
            return;
        }

        healthBar.Set(currentHealth / (float)maxHealth);
    }
}
