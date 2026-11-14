using UnityEngine;

[CreateAssetMenu(fileName = "NewVenue", menuName = "HorseRacing/Venue Data")]
public class VenueData : ScriptableObject
{
    public string venueName;
    public string trackSurface; // "Turf" or "Dirt"
    public float raceDistance = 100f; // Distance units

    public enum Weather { Sunny, Rainy, Muddy }
    public Weather weatherCondition;
}