using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideObstacleController : MonoBehaviour
{
    public bool isLeft;                                                     // To check which wall our obstacle will be on

    public float leftX;                                                     
    public float rightX;
    Transform child => transform.GetChild(0);

    private void OnEnable()
    {
        RandomiseObstaclePos();
    }

    [ContextMenu("Randomise Obstacle Position")]
    public void RandomiseObstaclePos()
    {
        // %50 chance for each side
        if (Random.Range(0, 2) == 1)
        {
            SetObstacle(leftX);
            // Flip the rotation accordingly
            child.localEulerAngles = new Vector3(child.localEulerAngles.x, -90, child.localEulerAngles.z);
            isLeft = true;
        }
        else
        {
            SetObstacle(rightX);
            // Flip the rotation accordingly
            child.localEulerAngles = new Vector3(child.localEulerAngles.x, 90, child.localEulerAngles.z);
            isLeft = false;
        }
    }

    // Sets the obstacles X position. Obstacle object should be child of this script.
    private void SetObstacle(float xPos)
    {
        // Change the childs X position.
        child.localPosition = new Vector3(xPos,0,0);

    }

}
