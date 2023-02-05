using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Set(float val) {
        spriteRenderer.size = new Vector2(val, 1);
    }
}
