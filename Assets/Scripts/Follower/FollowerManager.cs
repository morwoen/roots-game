using CooldownManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class FollowerManager : MonoBehaviour {
    public enum FollowerState {
        AtPlayer,
        Returning,
        Charging,
        Interacting,
    }

    [Header("Movement Speeds")]
    [SerializeField] private float atPlayerSpeed = 1;
    [SerializeField] private float returningSpeed = 1;
    [SerializeField] private float chargingSpeed = 1;

    [Header("Positioning")]
    [SerializeField] private float[] distances;
    [SerializeField] private int[] numberOfFollowers;
    [SerializeField] private float[] angleOffsets;
    [SerializeField] private float directedDistanceFromPlayer = 1;

    [Header("Charging")]
    [SerializeField] private float chargeDuration;
    [SerializeField] private float[] chargeDistances;
    private Vector3 chargeDirection;
    private Cooldown chargeTimer;
    private List<Vector3> chargePositionOffsets;

    [Header("Interacting")]
    [SerializeField] private float spinSpeed = 1;
    [SerializeField] private Transform movementTelegraph;
    private FollowerInteractable interactable;
    private float[] interactDistances;
    private float spinOffset = 0;

    [Header("Other")]
    [SerializeField] private PlayerInputController input;
    [SerializeField] private Rigidbody2D playerRB;

    private List<Follower> followers;
    private List<Vector3> basePositionOffsets;
    private FollowerState state = FollowerState.AtPlayer;

    public int ActiveFollowers {
        get {
            return followers.Count;
        }
    }

    private void OnEnable() {
        followers = FindObjectsOfType<Follower>().ToList();
        basePositionOffsets = GetLayeredPositionsAround(Vector3.zero, distances, numberOfFollowers, angleOffsets);
    }

    private void OnDisable() {
        chargeTimer?.Stop();
    }

    private void Update() {
        if (state == FollowerState.AtPlayer) {
            var pos = transform.position + input.LookDirection.To2DV3() * directedDistanceFromPlayer;

            for (int i = 0; i < followers.Count; i++) {
                var destination = pos + basePositionOffsets[i % basePositionOffsets.Count];
                var direction = destination - followers[i].transform.position;
                followers[i].rb.velocity = atPlayerSpeed * (direction.magnitude > .1 ? direction.normalized : direction);
            }

            if (input.Attack) {
                input.AttackPerformed();
                SwitchState(FollowerState.Charging);
                chargeDirection = input.LookDirection.To2DV3();
                var point = transform.position + chargeDirection * (chargingSpeed * chargeDuration);
                chargePositionOffsets = GetLayeredPositionsAround(point, chargeDistances, numberOfFollowers, angleOffsets);
                chargeTimer = Cooldown.Wait(chargeDuration).OnComplete(() => {
                    SwitchState(FollowerState.Returning);
                });
            }
        } else if (state == FollowerState.Returning) {
            var pos = transform.position + input.LookDirection.To2DV3() * directedDistanceFromPlayer;

            for (int i = 0; i < followers.Count; i++) {
                var destination = pos + basePositionOffsets[i % basePositionOffsets.Count];
                var direction = destination - followers[i].transform.position;
                followers[i].rb.velocity = returningSpeed * (direction.magnitude > 1 ? direction.normalized : direction);
            }
        } else if (state == FollowerState.Charging) {
            for (int i = 0; i < followers.Count; i++) {
                var direction = chargePositionOffsets[i] - followers[i].transform.position;
                followers[i].rb.velocity = returningSpeed * (direction.magnitude > 1 ? direction.normalized : direction);
            }
        } else if (state == FollowerState.Interacting) {
            if (interactable.shouldSpin) {
                spinOffset = (spinOffset + spinSpeed * Time.deltaTime) % 360;
            }
            var pos = GetLayeredPositionsAround(interactable.transform.position, interactDistances, numberOfFollowers, angleOffsets, spinOffset);
            for (int i = 0; i < followers.Count; i++) {
                var direction = pos[i] - followers[i].transform.position;
                followers[i].rb.velocity = atPlayerSpeed * (direction.magnitude > 1 ? direction.normalized : direction);
            }

            if (interactable is FollowerPickup) {
                if (input.LookDirection.magnitude == 0) {
                    movementTelegraph.gameObject.SetActive(false);
                } else {
                    movementTelegraph.gameObject.SetActive(true);
                    movementTelegraph.position = transform.position + input.LookDirection.To2DV3();
                }
            }
        }
    }

    private void SwitchState(FollowerState newState) {
        switch (newState) {
            case FollowerState.Returning:
                foreach (var follower in followers) {
                    follower.col.enabled = false;
                }

                chargeTimer = Cooldown.Wait(chargeDuration).OnComplete(() => {
                    SwitchState(FollowerState.AtPlayer);
                });

                break;
            case FollowerState.AtPlayer:
                foreach (var follower in followers) {
                    follower.col.enabled = true;
                }
                break;
        }

        state = newState;
    }

    private List<Vector3> GetLayeredPositionsAround(Vector3 center, float[] distances, int[] positions, float[] angleOffsets, float additionalOffset = 0) {
        List<Vector3> pos = new();
        for (int i = 0; i < distances.Length; i++) {
            pos.AddRange(GetPositionsAround(center, distances[i], positions[i], angleOffsets[i] + additionalOffset));
        }
        return pos;
    }

    private List<Vector3> GetPositionsAround(Vector3 center, float distance, int positions, float angleOffset) {
        List<Vector3> pos = new();
        for (int i = 0; i < positions; i++) {
            var angle = angleOffset + i * (360f / positions);
            Vector3 dir = Quaternion.Euler(new Vector3(0, 0, angle)) * Vector3.right;
            pos.Add(center + dir * distance);
        }
        return pos;
    }

    private void OnDrawGizmos() {
        Handles.Label(transform.position, state.ToString());
    }

    public void Interact(FollowerInteractable interactable) {
        if (state != FollowerState.Charging) return;
        chargeTimer?.Stop();
        this.interactable = interactable;
        interactDistances = chargeDistances.Select(d => d + interactable.distance).ToArray();

        if (interactable.shouldSpin) {
            foreach (var follower in followers) {
                follower.col.enabled = false;
            }
        }

        SwitchState(FollowerState.Interacting);

        StartCoroutine(ExecuteInteraction(interactable));
    }

    private IEnumerator ExecuteInteraction(FollowerInteractable interactable) {
        yield return interactable.Interact(this);

        SwitchState(FollowerState.Returning);
    }

    public void Spin() {
        spinOffset = (spinOffset + 100) % 360;
    }
}
