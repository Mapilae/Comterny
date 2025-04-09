using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class DirectingManager : MonoBehaviour
{
    public Volume volumePrefab;

    [Header("Other")] public GameObject prefab;

    private Camera cam;

    private Vector3 dftpos;
    private Animator dialogue;
    private DialogueManager dialogueManager;

    private Vector3 standingDefaultPos;
    private GameObject standings;

    private Animator stands;

    private void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();
        dialogue = dialogueManager.dialogWhole;
        standings = dialogueManager.standingImg.gameObject;
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
        string[] args = cmd.Split(';');
        for (int i = 0; i < args.Length; i++)
        {
            string[] spl = args[i].Split('_');
            if (spl.Length >= 2) RunCMDOne(spl[0], spl[1]);
            else RunCMDOne(spl[0], "");
        }
    }

    public void RunCMDOne(string cmd, string next)
    {
        switch (cmd)
        {
            case "sdturn":
                //sdturn_leftright
                TurnStanding();
                break;
            case "vol":
                //���� ����
                //vol_�����̸�_��
                ChangeVolume(next);

                break;
            case "shake": //���ĵ� ����
                StandingShake(0);
                break;
            case "flinch": //���ĵ� ����
                StandingFlinch(0);
                break;
            case "stretch":
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
        }
    }

    private void TurnStanding()
    {
        StartCoroutine(TurnStandingLeftRight());
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
        float size = 1f;
        Vector3 scale = stands.transform.localScale;
        WaitForSeconds wait = new(0.1f);

        for (int i = 0; i < 17; i++)
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
        Vector3 scale = stands.transform.parent.localScale;
        Vector3 pos = stands.transform.parent.position;
        WaitForSeconds wait = new(0.05f);
        float scaleToAdd = 0, posToAdd = 0;

        for (int i = 0; i < 21; i++)
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
        Vector3 scale = stands.transform.parent.localScale;
        Vector3 pos = stands.transform.parent.position;
        WaitForSeconds wait = new(0.05f);
        float scaleToAdd = 0, posToAdd = 0;
        for (int i = 0; i < 18; i++)
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
        Vector3 scale = Vector3.one;
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
        Vector3 scale = stands.transform.parent.localScale;
        float elapsed = scale.x;
        float delta = 5f;

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
        int isMinus = stands.transform.parent.localScale.x > 0 ? 1 : -1;
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
        StartCoroutine(StandShake(0.25f, 0.1f, index));
    }

    public void StandingFlinch(int index)
    {
        StartCoroutine(StandShake(0.05f, 0.075f, index));
    }

    private IEnumerator StandShake(float dur, float mag, int index)
    {
        Vector2 originalPos = stands.transform.localPosition;
        float elapsed = 0;

        while (elapsed < dur)
        {
            float x = Random.Range(-1, 1) * mag;
            float y = Random.Range(-1, 1.5f) * mag;

            stands.transform.localPosition = new Vector2(originalPos.x + x, originalPos.y + y);

            elapsed += Time.deltaTime;

            yield return null;
        }

        stands.transform.localPosition = originalPos;
    }

    #endregion
}