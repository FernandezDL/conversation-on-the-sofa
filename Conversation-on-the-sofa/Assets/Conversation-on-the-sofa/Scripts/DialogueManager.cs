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

    void Start()
    {
        if (inkJSONAsset != null)
        {
            story = new Story(inkJSONAsset.text);
            story.ChoosePathString("start");
            RefreshView();
        }
        else
        {
            Debug.LogError("Ink JSON Asset is missing.");
        }
    }

    void RefreshView()
    {
        dialogueText.text = "";
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        while (story.canContinue)
        {
            string nextLine = story.Continue().Trim();
            Debug.Log("Texto de Ink: " + nextLine);
            dialogueText.text += nextLine + "\n";
        }

        if (story.currentChoices.Count > 0)
        {
            foreach (Choice choice in story.currentChoices)
            {
                Debug.Log("Opción disponible: " + choice.text);

                GameObject button = Instantiate(choiceButtonPrefab, choicesContainer);
                button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
                button.GetComponent<Button>().onClick.AddListener(() => {
                    story.ChooseChoiceIndex(choice.index);
                    RefreshView();
                });
            }
        }
    }
}
