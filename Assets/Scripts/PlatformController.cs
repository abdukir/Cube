using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformController : MonoBehaviour
{
	public Transform platform_L;							// Left platform
	public Transform platform_R;                            // Right platform
	public Transform parentPlatfrom;                        // Transform of parent platform

	[Range(7,10)]
	public float spaceBetweenPlatforms;                     // Space between two platforms
	[Range(-3.8f, 3.8f)]
	public float spacePosition;                             // Position of space on screen

	private void OnEnable()
	{
		// This will be setted from level generator.
		//spaceBetweenPlatforms = Random.Range(7, 10);
		spacePosition = Random.Range(-3.8f, 3.8f);
		// Change the platform according to values.
		CalculatePosition(spacePosition);
	}
	// Calculates space between platforms and updates it accordingly.
	public void CalculateSpace(float space)
	{
		platform_L.localPosition = new Vector3(-space, 0, 0);
		platform_R.localPosition = new Vector3(space, 0, 0);
	}

	// Calculates platforms position.
	public void CalculatePosition(float pos)
	{
		parentPlatfrom.localPosition = new Vector3(pos, 0, 0);
	}
}
