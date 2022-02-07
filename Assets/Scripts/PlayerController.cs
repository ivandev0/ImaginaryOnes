using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float minimumX;
    public float maximumX;
    public float minimumY;
    public float maximumY;

    private new Camera camera;
    private float radius;

    void Start() {
        camera = Camera.main;
        radius = GetComponent<SphereCollider>().radius;
    }

    void Update() {
        var worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.x = Mathf.Clamp(worldPosition.x, minimumX + radius, maximumX - radius);
        worldPosition.y = Mathf.Clamp(worldPosition.y, minimumY + radius, maximumY - radius);
        worldPosition.z = 0;
        transform.position = worldPosition;
    }
}
