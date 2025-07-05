using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpMsg : MonoBehaviour
{
    public static PopUpMsg instance;

    public TMP_Text text;
    private Image frame;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        frame = text.transform.parent.GetComponent<Image>();
        frame.gameObject.SetActive(false);
    }

    public void MsgPopUp(string msg)
    {
        text.text = msg;
        var textWidth = text.preferredWidth + 40;
        frame.rectTransform.sizeDelta = new Vector2(textWidth, frame.rectTransform.rect.height);

        frame.gameObject.SetActive(true);
    }
}