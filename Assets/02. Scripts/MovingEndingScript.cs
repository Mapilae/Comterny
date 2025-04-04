using UnityEngine;
[System.Serializable]
public class EndingCredits
{
    public GameObject TextGrid;
    public GameObject Title;
    public GameObject Discription;
    
}

public class MovingEndingScript : MonoBehaviour
{
    public EndingCredits endingCredits = new EndingCredits();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        endingCredits.TextGrid.transform.position += new Vector3(0, Time.deltaTime*50, 0);
    }
}
