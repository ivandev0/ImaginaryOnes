using System;
using UnityEngine;

public class PlayerPart : ObjectWithBorder {
    public float freeFallingSpeed = 5f;
    private GameObject player;
    private new Rigidbody rigidbody;
    private SphereCollider sphereCollider;
    private float radius;
    private float playerRadius;

    public bool IsAttached { get; private set; } = false;
    private bool isDestroyed = false;

    new void Start() {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
        rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        radius = sphereCollider.radius * player.transform.localScale.x;
        playerRadius = sphereCollider.radius * transform.localScale.x;

        sphereCollider.radius *= 6;
    }

    void Update() {
        if (!IsAttached) {
            rigidbody.velocity = -transform.up * freeFallingSpeed;
        } else {
            var clampedPosition = Clamp(transform.position, radius / 2);
            if (clampedPosition != transform.position) {
                transform.position = clampedPosition;
            } else {
                if (Vector3.Distance(transform.position, player.transform.position) > playerRadius + radius) {
                    rigidbody.velocity = (player.transform.position - transform.position) * GetVelocityMultiplier();
                } else {
                    rigidbody.velocity = Vector3.zero;
                }
            }
        }
    }

    private float GetVelocityMultiplier() {
        var distance = (player.transform.position - transform.position).magnitude;
        if (distance <= radius + playerRadius) return 0;
        return distance * Mathf.Log(distance);
    }

    public void BlowUp(Action atTheEnd) {
        if (isDestroyed) return;
        isDestroyed = true;
        StartCoroutine(Explosive.BlowUp(gameObject, atTheEnd));
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;
        sphereCollider.radius = radius;
        sphereCollider.isTrigger = false;
        IsAttached = true;
        gameObject.tag = "PlayerPart";
    }
}
