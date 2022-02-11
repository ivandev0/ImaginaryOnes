using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nail : MonoBehaviour {
    public float speed;

    private new Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        rigidbody.velocity = -transform.up * speed;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            GameController.Instance.GameOver();
        } else if (other.gameObject.CompareTag("PlayerPart")) {
            other.gameObject.GetComponent<PlayerPart>().BlowUp(() => { Destroy(other.gameObject); });
        }
    }
}
