#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public class CarSceneSetup
{
    [MenuItem("Tools/Setup Car Scene")]
    public static void Setup()
    {
        // Create new scene
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Create ground (large plane)
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Ground";
        plane.transform.position = Vector3.zero;
        plane.transform.localScale = new Vector3(50f, 1f, 50f); // Make it larger
        Renderer groundRenderer = plane.GetComponent<Renderer>();
        if (groundRenderer != null)
        {
            groundRenderer.material.color = new Color(0.2f, 0.8f, 0.2f); // Green color
        }

        // Remove collider from ground (we'll add a flat one)
        Collider groundCollider = plane.GetComponent<Collider>();
        if (groundCollider != null) Object.DestroyImmediate(groundCollider);
        plane.AddComponent<BoxCollider>(); // Simple box collider for ground

        // Create circular track using cubes as track segments
        CreateCircularTrack();

        // Try to find the model named 'kara2'
        string[] guids = AssetDatabase.FindAssets("kara2 t:Model");
        if (guids == null || guids.Length == 0)
        {
            guids = AssetDatabase.FindAssets("kara2");
        }

        string modelPath = null;
        if (guids != null && guids.Length > 0)
        {
            modelPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        }

        if (string.IsNullOrEmpty(modelPath))
        {
            EditorUtility.DisplayDialog("Setup Car Scene", "Model 'kara2' not found in Assets.", "OK");
            return;
        }

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
        carInstance.transform.position = new Vector3(0f, 1f, 0f);

        // Add Rigidbody if missing
        Rigidbody rb = carInstance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = carInstance.AddComponent<Rigidbody>();
            rb.mass = 1200f;
        }
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);

        // Add collider
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

        // Create scene folder if needed
        System.IO.Directory.CreateDirectory("Assets/Scenes");

        // Save scene
        string scenePath = "Assets/Scenes/Main.unity";
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Setup Car Scene", "Scene created with circular track at " + scenePath, "OK");
    }

    private static void CreateCircularTrack()
    {
        float radius = 20f;           // Radius of the circular track
        int segments = 32;             // Number of segments
        float trackWidth = 8f;         // Width of the track
        float wallHeight = 0.5f;       // Height of the wall/barrier
        float segmentAngle = 360f / segments;

        GameObject trackParent = new GameObject("Track");

        for (int i = 0; i < segments; i++)
        {
            float angle = i * segmentAngle * Mathf.Deg2Rad;
            float nextAngle = (i + 1) * segmentAngle * Mathf.Deg2Rad;

            // Outer wall
            Vector3 outerPos = new Vector3(Mathf.Cos(angle) * (radius + trackWidth / 2), 0.25f, Mathf.Sin(angle) * (radius + trackWidth / 2));
            GameObject outerWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            outerWall.name = "OuterWall_" + i;
            outerWall.transform.parent = trackParent.transform;
            outerWall.transform.position = outerPos;
            outerWall.transform.localScale = new Vector3(1f, wallHeight, 2f);
            Renderer outerRenderer = outerWall.GetComponent<Renderer>();
            if (outerRenderer != null)
            {
                outerRenderer.material.color = new Color(0.8f, 0.2f, 0.2f); // Red
            }
            Object.DestroyImmediate(outerWall.GetComponent<Collider>());
            outerWall.AddComponent<BoxCollider>();

            // Inner wall
            Vector3 innerPos = new Vector3(Mathf.Cos(angle) * (radius - trackWidth / 2), 0.25f, Mathf.Sin(angle) * (radius - trackWidth / 2));
            GameObject innerWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            innerWall.name = "InnerWall_" + i;
            innerWall.transform.parent = trackParent.transform;
            innerWall.transform.position = innerPos;
            innerWall.transform.localScale = new Vector3(1f, wallHeight, 2f);
            Renderer innerRenderer = innerWall.GetComponent<Renderer>();
            if (innerRenderer != null)
            {
                innerRenderer.material.color = new Color(0.8f, 0.2f, 0.2f); // Red
            }
            Object.DestroyImmediate(innerWall.GetComponent<Collider>());
            innerWall.AddComponent<BoxCollider>();
        }
    }
}
#endif
