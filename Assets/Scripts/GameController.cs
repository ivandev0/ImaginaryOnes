using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : Singleton<GameController> {
    public GameObject player;
    public GameObject playButton;

    public float gameSpeed = 1.0f;

    private bool onBegin = true;
    private bool onPlay, onPause, onEnd;

    void Start() {
        playButton.GetComponent<Button>().onClick.AddListener(Begin);
    }

    void Update() {
        
    }

    public bool GameIsOn() {
        return onPlay;
    }

    public void Begin() {
        playButton.SetActive(false);
        onBegin = true;
        player.GetComponent<PlayerController>().MoveToTheScreenCenter(() => {
            onBegin = false;
            onPlay = true;
        });
    }
}
