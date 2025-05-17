using Ink.Runtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public TextAsset inkJSONAsset;
    public TextMeshProUGUI dialogueText;
    public GameObject choiceButtonPrefab;
    public Transform choicesContainer;
    public GameObject dialoguePanel;

    private Story story;
    private string currentLine="";
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private bool waitingForInput = false;
    private bool showingChoices = false;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (inkJSONAsset != null)
        {
            story = new Story(inkJSONAsset.text);
            story.ChoosePathString("start");
            ContinueStory();
        }
        else
        {
            Debug.LogError("Ink JSON Asset is missing.");
        }
    }

    void Update()
    {
        if (waitingForInput && !showingChoices)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                waitingForInput = false;
                ContinueStory();
            }
        }
    }

    void ContinueStory()
    {
        if (story.canContinue)
        {
            string line = story.Continue().Trim();
            StopAllCoroutines(); 
            StartCoroutine(TypeLine(line));
        }
        else if (story.currentChoices.Count > 0)
        {
            ShowChoices();
            waitingForInput = false;
            isTyping = false;
        }
        else
        {
            dialogueText.text = "";
            waitingForInput = false;
            isTyping = false;
        }
    }

    void ShowChoices()
    {
        dialogueText.text  ="";
        dialoguePanel.SetActive(false); 

        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        showingChoices = true;
        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            Choice choice = story.currentChoices[i];

            GameObject buttonGO = Instantiate(choiceButtonPrefab, choicesContainer, false);

            TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = choice.text;
            }
            else
            {
                Debug.LogWarning("There's no TextMeshProUGUI inside the button.");
            }

            int choiceIndex = choice.index;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                ClearChoices();
                story.ChooseChoiceIndex(choiceIndex);
                ContinueStory();
                dialoguePanel.SetActive(true);
            });
        }
        
        showingChoices = false;
    }

    void ClearChoices()
    {
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        isTyping = true;
        waitingForInput = false;

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f); // writing speed
        }

        dialogueText.text = line;
        isTyping = false;
        waitingForInput = true;
    }
}
