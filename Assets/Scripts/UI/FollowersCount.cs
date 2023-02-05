using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FollowersCount : MonoBehaviour {
    [SerializeField] private FollowerManager followerManager;
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable() {
        followerManager.onFollowerChange += OnFollowerChange;
        text.SetText(followerManager.ActiveFollowers.ToString());
    }

    private void OnDisable() {
        followerManager.onFollowerChange -= OnFollowerChange;
    }

    private void OnFollowerChange() {
        text.SetText(followerManager.ActiveFollowers.ToString());
    }
}
