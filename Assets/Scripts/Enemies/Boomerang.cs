using UnityEngine;

namespace Enemies {
    public class Boomerang : MonoBehaviour {
        public float speedY;
        public float rotationSpeed;
        public float movementRadiusX;
        public float movementRadiusY;

        private Vector3 center;
        private float angle;

        void Start() {
            center = transform.position;
        }

        void Update() {
            var x = center.x + movementRadiusX * Mathf.Cos(angle);
            var y = center.y + movementRadiusY * Mathf.Sin(angle);
            center = new Vector3(center.x, center.y - Time.deltaTime * speedY * GameController.Instance.gameSpeed, 0);
            transform.position = new Vector3( x, y, 0);
            angle += rotationSpeed * Time.deltaTime;
            angle %= 360;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                GameController.Instance.GameOver();
            } else if (other.gameObject.CompareTag("PlayerPart")) {
                other.gameObject.GetComponent<PlayerPart>().BlowUp(EnemyType.Boomerang, () => { Destroy(other.gameObject); });
            }
        }
    }
}
