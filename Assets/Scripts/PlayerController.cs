using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private float minimumX;
    private float maximumX;
    private float minimumY;
    private float maximumY;

    private new Camera camera;
    private float radius;
    private Material material;
    private const float movementSpeed = 20.0f;

    private static readonly int dissolve = Shader.PropertyToID("Dissolve");

    void Start() {
        camera = Camera.main;
        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;
        material = GetComponent<MeshRenderer>().sharedMaterial;

        var verticalExtent = camera.orthographicSize;
        var horizontalExtent = verticalExtent * Screen.width / Screen.height;

        minimumX = -horizontalExtent;
        maximumX = horizontalExtent;
        minimumY = -verticalExtent;
        maximumY = verticalExtent;
    }

    void Update() {
        if (!GameController.Instance.GameIsOn()) return;

        var worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.x = Mathf.Clamp(worldPosition.x, minimumX + radius, maximumX - radius);
        worldPosition.y = Mathf.Clamp(worldPosition.y, minimumY + radius, maximumY - radius);
        worldPosition.z = 0;
        transform.position = Vector3.MoveTowards(transform.position, worldPosition, movementSpeed * Time.deltaTime);
    }

    public void MoveToTheScreenCenter(Action atTheEnd) {
        material.SetFloat(dissolve, 0);
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
        StartCoroutine(BlowUpRoutine(atTheEnd));
    }

    private IEnumerator BlowUpRoutine(Action atTheEnd) {
        var initScale = transform.localScale.x;
        const int start = 0;
        const float end = 1;
        const float scaleCoef = 2f;
        material = GetComponent<MeshRenderer>().sharedMaterial;

        float timeElapsed = 0;
        const float lerpDuration = 0.25f;
        while (timeElapsed < lerpDuration) {
            var scale = Mathf.Lerp(start, end, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            var newLocalScale = initScale + scale * scaleCoef;
            transform.localScale = new Vector3(newLocalScale, newLocalScale, newLocalScale);
            material.SetFloat(dissolve, scale);
            yield return null;
        }

        var endLocalScale = initScale + end * scaleCoef;
        transform.position = new Vector3(endLocalScale, endLocalScale, endLocalScale);
        material.SetFloat(dissolve, end);
        atTheEnd();
    }
}
