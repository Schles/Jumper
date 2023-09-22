using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

class DestroyInTime
{
    public float timeToLive = 0.8f;
    public GameObject gameObject;

    public DestroyInTime(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
}

public class GameController : MonoBehaviour
{
    public InputAction resetAction;
    public GameObject playerPrefab;

    private GameObject player;

    public float gameTime = 0;
    public int fruitsCollected = 0;

    private bool IsRestarting = false;
    private float resetInSeconds;

    public Checkpoint activeCheckpoint;

    private List<DestroyInTime> toBeDestroy = new List<DestroyInTime>();
    
    public void Awake()
    {
        // assign a callback for the "jump" action.
        resetAction.performed += ctx => { OnReset(ctx); };
    }

    // Start is called before the first frame update
    void Start()
    {
        //player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        player = GameObject.FindWithTag("Player");
        print(player);
    }

    // Update is called once per frame
    void Update()
    {


        if (IsRestarting)
        {
            resetInSeconds -= Time.deltaTime;
        }

        if (IsRestarting && resetInSeconds < 0)
        {
            Reset();
            IsRestarting = false;
        }


        for (var i = 0; i < toBeDestroy.Count; i++)
        {
            toBeDestroy[i].timeToLive -= Time.deltaTime;

            if (toBeDestroy[i].timeToLive < 0)
            {
                Destroy(toBeDestroy[i].gameObject);
                toBeDestroy.RemoveAt(i);
            }
        }
        
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
        Fruit.OnFruitCollectedAction += OnFruitCollected;

    }

    public void OnDisable()
    {
        resetAction.Disable();
        Player.OnGameOverAction -= OnGameOver;
        Player.OnGameWonAction -= OnGameWon;
        Fruit.OnFruitCollectedAction -= OnFruitCollected;
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
        if (gameOver == "dead")
        {
            IsRestarting = true;
            resetInSeconds = 0.5f;
        } else
        {
            Reset();
        }

        
        
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
        this.IsRestarting = true;
        resetInSeconds = 0.5f;
    }

    public void OnFruitCollected(GameObject go)
    {
        fruitsCollected++;
        var a = new DestroyInTime(go);
        toBeDestroy.Add(a);
    }
    public void OnReset(InputAction.CallbackContext context)
    {
        GameState.Instance.lastReason = "Neustart";
        Reset();
    }

    private void Reset()
    {

        if (activeCheckpoint)
        {
            player.transform.position = activeCheckpoint.transform.position;    
        }
        else
        {
            player.transform.position = new Vector3(0f, 0f, player.transform.position.z);
        }
        
        player.GetComponent<Player>().isDead = false;

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
