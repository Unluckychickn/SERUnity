using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerData
{
    public void LoadData(PlayerSaveData data);
    public void SaveData(PlayerSaveData data);
    void LoadInventoryData(PlayerSaveData data);
    void SaveInventoryData(PlayerSaveData data);
}
