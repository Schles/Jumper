using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour, IDataPersistance
{
    public InputAction resetAction;

    public float gameTime = 0;


    private Vector3 spawnPoint;

    public void Awake()
    {
        // assign a callback for the "jump" action.
        resetAction.performed += ctx => { OnReset(ctx); };
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = FindFirstObjectByType<PlayerMovement>().transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;
    }



    public void OnEnable()
    {
        resetAction.Enable();
        EventManager.StartListening("gameOver", OnGameOver);
        EventManager.StartListening("finish", OnFinish);
    }

    public void OnDisable()
    {
        resetAction.Disable();
        EventManager.StopListening("gameOver", OnGameOver);
        EventManager.StopListening("finish", OnFinish);
    }


    public void OnGameOver(Dictionary<string, object> message)
    {
        Reset();
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        Reset();
    }

    private void OnFinish(Dictionary<string, object> message) 
    {       
        var levelName = SceneManager.GetActiveScene().name;
        float? bestTime = GameProgress.levelProgress[levelName].bestTime;
        if ( bestTime == null || gameTime < bestTime) {
            GameProgress.levelProgress[levelName].bestTime = gameTime;
        }
    }

    private void Reset()
    {

        // Vector3 spawnPoint;
        // if (Checkpoint.active)
        // {
        //     spawnPoint = Checkpoint.active.transform.position;
        // }
        // else
        // {
        //     spawnPoint = new Vector3(0f, 0f, 0f);
        // }

        

        EventManager.TriggerEvent("restart", new Dictionary<string, object> { { "spawnPoint", spawnPoint } });
    }

    public void LoadGame(GameData data)
    {
        Reset();
    }

    public void SaveGame(ref GameData data)
    {

    }
}
