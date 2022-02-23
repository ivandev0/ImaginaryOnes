using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Explosive {
	private static readonly int dissolve = Shader.PropertyToID("Dissolve");
	private static readonly int offset = Shader.PropertyToID("NoiseOffset");

	public static IEnumerator BlowUp(GameObject gameObject, Action atTheEnd) {
		gameObject.GetComponent<Collider>().isTrigger = true;

		var initScale = gameObject.transform.localScale.x;
		const int start = 0;
		const float end = 1;
		const float scaleCoef = 3f;
		var material = gameObject.GetComponent<MeshRenderer>().material;
		material.SetVector(offset, new Vector4(Random.Range(0, 10f), Random.Range(0, 10f), 0, 0));

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
		gameObject.GetComponent<Collider>().isTrigger = false;

		atTheEnd();
		material.SetFloat(dissolve, start);
	}
}