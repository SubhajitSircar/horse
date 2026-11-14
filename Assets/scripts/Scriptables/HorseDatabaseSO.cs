using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HorseDatabase", menuName = "HorseRacing/Horse Database")]
public class HorseDatabaseSO : ScriptableObject
{
    public List<HorseData> horses = new List<HorseData>();

    public HorseData GetHorseById(int id)
    {
        return horses.Find(h => h.horseID == id);
    }
}