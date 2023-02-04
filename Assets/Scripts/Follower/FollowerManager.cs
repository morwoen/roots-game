using CooldownManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.InputSystem.Utilities;
using static UnityEditor.PlayerSettings;

public class FollowerManager : MonoBehaviour {
    public enum FollowerState {
        AtPlayer,
        Returning,
        Charging,
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

    [Header("Attacking")]
    [SerializeField] private float chargeDuration;
    [SerializeField] private float[] chargeDistances;
    private Vector3 chargeDirection;
    private Cooldown chargeTimer;
    private List<Vector3> chargePositionOffsets;


    [Header("Other")]
    [SerializeField] private PlayerInputController input;
    [SerializeField] private Rigidbody2D playerRB;

    private List<Follower> followers;
    private List<Vector3> basePositionOffsets;
    private FollowerState state = FollowerState.AtPlayer;


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
                state = FollowerState.Charging;
                chargeDirection = input.LookDirection.To2DV3();
                var point = transform.position + chargeDirection * (chargingSpeed * chargeDuration);
                chargePositionOffsets = GetLayeredPositionsAround(point, chargeDistances, numberOfFollowers, angleOffsets);
                Cooldown.Wait(chargeDuration).OnComplete(() => {
                    state = FollowerState.Returning;

                    Cooldown.Wait(chargeDuration).OnComplete(() => {
                        state = FollowerState.AtPlayer;
                    });
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
        }
    }

    private List<Vector3> GetLayeredPositionsAround(Vector3 center, float[] distances, int[] positions, float[] angleOffsets) {
        List<Vector3> pos = new();
        for (int i = 0; i < distances.Length; i++) {
            pos.AddRange(GetPositionsAround(center, distances[i], positions[i], angleOffsets[i]));
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
}
