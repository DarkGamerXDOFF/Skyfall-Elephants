using UnityEngine;

public class BobbleHead2D : MonoBehaviour
{
    public HingeJoint2D hinge;

    [Header("Spring Settings")]
    public float springStrength = 10f;   // how strongly it returns to center
    public float damping = 1f;          // reduces oscillation

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float angle = hinge.jointAngle;                 // current angle relative to body
        float springForce = -angle * springStrength;    // Hooke's law: -kx
        float dampingForce = -rb.angularVelocity * damping;

        float torque = springForce + dampingForce;
        rb.AddTorque(torque);
    }
}
