using System.Linq;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
	private static T instance = null;
	public static T Instance {
		get {
			if (instance != null) {
				return instance;
			}

			T[] manager = FindObjectsOfType<T>();
			if (manager.Length > 1) {
				Debug.LogError($"Only one instance of {typeof(T).Name} can exists");
				return null;
			}
			instance = manager.Single();
			if (Application.isPlaying) DontDestroyOnLoad(instance);
			return instance;
		}
	}
}