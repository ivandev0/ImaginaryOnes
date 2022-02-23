using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ObjectWithBorder {
    private new Camera camera;
    private new Rigidbody rigidbody;
    private new SphereCollider collider;
    private float radius;
    private Vector3 initPosition;
    private Vector3 initScale;
    private const float movementSpeed = 20.0f;

    protected new void Start() {
        base.Start();
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<SphereCollider>();
        radius = collider.radius * transform.localScale.x;
        initPosition = transform.position;
        initScale = transform.localScale;
    }

    void Update() {
        collider.enabled = GameController.Instance.GameIsOn();
        if (!GameController.Instance.GameIsOn()) {
            rigidbody.velocity = Vector3.zero;
            return;
        }
        var worldPosition = Clamp(camera.ScreenToWorldPoint(Input.mousePosition), radius);
        transform.position = Vector3.MoveTowards(transform.position, worldPosition, movementSpeed * Time.deltaTime);
    }

    public void MoveToTheScreenCenter(Action atTheEnd) {
        StartCoroutine(MoveToTheScreenCenterRoutine(atTheEnd));
    }

    private IEnumerator MoveToTheScreenCenterRoutine(Action atTheEnd) {
        var start = transform.position;
        var end = Vector3.zero;

        float timeElapsed = 0;
        const float lerpDuration = 1.0f;
        while (timeElapsed < lerpDuration) {
            float t = timeElapsed / lerpDuration;
            t = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(start, end, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        atTheEnd();
    }

    public void BlowUp(Action atTheEnd) {
        StartCoroutine(Explosive.BlowUp(gameObject, () => {
            transform.position = initPosition;
            transform.localScale = initScale;
            atTheEnd();
        }));
    }
}
