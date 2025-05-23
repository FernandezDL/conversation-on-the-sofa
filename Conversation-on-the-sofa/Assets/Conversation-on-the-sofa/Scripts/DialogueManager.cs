using Ink.Runtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue")]
    public TextAsset inkJSONAsset;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    [Header("Choice Buttons")]
    public GameObject choiceButtonPrefab;
    public Transform choicesContainer;

    [Header("Slap Popup")]
    public GameObject slapPopup;
    private bool slapPopupActive = false;

    [Header("Male Character")]
    public GuyPath guyPath;

    [Header("Fade Panel")]
    public CanvasGroup fadePanel;
    public GameObject fadeCanvas;

    [Header("Info")]
    public GameObject infoButton;
    public GameObject nextButton;
    public GameObject prevButton;
    public GameObject InfoCanvas;
    public GameObject closeButton;

    private Story story;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private bool waitingForInput = false;
    private bool showingChoices = false;
    private bool infoPanelActive = false;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (slapPopup != null)
            slapPopup.SetActive(false);

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
        if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (infoPanelActive)
            return;

        if (slapPopupActive)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                slapPopup.SetActive(false);
                slapPopupActive = false;
                ContinueStory(); // Continue story
            }
            return; // Can't continue without closing popup
        }

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
            dialoguePanel.SetActive(true);
            string line = story.Continue().Trim();
            List<string> tags = story.currentTags;

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeLine(line));

            if (tags.Contains("walksAway"))
            {
                dialoguePanel.SetActive(false);
                StartCoroutine(guyPath.StandThenWalk());
            }
            else if (tags.Contains("slap"))
            {
                // Open Popup
                slapPopup.SetActive(true);
                slapPopupActive = true;

                // Hide DialoguePanel and buttons
                dialoguePanel.SetActive(false);
                choicesContainer.gameObject.SetActive(false);

                return;
            }
            else if (tags.Contains("fadeOut"))
            {
                fadeCanvas.SetActive(true);
                StartCoroutine(FadeAndLoadScene("FinalScene"));
                return;
            }
        }
        else if (story.currentChoices.Count > 0)
        {
            choicesContainer.gameObject.SetActive(true);
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
        dialogueText.text = "";
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
            buttonGO.GetComponent<Button>().onClick.AddListener(() =>
            {
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

    public IEnumerator FadeAndLoadScene(string sceneName)
    {
        float duration = 2f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0, 1, time / duration);
            yield return null;
        }

        yield return new WaitForSeconds(1f); // pausa negra
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void OnInfoButtonClicked()
    {
        InfoCanvas.SetActive(true);
        infoPanelActive = true;
    }

    public void OnCloseButtonClicked()
    {
        InfoCanvas.SetActive(false);
        infoPanelActive = false;

        if (waitingForInput)
        {
            dialoguePanel.SetActive(true);
        }

        if (story != null && story.currentChoices.Count > 0)
        {
            choicesContainer.gameObject.SetActive(true);
            ShowChoices();
        }
    }
}
