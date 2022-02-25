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
        spawnValues = new Vector3(horizontalExtent * 0.90f, verticalExtent * 1.5f, 0);

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
                    var enemy = wave.enemies[j];
                    if (wave.evenlyDistributed) {
                        var singleBucketWidth = spawnValues.x * 2 / wave.enemies.Length;
                        var spawnPosition = -spawnValues.x + singleBucketWidth / 2.0f + j * singleBucketWidth;
                        var singleBucket = new Vector3(spawnPosition, spawnValues.y, spawnValues.z);
                        var spawnRotation = wave.enemies[j].transform.rotation;
                        Instantiate(enemy, singleBucket, spawnRotation);
                    } else {
                        if (wave.positions.Length == wave.enemies.Length) {
                            var spawnPosition = new Vector3(wave.positions[j].x, spawnValues.y, spawnValues.z);
                            var spawnRotation = wave.enemies[j].transform.rotation;
                            Instantiate(enemy, spawnPosition, spawnRotation);
                        } else {
                            var spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                            var spawnRotation = wave.enemies[j].transform.rotation;
                            Instantiate(enemy, spawnPosition, spawnRotation);
                        }
                    }

                    if (wave.spawnRate != 0 && !wave.evenlyDistributed) {
                        yield return new WaitForSeconds(ClampTime(wave.spawnRate));
                    }
                }

                if (i != pattern.waves.Length - 1) {
                    yield return new WaitForSeconds(ClampTime(pattern.waveDelay));
                }
            }
            yield return new WaitForSeconds(ClampTime(enemiesForThisLevel.spawnDelay));
        }
    }

    private float ClampTime(float time) {
        return time / GameController.Instance.gameSpeed;
    }
}
