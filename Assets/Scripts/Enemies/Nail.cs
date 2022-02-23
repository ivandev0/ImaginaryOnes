using UnityEngine;

namespace Enemies {
    public class Nail : MonoBehaviour {
        public int health;
        public float speed;

        private new Rigidbody rigidbody;
        private bool IsDestroyed { get; set; }

        void Start() {
            rigidbody = GetComponent<Rigidbody>();
        }

        void Update() {
            rigidbody.velocity = Vector3.down * speed * GameController.Instance.gameSpeed;
        }

        private void OnTriggerEnter(Collider other) {
            if (IsDestroyed) return;

            if (other.gameObject.CompareTag("Player")) {
                GameController.Instance.GameOver();
            } else if (other.gameObject.CompareTag("PlayerPart")) {
                var wasTouched = other.gameObject.GetComponent<PlayerPart>().BlowUp(EnemyType.Nail, () => { Destroy(other.gameObject); });

                if (wasTouched) {
                    IsDestroyed = true;
                    health--;
                }

                if (health == 0) {
                    Destroy(gameObject);
                }
            }
        }
    }
}
