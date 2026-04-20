using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelManager
{
    // 判断某关能不能进入
    public static bool CanEnterLevel(int sceneNo)
    {
        // 第一关默认可进
        if (sceneNo <= 1)
            return true;

        // 上一关通关了，才能进
        return SaveManager.GetLevelClear(sceneNo - 1);
    }

    // 进入关卡
    public static bool EnterLevel(int sceneNo)
    {
        if (!CanEnterLevel(sceneNo))
        {
            Debug.Log("不能进入 Level " + sceneNo + "，上一关未通关");
            return false;
        }

        string sceneName = GetSceneName(sceneNo);
        SceneManager.LoadScene(sceneName);
        return true;
    }

    // 通关当前关卡
    public static void CompleteLevel(int sceneNo, int coins)
    {
        SaveManager.SetLevelClear(sceneNo);
        SaveManager.SetLevelCoins(sceneNo, coins);
    }

    // 获取场景名
    public static string GetSceneName(int sceneNo)
    {
        return "Level" + sceneNo;
    }
}