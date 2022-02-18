using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemiesByLevel {
    public GameObject[] enemies;
}

[System.Serializable]
public class Patterns {
    public GameObject[] enemies;
    public float delay;
}

public class EnemyController : Singleton<EnemyController> {
    public int nailsCount = 2;
    public float spawnWait = 1.0f;
    public float waveWait = 2.0f;
    public GameObject nail;
    public GameObject rocket;
    public GameObject cloud;
    public GameObject boomerang;

    private Vector3 spawnValues;

    void Start() {
        var verticalExtent = Camera.main.orthographicSize;
        var horizontalExtent = verticalExtent * Screen.width / Screen.height;

        spawnValues = new Vector3(horizontalExtent / 2, verticalExtent * 1.5f, 0);
    }

    void Update() {
        
    }

    public IEnumerator SpawnWaves() {
        var enemies = new[] { nail,/* rocket, cloud, boomerang*/ };
        while (GameController.Instance.GameIsOn()) {
            foreach (var enemy in enemies) {
                for (var i = 0; i < nailsCount; i++) {
                    var spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                    var spawnRotation = enemy.transform.rotation;
                    Instantiate(enemy, spawnPosition, spawnRotation);
                    yield return new WaitForSeconds(spawnWait);
                }
            }
            yield return new WaitForSeconds(waveWait);
        }
    }
}
