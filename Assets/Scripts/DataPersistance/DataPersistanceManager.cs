using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistanceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] public string fileName;

    private GameData gameData;
    public static DataPersistanceManager Instance { get; private set; }

    private FileDataHandler dataHandler;

    private List<IDataPersistance> dataPersistanceObjects;

    public bool persistentData = false;

    public void Start() {

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistanceObjects = FindAllDataPersistanceObjects();

        LoadGame();
    }

    public ref GameData GetGameData() {
        return ref gameData;
    }
   
    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        //IEnumerable<IDataPersistance> gameObjects = GameObject.FindObjectsOfType(typeof(MonoBehaviour)).OfType<IDataPersistance>();
        var gameObjects = FindObjectsByType(typeof(IDataPersistance), FindObjectsSortMode.None);;

        return new List<IDataPersistance>((IEnumerable<IDataPersistance>)gameObjects);
    }



    private void Awake() {
        
        if (Instance != null) {
            
            Debug.LogError("Found multiple instances of DataPersistanceManager.");
            return;
        }

        Instance = this;
    }

    public void NewGame() {
        this.gameData = new GameData();
    }

    public void SaveGame() {

        if (!persistentData) {
            Debug.Log("Data Persistence disabled. Exiting.");
            return;
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects) {
            dataPersistanceObject.SaveGame(ref gameData);
        }


        dataHandler.Save(gameData);
    }

    public void LoadGame() {
        if (!persistentData) {
            Debug.Log("Data Persistence disabled. Starting new game.");
            NewGame();

            return;
        }


        this.gameData = dataHandler.Load();

        if (this.gameData == null) {
            Debug.Log("No game data found. Starting new game.");
            NewGame();
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects) {
            dataPersistanceObject.LoadGame(gameData);
        }

    }


    private void OnApplicationQuit() {
        SaveGame();

    }

}
