# Unity Car Scene Setup

Tools to auto-create a simple Unity scene with a drivable car and circular track.

## How to use

1. Open this repository in Unity (version 6000.5.6f1 or compatible).
2. Wait for Unity to import assets (kara2.fbx should be in Assets/Models).
3. In Unity Editor, open the menu: **Tools → Setup Car Scene**.
   - This will create **Assets/Scenes/Main.unity** containing:
     - A large green ground plane (with BoxCollider)
     - A circular track with red outer and inner walls
     - A Car GameObject (from kara2.fbx) with Rigidbody and WheelColliders on all wheels
     - CarController component for driving with wheel physics
     - Main Camera with FollowCamera script
4. Press **Play** and control the car:
   - **W/A/S/D** or **Arrow Keys**: Drive and steer
   - **Space**: Brake

## Files included

- `Assets/Scripts/CarController.cs` — Car physics with WheelColliders and steering
- `Assets/Scripts/FollowCamera.cs` — Third-person camera following the car
- `Assets/Editor/CarSceneSetup.cs` — Editor menu tool to generate the scene with proper colliders
- `Assets/Models/kara2.fbx` — Car model (imported from Blender)
- `README.md` — This file
- `.gitignore` — Standard Unity gitignore

## How it works

### WheelColliders
- Each wheel (Válec) gets a WheelCollider component automatically
- Front wheels (determined by Z position) rotate based on steering input (A/D)
- All wheels apply motor torque for acceleration/deceleration
- Wheels have suspension and friction curves for realistic physics

### Ground and Track
- Ground uses a large BoxCollider (not MeshCollider) to prevent falling through
- Track walls are kinematic Rigidbodies with BoxColliders
- Car body has a BoxCollider for collision detection

## Notes

- **kara2.fbx** must be placed at `Assets/Models/kara2.fbx` for the tool to find it.
- The circular track is generated procedurally and includes outer/inner walls to keep the car on the path.
- The scene is automatically saved to `Assets/Scenes/Main.unity` after running the tool.
- Wheels are automatically detected by searching for objects named "Válec" in the model hierarchy.
- WheelColliders simulate realistic suspension and tire grip.

## Troubleshooting

- **Car is falling through ground**: Check that Ground has a BoxCollider and Rigidbody set to isKinematic
- **Steering not working**: Ensure front wheels are detected (check Console for wheel detection log)
- **Car not moving**: Check that CarController component is present and WheelColliders are on all wheels

## Future improvements

- Add lap timer and speedometer UI
- Add more track variations (figure-8, twisty paths, etc.)
- Add audio (engine, collision sounds)
- Add visual feedback (speedometer, steering indicator)
