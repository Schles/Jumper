using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader1 : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame updat
    // Update is called once per frame

    void Awake() {

        var levelName = SceneManager.GetActiveScene().name;
        print("MyScene " + levelName);

        if (!GameProgress.levelProgress.ContainsKey(levelName)) {
            GameProgress.levelProgress.Add(levelName, new LevelProgress());
        }   
    }

    void Start()
    {        
        GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
    }

    void OnEnable()
    {
        EventManager.StartListening("finish", OnFinish);
    }

    void OnDisable()
    {
        EventManager.StopListening("finish", OnFinish);
    }

    private void OnFinish(Dictionary<string, object> message) 
    {       
        StartCoroutine(LoadNextLevel());
    }

    IEnumerator LoadNextLevel()
    {
        animator.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("MainMenu");
    }
}
