using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;                 // odkaz na auto
    public Vector3 offset = new Vector3(0f, 5f, -8f);
    public float smoothSpeed = 5f;
    public float lookAtHeight = 1.5f;

    void LateUpdate()
    {
        if (target == null) return;
        // offset v lokálních souřadnicích auta (kamera za autem)
        Vector3 desiredPos = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
        transform.LookAt(target.position + Vector3.up * lookAtHeight);
    }
}
