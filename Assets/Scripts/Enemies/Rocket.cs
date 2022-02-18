using System.Linq;
using UnityEngine;

namespace Enemies {
    public class Rocket : MonoBehaviour {
        public float speed;
        public float blowRadius;

        private new Rigidbody rigidbody;

        void Start() {
            rigidbody = GetComponent<Rigidbody>();
        }

        void Update() {
            rigidbody.velocity = Vector3.down * speed * GameController.Instance.gameSpeed;
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("PlayerPart")) return;
            var selected = Physics.OverlapSphere(other.gameObject.transform.position, blowRadius);
            if (selected.Any(it => it.gameObject.CompareTag("Player"))) {
                GameController.Instance.GameOver();
            } else {
                var partsToExplode = selected.Where(it => it.gameObject.CompareTag("PlayerPart")).ToList();
                foreach (var col in partsToExplode) {
                    col.gameObject.GetComponent<PlayerPart>().BlowUp(EnemyType.Rocket, () => {
                        foreach (var col in partsToExplode) {
                            Destroy(col.gameObject);
                        }
                    });
                }

                Destroy(gameObject);
            }
        }
    }
}
