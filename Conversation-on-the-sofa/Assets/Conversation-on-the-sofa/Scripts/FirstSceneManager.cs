using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class FirstSceneManager : MonoBehaviour
{
    public TextMeshProUGUI StartText;
    public float typingSpeed = 0.03f;

    private string message = @"<i>Sometimes, a conversation happens too late. But its still needed.</i>
    \n\nTwo people.
    A shared space.
    Silence, then something more.

    \n\nThis is not about fixing what was broken.
    Its about daring to look at it one last time.";

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartText.text = "";
        StartCoroutine(TypeSentence());
    }

    IEnumerator TypeSentence()
    {
        foreach (char c in message)
        {
            StartText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("House");
    }
}
