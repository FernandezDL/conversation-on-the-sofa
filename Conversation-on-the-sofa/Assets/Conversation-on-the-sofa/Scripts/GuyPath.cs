using UnityEngine;
using System.Collections;

public class GuyPath : MonoBehaviour
{
    public Transform pathContainer;
    private Transform[] pathPoints;

    public float speed = 2f;
    private int currentPointIndex = 0;
    private bool walking = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        pathPoints = new Transform[pathContainer.childCount];
        for (int i = 0; i < pathContainer.childCount; i++)
        {
            pathPoints[i] = pathContainer.GetChild(i);
        }

        currentPointIndex = 0;

        StartCoroutine(StandThenWalk());
    }

    IEnumerator StandThenWalk()
    {
        // 🔹 Detenemos movimiento desde el inicio por si acaso
        walking = false;

        // 🔹 Disparar trigger de animación "stand"
        animator.SetTrigger("stand");

        // 🔹 Esperar la duración de la animación de "stand"
        yield return new WaitForSeconds(GetAnimationLength("stand"));

        // 🔹 Luego activar animación de caminar
        animator.SetTrigger("startWalking");
        walking = true;
    }

    void Update()
    {
        if (walking && currentPointIndex < pathPoints.Length)
        {
            Transform targetPoint = pathPoints[currentPointIndex];

            // Calcular dirección y rotación deseada
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            if (direction == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                180 * Time.deltaTime // velocidad de giro
            );

            // Calcular diferencia de ángulo
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            // Solo mover si ya casi terminó de rotar (tolerancia 5 grados)
            if (angleDifference < 5f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPoint.position,
                    speed * Time.deltaTime
                );

                // Avanzar al siguiente punto si ya llegó
                if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
                {
                    currentPointIndex++;
                }
            }
        }
    }

    float GetAnimationLength(string stateName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == stateName)
            {
                return clip.length;
            }
        }
        return 1f;
    }
}
