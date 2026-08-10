using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DialogueManager : MonoBehaviour
{

    public static DialogueManager Instance { get; private set; }

    //UI
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    //文字表示
    public float charInterval = 0.05f;
    public float dispDuration = 2f;

    //SE
    public AudioSource audioSource;
    public AudioClip charSE;

    private HashSet<string> shownDialogues = new HashSet<string>();
    private Coroutine currentRoutine;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }    
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    //IDを受け取り表示
    public bool IsShow(string id)
    {
        return shownDialogues.Contains(id);
    }
    //メッセージを表示する
    public void ShowDialogue(string message,string id)
    {
        shownDialogues.Add(id);
        if(currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(DialogueRoutine(message));
    }
    IEnumerator DialogueRoutine(string message)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        foreach (char c in message)
        {
            dialogueText.text += c;
            if(charSE != null)audioSource.PlayOneShot(charSE);
            yield return new WaitForSeconds(charInterval);
        }
        yield return new WaitForSeconds(dispDuration);
        dialoguePanel.SetActive(false);
    }

}
