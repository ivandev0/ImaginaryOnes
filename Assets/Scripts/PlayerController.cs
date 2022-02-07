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
    private const float movementSpeed = 20.0f;

    void Start() {
        camera = Camera.main;
        radius = GetComponent<SphereCollider>().radius;

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
}
