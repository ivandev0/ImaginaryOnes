using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct Waves {
    public GameObject[] enemies;
    public float[] positions;
    public bool evenlyDistributed;
    public float spawnRate;
}

[System.Serializable]
public struct Patterns {
    public Waves[] waves;
    public float waveDelay;
}

[System.Serializable]
public struct EnemiesByLevel {
    public Patterns[] enemies;
    public float[] probabilities;
    public float spawnDelay;
}

public class EnemyController : Singleton<EnemyController> {
    public GameObject nail, rocket, boomerangLarge, boomerangSmall, cloud;

    private EnemiesByLevel[] enemiesByLevels;
    private float afterBoomerangDelay = 1.5f;

    private Vector3 spawnValues;
    private float verticalExtent, horizontalExtent;

    private Patterns SingleWavePattern(
        GameObject model, int count, float[] xPositions = null, float spawnRate = 0, float waveDelay = 0
    ) {
        return new Patterns()
        {
            waves = new[]
            {
                new Waves()
                {
                    enemies = Enumerable.Repeat(model, count).ToArray(),
                    positions = xPositions ?? Array.Empty<float>(),
                    evenlyDistributed = false,
                    spawnRate = spawnRate
                }
            },
            waveDelay = waveDelay
        };
    }

    private void Awake() {
        enemiesByLevels = new[]
        {
            new EnemiesByLevel()
            {
                enemies = new[] { SingleWavePattern(nail, 1) },
                probabilities = new []{1.0f},
                spawnDelay = 1.5f
            },
            new EnemiesByLevel()
            {
                enemies = new[]
                {
                    SingleWavePattern(nail, 1),
                    SingleWavePattern(rocket, 1),
                },
                probabilities = new []{0.7f, 0.3f},
                spawnDelay = 1.25f
            },
            new EnemiesByLevel()
            {
                enemies = new[]
                {
                    SingleWavePattern(nail, 2),
                    SingleWavePattern(rocket, 1),
                    SingleWavePattern(boomerangLarge, 1, new []{ 0.0f }, waveDelay: afterBoomerangDelay)
                },
                probabilities = new []{0.5f, 0.3f, 0.2f},
                spawnDelay = 1.25f
            },
            new EnemiesByLevel()
            {
                enemies = new[]
                {
                    SingleWavePattern(nail, 3, waveDelay: 1),
                    SingleWavePattern(rocket, 1),
                    SingleWavePattern(boomerangLarge, 1, new []{ 0.0f }, waveDelay: afterBoomerangDelay),
                    SingleWavePattern(cloud, 1, new []{ 0.0f }, waveDelay: 0.5f),
                    SingleWavePattern(cloud, 1, new []{ 2.835f }, waveDelay: 0.5f),
                    SingleWavePattern(cloud, 1, new []{ -2.835f }, waveDelay: 0.5f),
                },
                probabilities = new []{0.3f, 0.3f, 0.3f, 0.1f / 3, 0.1f / 3, 0.1f / 3},
                spawnDelay = 1.25f
            },
            new EnemiesByLevel()
            {
                enemies = new[]
                {
                    SingleWavePattern(nail, 4, waveDelay: 1),
                    SingleWavePattern(rocket, 1),
                    SingleWavePattern(boomerangLarge, 1, new []{ 0.0f }, waveDelay: afterBoomerangDelay),
                    SingleWavePattern(boomerangSmall, 1, new []{ 2.67f }, waveDelay: afterBoomerangDelay),
                    SingleWavePattern(boomerangSmall, 1, new []{ -2.67f }, waveDelay: afterBoomerangDelay),
                    SingleWavePattern(cloud, 1, new []{ 0.0f }, waveDelay: 0.5f),
                    SingleWavePattern(cloud, 1, new []{ 2.835f }, waveDelay: 0.5f),
                    SingleWavePattern(cloud, 1, new []{ -2.835f }, waveDelay: 0.5f),
                },
                probabilities = new []{0.3f, 0.2f, 0.1f, 0.1f, 0.1f, 0.2f / 3,  0.2f / 3, 0.2f / 3},
                spawnDelay = 1.25f
            },
            new EnemiesByLevel()
            {
                enemies = new[]
                {
                    SingleWavePattern(nail, 15, spawnRate: 0.2f, waveDelay: 0.5f),
                    SingleWavePattern(boomerangSmall, 1, new []{ 2.67f }, waveDelay: afterBoomerangDelay),
                    SingleWavePattern(boomerangSmall, 1, new []{ -2.67f }, waveDelay: afterBoomerangDelay),
                    SingleWavePattern(cloud, 1, new []{ 0.0f }, waveDelay: 0.5f),
                    SingleWavePattern(cloud, 1, new []{ 2.835f }, waveDelay: 0.5f),
                    SingleWavePattern(cloud, 1, new []{ -2.835f }, waveDelay: 0.5f),
                },
                probabilities = new []{0.1f, 0.3f, 0.3f, 0.1f, 0.1f, 0.1f},
                spawnDelay = 1.25f
            },
            new EnemiesByLevel()
            {
                enemies = new[]
                {
                    SingleWavePattern(nail, 15, spawnRate: 0.2f, waveDelay: 0.5f),
                    new Patterns()
                    {
                        waves = new []
                        {
                            new Waves()
                            {
                                enemies = new []{nail, nail, nail, nail },
                                positions = Array.Empty<float>(),
                                evenlyDistributed = true,
                                spawnRate = 0
                            },
                            new Waves()
                            {
                                enemies = new []{nail, nail, nail, nail },
                                positions = Array.Empty<float>(),
                                evenlyDistributed = true,
                                spawnRate = 0
                            },
                            new Waves()
                            {
                                enemies = new []{nail, nail, nail, nail },
                                positions = Array.Empty<float>(),
                                evenlyDistributed = true,
                                spawnRate = 0
                            }
                        },
                        waveDelay = 0.5f
                    },
                    SingleWavePattern(rocket, 2, spawnRate: 0.25f),
                    SingleWavePattern(boomerangLarge, 1, new []{ 0.0f }, waveDelay: afterBoomerangDelay),
                    SingleWavePattern(cloud, 1, new []{ 0.0f }, waveDelay: 0.5f),
                    SingleWavePattern(cloud, 1, new []{ 2.835f }, waveDelay: 0.5f),
                    SingleWavePattern(cloud, 1, new []{ -2.835f }, waveDelay: 0.5f),
                },
                probabilities = new []{0.1f, 0.1f, 0.2f, 0.1f, 0.5f / 3, 0.5f / 3, 0.5f / 3},
                spawnDelay = 1.25f
            }
        };
    }

    void Start() {
        verticalExtent = Camera.main.orthographicSize;
        horizontalExtent = verticalExtent * Screen.width / Screen.height;
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
                var randomY = Random.Range(0, verticalExtent * 0.5f);
                var randomXPositions = GetRandomLocations(wave.enemies.Length, wave.spawnRate != 0.0f);
                for (var j = 0; j < wave.enemies.Length; j++) {
                    var enemy = wave.enemies[j];
                    var spawnY = spawnValues.y + randomY;
                    GameObject newEnemyObject;
                    if (wave.evenlyDistributed) {
                        var singleBucketWidth = spawnValues.x * 2 / wave.enemies.Length;
                        var spawnPosition = -spawnValues.x + singleBucketWidth / 2.0f + j * singleBucketWidth;
                        var singleBucket = new Vector3(spawnPosition, spawnY, spawnValues.z);
                        var spawnRotation = wave.enemies[j].transform.rotation;
                        newEnemyObject = Instantiate(enemy, singleBucket, spawnRotation);
                    } else {
                        if (wave.positions.Length == wave.enemies.Length) {
                            var spawnPosition = new Vector3(wave.positions[j], spawnY, spawnValues.z);
                            var spawnRotation = wave.enemies[j].transform.rotation;
                            newEnemyObject = Instantiate(enemy, spawnPosition, spawnRotation);
                        } else {
                            var spawnPosition = new Vector3(randomXPositions[j], spawnY, spawnValues.z);
                            var spawnRotation = wave.enemies[j].transform.rotation;
                            newEnemyObject = Instantiate(enemy, spawnPosition, spawnRotation);
                        }
                    }

                    newEnemyObject.GetComponent<Enemy>().Randomize();
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

    private List<float> GetRandomLocations(int count, bool totallyRandom) {
        var randomXPositions = new List<float>();

        var i = 0;
        while(i < count) {
            if (totallyRandom) {
                randomXPositions.Add(Random.Range(-spawnValues.x, spawnValues.x));
            } else {
                for (var j = 0; j < 10; j++) {
                    var randomX = Random.Range(-spawnValues.x, spawnValues.x);
                    if (randomXPositions.Any(it => Math.Abs(it - randomX) < horizontalExtent * 0.15f)) {
                        continue;
                    }
                    randomXPositions.Add(randomX);
                    break;
                }
            }

            i++;
        }

        return randomXPositions;
    }

    private float ClampTime(float time) {
        return time / GameController.Instance.gameSpeed;
    }
}
