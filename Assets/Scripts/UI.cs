using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour, IDataPersistance
{

    public float gameTime = 0;
    private int deathCount = 0;
    private int fruitsCollected = 0;
    private GameController gameController;
    private Label labelGameTime;
    private Label labelFruits;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (GameState.Instance == null)
        {
            return;
        }
        */
   
        if (labelFruits != null) {
            labelFruits.text = fruitsCollected.ToString() + " Fruit(s)";
        } else {
            //Debug.Log("missing UI.labelFruits");
        }
        
        if (labelGameTime != null) {
            labelGameTime.text = gameTimeToHumanTime(gameController.gameTime);
        } else {
            //Debug.Log("missing UI.labelGameTime");
        }
        
    }

    private void FixedUpdate()
    {
        gameTime += Time.deltaTime;
    }

    private void OnEnable()
    {

        EventManager.StartListening("gameOver", OnGameOver);
        EventManager.StartListening("collectedFruit", OnFruitCollected);
        
        gameController = GameObject.FindFirstObjectByType<GameController>();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        labelGameTime = root.Q<Label>("gameTime");
        labelFruits = root.Q<Label>("fruits");
    }

    
    public void OnDisable()
    {
        EventManager.StopListening("gameOver", OnGameOver);
        EventManager.StopListening("collectedFruit", OnFruitCollected);
    }

    private string gameTimeToHumanTime(float gameTime)
    {
        float minutes = Mathf.Floor((gameTime %= 3600) / 60);
        float seconds = Mathf.Floor(gameTime % 60);
        
        
        return this.formatNumber(minutes, 2) + ":" + this.formatNumber(seconds, 2);
    }

    private string formatNumber(float number, int minDigits)
    {
        if (number < Math.Pow(10, minDigits - 1)) 
        {
            string res = "";
            for (int i = 0; i < minDigits - 1; i++)
            {
                res += "0";
            }

            return res + number.ToString();
        } else
        {
            return number.ToString();
        }
    }

    public void LoadGame(GameData data)
    {
        this.fruitsCollected = data.fruitsCollected;
        this.deathCount = data.deathCount;
        this.gameTime = data.gameTime;
    }

    public void SaveGame(ref GameData data)
    {
        data.fruitsCollected = this.fruitsCollected;
        data.deathCount = this.deathCount;
        data.gameTime = this.gameTime;
    }

    
    public void OnFruitCollected(Dictionary<string, object> message)
    {
        fruitsCollected++;
    }

    
    private void OnGameOver(Dictionary<string, object> message)
    {
        deathCount++;
    }
}
