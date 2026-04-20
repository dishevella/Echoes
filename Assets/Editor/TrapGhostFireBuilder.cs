using UnityEngine;
using UnityEditor;

public class TrapGhostFireBuilder : MonoBehaviour
{
    [MenuItem("Tools/Build Ghost Fires For Triangle Traps")]
    public static void BuildGhostFires()
    {
        TriangleTrap[] traps = Object.FindObjectsByType<TriangleTrap>(FindObjectsSortMode.None);

        if (traps == null || traps.Length == 0)
        {
            Debug.LogWarning("场景里没有找到 TriangleTrap。");
            return;
        }

        GameObject ghostPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Prefab0/Fire.prefab"
        );

        if (ghostPrefab == null)
        {
            Debug.LogError("找不到鬼火 Prefab，请检查路径。");
            return;
        }

        GameObject container = GameObject.Find("GhostFireContainer");
        if (container == null)
        {
            container = new GameObject("GhostFireContainer");
        }

        int count = 0;

        foreach (TriangleTrap trap in traps)
        {
            if (trap == null) continue;

            // 防止重复生成：检查有没有已经绑定这个 trap 的鬼火
            GhostFireTrapVisual[] existingGhosts =
                Object.FindObjectsByType<GhostFireTrapVisual>(FindObjectsSortMode.None);

            bool alreadyExists = false;
            foreach (var ghost in existingGhosts)
            {
                if (ghost != null && ghost.targetTrap == trap)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists) continue;

            GameObject newGhost = (GameObject)PrefabUtility.InstantiatePrefab(ghostPrefab);
            if (newGhost == null) continue;

            Undo.RegisterCreatedObjectUndo(newGhost, "Create Ghost Fire");

            newGhost.name = "GhostFire_" + trap.name;
            newGhost.transform.position = trap.transform.position;
            newGhost.transform.rotation = Quaternion.identity;
            newGhost.transform.SetParent(container.transform);

            GhostFireTrapVisual visual = newGhost.GetComponent<GhostFireTrapVisual>();
            if (visual != null)
            {
                visual.targetTrap = trap;
            }

            count++;
        }

        Debug.Log($"生成完成，共创建 {count} 个鬼火。");
    }
}