using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : Singleton<GameController> {
    public GameObject player;
    public GameObject playButton;

    public float gameSpeed = 1.0f;

    private bool isBegin = true;
    private bool isPlay, isPause, isEnd;

    void Start() {
        playButton.GetComponent<Button>().onClick.AddListener(Begin);
    }

    void Update() {
        
    }

    public bool GameIsOn() {
        return isPlay;
    }

    public void Begin() {
        playButton.SetActive(false);
        isBegin = true;
        player.GetComponent<PlayerController>().MoveToTheScreenCenter(() => {
            isBegin = false;
            isPlay = true;
        });
    }

    public void GameOver() {
        isPlay = false;
        player.GetComponent<PlayerController>().BlowUp(() => {
            isEnd = true;
        });
    }
}
