using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void OnEnable() {
        currentHealth = maxHealth;
    }

    public bool Damage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            currentHealth = 0;

            Destroy(gameObject);
            return true;
        }

        return false;
    }

    private void OnDrawGizmos() {
        Handles.Label(transform.position, currentHealth.ToString());
    }
}
