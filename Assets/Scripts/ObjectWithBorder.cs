using UnityEngine;

public class ObjectWithBorder : MonoBehaviour {
	private float minimumX;
	private float maximumX;
	private float minimumY;
	private float maximumY;

	protected void Start() {
		var verticalExtent = Camera.main.orthographicSize;
		var horizontalExtent = verticalExtent * Screen.width / Screen.height;

		minimumX = -horizontalExtent;
		maximumX = horizontalExtent;
		minimumY = -verticalExtent;
		maximumY = verticalExtent;
	}

	protected Vector3 Clamp(Vector3 vector, float radius) {
		vector.x = Mathf.Clamp(vector.x, minimumX + radius, maximumX - radius);
		vector.y = Mathf.Clamp(vector.y, minimumY + radius, maximumY - radius);
		vector.z = 0;
		return vector;
	}
}