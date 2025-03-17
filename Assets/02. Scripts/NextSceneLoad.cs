using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneLoad : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hello World");
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.x < 0 && this.transform.position.y < 0)
        {
            SceneManager.LoadSceneAsync("SampleScene");
        }
    }
    
    public void ExitGame()
    {
        Debug.Log("Application Exit!");
        Application.Quit();
    }
    
}
