#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

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
        plane.transform.localScale = new Vector3(50f, 1f, 50f);
        Renderer groundRenderer = plane.GetComponent<Renderer>();
        if (groundRenderer != null)
        {
            groundRenderer.material.color = new Color(0.2f, 0.8f, 0.2f);
        }

        // Remove collider and replace with BoxCollider (convex, not mesh)
        Collider groundCollider = plane.GetComponent<Collider>();
        if (groundCollider != null) Object.DestroyImmediate(groundCollider);
        BoxCollider groundBox = plane.AddComponent<BoxCollider>();
        groundBox.size = new Vector3(100f, 0.1f, 100f); // Large collision box
        
        // Add Rigidbody to ground (kinematic so it doesn't fall)
        Rigidbody groundRb = plane.AddComponent<Rigidbody>();
        groundRb.isKinematic = true;
        groundRb.constraints = RigidbodyConstraints.FreezeAll;

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

        // Add Rigidbody to car
        Rigidbody rb = carInstance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = carInstance.AddComponent<Rigidbody>();
        }
        rb.mass = 1200f;
        rb.drag = 0.1f;
        rb.angularDrag = 0.3f;
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);

        // Remove all colliders from the car (we'll replace with WheelColliders on wheels)
        Collider[] allColliders = carInstance.GetComponentsInChildren<Collider>();
        foreach (Collider col in allColliders)
        {
            Object.DestroyImmediate(col);
        }

        // Add a simple body collider (BoxCollider) to the car body
        BoxCollider bodyCollider = carInstance.AddComponent<BoxCollider>();
        bodyCollider.size = new Vector3(1.5f, 1f, 3f);
        bodyCollider.center = new Vector3(0f, 0f, 0f);

        // Add WheelColliders to wheels
        SetupWheelColliders(carInstance);

        // Add CarController script
        if (carInstance.GetComponent<CarController>() == null)
        {
            carInstance.AddComponent<CarController>();
        }

        // Create camera
        GameObject camGO = new GameObject("Main Camera");
        Camera cam = camGO.AddComponent<Camera>();
        camGO.tag = "MainCamera"";
        camGO.transform.position = carInstance.transform.position + new Vector3(0f, 5f, -8f);
        var follow = camGO.AddComponent<FollowCamera>();
        follow.target = carInstance.transform;

        // Create scene folder if needed
        System.IO.Directory.CreateDirectory("Assets/Scenes");

        // Save scene
        string scenePath = "Assets/Scenes/Main.unity";
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Setup Car Scene", "Scene created with circular track and WheelColliders at " + scenePath, "OK");
    }

    private static void SetupWheelColliders(GameObject carInstance)
    {
        // Najdi všechny Válce (kola) v modelu
        Transform[] allChildren = carInstance.GetComponentsInChildren<Transform>();
        List<Transform> wheels = new List<Transform>();

        foreach (Transform child in allChildren)
        {
            string name = child.name.ToLower();
            // Hledáme objekty pojmenované "Válec" (případně s číslem)
            if (name.Contains("válec") || name.Contains("valec") || name.Contains("wheel") || name.Contains("tire"))
            {
                wheels.Add(child);
            }
        }

        Debug.Log($"Nalezeno {wheels.Count} kol: {string.Join(", ", wheels.ConvertAll(w => w.name))}");

        // Přidej WheelCollider na každé kolo
        foreach (Transform wheel in wheels)
        {
            // Odstraň staré colliders
            Collider[] existingColliders = wheel.GetComponents<Collider>();
            foreach (Collider col in existingColliders)
            {
                Object.DestroyImmediate(col);
            }

            // Přidej WheelCollider
            WheelCollider wc = wheel.gameObject.AddComponent<WheelCollider>();
            wc.radius = 0.5f;           // Poloměr kola
            wc.mass = 50f;              // Hmotnost kola
            wc.wheelDampingRate = 0.25f;
            wc.forceAppPointDistance = 0f;
            wc.center = Vector3.zero;
            wc.suspensionDistance = 0.3f;
            wc.suspensionSpring = new JointSpring { spring = 35000f, damper = 4500f, targetPosition = 0.5f };
            wc.forwardFriction = new WheelFrictionCurve { extremumSlip = 0.4f, extremumValue = 1f, asymptoteSlip = 0.8f, asymptoteValue = 0.5f };
            wc.sidewaysFriction = new WheelFrictionCurve { extremumSlip = 0.2f, extremumValue = 1f, asymptoteSlip = 0.5f, asymptoteValue = 0.75f };
        }
    }

    private static void CreateCircularTrack()
    {
        float radius = 20f;
        int segments = 32;
        float trackWidth = 8f;
        float wallHeight = 0.5f;
        float segmentAngle = 360f / segments;

        GameObject trackParent = new GameObject("Track");
        Rigidbody trackRb = trackParent.AddComponent<Rigidbody>();
        trackRb.isKinematic = true;
        trackRb.constraints = RigidbodyConstraints.FreezeAll;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * segmentAngle * Mathf.Deg2Rad;

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
                outerRenderer.material.color = new Color(0.8f, 0.2f, 0.2f);
            }
            Object.DestroyImmediate(outerWall.GetComponent<Collider>());
            BoxCollider outerBox = outerWall.AddComponent<BoxCollider>();
            outerBox.isTrigger = false;
            Rigidbody outerRb = outerWall.AddComponent<Rigidbody>();
            outerRb.isKinematic = true;

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
                innerRenderer.material.color = new Color(0.8f, 0.2f, 0.2f);
            }
            Object.DestroyImmediate(innerWall.GetComponent<Collider>());
            BoxCollider innerBox = innerWall.AddComponent<BoxCollider>();
            innerBox.isTrigger = false;
            Rigidbody innerRb = innerWall.AddComponent<Rigidbody>();
            innerRb.isKinematic = true;
        }
    }
}
#endif
