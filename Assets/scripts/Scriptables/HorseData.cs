using UnityEngine;

[CreateAssetMenu(fileName = "NewHorse", menuName = "HorseRacing/Horse Data")]
public class HorseData : ScriptableObject
{
    public int horseID;
    public string horseName;
    public Sprite horseSprite;

    [Header("Core Stats (1-100)")]
    public float speed = 80f;
    public float stamina = 80f;
    public float acceleration = 80f;
    public float wisdom = 80f;

    [Header("Preferences")]
    public string preferredSurface = "Turf"; // "Turf" or "Dirt"

    [Header("Ultimate Ability")]
    public string ultimateName;
    public enum UltimateType { SpeedBurst, StaminaRecovery, PositionSurge, LuckyBooster }
    public UltimateType ultimateType;
}