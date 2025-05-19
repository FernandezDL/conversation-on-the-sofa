using UnityEngine;
using UnityEngine.UI;

public class InstructionsPanel : MonoBehaviour
{
    public Image imageComponent;
    public Sprite[] instructionImages;

    private int currentIndex = 0;

    void Start()
    {
        ShowCurrentImage();
    }

    public void NextImage()
    {
        if (currentIndex < instructionImages.Length - 1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
        }

        ShowCurrentImage();
    }

    public void PreviousImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
        }
        else
        {
            currentIndex = 1;
        }

        ShowCurrentImage();
    }

    void ShowCurrentImage()
    {
        imageComponent.sprite = instructionImages[currentIndex];
    }
}
