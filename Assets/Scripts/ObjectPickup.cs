using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupDistance = 3f;
    [SerializeField] private float holdDistance = 2f;
    [SerializeField] private float minHoldDistance = 1f;
    [SerializeField] private float maxHoldDistance = 5f;
    [SerializeField] private float scrollSpeed = 1f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private KeyCode rotateKey = KeyCode.R;

    [Header("Physics Settings")]
    [SerializeField] private float pickupForce = 150f;
    [SerializeField] private float throwForce = 10f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f;

    private Camera playerCamera;
    private GameObject heldObject;
    private Rigidbody heldObjectRigidbody;
    private float currentHoldDistance;
    private bool isRotating;
    private Vector3 lastMousePosition;

    void Start()
    {
        playerCamera = Camera.main;
        currentHoldDistance = holdDistance;
        Debug.Log("ObjectPickup script initialized. Camera found: " + (playerCamera != null));
    }

    void Update()
    {
        if (heldObject == null)
        {
            // Try to pick up object
            if (Input.GetKeyDown(pickupKey))
            {
                TryPickupObject();
            }

            // Highlight object when looking at it (optional)
            CheckForPickupable();
        }
        else
        {
            // Drop object
            if (Input.GetKeyDown(pickupKey))
            {
                DropObject(false);
            }

            // Throw object
            if (Input.GetMouseButtonDown(0))
            {
                DropObject(true);
            }

            // Adjust hold distance with mouse scroll
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                currentHoldDistance += scroll * scrollSpeed;
                currentHoldDistance = Mathf.Clamp(currentHoldDistance, minHoldDistance, maxHoldDistance);
            }

            // Rotation mode
            if (Input.GetKeyDown(rotateKey))
            {
                isRotating = !isRotating;
                if (isRotating)
                {
                    lastMousePosition = Input.mousePosition;
                }
            }

            // Move held object
            MoveObject();

            // Rotate object if in rotation mode
            if (isRotating)
            {
                RotateObject();
            }
        }
    }

    void FixedUpdate()
    {
        if (heldObject != null && heldObjectRigidbody != null)
        {
            // Calculate target position
            Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * currentHoldDistance;

            // Smoothly move object using physics
            Vector3 direction = targetPosition - heldObject.transform.position;
            heldObjectRigidbody.linearVelocity = direction * pickupForce * Time.fixedDeltaTime;
        }
    }

    private void TryPickupObject()
    {
        Debug.Log("Trying to pickup object...");
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.name + " with tag: " + hit.collider.tag);
            if (hit.collider.tag == "Moveable")
            {
                Debug.Log("Found Moveable object: " + hit.collider.name);
                PickupObject(hit.collider.gameObject);
            }
            else
            {
                Debug.Log("Object is not tagged as Moveable. Current tag: " + hit.collider.tag);
            }
        }
        else
        {
            Debug.Log("No object in range (distance: " + pickupDistance + "m)");
        }
    }

    private void CheckForPickupable()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupDistance))
        {
            if (hit.collider.tag == "Moveable")
            {
                // You can add UI feedback here (e.g., show "Press E to pick up")
                Debug.DrawLine(playerCamera.transform.position, hit.point, Color.green);
            }
        }
    }

    private void PickupObject(GameObject obj)
    {
        heldObject = obj;
        heldObjectRigidbody = obj.GetComponent<Rigidbody>();

        if (heldObjectRigidbody != null)
        {
            // Configure rigidbody for pickup
            heldObjectRigidbody.useGravity = false;
            heldObjectRigidbody.linearDamping = 10f;
            heldObjectRigidbody.angularDamping = 5f;
            heldObjectRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            Debug.Log("✓ Picked up: " + obj.name);
        }
        else
        {
            Debug.LogWarning("Object " + obj.name + " has no Rigidbody! Cannot pick up.");
            heldObject = null;
            return;
        }

        // Reset hold distance
        currentHoldDistance = holdDistance;
        isRotating = false;
    }

    private void DropObject(bool throw_object)
    {
        if (heldObject == null) return;

        string objectName = heldObject.name;

        if (heldObjectRigidbody != null)
        {
            // Reset rigidbody settings
            heldObjectRigidbody.useGravity = true;
            heldObjectRigidbody.linearDamping = 0f;
            heldObjectRigidbody.angularDamping = 0.05f;

            // Apply throw force if needed
            if (throw_object)
            {
                heldObjectRigidbody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.VelocityChange);
                Debug.Log("✓ Threw: " + objectName);
            }
            else
            {
                Debug.Log("✓ Dropped: " + objectName);
            }
        }

        heldObject = null;
        heldObjectRigidbody = null;
        isRotating = false;
    }

    private void MoveObject()
    {
        if (heldObject == null) return;

        // This is handled in FixedUpdate for physics-based movement
    }

    private void RotateObject()
    {
        if (heldObject == null) return;

        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

        // Rotate object based on mouse movement
        heldObject.transform.Rotate(playerCamera.transform.up, -mouseDelta.x * rotationSpeed * Time.deltaTime, Space.World);
        heldObject.transform.Rotate(playerCamera.transform.right, mouseDelta.y * rotationSpeed * Time.deltaTime, Space.World);

        lastMousePosition = Input.mousePosition;
    }

    void OnDrawGizmosSelected()
    {
        // Draw pickup distance in editor
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerCamera.transform.position, pickupDistance);
        }
    }
}
