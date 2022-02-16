using UnityEngine;

namespace Enemies {
    public class Cloud : MonoBehaviour {
        public float speedY;
        public float speedX;
        public float movementRadiusX;

        private Vector3 center;
        private float angle;

        void Start() {
            center = transform.position;
        }

        void Update() {
            transform.position = new Vector3( center.x + movementRadiusX * Mathf.Cos(angle), -speedY, 0);
            angle += speedX * Time.deltaTime;
            angle %= 360;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                GameController.Instance.GameOver();
            } else if (other.gameObject.CompareTag("PlayerPart")) {
                other.gameObject.GetComponent<PlayerPart>().BlowUp(() => { Destroy(other.gameObject); });
            }
        }
    }
}
