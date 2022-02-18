using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Border : MonoBehaviour {
    void Start() {
        var boxCollider = GetComponent<BoxCollider>();
        var verticalExtent = Camera.main.orthographicSize;
        var horizontalExtent = verticalExtent * Screen.width / Screen.height;

        transform.position = new Vector3(0, -verticalExtent * 1.5f, 0);
        boxCollider.size = new Vector3(2 * horizontalExtent, 1f, 1f);
    }

    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other) {
        Destroy(other.gameObject);
    }
}
