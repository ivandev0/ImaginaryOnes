using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rocket : MonoBehaviour {
    public float speed;
    public float blowRadius;

    private new Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        rigidbody.velocity = -transform.up * speed;
    }

    private void OnTriggerEnter(Collider other) {
        var selected = Physics.OverlapSphere(other.gameObject.transform.position, blowRadius);
        if (selected.Any(it => it.gameObject.CompareTag("Player"))) {
            GameController.Instance.GameOver();
        } else {
            var partsToExplode = selected.Where(it => it.gameObject.CompareTag("PlayerPart")).ToList();
            foreach (var col in partsToExplode) {
                col.gameObject.GetComponent<PlayerPart>().BlowUp(() => {
                    foreach (var col in partsToExplode) {
                        Destroy(col.gameObject);
                    }
                });
            }
            Destroy(gameObject);
        }
    }
}
