using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GuyPath : MonoBehaviour
{
    public Transform pathContainer;
    private Transform[] pathPoints;

    public float speed = 2f;
    private int currentPointIndex = 0;
    private bool walking = false;
    private Animator animator;

    [Header("Fade Panel")]
    public CanvasGroup fadePanel;
    public GameObject fadeCanvas;
    private bool triggeredEnding = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        pathPoints = new Transform[pathContainer.childCount];
        for (int i = 0; i < pathContainer.childCount; i++)
        {
            pathPoints[i] = pathContainer.GetChild(i);
        }

        currentPointIndex = 0;
    }

    public IEnumerator StandThenWalk()
    {
        // Stop walking (just in case)
        walking = false;

        animator.SetTrigger("stand"); //Start standing animation
        yield return new WaitForSeconds(1.5f); // Wait for the animation

        // Start walking
        animator.SetTrigger("startWalking");
        walking = true;
    }

    void Update()
    {
        if (walking && currentPointIndex < pathPoints.Length)
        {
            Transform targetPoint = pathPoints[currentPointIndex];

            // Rotate to the next point
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            if (direction == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                180 * Time.deltaTime
            );

            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            if (angleDifference < 5f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPoint.position,
                    speed * Time.deltaTime
                );

                // walk to next point
                if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
                {
                    currentPointIndex++;

                    if (!triggeredEnding && currentPointIndex == pathPoints.Length - 6)
                    {
                        triggeredEnding = true;
                        fadeCanvas.SetActive(true);
                        StartCoroutine(FadeAndLoadFinalScene());
                    }
                }
            }
        }
    }

    IEnumerator FadeAndLoadFinalScene()
    {
        float duration = 1.5f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0, 1, time / duration);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("FinalScene");
    }
}
