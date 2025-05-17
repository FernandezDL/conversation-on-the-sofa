using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class FinalEndingManager : MonoBehaviour
{
    public TextMeshProUGUI finalText;
    public GameObject buttonPanel;
    public float typingSpeed = 0.03f;

    private string message = "Sometimes it's not about fixing\nit's just about understanding why it broke.";

    void Start()
    {
        finalText.text = "";
        buttonPanel.SetActive(false);
        StartCoroutine(TypeSentence());
    }

    IEnumerator TypeSentence()
    {
        foreach (char c in message)
        {
            finalText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(8f);
        buttonPanel.SetActive(true);
        finalText.text = "";
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
