using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    float horizontalInput;
    float verticalInput;


    [Header("Zoom")]
    public float zoomSpeed = 10f;
   

  

    public float minX, maxX;
    public float minY, maxY;
    public float minZ, maxZ;


    Vector3 moveDirection;

    public float sensX;
    public float sensY;

    public Transform orientation;

    public PanelManager panelManager;

    float xRotation;
    float yRotation;
    
    private void Start()
    {
       
        xRotation = 20f;
        yRotation = 0f;
      
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0f, 0f, 0f);

      
    }
   

    private void Update()
    {

        if (panelManager != null && panelManager.isPanelOpen)
            return;

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation= Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation= Quaternion.Euler(0, yRotation, 0);
        MyInput();
        HandleZoom();
        MoveCamera();

    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput= Input.GetAxisRaw("Vertical");
    }

    private void MoveCamera()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (Input.GetKey(KeyCode.LeftShift))
            moveDirection += Vector3.up;

        if (Input.GetKey(KeyCode.LeftControl))
            moveDirection += Vector3.down;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        Vector3 p = transform.position;

        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.y = Mathf.Clamp(p.y, minY, maxY);
        p.z = Mathf.Clamp(p.z, minZ, maxZ);

        transform.position = p; 
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            // Új pozíció a zoom irányába
            Vector3 newPosition = transform.position + transform.forward * scroll * zoomSpeed;

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);


            transform.position = newPosition;
            
        }
    }
}
