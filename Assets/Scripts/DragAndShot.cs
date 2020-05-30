using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndShot : MonoBehaviour
{
    [SerializeField]
    private float power = 2f;                                           // Power of the shot.
    private Rigidbody rb => GetComponent<Rigidbody>();                  // Reference to our Rigidbody.

    public Vector2 minPower;                                            // Minimum power we can apply.
    public Vector2 maxPower;                                            // Maximum power we can apply.
    
    [SerializeField]
    private Camera cam = null;                                          // Reference to our camera.
    
    private Vector3 force;                                              // To store calculated force vector.
    public Vector3 startPointScreen;
    private Vector3 startPointWorld;                                    // To store start point of our touch/click.
    private Vector3 endPoint;                                           // To store end point of our touch/click.

    public bool isFirstStart = true;                                           // To check if we just started the game. We'll enable gravity after user does his first drag.

    private LineRenderer lr => GetComponent<LineRenderer>();            // Reference to our Line Renderer.

    void Update()
    {
        // These controls will be changed to touch controls instead of mouse controls. Touch controls gives better control
        // on mobile devices.

        // Check if we are using the touch / mouse controls.
        if (Input.GetMouseButtonDown(0))
        {
            // Get the starting point of the touch / click position based on camera distance
            startPointScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20);
            //startPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, 20));
            
            // Start zoom effect.
            CameraManager.Instance.zoom = true;
            // Slowdown time.
            TimeManager.Instance.time = false;
        }
        else if (Input.GetMouseButton(0))
        {
            // Update the starting point of the touch / click position based on camera distance
            startPointWorld = cam.ScreenToWorldPoint(startPointScreen);
            // Get the ending point of the touch / click position based on camera distance
            endPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20));

            // Render the trejectory line.
            RenderLine(new Vector3(startPointWorld.x, startPointWorld.y, 0), new Vector3(endPoint.x, endPoint.y, 0));

            // Continue zoom effect.
            CameraManager.Instance.zoom = true;
            // Slowdown time.
            TimeManager.Instance.DoSlowMotion();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Update the ending point of the touch / click position based on camera distance
            endPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20));

            // Calculate the force by finding the distance between starting point and ending point. Also we clamp the power
            // between maxPower and minPower.
            force = new Vector3(Mathf.Clamp(startPointWorld.x - endPoint.x, minPower.x, maxPower.x), Mathf.Clamp(startPointWorld.y - endPoint.y, minPower.y, maxPower.y), 0);
            // Check if player made a reasonable amount of dragging. If so zero out the velocity to easly override velocity.
            if (Vector3.Distance(startPointWorld, endPoint) > 0.5f)
            {
                // Make our velocity zezro.
                rb.velocity = Vector3.zero;
            }
            // Add calculated force multiplied power to easly tune it.
            rb.AddForce(force * power, ForceMode.VelocityChange);
            
            // Add random torque based on our force.
            Vector3 torqVector = force * power;
            // Create random value to give some rotation in Z axis
            torqVector.z = Random.Range(10f, 20f);
            // Add the torque force to our rigidbody
            rb.AddTorque(torqVector);

            if (isFirstStart)
            {
                isFirstStart = false;
                rb.useGravity = true;
            }

            // Stop zoom effect.
            CameraManager.Instance.zoom = false;
            // Normalize time.
            TimeManager.Instance.time = true;
            TimeManager.Instance.waitTime = 0.5f;

            // Clear the trejectory line.
            EndLine();
        }
    }

    // Renders 2 point line.
    private void RenderLine(Vector3 _startPoint, Vector3 _endPoint)
    {
        // Increase point count in the renderer.
        lr.positionCount = 2;
        // Create array of Vector3's to store line point positions.
        Vector3[] points = new Vector3[2];
        // Set the positions.
        points[0] = _startPoint;
        points[1] = _endPoint;
        lr.SetPositions(points);
    }

    public void EndLine()
    {
        // Zero out the list to clear line renderer points.
        lr.positionCount = 0;
    }
}
