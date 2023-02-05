using CooldownManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FollowerCleanse : FollowerInteractable {
    [SerializeField] private int followersRequired;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image lockImage;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color corruptedColor;
    [SerializeField] private Color cleansedColor;
    [SerializeField] private Material cleansedMaterial;
    [SerializeField] private TilemapRenderer rootRenderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip cleanseClip;
    [SerializeField] private AudioClip notEnoughFollowersClip;

    private bool isCleansed = false;
    private Cooldown colorReset;

    private void OnEnable() {
        spriteRenderer.color = corruptedColor;
        text.SetText(followersRequired.ToString());
        if (followersRequired == 0) {
            Destroy(lockImage.gameObject);
            Destroy(text.gameObject);
        }
    }

    public override IEnumerator Interact(FollowerManager manager) {
        yield return new WaitForSeconds(1);

        if (!isCleansed && manager.ActiveFollowers < followersRequired) {
            text.color = Color.red;
            lockImage.color = Color.red;

            colorReset = Cooldown.Wait(3).Always(() => {
                text.color = Color.white;
                lockImage.color = Color.white;
            });

            audioSource.PlayOneShot(notEnoughFollowersClip);

            yield break;
        }

        colorReset?.Stop();
        if (lockImage) {
            Destroy(lockImage.gameObject);
        }
        if (text) {
            Destroy(text.gameObject);
        }

        yield return new WaitForSeconds(2);

        if (!isCleansed) {
            audioSource.PlayOneShot(cleanseClip);
            spriteRenderer.color = cleansedColor;
            Cooldown.Wait(1).OnComplete(() => {
                rootRenderer.material = cleansedMaterial;
                FindObjectOfType<EndGameMenu>().PoolCleansed();
            });
        }
        isCleansed = true;
    }
}
