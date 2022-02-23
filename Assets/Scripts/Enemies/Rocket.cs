using System.Collections;
using System.Linq;
using UnityEngine;

namespace Enemies {
    public class Rocket : MonoBehaviour {
        public float speed;
        public float blowRadius;

        private bool ready;

        private new Rigidbody rigidbody;

        void Start() {
            rigidbody = GetComponent<Rigidbody>();
            StartCoroutine(MoveToStartPosition());
        }

        void Update() {
            if (!ready) return;
            rigidbody.velocity = Vector3.down * speed * GameController.Instance.gameSpeed;
        }

        private IEnumerator MoveToStartPosition() {
            var cameraSize = Camera.main.orthographicSize;
            var height = GetComponent<CapsuleCollider>().height * transform.localScale.y;
            var start = transform.position;
            var end = new Vector3(start.x, cameraSize - height / 1.5f, 0);
            float t = 0;

            while ((transform.position - end).sqrMagnitude > 0.0001f) {
                transform.position = Vector3.Lerp(start, end, t);
                t += 0.5f * Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(1.0f);
            ready = true;
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("PlayerPart")) return;
            if (other.GetComponent<PlayerPart>()?.IsInvisible == true) return;
            var selected = Physics.OverlapSphere(other.gameObject.transform.position, blowRadius);
            if (selected.Any(it => it.gameObject.CompareTag("Player"))) {
                GameController.Instance.GameOver();
            } else {
                var partsToExplode = selected.Where(it => it.gameObject.CompareTag("PlayerPart"))
                    .Where(it => !it.gameObject.GetComponent<PlayerPart>().IsInvisible)
                    .ToList();
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
