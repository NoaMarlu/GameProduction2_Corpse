using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    [TextArea(2, 5)]
    public string message;
    public string dialogueID;

    private bool isShow = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isShow) return;
        if (collider.GetComponent<Player>() == null) return;
        //Šù“Ç‚È‚çreturn
        if (DialogueManager.Instance.IsShow(dialogueID)) return;
        isShow = true;
        DialogueManager.Instance.ShowDialogue(message, dialogueID);
    }

}
