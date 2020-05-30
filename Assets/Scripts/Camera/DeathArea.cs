using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathArea : MonoBehaviour
{
	public static DeathArea Instance { set; get; }

	// Reference to our camera
	public Transform cam;

	// Offset between camera and death area.
	public float followOffset = 11f;

	public bool isFollowing;

	private void Awake()
	{
		Instance = this;
	}

	private void LateUpdate()
	{
		// Follow the camera with offset.
		if (isFollowing)
		{
			transform.position = new Vector3(transform.position.x,cam.position.y - followOffset, transform.position.z);
		}
	}
}
