#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CarSceneSetup
{
    [MenuItem("Tools/Setup Car Scene")]
    public static void Setup()
    {
        // Create new scene
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Create ground
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Ground";
        plane.transform.position = Vector3.zero;

        // Try to find the model named 'kara2' anywhere in Assets
        string[] guids = AssetDatabase.FindAssets("kara2 t:Model");
        if (guids == null || guids.Length == 0)
        {
            guids = AssetDatabase.FindAssets("kara2");
        }

        if (guids == null || guids.Length == 0)
        {
            EditorUtility.DisplayDialog("Setup Car Scene", "Model 'kara2' not found in Assets. Please place kara2.fbx into Assets/Models or search for it.", "OK");
            return;
        }

        string modelPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);

        if (modelPrefab == null)
        {
            EditorUtility.DisplayDialog("Setup Car Scene", "Failed to load model at " + modelPath, "OK");
            return;
        }

        // Instantiate model
        GameObject carInstance = PrefabUtility.InstantiatePrefab(modelPrefab) as GameObject;
        if (carInstance == null)
        {
            carInstance = Object.Instantiate(modelPrefab);
        }
        carInstance.name = "Car";
        carInstance.transform.position = new Vector3(0f, 0.5f, 0f);

        // Add Rigidbody if missing
        Rigidbody rb = carInstance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = carInstance.AddComponent<Rigidbody>();
            rb.mass = 1200f;
        }
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);

        // Add collider: try MeshCollider (convex) otherwise BoxCollider
        Collider col = carInstance.GetComponent<Collider>();
        if (col == null)
        {
            var meshFilter = carInstance.GetComponentInChildren<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                MeshCollider mc = carInstance.AddComponent<MeshCollider>();
                mc.convex = true;
            }
            else
            {
                carInstance.AddComponent<BoxCollider>();
            }
        }

        // Add CarController script if not present
        if (carInstance.GetComponent<CarController>() == null)
        {
            carInstance.AddComponent<CarController>();
        }

        // Create camera
        GameObject camGO = new GameObject("Main Camera");
        Camera cam = camGO.AddComponent<Camera>();
        camGO.tag = "MainCamera";
        camGO.transform.position = carInstance.transform.position + new Vector3(0f, 5f, -8f);
        var follow = camGO.AddComponent<FollowCamera>();
        follow.target = carInstance.transform;

        // Ensure Scenes folder exists
        System.IO.Directory.CreateDirectory("Assets/Scenes");

        // Save scene
        string scenePath = "Assets/Scenes/Main.unity";
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Setup Car Scene", "Scene created and saved to " + scenePath, "OK");
    }
}
#endif
