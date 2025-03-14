using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;
    public int chapter = -1;

    private void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(this);
    }
    private void Start()
    {
        Debug.Log("Game Scene Manager is Here!");
    }

    public void LoadScene(int index)
    {
        Debug.Log("æ¿ ¿Ãµø! " + index);
        SceneManager.LoadScene(index);
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log("æ¿ ¿Ãµø! " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Debug.Log("∞‘¿” ¡æ∑·!");
        Application.Quit();
    }

}
