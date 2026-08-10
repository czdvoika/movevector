using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float motorForce = 3000f;        // síla motoru aplikovaná na kola
    public float maxSpeed = 25f;            // maximální rychlost
    public float steeringAngle = 30f;       // maximální úhel natočení přední kola
    public float brakeTorque = 3000f;       // síla brzdy
    public float friction = 1f;             // tření kol

    private WheelCollider[] wheelColliders;
    private Transform[] wheelMeshes;        // Transformy válců pro vizuální rotaci
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1200f;
        }
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);
        rb.drag = 0.1f;          // malé vzdušní tření
        rb.angularDrag = 0.3f;   // rotační tření

        // Najdi všechna kola (WheelColliders)
        wheelColliders = GetComponentsInChildren<WheelCollider>();
        if (wheelColliders.Length == 0)
        {
            Debug.LogError("CarController: Žádné WheelColliders nenalezeny! Přidej WheelColliders na kola.");
            enabled = false;
            return;
        }

        Debug.Log($"CarController: Nalezeno {wheelColliders.Length} kol (WheelColliders)");

        // Najdi vizuální meshes válců
        wheelMeshes = new Transform[wheelColliders.Length];
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelMeshes[i] = wheelColliders[i].transform;
        }
    }

    void FixedUpdate()
    {
        if (rb == null || wheelColliders.Length == 0) return;

        float vertical = Input.GetAxis("Vertical");   // W/S nebo šipky - akcelerace/brzda
        float horizontal = Input.GetAxis("Horizontal"); // A/D nebo šipky - řízení
        bool isBraking = Input.GetKey(KeyCode.Space);

        // Omez maximální rychlost
        float currentSpeed = rb.velocity.magnitude;
        if (currentSpeed > maxSpeed && vertical > 0)
        {
            vertical = 0;
        }

        ApplyMotor(vertical, isBraking);
        ApplySteering(horizontal);
        UpdateWheelMeshes();
    }

    private void ApplyMotor(float vertical, bool isBraking)
    {
        float motorInput = vertical;

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            WheelCollider wheel = wheelColliders[i];

            if (isBraking)
            {
                wheel.motorTorque = 0;
                wheel.brakeTorque = brakeTorque;
            }
            else
            {
                wheel.motorTorque = motorInput * motorForce;
                wheel.brakeTorque = 0;
            }
        }
    }

    private void ApplySteering(float horizontal)
    {
        float currentSteeringAngle = horizontal * steeringAngle;

        // Aplikuj řízení jen na přední kola (index 0 a 1 nebo podle pozice)
        // Obvykle: index 0,1 = přední; index 2,3 = zadní
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            WheelCollider wheel = wheelColliders[i];
            Vector3 wheelPos = wheel.transform.position;
            
            // Určí přední kola podle Y pozice (vyšší Y = přední, nižší = zadní)
            // nebo podle názvu obsahujícího "003" a bez čísla (Válec = přední)
            bool isFrontWheel = wheelPos.z > 0; // přední jsou dál vpředu (kladná Z)

            if (isFrontWheel)
            {
                wheel.steerAngle = currentSteeringAngle;
            }
            else
            {
                wheel.steerAngle = 0; // zadní kola se nenatáčejí
            }
        }
    }

    private void UpdateWheelMeshes()
    {
        // Vizuálně rotuj mesh kol podle jejich fyzické rotace
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            WheelCollider wheel = wheelColliders[i];
            Transform wheelMesh = wheelMeshes[i];

            // Získej aktuální rotaci kolesa
            Vector3 wheelPosition = wheelMesh.position;
            Quaternion wheelRotation = wheelMesh.rotation;

            wheel.GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh.position = wheelPosition;
            wheelMesh.rotation = wheelRotation;
        }
    }
}
