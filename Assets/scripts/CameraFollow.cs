using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public RaceManager raceManager;
    public float xOffset = 5f;
    public float smoothSpeed = 5f;
    public float minX = -22f;
    public float maxX = 22f;

    void LateUpdate()
    {
        if (raceManager == null || raceManager.activeRacers.Count == 0) return;

        float leadX = float.MinValue;
        foreach (HorseRacer racer in raceManager.activeRacers)
        {
            if (racer != null && racer.CurrentX > leadX)
            {
                leadX = racer.CurrentX;
            }
        }

        if (leadX > float.MinValue)
        {
            float targetX = Mathf.Clamp(leadX + xOffset, minX, maxX);
            Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        }
    }
}