# Unity Car Scene Setup

Tools to auto-create a simple Unity scene with a drivable car and circular track.

## How to use

1. Open this repository in Unity (version 6000.5.6f1 or compatible).
2. Wait for Unity to import assets (kara2.fbx should be in Assets/Models).
3. In Unity Editor, open the menu: **Tools → Setup Car Scene**.
   - This will create **Assets/Scenes/Main.unity** containing:
     - A large green ground plane
     - A circular track with red outer and inner walls
     - A Car GameObject (from kara2.fbx) with Rigidbody and collider
     - CarController component for driving
     - Main Camera with FollowCamera script
4. Press **Play** and control the car:
   - **W/A/S/D** or **Arrow Keys**: Drive and steer
   - **Space**: Brake

## Files included

- `Assets/Scripts/CarController.cs` — Car physics and input handling
- `Assets/Scripts/FollowCamera.cs` — Third-person camera following the car
- `Assets/Editor/CarSceneSetup.cs` — Editor menu tool to generate the scene
- `Assets/Models/kara2.fbx` — Car model (imported from Blender)
- `README.md` — This file
- `.gitignore` — Standard Unity gitignore

## Notes

- **kara2.fbx** must be placed at `Assets/Models/kara2.fbx` for the tool to find it.
- The circular track is generated procedurally and includes outer/inner walls to keep the car on the path.
- The scene is automatically saved to `Assets/Scenes/Main.unity` after running the tool.
- No textures are applied; the car will use imported materials or Unity defaults.

## Future improvements

- Add WheelColliders for more realistic physics
- Add lap timer and speedometer UI
- Add more track variations (figure-8, twisty paths, etc.)
- Add audio (engine, collision sounds)
