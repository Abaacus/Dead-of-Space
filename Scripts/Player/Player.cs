using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public Transform cam;
    float verticalLookRotation;

    public GravityBody gb;
    [SerializeField]
    float mass = 1f;
    [SerializeField]
    float jumpPower = 1f;

    public BaseGun gun;

    public float crystalCount;
    public TextMeshProUGUI crystalCountGUI;

    public bool lockPlayer;


    void Start()
    {
        gb = new GravityBody(GetComponent<Rigidbody>(), SpinType.point, mass);
        gun = GetComponentInChildren<BaseGun>();
        UpdateCrystalCounter();
    }

    void Update()
    {
        if (!lockPlayer)
        {
            gb.Spin(Input.GetAxis("Mouse X") * mouseSensitivityX, 0.1f);

            verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -verticalClampAngle, verticalClampAngle);
            cam.localEulerAngles = Vector3.left * verticalLookRotation;

            if (Input.GetButton("Jump"))
            {
                gb.Boost(jumpPower);
            }

            if (Input.GetButton("Fire1"))
            {
                gun.FireGun(cam.transform.forward);
            }

            gb.Orbit(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * speed);
        }
    }

    public void UpdateCrystalCounter(int crystalIncrease = 0)
    {
        crystalCount += crystalIncrease;
        crystalCountGUI.text = "x" + crystalCount;
    }
}
