using UnityEngine;

namespace Enemies {
    public class Nail : MonoBehaviour, Enemy {
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

        public void Randomize() {
            //
        }

        private void OnTriggerEnter(Collider other) {
            if (IsDestroyed) return;

            if (other.gameObject.CompareTag("Player")) {
                GameController.Instance.GameOver();
            } else if (other.gameObject.CompareTag("PlayerPart")) {
                var wasTouched = other.gameObject.GetComponent<PlayerPart>().BlowUp(EnemyType.Nail, () => {
                    if (other.gameObject != null) Destroy(other.gameObject);
                });

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
