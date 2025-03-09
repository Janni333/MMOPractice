using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MapTool {

    [MenuItem("Map Tools/Export Teleporters")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        //储存当前场景
        //  获取场景
        Scene current = EditorSceneManager.GetActiveScene();
        string currentName = current.name;
        //  保存场景
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }

        //传送点对象列表
        List<TeleporterObject> allTeleporters = new List<TeleporterObject>();

        //遍历地图
        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene {0} not existed!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            TeleporterObject[] sceneteleporters = GameObject.FindObjectsOfType<TeleporterObject>();

            foreach (var teleporter in sceneteleporters)
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.teleporterId))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图：{0} 中配置的Teleporter：[{1}]不存在", map.Value.Resource, teleporter.teleporterId), "确定");
                    return;
                }

                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.teleporterId];
                if (def.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图：{0} 中配置的Teleporter:[{1}] MapID:{2} 错误", map.Value.Resource, teleporter.teleporterId, def.MapID), "确定");
                    return;
                }

                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }

        DataManager.Instance.SaveTeleporters();

        //场景恢复
        EditorSceneManager.OpenScene("Assets/Levels/" + currentName + ".unity");

        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");
    }

    [MenuItem("Map Tools/Export Spawner")]
    public static void ExportSpawners()
    {
        DataManager.Instance.Load();

        Scene Cur = EditorSceneManager.GetActiveScene();
        string curname = Cur.name;
        if (Cur.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }

        if (DataManager.Instance.SpawnPoints == null)
        {
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();
        }

        //遍历地图
        foreach (var map in DataManager.Instance.Maps)
        {
            //校验是否有该地图
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene{0} not existed", sceneFile);
                continue;
            }
            //打开地图
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            //录入信息
            //  获取该地图下所有刷怪点物体
            SpawnObject[] spawners = GameObject.FindObjectsOfType<SpawnObject>();
            //  校验外层字典
            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.Value.ID))
            {
                DataManager.Instance.SpawnPoints[map.Value.ID] = new Dictionary<int, SpawnPointDefine>();
            }
            //  检验内层字典
            foreach (var sp in spawners)
            {
                if (!DataManager.Instance.SpawnPoints[map.Value.ID].ContainsKey(sp.ID))
                {
                    DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID] = new SpawnPointDefine();
                }

                SpawnPointDefine def = DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID];
                def.ID = sp.ID;
                def.MapID = map.Value.ID;
                def.Position = GameObjectTool.WorldToLogicN(sp.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(sp.transform.forward);
            }
        }

        DataManager.Instance.SaveSpawnPoints();
        EditorSceneManager.OpenScene("Assets/Levels/" + curname + ".unity");
        EditorUtility.DisplayDialog("提示", "刷怪点导出完成", "确定");
    }

    [MenuItem("Map Tools/Generate NavData")]
    public static void GenerateNavData()
    {
        Material red = new Material(Shader.Find("Particles/Alpha Blended"));
        red.color = Color.red;
        red.SetColor("_TintColor", Color.red);
        red.enableInstancing = true;
        GameObject go = GameObject.Find("MinimapBoudingBox");
        if (go != null)
        {
            GameObject root = new GameObject("Root");
            BoxCollider bound = go.GetComponent<BoxCollider>();
            float step = 1f;
            for (float x = bound.bounds.min.x; x < bound.bounds.max.x; x += step)
            {
                for (float z = bound.bounds.min.z; z < bound.bounds.max.z; z += step)
                {
                    for (float y = bound.bounds.min.y; y < bound.bounds.max.y + 5f; y += step)
                    {
                        var pos = new Vector3(x, y, z);
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(pos, out hit, 0.5f, NavMesh.AllAreas))
                        {
                            if (hit.hit)
                            {
                                var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                box.name = "Hit" + hit.mask;
                                box.GetComponent<MeshRenderer>().sharedMaterial = red;
                                box.transform.SetParent(root.transform, true);
                                box.transform.localPosition = pos;
                                box.transform.localScale = Vector3.one * 0.9f;
                            }
                        }
                    }
                }
            }
        }
    }
}
