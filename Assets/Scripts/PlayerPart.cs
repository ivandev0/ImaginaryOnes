using System;
using System.Collections;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerPart : ObjectWithBorder {
    public float freeFallingSpeed = 5f;

    private GameObject player;
    private new Rigidbody rigidbody;
    private SphereCollider sphereCollider;
    private MeshRenderer meshRenderer;
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

    private Coroutine imposterCoroutine;

    private static readonly int alpha = Shader.PropertyToID("_Alpha");
    private static readonly int offset = Shader.PropertyToID("_Offset");

    new void Start() {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
        rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        radius = sphereCollider.radius;
        playerRadius = player.GetComponent<SphereCollider>().radius;
        meshRenderer = GetComponent<MeshRenderer>();

        transform.rotation = Random.rotation;
        sphereCollider.radius *= 6;

        if (IsImposter) {
            imposterCoroutine = StartCoroutine(FlashNow());
        }
    }

    void Update() {
        if (!IsAttached) {
            rigidbody.velocity = Vector3.down * freeFallingSpeed;
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

        if (IsImposter && !IsDestroyed) {
            meshRenderer.materials[1].SetVector(offset, Utils.GetRandomVector4());
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
        if (enemy != null) GetComponent<AudioSource>().Play();
        StartCoroutine(Explosive.BlowUp(gameObject, atTheEnd));
        return true;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Player") || IsDestroyed) return;
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
        var imposterMaterial = meshRenderer.materials[1];
        StopCoroutine(imposterCoroutine);

        var (first, second) = PlayerPartsController.Instance.GetTwoRandomParts();
        if (first == null && second == null) {
            var center = player.transform.position;
            StartCoroutine(MoveAlong((transform.position - center).normalized));
            StartCoroutine(ChangeToImposter((transform.position - center).normalized, imposterMaterial));
        } else if (first == null || second == null) {
            var obj = first == null ? second : first;
            var center = (obj.transform.position + transform.position) / 2;
            StartCoroutine(obj.GetComponent<PlayerPart>().MoveAlong((obj.transform.position - center).normalized));
            StartCoroutine(obj.GetComponent<PlayerPart>().ChangeToImposter((obj.transform.position - center).normalized, imposterMaterial));
            StartCoroutine(MoveAlong((transform.position - center).normalized));
            StartCoroutine(ChangeToImposter((transform.position - center).normalized, imposterMaterial));
        } else {
            var center = (first.transform.position + second.transform.position + transform.position) / 3;
            StartCoroutine(first.GetComponent<PlayerPart>().MoveAlong((first.transform.position - center).normalized));
            StartCoroutine(first.GetComponent<PlayerPart>().ChangeToImposter((first.transform.position - center).normalized, imposterMaterial));
            StartCoroutine(second.GetComponent<PlayerPart>().MoveAlong((second.transform.position - center).normalized));
            StartCoroutine(second.GetComponent<PlayerPart>().ChangeToImposter((second.transform.position - center).normalized, imposterMaterial));
            StartCoroutine(MoveAlong((transform.position - center).normalized));
            StartCoroutine(ChangeToImposter((transform.position - center).normalized, imposterMaterial));
        }
    }

    public IEnumerator MoveAlong(Vector3 vector) {
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

    private IEnumerator ChangeToImposter(Vector3 vector, Material imposterMaterial) {
        IsImposter = true;
        meshRenderer.materials = new[] { meshRenderer.materials[0], imposterMaterial };
        imposterMaterial = meshRenderer.materials[1];

        var endValue = transform.position + vector * 10;
        var totalDistance = (transform.position - endValue).sqrMagnitude;
        var distance = totalDistance;
        var time = 0.0f;
        while (distance > 1e-1 && !IsDestroyed) {
            float t = 1 - distance / totalDistance;
            t = t * t * (3f - 2f * t);
            rigidbody.velocity = vector * (10 * t + 1);

            imposterMaterial.SetFloat(alpha, time);
            time += Time.deltaTime;

            distance = (transform.position - endValue).sqrMagnitude;
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator FlashNow () {
        float totalSeconds = 0.25f;
        float maxIntensity = 1f;
        var material = meshRenderer.materials[1];

        while (!IsDestroyed) {
            float waitTime = totalSeconds / 2;
            while (material.GetFloat(alpha) < maxIntensity) {
                material.SetFloat(alpha, material.GetFloat(alpha) + Time.deltaTime / waitTime);
                yield return null;
            }

            while (material.GetFloat(alpha) > 0) {
                material.SetFloat(alpha, material.GetFloat(alpha) - Time.deltaTime / waitTime);
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(0.75f, 1.5f));
        }
    }
}
