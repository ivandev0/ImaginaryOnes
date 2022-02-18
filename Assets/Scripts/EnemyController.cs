using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Waves {
    public GameObject[] enemies;
    public Vector3[] positions;
    public bool evenlyDistributed;
    public float spawnRate;
}

[System.Serializable]
public class Patterns {
    public Waves[] waves;
    public float waveDelay;
}

[System.Serializable]
public class EnemiesByLevel {
    public Patterns[] enemies;
    public float[] probabilities;
    public float spawnDelay;
}

public class EnemyController : Singleton<EnemyController> {
    public EnemiesByLevel[] enemiesByLevels;

    private Vector3 spawnValues;

    void Start() {
        var verticalExtent = Camera.main.orthographicSize;
        var horizontalExtent = verticalExtent * Screen.width / Screen.height;
        spawnValues = new Vector3(horizontalExtent, verticalExtent * 1.5f, 0);

        if (enemiesByLevels.Length != GameController.maxGameLevel + 1) {
            throw new ArgumentException("Enemies description must be equal to (maxGameLevel + 1)");
        }
    }

    public IEnumerator SpawnWaves() {
        while (GameController.Instance.GameIsOn()) {
            var enemiesForThisLevel = enemiesByLevels[GameController.Instance.gameLevel];
            var index = Utils.GetRandomIndexWithGivenProbabilities(enemiesForThisLevel.probabilities);
            var pattern = enemiesForThisLevel.enemies[index];

            for (var i = 0; i < pattern.waves.Length; i++) {
                var wave = pattern.waves[i];
                for (int j = 0; j < wave.enemies.Length; j++) {
                    if (wave.evenlyDistributed) {
                        // TODO
                    } else {
                        if (wave.positions.Length == wave.enemies.Length) {
                            var spawnPosition = new Vector3(wave.positions[j].x, spawnValues.y, spawnValues.z);
                            var spawnRotation = wave.enemies[j].transform.rotation;
                            Instantiate(wave.enemies[j], spawnPosition, spawnRotation);
                        } else {
                            var spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                            var spawnRotation = wave.enemies[j].transform.rotation;
                            Instantiate(wave.enemies[j], spawnPosition, spawnRotation);
                        }
                    }

                    if (wave.spawnRate != 0) {
                        yield return new WaitForSeconds(wave.spawnRate);
                    }
                }

                if (i != pattern.waves.Length - 1) {
                    yield return new WaitForSeconds(pattern.waveDelay);
                }
            }
            yield return new WaitForSeconds(enemiesForThisLevel.spawnDelay);
        }
    }
}
