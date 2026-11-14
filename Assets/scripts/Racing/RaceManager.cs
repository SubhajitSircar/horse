using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    [Header("Database & Venue")]
    public HorseDatabaseSO horseDatabase;
    public VenueData currentVenue;

    [Header("Track Setup")]
    public Transform startLine;
    public Transform finishLine;
    public Transform[] laneSpawnPoints; // 8 spawn points from top (Lane 1) to bottom (Lane 8)
    public GameObject horsePrefab;
    public Transform horsesParent;

    [Header("UI Reference")]
    public LuckyBettingUI bettingUI;

    // Hidden from player until race ends!
    private int hiddenLuckyNumber;
    [HideInInspector] public int playerPickedNumber = -1;

    public List<HorseRacer> activeRacers = new List<HorseRacer>();
    public List<HorseRacer> finishOrder = new List<HorseRacer>();
    private bool raceStarted = false;

    void Start()
    {
        PrepareRaceLineup();
    }

    public void PrepareRaceLineup()
    {
        if (horseDatabase == null || horseDatabase.horses.Count < 8)
        {
            Debug.LogError("Master Horse Database is unassigned or has fewer than 8 horses!");
            return;
        }

        // 1. Secretly roll the game's Lucky Number (1 to 8) - DO NOT LOG OR REVEAL!
        hiddenLuckyNumber = Random.Range(1, 9);
        finishOrder.Clear();
        activeRacers.Clear();
        raceStarted = false;

        // 2. Shuffle & pick 8 unique horses
        List<HorseData> shuffledPool = new List<HorseData>(horseDatabase.horses);
        for (int i = 0; i < shuffledPool.Count; i++)
        {
            HorseData temp = shuffledPool[i];
            int randomIndex = Random.Range(i, shuffledPool.Count);
            shuffledPool[i] = shuffledPool[randomIndex];
            shuffledPool[randomIndex] = temp;
        }

        float spawnX = startLine != null ? startLine.position.x : -29.5f;
        float finishX = finishLine != null ? finishLine.position.x : 29.5f;

        // 3. Spawn horses at StartLine (Lane 1 = top, Lane 8 = bottom)
        for (int i = 0; i < 8 && i < laneSpawnPoints.Length; i++)
        {
            HorseData data = shuffledPool[i];
            Vector3 spawnPos = new Vector3(spawnX, laneSpawnPoints[i].position.y, -0.3f);

            GameObject horseObj = Instantiate(horsePrefab, spawnPos, Quaternion.identity, horsesParent);
            HorseRacer racer = horseObj.GetComponent<HorseRacer>();

            int laneNumber = i + 1; // 1 to 8 from top to bottom
            bool isSecretLucky = (laneNumber == hiddenLuckyNumber);
            racer.SetupHorse(data, laneNumber, isSecretLucky, currentVenue, finishX, this);

            activeRacers.Add(racer);
        }

        // 4. Open Pre-Race Pick UI
        if (bettingUI != null)
        {
            bettingUI.ShowPreRacePanel(activeRacers, currentVenue);
        }
    }

    public void LockInPickAndStartRace(int chosenNumber)
    {
        if (raceStarted) return;

        playerPickedNumber = chosenNumber;
        raceStarted = true;

        Debug.Log($"🎲 Player locked in Lucky Number #{playerPickedNumber}! Race starting...");

        foreach (HorseRacer racer in activeRacers)
        {
            racer.BeginRace();
        }
    }

    public void NotifyHorseFinished(HorseRacer racer)
    {
        if (!finishOrder.Contains(racer))
        {
            finishOrder.Add(racer);
            Debug.Log($"🏅 #{finishOrder.Count}: Lane #{racer.horseNumber} - {racer.data.horseName}");

            // Once all 8 horses finish, reveal the Hidden Lucky Number & results
            if (finishOrder.Count >= activeRacers.Count)
            {
                RevealRaceResults();
            }
        }
    }

    private void RevealRaceResults()
    {
        HorseRacer winner = finishOrder[0];
        bool guessedLuckyNumber = (playerPickedNumber == hiddenLuckyNumber);
        bool pickedWinningHorse = (winner.horseNumber == playerPickedNumber);

        Debug.Log($"🔮 REVEAL: Hidden Lucky Number was #{hiddenLuckyNumber} | Your Pick: #{playerPickedNumber} | Winner: #{winner.horseNumber} ({winner.data.horseName})");

        if (bettingUI != null)
        {
            bettingUI.ShowResultsPanel(finishOrder, hiddenLuckyNumber, playerPickedNumber, guessedLuckyNumber, pickedWinningHorse);
        }
    }

    public void RestartRace()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}