using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public static Player instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + " found");
        }

        instance = this;
    }

    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;
    public float verticalClampAngle = 60f;
    [SerializeField]
    float speed = 0.5f;

    [SerializeField]
    Transform cam;
    float verticalLookRotation;

    public GravityBody gravityBody;
    [SerializeField]
    float mass = 1f;
    [SerializeField]
    float jumpPower = 1f;

    void Start()
    {
        gravityBody = new GravityBody(GetComponent<Rigidbody>(), mass);
    }

    void Update()
    {
        gravityBody.Spin(Input.GetAxis("Mouse X") * mouseSensitivityX);

        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -verticalClampAngle, verticalClampAngle);
        cam.localEulerAngles = Vector3.left * verticalLookRotation;

        if (Input.GetButton("Jump"))
        {
            gravityBody.Boost(jumpPower);
        }
    }

    void FixedUpdate()
    {
        gravityBody.Orbit(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed);
    }
}
