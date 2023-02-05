using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private HealthBar healthBar;
    private int currentHealth;

    private void OnEnable() {
        currentHealth = maxHealth;
        if (healthBar) {
            healthBar.Set(1);
        }
    }

    public bool Damage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            currentHealth = 0;

            Destroy(gameObject);
            return true;
        }

        if (healthBar) {
            healthBar.Set(currentHealth / (float)maxHealth);
        }

        return false;
    }
}
