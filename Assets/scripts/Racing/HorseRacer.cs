using UnityEngine;

public class HorseRacer : MonoBehaviour
{
    public HorseData data;
    public int horseNumber;
    public bool isHiddenLuckyHorse;

    private float currentSpeed;
    private float currentStamina;
    private float startX;
    private float finishLineX = 29.5f;
    private float totalRaceDistance = 59f;
    private bool isRacing = false;
    private bool hasFinished = false;
    private bool ultimateTriggered = false;
    private VenueData currentVenue;
    private RaceManager raceManager;

    public float CurrentX => transform.position.x;
    public bool HasFinished => hasFinished;

    public void SetupHorse(HorseData horseData, int number, bool isSecretLucky, VenueData venue, float finishX, RaceManager manager)
    {
        data = horseData;
        horseNumber = number;
        isHiddenLuckyHorse = isSecretLucky;
        currentVenue = venue;
        raceManager = manager;

        startX = transform.position.x;
        finishLineX = finishX;
        totalRaceDistance = Mathf.Max(1f, finishLineX - startX);

        currentStamina = data.stamina * 10f;
        currentSpeed = data.acceleration * 0.05f;
        isRacing = false;
        hasFinished = false;
        ultimateTriggered = false;

        if (currentVenue != null)
        {
            if (data.preferredSurface == currentVenue.trackSurface)
                currentSpeed *= 1.08f;

            if (currentVenue.weatherCondition == VenueData.Weather.Muddy && data.stamina < 80f)
                currentSpeed *= 0.92f;
        }

        // Hidden Lucky Horse gets a major secret advantage (no console reveal!)
        if (isHiddenLuckyHorse)
        {
            currentStamina *= 1.5f;
            currentSpeed *= 1.25f;
        }
    }

    public void BeginRace()
    {
        isRacing = true;
    }

    void Update()
    {
        if (!isRacing || hasFinished) return;

        float targetSpeed = data.speed * 0.1f;
        if (isHiddenLuckyHorse)
        {
            targetSpeed *= 1.18f; // Secret Lucky Blessing keeps top speed high
        }

        if (currentStamina > 0)
        {
            currentStamina -= Time.deltaTime * (101f - data.wisdom);
        }
        else if (!isHiddenLuckyHorse)
        {
            targetSpeed *= 0.6f; // Non-lucky horses hit fatigue wall when out of stamina
        }

        float distanceCovered = transform.position.x - startX;
        if (!ultimateTriggered && distanceCovered >= (totalRaceDistance * 0.5f))
        {
            TriggerUltimate();
            ultimateTriggered = true;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * (data.acceleration * 0.1f));
        transform.position += Vector3.right * currentSpeed * Time.deltaTime;

        if (transform.position.x >= finishLineX)
        {
            hasFinished = true;
            if (raceManager != null)
            {
                raceManager.NotifyHorseFinished(this);
            }
        }
    }

    void TriggerUltimate()
    {
        Debug.Log($"✨ ULT ACTIVATED: #{horseNumber} {data.horseName} uses {data.ultimateName}!");

        switch (data.ultimateType)
        {
            case HorseData.UltimateType.SpeedBurst:
                currentSpeed += 3.5f;
                break;
            case HorseData.UltimateType.StaminaRecovery:
                currentStamina += 50f;
                break;
            case HorseData.UltimateType.LuckyBooster:
                if (isHiddenLuckyHorse) currentSpeed += 5f;
                break;
            default:
                currentSpeed += 2.2f;
                break;
        }
    }
}