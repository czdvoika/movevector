using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 800f;      // síla aplikovaná vpřed
    public float maxSpeed = 20f;          // rychlost v m/s
    public float steering = 120f;         // rychlost natáčení
    public float brakeForce = 1200f;      // síla brzdy (při podržení Space)

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // mírně snížit těžiště, aby auto bylo stabilnější
            rb.centerOfMass = new Vector3(0f, -0.5f, 0f);
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        float forward = Input.GetAxis("Vertical");   // W/S nebo šipky
        float steer = Input.GetAxis("Horizontal");   // A/D nebo šipky

        // Řízení (otáčení vozu podle vstupu)
        // menší natáčení při vyšší rychlosti pro realističtější pocit
        float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / maxSpeed);
        float steerAmount = steer * steering * (1f - 0.6f * speedFactor) * Time.fixedDeltaTime;
        Quaternion turn = Quaternion.Euler(0f, steerAmount, 0f);
        rb.MoveRotation(rb.rotation * turn);

        // Pohyb vpřed/zpět jako síla ve směru předu vozidla
        Vector3 force = transform.forward * forward * acceleration * Time.fixedDeltaTime;
        rb.AddForce(force, ForceMode.Acceleration);

        // Brzda: držení Space zpomalí vozidlo
        if (Input.GetKey(KeyCode.Space))
        {
            if (rb.velocity.magnitude > 0.1f)
            {
                Vector3 brake = -rb.velocity.normalized * brakeForce * Time.fixedDeltaTime;
                rb.AddForce(brake, ForceMode.Acceleration);
            }
        }

        // Omezíme maximální rychlost
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
