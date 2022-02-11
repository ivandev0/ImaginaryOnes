using System;
using System.Collections;
using UnityEngine;

public static class Explosive {
	public static readonly int dissolve = Shader.PropertyToID("Dissolve");

	public static IEnumerator BlowUp(GameObject gameObject, Action atTheEnd) {
		var initScale = gameObject.transform.localScale.x;
		const int start = 0;
		const float end = 1;
		const float scaleCoef = 2f;
		var material = gameObject.GetComponent<MeshRenderer>().material;

		float timeElapsed = 0;
		const float lerpDuration = 0.25f;
		while (timeElapsed < lerpDuration) {
			var scale = Mathf.Lerp(start, end, timeElapsed / lerpDuration);
			timeElapsed += Time.deltaTime;
			var newLocalScale = initScale + scale * scaleCoef;
			gameObject.transform.localScale = new Vector3(newLocalScale, newLocalScale, newLocalScale);
			material.SetFloat(dissolve, scale);
			yield return null;
		}

		var endLocalScale = initScale + end * scaleCoef;
		gameObject.transform.position = new Vector3(endLocalScale, endLocalScale, endLocalScale);
		material.SetFloat(dissolve, end);

		atTheEnd();
	}
}