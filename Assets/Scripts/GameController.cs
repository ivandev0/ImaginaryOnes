using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : Singleton<GameController> {
    public GameObject player;
    public GameObject playButton;
    public GameObject gameOverView;
    public GameObject scoreText;
    public float deltaNextLevel = 10;
    public float offsetNextLevel = 5;

    private float localGameSpeed = 1.0f;
    public float gameSpeed = 1.0f;
    public int gameLevel = 0;
    private const int maxLocalGameSpeed = 2;
    public const int maxGameLevel = 7;

    private bool isBegin = true;
    private bool isPlay, isPause, isEnd;
    private float score;
    private Coroutine enemySpawnRoutine;
    private Coroutine partsSpawnRoutine;
    private Coroutine scoreRoutine;
    private Coroutine gameLevelRoutine;
    private Coroutine increaseGameSpeedRoutine;

    void Start() {
        playButton.SetActive(true);

        playButton.GetComponent<Button>().onClick.AddListener(Begin);
        gameOverView.GetComponentInChildren<Button>().onClick.AddListener(Begin);
    }

    void Update() {
        var speedUp = PlayerPartsController.Instance.GetSpeedUpCount() * 0.05f;
        var slowDown = PlayerPartsController.Instance.GetSlowDownCount() * 0.05f;
        gameSpeed = Mathf.Clamp(localGameSpeed + speedUp - slowDown, 0.5f, float.MaxValue);
    }

    public bool GameIsOn() {
        return isPlay;
    }

    public void Begin() {
        playButton.SetActive(false);
        gameOverView.SetActive(false);
        scoreText.SetActive(true);
        score = gameLevel = 0;
        gameSpeed = localGameSpeed = 1;
        SetScore();

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            Destroy(enemy);
        }

        isBegin = true;
        player.GetComponent<PlayerController>().MoveToTheScreenCenter(() => {
            isBegin = false;
            isPlay = true;
            enemySpawnRoutine = StartCoroutine(EnemyController.Instance.SpawnWaves());
            partsSpawnRoutine = StartCoroutine(PlayerPartsController.Instance.SpawnWaves());
            scoreRoutine = StartCoroutine(CountScore());
            gameLevelRoutine = StartCoroutine(GameLevelRoutine());
            increaseGameSpeedRoutine = StartCoroutine(IncreaseGameSpeed());
        });
    }

    public void GameOver() {
        isPlay = false;
        StopCoroutine(enemySpawnRoutine);
        StopCoroutine(partsSpawnRoutine);
        StopCoroutine(scoreRoutine);
        if (gameLevelRoutine != null) StopCoroutine(gameLevelRoutine);
        if (increaseGameSpeedRoutine != null) StopCoroutine(increaseGameSpeedRoutine);

        foreach (var unattachedPlayerPart in GameObject.FindGameObjectsWithTag("UnattachedPlayerPart")) {
            unattachedPlayerPart.GetComponent<PlayerPart>().BlowUp(null, () => { Destroy(unattachedPlayerPart); });
        }
        foreach (var playerPart in GameObject.FindGameObjectsWithTag("PlayerPart")) {
            playerPart.GetComponent<PlayerPart>().BlowUp(null, () => { Destroy(playerPart); });
        }

        player.GetComponent<PlayerController>().BlowUp(() => {
            isEnd = true;
            gameOverView.SetActive(true);
            scoreText.SetActive(false);
            gameOverView.GetComponentInChildren<Text>().text = "Score: " + Mathf.FloorToInt(score);
        });
    }

    private void SetScore() {
        scoreText.GetComponent<Text>().text = "Score: " + Mathf.FloorToInt(score);
    }

    private IEnumerator CountScore() {
        while (GameIsOn()) {
            yield return new WaitForSeconds(1);
            score += gameSpeed * (PlayerPartsController.Instance.GetPlayersPartsCount() + 1);
            SetScore();
        }
    }

    private IEnumerator GameLevelRoutine() {
        while (GameIsOn() && gameLevel < maxGameLevel) {
            yield return new WaitForSeconds(deltaNextLevel + gameLevel * offsetNextLevel);
            gameLevel++;
        }
    }

    private IEnumerator IncreaseGameSpeed() {
        while (GameIsOn() && localGameSpeed < maxLocalGameSpeed) {
            yield return new WaitForSeconds(1f);
            localGameSpeed += 0.1f;
        }
    }
}
