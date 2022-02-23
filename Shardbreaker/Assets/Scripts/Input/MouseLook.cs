using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private float sensitivityX = 1f;
    private float sensitivityY = 1f;
    private float mouseX;
    private float mouseY;
    [SerializeField] Transform playerPivot;
    [SerializeField] float xClamp = 85f;
    private float xRotation = 0f;
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        UpdateRotation();
    }

    public void SetSensitivity(float sensitivity)
    {
        sensitivityX = sensitivity;
        sensitivityY = sensitivity;
    }

    public void ReceiveInput(Vector2 input)
    {
        if (!player.InputEnabled) return;

        mouseX = input.x * sensitivityX;
        mouseY = input.y * sensitivityY;
    }

    public void ResetRotation()
    {
        Debug.Log("TEST");
        transform.Rotate(Vector3.up, 0 * Time.deltaTime);
        playerPivot.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void UpdateRotation()
    {
        if (!player.InputEnabled) return;

        transform.Rotate(Vector3.up, mouseX * Time.deltaTime);

        xRotation -= mouseY * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        playerPivot.eulerAngles = targetRotation;
    }

}
