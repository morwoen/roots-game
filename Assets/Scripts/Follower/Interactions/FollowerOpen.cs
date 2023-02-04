using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowerOpen : FollowerInteractable {
    [SerializeField] private Sprite openSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D col;
    [SerializeField] private int followersRequired;

    public override IEnumerator Interact(FollowerManager manager) {
        yield return new WaitForSeconds(1);

        if (manager.ActiveFollowers < followersRequired) {
            // TODO: Feedback
            yield break;
        }

        yield return new WaitForSeconds(1);

        spriteRenderer.sprite = openSprite;
        col.enabled = false;
    }

    private void Reset() {
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}
