using UnityEngine;

public class NonPlayerCharacter : MovingFootPrint
{
    public string characterName;

    [Header("If AutoPlay, set true")] public bool isAutoPlay;

    public string autoPlayID;
    // public DialogueContent dialogueContent;

    //private GameObject convObj;

    private void Start()
    {
        OnStartDefaultSetting();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAutoPlay) return;
        /*if (collision.CompareTag("Player"))
            if (convObj != null)
                convObj.SetActive(true);*/
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAutoPlay) return;
        /*if (collision.CompareTag("Player"))
            if (convObj != null)
                convObj.SetActive(false);*/
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isAutoPlay) return;
        /*if (collision.CompareTag("Player") && !DialogueManager.isPlayingDialogue)
            if (convObj != null)
                convObj.SetActive(true);*/
    }

    public void OnStartDefaultSetting()
    {
        //if (convObj != null) convObj.SetActive(false);

        if (isAutoPlay) StartConversation(autoPlayID);
    }

    public void StartConversation(string ID)
    {
        if (ID.Equals("")) return;

        Debug.Log("StartConversation | 말 걸음: " + ID);
        DialogueManager.instance.StartDialogueInManager(ID);
    }
}