using System;
using Random = UnityEngine.Random;

public class Utils {
	public static Tuple<int, int> GetTwoRandomUniqueNumbers(int max) {
		var first = Random.Range(0, max);
		var second = first;
		while (second == first) {
			second = Random.Range(0, max);
		}

		return new Tuple<int, int>(first, second);
	}
}