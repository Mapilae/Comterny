using UnityEngine;

public class NonPlayerCharacter : MovingFootPrint
{
    public string characterName;
    // public DialogueContent dialogueContent;

    public GameObject convObj;


    private void Start()
    {
        if (convObj == null)
            convObj = transform.GetChild(0).gameObject;
        convObj.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) convObj.SetActive(true);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) convObj.SetActive(false);
    }

    public void StartConversation(string ID)
    {
        if (!convObj.activeInHierarchy) return;
        Debug.Log("StartConversation | 말 걸음: " + ID);
        DialogueManager.instance.StartDialogueInManager(ID);
        // dialogueContent.StartDialogueInManager(ID);
        convObj.SetActive(false);
    }
}