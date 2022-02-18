using UnityEngine;

namespace Enemies {
    public class Nail : MonoBehaviour {
        public float speed;

        private new Rigidbody rigidbody;

        void Start() {
            rigidbody = GetComponent<Rigidbody>();
        }

        void Update() {
            rigidbody.velocity = Vector3.down * speed * GameController.Instance.gameSpeed;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                GameController.Instance.GameOver();
            } else if (other.gameObject.CompareTag("PlayerPart")) {
                other.gameObject.GetComponent<PlayerPart>().BlowUp(EnemyType.Nail, () => { Destroy(other.gameObject); });
            }
        }
    }
}
