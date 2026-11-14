#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class HorseDatabaseInitializer : MonoBehaviour
{
    [MenuItem("Horse Racing/Generate 25 Horse Assets & Database")]
    public static void GenerateHorses()
    {
        string folderPath = "Assets/Scriptables/Horses";
        if (!AssetDatabase.IsValidFolder("Assets/Scriptables"))
            AssetDatabase.CreateFolder("Assets", "Scriptables");
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/Scriptables", "Horses");

        string dbPath = "Assets/Scriptables/MasterHorseDatabase.asset";
        HorseDatabaseSO database = AssetDatabase.LoadAssetAtPath<HorseDatabaseSO>(dbPath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<HorseDatabaseSO>();
            AssetDatabase.CreateAsset(database, dbPath);
        }
        database.horses.Clear();

        string[] names = {
            "Thunder Bolt", "Iron Vanguard", "Shadow Monarch", "Blazing Comet", "Lucky Seven",
            "Crimson Ghost", "Dust Devil", "Silver Bullet", "Golden Crown", "Night Runner",
            "Storm Bringer", "Zenith Prime", "Phantom Step", "Solaris", "Wild Mustang",
            "Apex Predator", "Azure Wave", "Iron Hoof", "Lucky Strike", "Ember Spark",
            "Frost Bite", "Titanium Step", "Gale Force", "Mystic Rune", "Omega Star"
        };

        string[] ultNames = {
            "Lightning Flash", "Iron Lungs", "Midnight Prowl", "Solar Flare", "Seven's Grace",
            "Turf Phantom", "Sandstorm", "Ricochet", "Regal Pace", "Neon Shift",
            "Tempest Roar", "Peak Form", "Ghostly Weave", "Endless Horizon", "Unbridled Fury",
            "Killer Instinct", "Tidal Surge", "Tectonic Stride", "Jackpot", "Spontaneous Combustion",
            "Ice Cold Focus", "Unshakable Will", "Tailwind", "Prophecy", "Supernova"
        };

        HorseData.UltimateType[] ultTypes = {
            HorseData.UltimateType.SpeedBurst, HorseData.UltimateType.StaminaRecovery, HorseData.UltimateType.PositionSurge, HorseData.UltimateType.SpeedBurst, HorseData.UltimateType.LuckyBooster,
            HorseData.UltimateType.SpeedBurst, HorseData.UltimateType.SpeedBurst, HorseData.UltimateType.SpeedBurst, HorseData.UltimateType.StaminaRecovery, HorseData.UltimateType.PositionSurge,
            HorseData.UltimateType.SpeedBurst, HorseData.UltimateType.SpeedBurst, HorseData.UltimateType.PositionSurge, HorseData.UltimateType.StaminaRecovery, HorseData.UltimateType.SpeedBurst,
            HorseData.UltimateType.PositionSurge, HorseData.UltimateType.SpeedBurst, HorseData.UltimateType.SpeedBurst, HorseData.UltimateType.LuckyBooster, HorseData.UltimateType.SpeedBurst,
            HorseData.UltimateType.StaminaRecovery, HorseData.UltimateType.PositionSurge, HorseData.UltimateType.SpeedBurst, HorseData.UltimateType.StaminaRecovery, HorseData.UltimateType.SpeedBurst
        };

        for (int i = 0; i < 25; i++)
        {
            string assetPath = $"{folderPath}/Horse_{i + 1:00}_{names[i].Replace(" ", "")}.asset";
            HorseData horse = AssetDatabase.LoadAssetAtPath<HorseData>(assetPath);

            if (horse == null)
            {
                horse = ScriptableObject.CreateInstance<HorseData>();
                AssetDatabase.CreateAsset(horse, assetPath);
            }

            horse.horseID = i + 1;
            horse.horseName = names[i];
            horse.speed = Random.Range(70f, 98f);
            horse.stamina = Random.Range(70f, 98f);
            horse.acceleration = Random.Range(70f, 98f);
            horse.wisdom = Random.Range(70f, 98f);
            horse.preferredSurface = (i % 2 == 0) ? "Turf" : "Dirt";
            horse.ultimateName = ultNames[i];
            horse.ultimateType = ultTypes[i];

            EditorUtility.SetDirty(horse);
            database.horses.Add(horse);
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ Successfully generated all 25 Horses and populated Master Database!");
    }
}
#endif