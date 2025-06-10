using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RansomewareBoss : MonoBehaviour
{
    public Transform patternParent;
    public List<Transform> pattern;
    
    
    
    void Start()
    {
        pattern = new List<Transform>();
        for (int i = 0; i < patternParent.childCount; i++)
        {
            pattern.Add(patternParent.GetChild(i)); 
            pattern[i].gameObject.SetActive(false);
        }
        
        StartCoroutine(StartPattern());
    }

    IEnumerator StartPattern()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            int index = Random.Range(0, pattern.Count);
            Transform selectedPattern = pattern[index];

            for (int i=0; i < selectedPattern.childCount; i++)
            {
                Transform chain = selectedPattern.GetChild(i);
                chain.GetComponent<SpriteRenderer>().color = Color.grey;
                chain.GetComponent<Collider2D>().enabled = false;
            }
            
            selectedPattern.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.75f);
            for (int i=0; i < selectedPattern.childCount; i++)
            {
                Transform chain = selectedPattern.GetChild(i);
                chain.GetComponent<SpriteRenderer>().color = Color.white;
                chain.GetComponent<Collider2D>().enabled = true;
            }
            
            yield return new WaitForSeconds(0.75f);
           
            selectedPattern.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
    }
}
