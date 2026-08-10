# Tools to auto-create a simple Unity scene using the kara2.fbx model (as Car)

This branch adds scripts and an Editor tool that will create a playable scene using the existing Assets/Models/kara2.fbx model from this repository.

How to use
1. Open this repository/branch in Unity (your selected Unity version: 6000.5.6f1).
2. Wait for Unity to import assets (kara2.fbx should be visible under Assets/Models or similar).
3. In Unity Editor, open the menu: Tools -> Setup Car Scene. This will create Assets/Scenes/Main.unity containing:
   - a Ground plane
   - an instantiated Car GameObject from kara2.fbx with Rigidbody and a collider
   - a Main Camera with FollowCamera attached
   - the Car GameObject will have the CarController component attached
4. Press Play and control the car with W/A/S/D (or arrow keys). Hold Space to brake.

Notes
- The Editor script tries to find the model by name "kara2". If your model is in a different path or named differently, move/rename it to Assets/Models/kara2.fbx.
- No textures/materials were added. The model will use its imported materials (if any) or Unity defaults.
- The project does not include heavy binary files; kara2.fbx is under 100 MB as you indicated.
- For a more realistic driving feel, we can add WheelColliders and tuning later.

Files added
- Assets/Scripts/CarController.cs
- Assets/Scripts/FollowCamera.cs
- Assets/Editor/CarSceneSetup.cs (menu tool)
- .gitignore
- README.md (this file)

If you want, I can open a Pull Request from add-unity-car-scene into main after you review these changes.
