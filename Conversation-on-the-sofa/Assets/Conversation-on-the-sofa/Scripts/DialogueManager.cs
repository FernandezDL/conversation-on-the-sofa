using Ink.Runtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextAsset inkJSONAsset;
    public TextMeshProUGUI dialogueText;
    public GameObject choiceButtonPrefab;
    public Transform choicesContainer;

    private Story story;
    private string currentLine;
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
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                ContinueStory();
            }
        }
    }

    void ContinueStory()
    {
        if (story.canContinue)
        {
            string line = story.Continue().Trim();
            dialogueText.text = line;
            Debug.Log("Texto de Ink: " + line);
            waitingForInput = true;
            showingChoices = false;
        }
        else if (story.currentChoices.Count > 0)
        {
            ShowChoices();
            showingChoices = true;
            waitingForInput = false;
        }
        else
        {
            dialogueText.text = "Fin del diálogo.";
            waitingForInput = false;
            showingChoices = false;
        }
    }

    void ShowChoices()
    {
        dialogueText.text  ="";
        // Limpia botones anteriores
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        Debug.Log("Total de opciones disponibles: " + story.currentChoices.Count);

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
                Debug.LogWarning("No se encontró TextMeshProUGUI dentro del botón.");
            }

            int choiceIndex = choice.index;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                ClearChoices();
                story.ChooseChoiceIndex(choiceIndex);
                ContinueStory();
            });
        }
    }

    void ClearChoices()
    {
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);
    }
}
