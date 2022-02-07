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
        var worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.x = Mathf.Clamp(worldPosition.x, minimumX + radius, maximumX - radius);
        worldPosition.y = Mathf.Clamp(worldPosition.y, minimumY + radius, maximumY - radius);
        worldPosition.z = 0;
        transform.position = worldPosition;
    }
}
