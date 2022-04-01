using UnityEngine;

public class FaceController: MonoBehaviour {
	private GameObject player;
	private new Rigidbody rigidbody;
	private Vector3 initPosition;

	private void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		rigidbody = GetComponent<Rigidbody>();
		initPosition = transform.position;
	}

	private void Update() {
		if (!GameController.Instance.GameIsActive()) {
			transform.position = initPosition;
			return;
		}

		var playerPosition = player.transform.position;
		var position = transform.position;
		if ((playerPosition - position).magnitude > 0.75f) {
			var normal = (position - playerPosition).normalized;
			var newPosition = playerPosition + normal * 0.75f;
			newPosition.z = position.z;
			transform.position = newPosition;
		} else {
			rigidbody.velocity = (player.transform.position - transform.position) * GetVelocityMultiplier();
		}
	}

	private float GetVelocityMultiplier() {
		var position = transform.position;
		position.z = 0;

		var distance = (player.transform.position - position).magnitude;
		if (distance <= 0.01f) return 0;
		return Mathf.Max(5, Mathf.Pow(2, distance));
	}
}