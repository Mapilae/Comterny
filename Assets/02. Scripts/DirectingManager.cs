using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class DirectingManager : MonoBehaviour
{
    public Volume volumePrefab;
    [Header("CHOICE")] public GameObject choiceWholeObj;
    public choice[] choices;

    [Header("Other")] public GameObject prefab;

    private Camera cam;

    private Vector3 dftpos;
    public Animator letterboxAnim;
    private Animator panelAnim;
    public bool isLetterboxOn = false; //letterbox on/off
    private DialogueManager dialogueManager;

    private Vector3 standingDefaultPos;
    private GameObject standings;

    private Animator stands;
    
    

    private void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();
        panelAnim = dialogueManager.dialogue.wholeObj.GetComponent<Animator>();
        standings = dialogueManager.dialogue.standingImg.gameObject;
        stands = standings.GetComponent<Animator>();
        dftpos = stands.transform.parent.position;
        cam = Camera.main;

        // SetDefaultPos();
    }

    private void SetDefaultPos()
    {
        //���ĵ� parent(��ü) ��ġ ����. ĳ�� ������ �ٰ��ð� �ʱ�ȭ��
        standingDefaultPos = stands.transform.parent.position;
    }

    public void RunCMD(string cmd)
    {
        if (cmd.Equals("")) return;
        var args = cmd.Split(';');
        for (var i = 0; i < args.Length; i++)
        {
            var spl = args[i].Split('_');
            if (spl.Length >= 2) RunCMDOne(spl[0], spl[1]);
            else RunCMDOne(spl[0], "");
        }
    }

    public void RunCMDOne(string cmd, string next)
    {
        switch (cmd)
        {
            case "close":
                //다이얼로그 종료
                dialogueManager.EndDialogue();
                break;
            case "move":
                dialogueManager.StartDialogueInManager(next);
                break;
            case "CHOICE": //CHOICE_2
                ChoiceMode(next);
                break;
            case "sdturn":
                //sdturn_leftright
                TurnStanding();
                break;
            case "vol":
                //���� ����
                //vol_�����̸�_��
                ChangeVolume(next);

                break;
            case "open":
                //특정 오브젝트의 태그를 검색하여 그것을 활성화 함(단일이어야함!) 다이얼로그는 자동으로 종료함. (close와 동일)
                dialogueManager.EndDialogue();
                EnableObject(next, true);
                break;
            case "enable":
                //특정 오브젝트의 태그를 검색하여 그것을 활성화 함(단일이어야함!)
                EnableObject(next, true);
                break;
            case "disable":
                //특정 오브젝트의 태그를 검색하여 그것을 비활성화 함(단일이어야함!)
                EnableObject(next, false);
                break;
            case "questGive":
                AddQuest(next);
                break;
            case "questRemove":
                RemoveQuest(next);
                break;
            case "shake": //���ĵ� ����
                StandingShake(0);
                break;
            case "flinch": //���ĵ� ����
                StandingFlinch(0);
                break;

            case "bgm":
                Debug.Log("bgm : " + next);
                if (next.Equals("stop"))
                    SoundManager.instance.StopBGM();
                else if (next.Equals("end")) SoundManager.instance.EndBGM();
                else if (next.Equals("pause")) SoundManager.instance.PauseBGM();
                else if (next.Equals("play")) SoundManager.instance.ReplayBGM();
                else
                    SoundManager.instance.PlayBGM(next);
                break;
            case "se":
                if (next.Equals("stop")) SoundManager.instance.StopSound();
                else SoundManager.instance.PlaySound(next);
                break;
            case "amb":
                if (next.Contains("stop:"))
                    SoundManager.instance.StopAmbience(next.Split(':')[1]); //amb_stop:clock
                else
                    SoundManager.instance.PlayAmbience(next);
                //                    Debug.Log("amb_" + next);
                break;
            case "movescene":
                MoveScene(next);
                break;
            case "camshake":
                CameraShake();
                break;
            case "addVar":
                PlayerData.instance.AddVariable(int.Parse(next));
                break;
            case "addItem":
                PlayerData.instance.AddItem(int.Parse(next));
                break;
            /*case "stretch":
                //���ĵ� �ø���, ��ǰ, ������ ��.
                if (next.Contains("left")) StandingStretch(0);
                if (next.Contains("center")) StandingStretch(1);
                if (next.Contains("right")) StandingStretch(2);
                break;
            case "sighshort":
                if (next.Contains("left")) Sigh(0, 0);
                if (next.Contains("center")) Sigh(1, 0);
                if (next.Contains("right")) Sigh(2, 0);
                break;
            case "sighlong":
                if (next.Contains("left")) Sigh(0, 1);
                if (next.Contains("center")) Sigh(1, 1);
                if (next.Contains("right")) Sigh(2, 1);
                break;
            case "goleft": //go left
                if (next.Contains("left")) StandingGoLeft(0, 2);
                if (next.Contains("center")) StandingGoLeft(1, 2);
                if (next.Contains("right")) StandingGoLeft(2, 2);
                break;
            case "goleftlittle": //go left little
                if (next.Contains("left")) StandingGoLeft(0, 0);
                if (next.Contains("center")) StandingGoLeft(1, 0);
                if (next.Contains("right")) StandingGoLeft(2, 0);
                break;
            case "goleftlittleleft":
                if (next.Contains("left")) StandingGoLeft(0, 1);
                if (next.Contains("center")) StandingGoLeft(1, 1);
                if (next.Contains("right")) StandingGoLeft(2, 1);
                break;
            case "gobackleft": //go back left
                if (next.Contains("left")) StandingGoBackLeft(0, false);
                if (next.Contains("center")) StandingGoBackLeft(1, false);
                if (next.Contains("right")) StandingGoBackLeft(2, false);
                break;
            case "gobackleftlittle": //go back left little
                if (next.Contains("left")) StandingGoBackLeft(0, true);
                if (next.Contains("center")) StandingGoBackLeft(1, true);
                if (next.Contains("right")) StandingGoBackLeft(2, true);
                break;
            case "goright": //go right
                if (next.Contains("left")) StandingGoRight(0, 2);
                if (next.Contains("center")) StandingGoRight(1, 2);
                if (next.Contains("right")) StandingGoRight(2, 2);
                break;
            case "gorightlittle": //go right little
                if (next.Contains("left")) StandingGoRight(0, 0);
                if (next.Contains("center")) StandingGoRight(1, 0);
                if (next.Contains("right")) StandingGoRight(2, 0);
                break;
            case "gorightlittleright":
                if (next.Contains("left")) StandingGoRight(0, 1);
                if (next.Contains("center")) StandingGoRight(1, 1);
                if (next.Contains("right")) StandingGoRight(2, 1);
                break;
            case "gobackright": //go back right
                if (next.Contains("left")) StandingGoBackRight(0, false);
                if (next.Contains("center")) StandingGoBackRight(1, false);
                if (next.Contains("right")) StandingGoBackRight(2, false);
                break;
            case "gobackrightlittle": //go back right little
                if (next.Contains("left")) StandingGoBackRight(0, true);
                if (next.Contains("center")) StandingGoBackRight(1, true);
                if (next.Contains("right")) StandingGoBackRight(2, true);
                break;
            case "flip":
                if (next.Contains("left")) StandingFlip(0);
                if (next.Contains("center")) StandingFlip(1);
                if (next.Contains("right")) StandingFlip(2);
                break;
            case "clearmoving": //clear moving, ���ĵ� ���ڸ���
                if (next.Contains("left")) StandingClear(0);
                if (next.Contains("center")) StandingClear(1);
                if (next.Contains("right")) StandingClear(2);
                if (next.Contains("all"))
                    for (int i = 0; i < 3; i++)
                        StandingClear(i);
                break;
            case "comeclose0":
                if (next.Contains("left")) StandingComeClose(0, 0);
                if (next.Contains("center")) StandingComeClose(1, 0);
                if (next.Contains("right")) StandingComeClose(2, 0);
                break;
            case "comeclose1":
                if (next.Contains("left")) StandingComeClose(0, 1);
                if (next.Contains("center")) StandingComeClose(1, 1);
                if (next.Contains("right")) StandingComeClose(2, 1);
                break;
            case "comeclose2":
                if (next.Contains("left")) StandingComeClose(0, 2);
                if (next.Contains("center")) StandingComeClose(1, 2);
                if (next.Contains("right")) StandingComeClose(2, 2);
                break;
            */
        }
    }

    private void MoveScene(string sceneName)
    {
        if (sceneName.Equals("")) LodingScene.instance.LoadScene();
        else LodingScene.instance.LoadScene(sceneName);
    }

    private void TurnStanding()
    {
        StartCoroutine(TurnStandingLeftRight());
    }

    public void CheckCmdImportant(List<Cell> dialogueLeft, int startIndex)
    {
        if (dialogueLeft == null) return;
        if (startIndex >= dialogueLeft.Count) return;
        DialogueManager.instance.typeWriter.isWhenTypingEndCalled = true;
        //스킵해도 중요한건 실행되게 하는 코드
        //이미 스킵 상황에서 DialogueManager.EndDialogue()를 호출하면서 TypeWriter.StopTyping()도 같이 호출되므로, currentIndex의 after_cmd는 실행되었을 것이다.
        //따라서 현재까지의 인덱스는 모두 커맨드가 실행된 것으로 간주하고, 현재+1의 인덱스들만 루프를 돌며 실행하도록 했다.
        for (var i = startIndex + 1; i < dialogueLeft.Count; i++)
        {
            //지나친 start_cmd와 after_cmd를 동시실행시키기
            var cmds = GetImportantCmd($"{dialogueLeft[i].start_cmd};{dialogueLeft[i].after_cmd}");
            if (cmds.Equals("")) continue;
            RunCMD(cmds);
        }
    }

    private string GetImportantCmd(string cmds)
    {
        var spl = cmds.Split(';');
        var filtered = new List<string>();
        for (var i = 0; i < spl.Length; i++)
        {
            if (spl[i].Equals("")) continue;
            switch (spl[i].Split('_')[0])
            {
                case "questGive":
                    filtered.Add(spl[i]);
                    break;
                case "questRemove":
                    filtered.Add(spl[i]);
                    break;
            }
        }

        if (filtered.Count == 0) return "";
        return string.Join(';', filtered);
    }

    public void LetterboxOn()
    {
        if (isLetterboxOn) return;
        isLetterboxOn = true;
        letterboxAnim.SetTrigger("letterboxOn");
        panelAnim.SetTrigger("letterboxOn");
    }
    
    public void LetterboxOff()
    {
        if (!isLetterboxOn) return;
        isLetterboxOn = false;
        letterboxAnim.SetTrigger("letterboxOff");
        panelAnim.SetTrigger("letterboxOff");
    }

    
    private IEnumerator TurnStandingLeftRight()
    {
        yield return new WaitForSeconds(0.5f);

        standings.transform.localScale = new Vector3(-1, 1, 1);
        yield return new WaitForSeconds(0.5f);

        standings.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(0.75f);
        standings.transform.localScale = new Vector3(-1, 1, 1);
        yield return new WaitForSeconds(0.5f);

        standings.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(0.5f);
    }

    private void ChangeVolume(string name)
    {
        StartCoroutine(VolumeFade(name, name.Equals("")));
    }

    private IEnumerator VolumeFade(string name, bool isOn)
    {
        volumePrefab = null; //Instantiate(dspr.FindVolume(name), this.transform).GetComponent<Volume>();
        if (isOn)
        {
            volumePrefab.profile = volumePrefab.GetComponent<Volume>().profile;
            while (volumePrefab.weight < 1)
            {
                volumePrefab.weight += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            volumePrefab.weight = 1;
        }
        else
        {
            while (volumePrefab.weight > 0)
            {
                volumePrefab.weight -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            volumePrefab.weight = 0;
            Destroy(volumePrefab.gameObject);
            volumePrefab = null;
        }
    }

    [Serializable]
    public struct choice
    {
        public GameObject obj;
        public TMP_Text text;
        public string cmd;
    }

    #region CMD

    private void EnableObject(string tag, bool isEnable)
    {
        var obj = GameObjectManager.instance.FindGameObjectWithTag(tag);
        if (obj != null) obj.SetActive(isEnable);
    }

    private void AddQuest(string quest)
    {
        QuestManager.instance.AddQuest(quest);
    }

    private void RemoveQuest(string quest)
    {
        QuestManager.instance.RemoveQuest(quest);
    }

    #endregion


    #region CHOICE

    private void ChoiceMode(string next)
    {
        var count = int.Parse(next);

        choiceWholeObj.SetActive(true);
        foreach (var obj in choices)
            obj.obj.SetActive(false);
        //choice 값들 텍스트 등으로 접근하여 반영하기
        StartCoroutine(IEChoiceEnable(count, dialogueManager.crtIndex)); //선택지 개수를 파라미터로 넘기기
    }

    private IEnumerator IEChoiceEnable(int choiceCount, int crtIndex)
    {
        crtIndex++;
        //choice개수에 따라 choiceObjs 텀을 두고 활성화하기
        for (var i = 0; i < choiceCount; i++)
        {
            yield return new WaitForSeconds(0.2f);
            choices[i].obj.SetActive(true);
            //crtIndex는 dialogueManager에서 RunCMD 실행후 ++되어서 0.2초가 지난 이 시점에서는 기존보다 +1된상태로 있기 때문에
            //crtIndex+i로 하였다. (버그)
            Debug.Log(dialogueManager.dialogueList[crtIndex + i].content);
            choices[i].text.text = dialogueManager.dialogueList[crtIndex + i].content;
            choices[i].cmd = dialogueManager.dialogueList[crtIndex + i].after_cmd;
            // Debug.Log(i + "'s crtIndex: " + (dialogueManager.crtIndex + i));
        }

        dialogueManager.canClickToNext = false;
    }

    public void OnChoiceButtonClicked(int index)
    {
        Debug.Log("choice : " + choices[index].cmd);

        choiceWholeObj.SetActive(false);
        dialogueManager.canClickToNext = true;
        RunCMD(choices[index].cmd);
    }

    #endregion


    #region Standing

    private void StandingStretch(int index)
    {
        StartCoroutine(StandStretch(index));
    }

    private IEnumerator StandStretch(int index)
    {
        /*
         45f 1.08
        65f 1.1
        85f 1
        */
        var size = 1f;
        var scale = stands.transform.localScale;
        WaitForSeconds wait = new(0.1f);

        for (var i = 0; i < 17; i++)
        {
            stands.transform.parent.localScale = new Vector3(scale.x, scale.y * size, scale.z);
            if (i <= 9)
                size += 0.01f;
            else if (i <= 12)
                size += 0.0025f;
            else
                size -= 0.025f;
            yield return wait;
        }

        stands.transform.parent.localScale = new Vector3(scale.x, scale.y, scale.z);
    }

    private void Sigh(int index, int level)
    {
        switch (level)
        {
            case 0:
                StartCoroutine(SighShort(index));
                break;
            case 1:
                StartCoroutine(SighLong(index));
                break;
        }
    }

    private IEnumerator SighLong(int index)
    {
        /*
        0
        pos y0
        scale y1
        60
        pos y-0.4
        scale y0.95
        90
        pos y-0.4
        scale y0.95
        105
        pos y0
        scale y1
         */
        var scale = stands.transform.parent.localScale;
        var pos = stands.transform.parent.position;
        WaitForSeconds wait = new(0.05f);
        float scaleToAdd = 0, posToAdd = 0;

        for (var i = 0; i < 21; i++)
        {
            stands.transform.parent.position = new Vector3(pos.x, pos.y + posToAdd, pos.z);
            stands.transform.parent.localScale = new Vector3(scale.x, scale.y + scaleToAdd, scale.z);
            if (i <= 12)
            {
                posToAdd -= 0.03f * 0.1f;
                scaleToAdd -= 0.004f * 0.1f;
            }
            else if (i > 18)
            {
                posToAdd += 0.18f * 0.1f;
                scaleToAdd += 0.024f * 0.1f;
            }

            yield return wait;
        }

        stands.transform.parent.position = pos;
        stands.transform.parent.localScale = scale;
    }

    private IEnumerator SighShort(int index)
    {
        /*
         0
        pos y0
        scale y1
        20
        pos y-0.4
        scale y0.95
        45
        pos y-0.4
        scale y0.95
        90
        pos y0
        scale y1
         */
        var scale = stands.transform.parent.localScale;
        var pos = stands.transform.parent.position;
        WaitForSeconds wait = new(0.05f);
        float scaleToAdd = 0, posToAdd = 0;
        for (var i = 0; i < 18; i++)
        {
            stands.transform.parent.position = new Vector3(pos.x, pos.y + posToAdd, pos.z);
            stands.transform.parent.localScale = new Vector3(scale.x, scale.y + scaleToAdd, scale.z);
            if (i <= 4)
            {
                posToAdd -= 0.1f * 0.1f;
                scaleToAdd -= 0.0125f * 0.1f;
            }
            else if (i > 9)
            {
                posToAdd += 0.0444f * 0.1f;
                scaleToAdd += 0.0055f * 0.1f;
            }

            yield return wait;
        }

        stands.transform.parent.position = pos;
        stands.transform.parent.localScale = scale;
    }

    public void StandingClear(int index)
    {
        stands.transform.parent.position = dftpos;
        stands.SetTrigger("clear");
        var scale = Vector3.one;
        stands.transform.parent.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
    }

    public void StandingGoRight(int index, int level)
    {
        switch (level)
        {
            case 0:
                stands.SetTrigger("go_right_little");
                break;
            case 1:
                stands.SetTrigger("go_right_little_right");
                break;
            case 2:
                stands.SetTrigger("go_right");
                break;
        }
    }

    public void StandingGoBackRight(int index, bool isLittle)
    {
        if (isLittle)
            stands.SetTrigger("goback_right_little");
        else stands.SetTrigger("goback_right");
    }

    public void StandingGoLeft(int index, int level)
    {
        switch (level)
        {
            case 0:
                stands.SetTrigger("go_left_little");
                break;
            case 1:
                stands.SetTrigger("go_left_little_left");
                break;
            case 2:
                stands.SetTrigger("go_left");
                break;
        }
    }

    public void StandingGoBackLeft(int index, bool isLittle)
    {
        if (isLittle)
            stands.SetTrigger("goback_left_little");
        else stands.SetTrigger("goback_left");
    }

    public void StandingFlip(int index)
    {
        StartCoroutine(StandFlip(index));
    }

    private IEnumerator StandFlip(int index)
    {
        var scale = stands.transform.parent.localScale;
        var elapsed = scale.x;
        var delta = 5f;

        if (scale.x > 0)
            //1 --> -1
            while (elapsed > -scale.x)
            {
                stands.transform.parent.localScale = new Vector3(elapsed, scale.y, scale.z);

                elapsed -= Time.deltaTime * delta;

                yield return null;
            }
        else
            while (elapsed < -scale.x)
            {
                stands.transform.parent.localScale = new Vector3(elapsed, scale.y, scale.z);

                elapsed += Time.deltaTime * delta;

                yield return null;
            }

        stands.transform.parent.localScale = new Vector3(-scale.x, scale.y, scale.z);
    }

    public void StandingComeClose(int index, int level)
    {
        var isMinus = stands.transform.parent.localScale.x > 0 ? 1 : -1;
        Vector3 dftPos = new(stands.transform.parent.position.x, standingDefaultPos.y,
            stands.transform.parent.position.z);
        switch (level)
        {
            case 0: //reset
                stands.transform.parent.position = dftPos;
                stands.transform.parent.localScale = new Vector3(isMinus * 1, 1, 1);
                break;
            case 1: //close little
                //-0.4, 1.15
                stands.transform.parent.position = dftPos + new Vector3(0, -0.4f, 0);
                stands.transform.parent.localScale = new Vector3(isMinus * 1.15f, 1.15f, 1.15f);
                break;
            case 2: //close lot
                //-1.15, 1.4
                stands.transform.parent.position = dftPos + new Vector3(0, -1.15f, 0);
                stands.transform.parent.localScale = new Vector3(isMinus * 1.4f, 1.4f, 1.4f);
                break;
        }
    }


    public void StandingShake(int index)
    {
        StartCoroutine(ShakeUI(stands.GetComponent<RectTransform>(), 0.4f, 7f));
    }

    public void StandingFlinch(int index)
    {
        StartCoroutine(ShakeUI(stands.GetComponent<RectTransform>(), 0.1f, 5f));
    }

    public IEnumerator ShakeTransform(Transform target, float duration, float magnitude, float? delay = null,
        bool useTempParent = false)
    {
        var originalParent = target.parent;
        var originalSiblingIndex = target.GetSiblingIndex();
        var originalPosition = target.localPosition;

        var shakeTransform = target;
        Transform tempParent = null;

        if (useTempParent)
        {
            tempParent = new GameObject("TempShakeParent").transform;
            tempParent.SetParent(originalParent, true);
            tempParent.SetSiblingIndex(originalSiblingIndex);
            tempParent.localPosition = originalPosition;
            tempParent.localRotation = Quaternion.identity;
            tempParent.localScale = Vector3.one;

            target.SetParent(tempParent, true);
            target.localPosition = Vector3.zero;

            shakeTransform = tempParent;
        }

        var elapsed = 0f;
        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude;
            var y = Random.Range(-1f, 1f) * magnitude;

            shakeTransform.localPosition = new Vector3(x, y, shakeTransform.localPosition.z);

            elapsed += Time.deltaTime;
            yield return delay ?? null;
        }

        shakeTransform.localPosition = Vector3.zero;

        if (useTempParent)
        {
            target.SetParent(originalParent, true);
            target.SetSiblingIndex(originalSiblingIndex);
            target.localPosition = originalPosition;

            Destroy(tempParent.gameObject);
        }
    }

    public IEnumerator ShakeUI(RectTransform target, float duration, float magnitude, float? delay = null,
        bool useTempParent = false)
    {
        var originalParent = target.parent;
        var originalSiblingIndex = target.GetSiblingIndex();
        var originalAnchoredPosition = target.anchoredPosition;

        var shakeTarget = target;
        RectTransform tempParent = null;

        if (useTempParent)
        {
            var tempGO = new GameObject("TempShakeParent", typeof(RectTransform));
            tempParent = tempGO.GetComponent<RectTransform>();

            tempParent.SetParent(originalParent, false);
            tempParent.SetSiblingIndex(originalSiblingIndex);

            tempParent.anchorMin = target.anchorMin;
            tempParent.anchorMax = target.anchorMax;
            tempParent.pivot = target.pivot;
            tempParent.sizeDelta = target.sizeDelta;
            tempParent.anchoredPosition = target.anchoredPosition;

            target.SetParent(tempParent, false);
            target.anchoredPosition = Vector2.zero;

            shakeTarget = tempParent;
        }

        var elapsed = 0f;
        var basePos = shakeTarget.anchoredPosition;
        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude;
            var y = Random.Range(-1f, 1f) * magnitude;

            shakeTarget.anchoredPosition = basePos + new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return delay ?? null;
        }

        shakeTarget.anchoredPosition = basePos;

        if (useTempParent)
        {
            target.SetParent(originalParent, false);
            target.SetSiblingIndex(originalSiblingIndex);
            target.anchoredPosition = originalAnchoredPosition;

            Destroy(tempParent.gameObject);
        }
    }


    private void CameraShake()
    {
        StartCoroutine(ShakeTransform(GameObject.FindGameObjectWithTag("MainCamera").transform, 3f, 0.1f, 0.05f, true));
    }

    #endregion
}