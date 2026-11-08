using UnityEngine;

public class HorseRacer : MonoBehaviour
{
    public int horseNumber = 1;
    public float speed = 3f;

    public float finishLineX = 29.3f;

    private bool hasFinished = false;

    void Update()
    {
        if (hasFinished)
            return;

        transform.localPosition +=
            Vector3.right * speed * Time.deltaTime;

        if (transform.localPosition.x >= finishLineX)
        {
            hasFinished = true;

            Debug.Log("Horse " + horseNumber + " finished!");
        }
    }
}