using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelSelectUI : MonoBehaviour
{


    public VisualTreeAsset levelItemPrefab;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;     
        VisualElement levelSelect = root.Query<VisualElement>("levelSelect");
        for(var i = 0; i < GameProgress.levels.Count; i++) {

            var item = buildLevelItem(GameProgress.levels[i]);
            levelSelect.Add(item);
        }           
    }

    
    private VisualElement buildLevelItem(string levelName) {
            VisualElement levelItem = levelItemPrefab.Instantiate();

            //var levelName = GameProgress.levels[i];

            ((Label) levelItem.Query<Label>("levelName")).text = levelName;

            if (GameProgress.levelProgress.ContainsKey(levelName)) {
                ((Label) levelItem.Query<Label>("bestTimeValue")).text = GameProgress.levelProgress[levelName].bestTime.ToString();
                ((Label) levelItem.Query<Label>("fruitsValue")).text = GameProgress.levelProgress[levelName].collectedFruits.Count.ToString();
            }            

            levelItem.RegisterCallback<ClickEvent>(ev => LoadLevel(levelName));

            return levelItem;
    }

    public void OnDisable()
    {

    }

    public void LoadLevel(string level) {
        print("load " + level);
        SceneManager.LoadScene(level);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    private void FixedUpdate()
    {
 
    }
}
