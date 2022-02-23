using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ObjectWithBorder {
    private new Camera camera;
    private new Rigidbody rigidbody;
    private float radius;
    private Material material;
    private Vector3 initPosition;
    private Vector3 initScale;
    private const float movementSpeed = 20.0f;

    protected new void Start() {
        base.Start();
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;
        material = GetComponent<MeshRenderer>().material;
        initPosition = transform.position;
        initScale = transform.localScale;
    }

    void Update() {
        if (!GameController.Instance.GameIsOn()) {
            rigidbody.velocity = Vector3.zero;
            return;
        }
        var worldPosition = Clamp(camera.ScreenToWorldPoint(Input.mousePosition), radius);
        transform.position = Vector3.MoveTowards(transform.position, worldPosition, movementSpeed * Time.deltaTime);
    }

    public void MoveToTheScreenCenter(Action atTheEnd) {
        material.SetFloat(Explosive.dissolve, 0);
        transform.position = initPosition;
        transform.localScale = initScale;
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
        StartCoroutine(Explosive.BlowUp(gameObject, atTheEnd));
    }
}
