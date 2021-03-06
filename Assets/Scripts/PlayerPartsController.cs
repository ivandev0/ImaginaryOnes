using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class LevelStats {
    public float[] probabilities = new float[6];
    public int partsCount;
    public float spawnWait;
    public float waveWait;
}

[Serializable]
public class Materials {
    public Material[] materials;
}

public class PlayerPartsController : Singleton<PlayerPartsController> {
    public LevelStats[] stats;
    public GameObject playerPart;

    public Materials[] commonMaterials;
    public Material speedUpMat;
    public Material slowDownMat;
    public Material protectedMat;
    public Material invisibleMat;
    public Material imposterMat;

    private Vector3 spawnValues;
    private Material[] buffedMaterials;
    private Material[] currentMaterials;

    void Start() {
        var verticalExtent = Camera.main.orthographicSize;
        var horizontalExtent = verticalExtent * Screen.width / Screen.height;
        spawnValues = new Vector3(horizontalExtent * 0.95f, verticalExtent * 1.5f, 0);

        if (stats.Length != GameController.maxGameLevel + 1) {
            throw new ArgumentException("Level stats length must be equal to (maxGameLevel + 1)");
        }

        buffedMaterials = new[] { speedUpMat, slowDownMat, protectedMat, invisibleMat, imposterMat };

        foreach (var stat in stats) {
            if (stat.probabilities.Length != buffedMaterials.Length + 1) {
                throw new ArgumentException("Probabilities length must be equal to materials count");
            }

            if (Math.Abs(stat.probabilities.Sum() - 1) > 1e-5) {
                throw new ArgumentException("Probabilities sum must be equal to 1");
            }
        }
    }

    public void UpdateMaterials() {
        currentMaterials = commonMaterials[GameController.Instance.currentMaterialsIndex].materials;
    }

    public Material GetFirstMaterial() {
        return currentMaterials[0];
    }

    public IEnumerator SpawnWaves() {
        while (GameController.Instance.GameIsOn()) {
            var currentStats = stats[GameController.Instance.gameLevel];
            for (var i = 0; i < currentStats.partsCount; i++) {
                var spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                GetNextPart(currentStats, spawnPosition);
                yield return new WaitForSeconds(currentStats.spawnWait);
            }
            yield return new WaitForSeconds(currentStats.waveWait);
        }
    }

    public int GetPlayersPartsCount() {
        return transform.Cast<Transform>().Count(child => child.GetComponent<PlayerPart>().IsAttached);
    }

    public Tuple<GameObject, GameObject> GetTwoRandomParts() {
        var allParts = transform.Cast<Transform>().Where(child => child.GetComponent<PlayerPart>().IsAttached).ToList();
        switch (allParts.Count) {
            case 0: return new Tuple<GameObject, GameObject>(null, null);
            case 1: return new Tuple<GameObject, GameObject>(allParts[0].gameObject, null);
            case 2: return new Tuple<GameObject, GameObject>(allParts[0].gameObject, allParts[1].gameObject);
            default:
                var (first, second) = Utils.GetTwoRandomUniqueNumbers(allParts.Count);
                return new Tuple<GameObject, GameObject>(allParts[first].gameObject, allParts[second].gameObject);
        }
    }

    public int GetSpeedUpCount() {
        return transform.Cast<Transform>().Count(child => {
            var playerPartComp = child.GetComponent<PlayerPart>();
            return playerPartComp.IsAttached && playerPartComp.IsSpeedUp;
        });
    }

    public int GetSlowDownCount() {
        return transform.Cast<Transform>().Count(child => {
            var playerPartComp = child.GetComponent<PlayerPart>();
            return playerPartComp.IsAttached && playerPartComp.IsSlowDown;
        });
    }

    private GameObject GetNextPart(LevelStats stat, Vector3 position) {
        var index = Utils.GetRandomIndexWithGivenProbabilities(stat.probabilities);
        var obj = Instantiate(playerPart, position, Quaternion.identity, transform);
        var randomCommonMaterial = currentMaterials[Random.Range(0, currentMaterials.Length)];

        if (index == 0) {
            obj.GetComponent<MeshRenderer>().material = randomCommonMaterial;
            return obj;
        }

        var bufferMaterial = buffedMaterials[index - 1];
        obj.GetComponent<MeshRenderer>().material = bufferMaterial;

        var playerPartScript = obj.GetComponent<PlayerPart>();
        switch (index) {
            // case 0: break;
            case 1: playerPartScript.IsSpeedUp = true; break;
            case 2: playerPartScript.IsSlowDown = true; break;
            case 3: {
                obj.GetComponent<MeshRenderer>().materials = new[] { randomCommonMaterial, bufferMaterial };
                playerPartScript.IsProtected = true;
                break;
            }
            case 4: playerPartScript.IsInvisible = true; break;
            case 5: {
                obj.GetComponent<MeshRenderer>().materials = new[] { randomCommonMaterial, bufferMaterial };
                playerPartScript.IsImposter = true;
                break;
            }
            default: throw new ArgumentException("Unsupported property");
        }

        return obj;
    }
}
