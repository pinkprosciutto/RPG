using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueBox;
    [SerializeField] Text dialogueText;
    [SerializeField] int textSpeed;

    public event Action OnShowDialouge;
    public event Action OnCloseDialouge;

    public static DialogueManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    Dialogue dialogue;
    Action onDialogueFinished;
    int currentLine = 0;
    bool textIsOver;

    public bool IsShowingDialogue { get; private set; }

    public IEnumerator ShowDialogue(Dialogue dialogue, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialouge?.Invoke();

        IsShowingDialogue = true;
        this.dialogue = dialogue;
        onDialogueFinished = onFinished;

        dialogueBox.SetActive(true);
        StartCoroutine(TypeDialogue(dialogue.Lines[0]));
    }

    public IEnumerator TypeDialogue(string line)
    {
        textIsOver = false;

        dialogueText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / textSpeed);
        }

        textIsOver = true;
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && textIsOver == true)
        {
            currentLine++;
            if(currentLine < dialogue.Lines.Count)
            {
                StartCoroutine(TypeDialogue(dialogue.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                IsShowingDialogue = false;
                dialogueBox.SetActive(false);
                onDialogueFinished?.Invoke();
                OnCloseDialouge?.Invoke();
            }
        }
    }

}
