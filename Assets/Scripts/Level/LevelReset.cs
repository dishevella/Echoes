using UnityEngine;
using System.Collections.Generic;

public class LevelReset : MonoBehaviour
{
    public static LevelReset instance;

    [System.Serializable]
    public class ReplaceableObject
    {
        public string spawnID;
        public GameObject levelObject;
        public Transform spawnPosition;
    }

    public List<ReplaceableObject> objects = new List<ReplaceableObject>();

    // ⭐ 核心：用 spawnID 管唯一实例
    private Dictionary<string, GameObject> spawnedDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        ObjectRespawn();
    }

    public void ObjectRespawn()
    {
        foreach (var obj in objects)
        {
            if (obj.levelObject == null || string.IsNullOrEmpty(obj.spawnID))
                continue;

            // 1️⃣ 如果这个ID已经有实例 → 先删除
            if (spawnedDict.ContainsKey(obj.spawnID))
            {
                GameObject oldObj = spawnedDict[obj.spawnID];

                if (oldObj != null)
                {
                    Destroy(oldObj);
                }

                spawnedDict[obj.spawnID] = null;
            }

            // 2️⃣ 生成新实例
            GameObject newObj = Instantiate(obj.levelObject, obj.spawnPosition.position, Quaternion.identity);

            // 3️⃣ 记录
            spawnedDict[obj.spawnID] = newObj;
        }
    }

    // ⭐ 可选：手动注册（如果你有动态生成）
    public void Register(string spawnID, GameObject obj)
    {
        spawnedDict[spawnID] = obj;
    }

    // ⭐ 可选：物体死亡时主动注销
    public void Unregister(string spawnID)
    {
        if (spawnedDict.ContainsKey(spawnID))
        {
            spawnedDict[spawnID] = null;
        }
    }

    // ⭐ 可选：获取当前实例
    public GameObject Get(string spawnID)
    {
        if (spawnedDict.ContainsKey(spawnID))
        {
            return spawnedDict[spawnID];
        }

        return null;
    }
}