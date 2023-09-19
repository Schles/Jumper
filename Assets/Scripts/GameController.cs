using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public InputAction resetAction;
    public GameObject playerPrefab;

    public float gameTime = 0;
    public int fruitsCollected = 0;

    public void Awake()
    {
        // assign a callback for the "jump" action.
        resetAction.performed += ctx => { OnReset(ctx); };
    }

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        gameTime += Time.deltaTime;
        this.PlayerOutOfBounds();
    }

    public void OnEnable()
    {
        resetAction.Enable();
        Player.OnGameOverAction += OnGameOver;
        Player.OnGameWonAction += OnGameWon;
        Player.OnFruitCollectedAction += OnFruitCollected;

    }

    public void OnDisable()
    {
        resetAction.Disable();
        Player.OnGameOverAction -= OnGameOver;
        Player.OnGameWonAction -= OnGameWon;
        Player.OnFruitCollectedAction -= OnFruitCollected;
    }

    private void PlayerOutOfBounds()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player && player.transform.position.y < -30)
        {
            this.OnGameOver("Aus der Map gefallen");
        }
    }





    public void OnGameOver(string gameOver)
    {
        GameState.Instance.lastReason = gameOver;
        Reset();
    }

    public void OnGameWon(string gameWon)
    {
        GameState.Instance.lastReason = gameWon;

        GameState.Instance.lastTime = this.gameTime;
        GameState.Instance.lastFruitsCollected = this.fruitsCollected;

        bool isBetterResult = (GameState.Instance.bestFruitsCollected <= fruitsCollected && this.gameTime < GameState.Instance.bestTime);

        if (GameState.Instance.bestTime <= 0f || isBetterResult)
        {
            GameState.Instance.bestTime = this.gameTime;
            GameState.Instance.bestFruitsCollected = fruitsCollected;
        }
        Reset();
    }

    public void OnFruitCollected(GameObject go)
    {
        fruitsCollected++;
        Destroy(go);
    }
    public void OnReset(InputAction.CallbackContext context)
    {
        GameState.Instance.lastReason = "Neustart";
        Reset();
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
