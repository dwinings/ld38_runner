using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

namespace LD38Runner {
  public class LevelImporter : AssetPostprocessor {
    // Save the width and the height of a layer, we need it for a few places.
    private static int width;
    private static int height;

    // These are all prefabs we can instantiate
    private static Dictionary<string, GameObject> objectPrefabMap = null;
    private static GameObject chunkObj;

    private static readonly String PREFAB_DIR = "Assets/Prefabs";
    private static readonly String CHUNK_PREFAB_OUTPUT_DIR = "Assets/Prefabs/Level Chunks";
    private static readonly String CHUNK_SRC_DIR = "Assets/Level Chunks";

    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
      foreach (var assetPath in importedAssets) {
        if (!assetPath.EndsWith(".tmx")) {
          return;
        }
        ImportTMXLevel(assetPath);
      }
    }

    // High level entry point into the importing process.
    public static GameObject ImportTMXLevel(string fileName) {
      width = -1;
      height = -1;
      var mapFile = XDocument.Load(fileName);
      var gidPrefabMap = ReadTileSet(mapFile, fileName);
      var chunkName = Path.GetFileNameWithoutExtension(fileName);

      if (GameObject.Find(chunkName) != null) {
        GameObject.DestroyImmediate(GameObject.Find(chunkName));
      }

      chunkObj = new GameObject(chunkName);
      var chunk = chunkObj.AddComponent<LevelChunk>();


      foreach (var layer in mapFile.Document.Root.Elements("layer")) {
        SpawnPrefabsFromLayer(layer, gidPrefabMap);
      }

      if (mapFile.Document.Root.Element("properties") != null) {
        foreach (var mapProp in mapFile.Document.Root.Element("properties").Elements("property")) {
          var propName = mapProp.Attribute("name").Value;
          var propValue = mapProp.Attribute("value").Value;
          switch (propName) {
            case "start_height":
              chunk.start_height = int.Parse(propValue);
              break;
            case "end_height":
              chunk.end_height = int.Parse(propValue);
              break;
            case "starting_x":
              chunk.starting_x = float.Parse(propValue);
              break;
            case "height":
              chunk.height = int.Parse(propValue);
              break;
            case "width":
              chunk.width = int.Parse(propValue);
              break;
            default:
              Debug.Log(string.Format("Unknown map prop {0}, ignoring.", propName));
              break;
          }
        }
      }
      return chunkObj;
    }

    private static Dictionary<String, GameObject> TilePrefabMap() {
      if (objectPrefabMap != null)
        return objectPrefabMap;

      objectPrefabMap = new Dictionary<string, GameObject>();
      var allAssetGuids = AssetDatabase.FindAssets("", new String[] { PREFAB_DIR });
      foreach (string guid in allAssetGuids) {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        if (!assetPath.EndsWith(".prefab"))
          continue;

        string tileName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
        if (objectPrefabMap.ContainsKey(tileName)) {
          Debug.Log(string.Format("Can't add asset at {0} as {1} because of a name conflict", assetPath, tileName));
        } else {
          Debug.Log(string.Format("Adding prefab from {0} as tile {1}.", assetPath, tileName));
          GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
          objectPrefabMap.Add(tileName, prefab);
        }
      }

      return objectPrefabMap;
    }

    // This maps the type properties defined in the tileset to the logical game objects we'd want to create for our level.
    private static Dictionary<uint, string> generateGIDPrefabMap(List<XElement> tiles, uint firstGid) {
      TilePrefabMap();

      var gidMap = new Dictionary<uint, string>();
      foreach (var tile in tiles) {
        uint gid = Convert.ToUInt32(tile.Attribute("id").Value) + firstGid;
        XElement typeProperty = null;
        var props = tile.Element("properties").Elements("property");
        foreach (var prop in props) {
          if (prop.HasAttributes && prop.Attribute("name").Value == "type") {
            typeProperty = prop;
            break;
          }
        }

        if (typeProperty == null) {
          continue;
        }

        var type = typeProperty.Attribute("value").Value.ToString();
        if (!gidMap.ContainsKey(gid)) {
          gidMap.Add(gid, type.ToLower());
        }
      }
      return gidMap;
    }

    // Iterate through the encoded CSV that defines the tile GIDS, and spawn any prefabs that we recognize for that layer.
    private static void SpawnPrefabsFromLayer(XElement layer, Dictionary<uint, string> gidPrefabMap) {
      var layerWidth = Convert.ToInt32(layer.Attribute("width").Value);
      var layerHeight = Convert.ToInt32(layer.Attribute("height").Value);
      if ((layerWidth != width && width >= 0) || (layerHeight != height && height >= 0)) {
        throw new Exception("Layers have different dimensions, wtf");
      } else {
        height = layerHeight;
        width = layerWidth;
      }

      string rawStr = layer.Element("data").Value.ToString();
      // Xml.Linq adds gross whitespace to the string, so we gotta clear out the empty split elements.
      var gidArray = rawStr.Split(",\n\r".ToCharArray()).Where(s => s.Length > 0).ToList();
      for (int idx = 0; idx < gidArray.Count(); idx++) {
        var gid = Convert.ToUInt32(gidArray[idx]);
        var x = idx % width;
        var y = idx / width;
        var position = translateFromCSV(x, y);
        if (gidPrefabMap.ContainsKey(gid)) {
          SpawnGameObject(gidPrefabMap[gid], position);
        }
      }
    }

    // Not all logical objects want to be instantiated in the same way, so we break them out here to handle that.
    private static void SpawnGameObject(string prefabName, Vector3 position) {
      GameObject instance = null;
      UnityEngine.Object prefab = null;

      if (objectPrefabMap.ContainsKey(prefabName)) {
        prefab = objectPrefabMap[prefabName];
        instance = GameObject.Instantiate(prefab, position, Quaternion.identity) as GameObject;
      } else {
        Debug.Log(string.Format("Warning: Couldn't resolve tiled attribute {0} to any prefab.", prefabName));
      }

      if (instance != null) {
        instance.transform.SetParent(chunkObj.transform);
      }
    }

    // The map can either define the tileset inline, or point to a file.
    // Our files should probably do that latter, but why not handle both?
    // Things will get craaaaay if you add multiple tilesets, but we could
    // augment this method to handle that if it comes to it.
    //
    // Oh, and we need the base path, because the .tsx paths are relative to the .tmx file.
    // Not the best file format definition but eh.
    private static Dictionary<uint, string> ReadTileSet(XDocument root, string basePath) {
      var tilesetNode = root.Document.Root.Element("tileset");
      uint firstGid = Convert.ToUInt32(tilesetNode.Attribute("firstgid").Value);

      // Read tileset from file
      if (tilesetNode.Attribute("source") != null) {
        var path = tilesetNode.Attribute("source").Value;
        path = Path.Combine(Path.GetDirectoryName(basePath), path);
        tilesetNode = XDocument.Load(path).Document.Root;
      }

      var gidPrefabNameMap = generateGIDPrefabMap(tilesetNode.Elements("tile").ToList(), firstGid);
      return gidPrefabNameMap;
    }

    // CSV is defined top-left to bottom right, our map is bottom-left to top-right.
    // ALSO we add 2 to our width and height to add an outer wall.
    private static Vector3 translateFromCSV(int x, int y) {
      return new Vector3(x, -y, 0f);
    }

    [MenuItem("LD38 Runner/Regenerate Chunk List")]
    static void RegenerateLevelPrefabs() {
      var levelFiles = Directory.GetFiles(CHUNK_SRC_DIR).Where(fileName => fileName.EndsWith(".tmx"));

      var loaderPrefab = AssetDatabase.LoadAssetAtPath(PREFAB_DIR + "/GameManager.prefab", typeof(GameObject));
      var loaderObject = UnityEngine.Object.Instantiate(loaderPrefab) as GameObject;

      var chunkList = loaderObject.GetComponent<LevelChunkList>();
      if (chunkList.levelChunks != null) {
        chunkList.levelChunks.Clear();
      } else {
        chunkList.levelChunks = new List<GameObject>();
      }

      try {
        int idx = 0;
        int total = levelFiles.Count();
        foreach (string levelFilePath in levelFiles) {
          idx += 1;
          var baseName = Path.GetFileNameWithoutExtension(levelFilePath);
          EditorUtility.DisplayCancelableProgressBar("Regenerating Levels", string.Format("{0} - {1}/{2}", baseName, idx, total), (float)idx / (float)total);
          var level = ImportTMXLevel(levelFilePath);
          var prefab = PrefabUtility.CreateEmptyPrefab(CHUNK_PREFAB_OUTPUT_DIR + "/" + baseName + ".prefab");
          chunkList.levelChunks.Add(PrefabUtility.ReplacePrefab(level, prefab, ReplacePrefabOptions.ConnectToPrefab));
          UnityEngine.Object.DestroyImmediate(level);
        }

        PrefabUtility.ReplacePrefab(loaderObject, loaderPrefab, ReplacePrefabOptions.ConnectToPrefab);
        UnityEngine.Object.DestroyImmediate(loaderObject);
      } finally {
        EditorUtility.ClearProgressBar();
      }
    }
  }
}
