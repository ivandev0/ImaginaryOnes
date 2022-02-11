using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : Singleton<GameController> {
    public GameObject player;
    public GameObject playButton;
    public GameObject gameOverView;

    public float gameSpeed = 1.0f;

    private bool isBegin = true;
    private bool isPlay, isPause, isEnd;
    private Coroutine enemySpawnRoutine;
    private Coroutine partsSpawnRoutine;

    void Start() {
        playButton.SetActive(true);

        playButton.GetComponent<Button>().onClick.AddListener(Begin);
        gameOverView.GetComponentInChildren<Button>().onClick.AddListener(Begin);
    }

    void Update() {
        
    }

    public bool GameIsOn() {
        return isPlay;
    }

    public void Begin() {
        playButton.SetActive(false);
        gameOverView.SetActive(false);

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            Destroy(enemy);
        }

        isBegin = true;
        player.GetComponent<PlayerController>().MoveToTheScreenCenter(() => {
            isBegin = false;
            isPlay = true;
            enemySpawnRoutine = StartCoroutine(EnemyController.Instance.SpawnWaves());
            partsSpawnRoutine = StartCoroutine(PlayerPartsController.Instance.SpawnWaves());
        });
    }

    public void GameOver() {
        isPlay = false;
        StopCoroutine(enemySpawnRoutine);
        StopCoroutine(partsSpawnRoutine);

        foreach (var unattachedPlayerPart in GameObject.FindGameObjectsWithTag("UnattachedPlayerPart")) {
            unattachedPlayerPart.GetComponent<PlayerPart>().BlowUp(() => { Destroy(unattachedPlayerPart); });
        }
        foreach (var playerPart in GameObject.FindGameObjectsWithTag("PlayerPart")) {
            playerPart.GetComponent<PlayerPart>().BlowUp(() => { Destroy(playerPart); });
        }

        player.GetComponent<PlayerController>().BlowUp(() => {
            isEnd = true;
            gameOverView.SetActive(true);
            gameOverView.GetComponentInChildren<Text>().text = "Score: " + 0;
        });
    }
}
