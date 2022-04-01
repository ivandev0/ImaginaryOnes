using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : Singleton<GameController> {
    public GameObject player;
    public GameObject playButton;
    public GameObject restartButton;
    public GameObject scoreText;
    public GameObject endMessage;
    public float deltaNextLevel = 10;
    public float offsetNextLevel = 5;

    private float localGameSpeed = 1.0f;
    public float gameSpeed = 1.0f;
    public int gameLevel = 0;
    public int currentMaterialsIndex = 0;
    private const int maxLocalGameSpeed = 2;
    public const int maxGameLevel = 6;
    public const float minGameSpeed = 0.25f;
    public const float maxGameSpeed = 4;

    private bool hasStarted, isBegin = true, isPlay, isOver;
    private float score;
    private Coroutine enemySpawnRoutine;
    private Coroutine partsSpawnRoutine;
    private Coroutine scoreRoutine;
    private Coroutine gameLevelRoutine;
    private Coroutine increaseGameSpeedRoutine;

    void Start() {
        playButton.SetActive(true);

        playButton.GetComponent<Button>().onClick.AddListener(Begin);
        restartButton.GetComponentInChildren<Button>().onClick.AddListener(Begin);
    }

    void Update() {
        var speedUp = PlayerPartsController.Instance.GetSpeedUpCount() * 0.25f;
        var slowDown = PlayerPartsController.Instance.GetSlowDownCount() * 0.25f;
        gameSpeed = Mathf.Clamp(localGameSpeed + speedUp - slowDown, minGameSpeed, maxGameSpeed);
    }

    public bool GameIsOn() {
        return isPlay;
    }

    public bool GameIsActive() {
        return isBegin || isPlay || isOver;
    }

    public void Begin() {
        playButton.SetActive(false);
        restartButton.SetActive(false);
        endMessage.SetActive(false);
        score = gameLevel = 0;
        gameSpeed = localGameSpeed = 1;
        if (hasStarted) {
            currentMaterialsIndex = Random.Range(0, PlayerPartsController.Instance.commonMaterials.Length);
            StartCoroutine(Background.Instance.ChangeColor(1.0f));
        }
        PlayerPartsController.Instance.UpdateMaterials();
        player.GetComponent<PlayerController>().SetMaterial(PlayerPartsController.Instance.GetFirstMaterial());
        SetScore();

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            Destroy(enemy);
        }

        isBegin = true;
        isOver = false;
        hasStarted = true;
        player.GetComponent<PlayerController>().MoveToTheScreenCenter(() => {
            isBegin = false;
            isPlay = true;
            player.GetComponent<SphereCollider>().isTrigger = false;
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
            restartButton.SetActive(true);
        });
    }

    private IEnumerator GameWon() {
        isOver = true;
        StopCoroutine(enemySpawnRoutine);
        StopCoroutine(partsSpawnRoutine);
        if (gameLevelRoutine != null) StopCoroutine(gameLevelRoutine);
        if (increaseGameSpeedRoutine != null) StopCoroutine(increaseGameSpeedRoutine);

        while (GameObject.FindGameObjectsWithTag("Enemy").Length != 0) {
            yield return new WaitForSeconds(0.25f);
        }

        StopCoroutine(scoreRoutine);
        isPlay = false;
        player.GetComponent<PlayerController>().MoveToTheScreenCenter(() => { });

        foreach (var playerPart in GameObject.FindGameObjectsWithTag("PlayerPart")) {
            StartCoroutine(
                playerPart.GetComponent<PlayerPart>()
                    .MoveAlong(player.transform.position - playerPart.transform.position)
            );
        }

        endMessage.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        restartButton.SetActive(true);
    }

    private void SetScore() {
        scoreText.GetComponent<Text>().text = Mathf.FloorToInt(score).ToString();
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

        /*if (gameLevel == maxGameLevel) {
            yield return new WaitForSeconds(deltaNextLevel + gameLevel * offsetNextLevel);
            StartCoroutine(GameWon());
        }*/
    }

    private IEnumerator IncreaseGameSpeed() {
        while (GameIsOn() && localGameSpeed < maxLocalGameSpeed) {
            yield return new WaitForSeconds(5f);
            localGameSpeed += 0.1f;
        }
    }
}
