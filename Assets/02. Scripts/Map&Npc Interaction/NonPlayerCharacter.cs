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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !DialogueManager.isPlayingDialogue) convObj.SetActive(true);
    }

    public void StartConversation(string ID)
    {
        if (!convObj.activeInHierarchy)
        {
            Debug.Log("ConvObj is not active. return.");
            return;
        }

        if (ID.Equals("")) return;

        Debug.Log("StartConversation | 말 걸음: " + ID);
        DialogueManager.instance.StartDialogueInManager(ID);
        PlayerMovement.canMove = false;
        // dialogueContent.StartDialogueInManager(ID);
        convObj.SetActive(false);
    }
}