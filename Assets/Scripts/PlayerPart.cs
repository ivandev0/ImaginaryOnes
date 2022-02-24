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
    public bool IsImposter { get; set; }

    private static readonly int texture = Shader.PropertyToID("Texture");

    new void Start() {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
        rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        radius = sphereCollider.radius;
        playerRadius = player.GetComponent<SphereCollider>().radius;

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

    public bool BlowUp(EnemyType? enemy, Action atTheEnd) {
        if (IsDestroyed || IsInvincible) return false;
        switch (enemy) {
            case EnemyType.Nail:
            case EnemyType.Rocket:
            case EnemyType.Boomerang:
                if (IsProtected) { RemoveProtection(); return true; }
                if (IsInvisible) return false;
                break;
            case EnemyType.Cloud:
            case null: RemoveProtection(); break;
            default:
                throw new ArgumentOutOfRangeException(nameof(enemy), enemy, null);
        }
        IsDestroyed = true;
        StartCoroutine(Explosive.BlowUp(gameObject, atTheEnd));
        return true;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;
        if (IsImposter) {
            sphereCollider.isTrigger = false;
            Betray();
            return;
        }

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
        var renderer = GetComponent<MeshRenderer>();
        renderer.materials = new[] { renderer.materials[0] };
    }

    private void Betray() {
        var (first, second) = PlayerPartsController.Instance.GetTwoRandomParts();
        if (first == null && second == null) {
            StartCoroutine(MoveAlong((transform.position - player.transform.position).normalized));
        } else if (first == null || second == null) {
            var obj = first == null ? second : first;
            var center = (obj.transform.position + transform.position) / 2;
            StartCoroutine(obj.GetComponent<PlayerPart>().MoveAlong((obj.transform.position - center).normalized));
            StartCoroutine(MoveAlong((transform.position - center).normalized));
        } else {
            var center = (first.transform.position + second.transform.position + transform.position) / 3;
            StartCoroutine(first.GetComponent<PlayerPart>().MoveAlong((first.transform.position - center).normalized));
            StartCoroutine(second.GetComponent<PlayerPart>().MoveAlong((second.transform.position - center).normalized));
            StartCoroutine(MoveAlong((transform.position - center).normalized));
        }
    }

    // TODO change material
    private IEnumerator MoveAlong(Vector3 vector) {
        IsAttached = false;
        sphereCollider.enabled = false;

        var endValue = transform.position + vector * 10;
        var totalDistance = (transform.position - endValue).sqrMagnitude;
        var distance = totalDistance;
        while (distance > 1e-1 && !IsDestroyed) {
            float t = 1 - distance / totalDistance;
            t = t * t * (3f - 2f * t);
            rigidbody.velocity = vector * (10 * t + 1);

            distance = (transform.position - endValue).sqrMagnitude;
            yield return null;
        }

        Destroy(gameObject);
    }
}
