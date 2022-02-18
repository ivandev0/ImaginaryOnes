using System;
using System.Collections;
using Enemies;
using UnityEngine;

public class PlayerPart : ObjectWithBorder {
    public float freeFallingSpeed = 5f;
    private GameObject player;
    private new Rigidbody rigidbody;
    private SphereCollider sphereCollider;
    private float radius;
    private float playerRadius;

    public bool IsAttached { get; private set; }
    private bool IsDestroyed { get; set; }
    private bool IsInvincible { get; set; }
    public bool IsSpeedUp { get; set; }
    public bool IsSlowDown { get; set; }
    public bool IsProtected { get; set; }
    public bool IsInvisible { get; set; }
    public bool IsImposter;

    private static readonly int texture = Shader.PropertyToID("Texture");

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

    public void BlowUp(EnemyType? enemy, Action atTheEnd) {
        if (IsDestroyed || IsInvincible) return;
        switch (enemy) {
            case EnemyType.Nail:
            case EnemyType.Rocket:
            case EnemyType.Boomerang:
                if (IsProtected) { RemoveProtection(); return; }
                if (IsInvisible) return;
                break;
            case EnemyType.Cloud:
            case null: break;
            default:
                throw new ArgumentOutOfRangeException(nameof(enemy), enemy, null);
        }
        IsDestroyed = true;
        StartCoroutine(Explosive.BlowUp(gameObject, atTheEnd));
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;
        sphereCollider.radius = radius;
        sphereCollider.isTrigger = false;
        IsAttached = true;
        gameObject.tag = "PlayerPart";
    }

    private IEnumerator MakeInvincible(float time) {
        IsInvincible = true;
        yield return new WaitForSeconds(time);
        IsInvincible = false;
    }

    private void RemoveProtection() {
        IsProtected = false;
        StartCoroutine(MakeInvincible(0.25f));
        GetComponent<MeshRenderer>().material.SetTexture(texture, null);
    }
}
