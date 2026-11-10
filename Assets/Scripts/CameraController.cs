using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector3 movementDirection;
    bool isMoving = false;
    [SerializeField] float speed;
    void Start()
    {
        
    }
    public void OnMovingCamera(InputAction.CallbackContext onMove)
    {
        if (onMove.performed)
        {
            Vector3 direction = onMove.ReadValue<Vector3>();

            movementDirection = direction.normalized;
            isMoving = true;
        }
        else if (onMove.canceled)
        {
            movementDirection = Vector3.zero;
            isMoving = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.position += movementDirection * speed * Time.deltaTime;
        }
    }
}
