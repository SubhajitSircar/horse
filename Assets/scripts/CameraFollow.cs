
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Race Settings")]
    public Transform horsesParent;

    [Header("Camera Settings")]
    public float followOffset = 3f;
    public float smoothTime = 0.25f;

    private float initialCameraX;
    private float velocityX;

    void Start()
    {
        initialCameraX = transform.position.x;
    }

    void LateUpdate()
    {
        if (horsesParent == null || horsesParent.childCount == 0)
            return;

        // Find the horse currently furthest ahead
        float leadingHorseX = float.MinValue;

        foreach (Transform horse in horsesParent)
        {
            if (horse.position.x > leadingHorseX)
            {
                leadingHorseX = horse.position.x;
            }
        }

        // Keep the leading horse slightly right of screen center
        float targetX = Mathf.Max(
            initialCameraX,
            leadingHorseX - followOffset
        );

        // Smooth horizontal camera movement
        float newX = Mathf.SmoothDamp(
            transform.position.x,
            targetX,
            ref velocityX,
            smoothTime
        );

        transform.position = new Vector3(
            newX,
            transform.position.y,
            transform.position.z
        );
    }
}
