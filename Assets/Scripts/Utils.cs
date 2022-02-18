using System;
using Random = UnityEngine.Random;

public static class Utils {
	public static Tuple<int, int> GetTwoRandomUniqueNumbers(int max) {
		var first = Random.Range(0, max);
		var second = first;
		while (second == first) {
			second = Random.Range(0, max);
		}

		return new Tuple<int, int>(first, second);
	}

	public static int GetRandomIndexWithGivenProbabilities(float[] probabilities) {
		var cumulativeProbs = new float[probabilities.Length];
		cumulativeProbs[0] = probabilities[0];
		for (int i = 1; i < probabilities.Length; i++) {
			cumulativeProbs[i] = cumulativeProbs[i - 1] + probabilities[i];
		}

		var rnd = Random.Range(0.0f, 1.0f);
		for (var i = 0; i < cumulativeProbs.Length; i++) {
			if (cumulativeProbs[i] > rnd) {
				return i;
			}
		}

		// Not reachable
		return cumulativeProbs.Length - 1;
	}
}