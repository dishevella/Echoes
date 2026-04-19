using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class LevelData
{
    public int sceneNo;
    public bool isClear;
    public int coins;
}

[System.Serializable]
public class GameData
{
    public List<LevelData> levels = new List<LevelData>();
}

public static class SaveManager
{
    private static string key = "GAME_DATA";

    private static GameData data;

    // ---------- 初始化 ----------
    private static void Init()
    {
        if (data != null) return;

        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            data = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            data = new GameData();
        }
    }

    // ---------- 保存 ----------
    private static void Save()
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    // ---------- 获取某关数据 ----------
    private static LevelData GetLevel(int sceneNo)
    {
        Init();

        foreach (var level in data.levels)
        {
            if (level.sceneNo == sceneNo)
                return level;
        }

        // 没有就创建
        LevelData newLevel = new LevelData
        {
            sceneNo = sceneNo,
            isClear = false,
            coins = 0
        };

        data.levels.Add(newLevel);
        return newLevel;
    }

    // ---------- 设置通关 ----------
    public static void SetLevelClear(int sceneNo)
    {
        LevelData level = GetLevel(sceneNo);
        level.isClear = true;
        Save();
    }

    // ---------- 设置金币（只存最高） ----------
    public static void SetLevelCoins(int sceneNo, int coins)
    {
        LevelData level = GetLevel(sceneNo);

        if (coins > level.coins)
        {
            level.coins = coins;
            Save();
        }
    }

    // ---------- 读取 ----------
    public static bool GetLevelClear(int sceneNo)
    {
        return GetLevel(sceneNo).isClear;
    }

    public static int GetLevelCoins(int sceneNo)
    {
        return GetLevel(sceneNo).coins;
    }

    // ---------- 调试 ----------
    public static void ClearAll()
    {
        PlayerPrefs.DeleteKey(key);
        data = null;
    }
}