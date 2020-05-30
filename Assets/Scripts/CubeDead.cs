using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDead : MonoBehaviour {

	public GameObject[] cubes;												// Array of the child cubes.
	public float explosionForce;											// Force that'll be applied to little cubes when death comes.....

	private List<Vector3> cubePos = new List<Vector3>();					// List to store starting position of the cubes.

	public float reviveTime = 1f;											// Time between two points of lerping.
	public float reviveSpeed = 1f;											// Speed of the lerp.

	// When object is enabled
	private void OnEnable()
	{
		for (int i = 0; i < cubes.Length; i++)
		{
			// Populate our list with start position values of our cubes.
			cubePos.Add(cubes[i].transform.localPosition);
		}
	}


	// When object is disabled
	private void OnDisable()
	{
		// This ection will be used when object pooling added.
	}

	[ContextMenu("Explode")]
	public void Explode()
	{
		for (int i = 0; i < cubes.Length; i++)
		{
			// Enable all the little cubes.
			cubes[i].SetActive(true);
			// Re-enable their collider because we will disable their collider while using revive effect to avoid collision between them.
			cubes[i].GetComponent<BoxCollider>().enabled = true;
			// Also disable their kinematic feature. This will allow it's rigidbody to work again.
			cubes[i].GetComponent<Rigidbody>().isKinematic = false;
			// Add explosion force to every object.
			cubes[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, 1f);

		}
		// Invoke StopCubes function to stop cubes. This will be changed to a better solution.
		Invoke("StopCubes", 3);
	}
	
	public void StopCubes()
	{
		for (int i = 0; i < cubes.Length; i++)
		{
			// Disable all cubes collider to avoid collision between them while reviving.
			cubes[i].GetComponent<BoxCollider>().enabled = false;
			// Also make them kinematic to stop them.
			cubes[i].GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	private void Update()
	{
		// To test revive effect
		if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.Escape))
		{
			Revive();
		}
	}

	
	[ContextMenu("revive")] // To test it from inspector
	public void Revive()
	{
		// Call the coroutine and get the callback value from it. Use that value to position our objects.
		StartCoroutine(CameraManager.Instance.OnRevive((returnedPos) => {
			PlayerController.Instance.transform.position = returnedPos;
			transform.position = returnedPos;
		}));
		StartCoroutine(Revive_Coroutine());
	}

	// We need Coroutines to use time while we don't want to use Update callback.
	private IEnumerator Revive_Coroutine()
	{
		// Start zoom effect
		CameraManager.Instance.zoom = true;
		// Start a timer within for loop to save some delicious resources
		for (float t = 0.0f; t < reviveTime; t += Time.deltaTime)
		{
			for (int i = 0; i < cubes.Length; i++)
			{
				// Lerp every cubes position to their start position which we saved at OnEnable callback
				cubes[i].transform.localPosition = Vector3.Lerp(cubes[i].transform.localPosition, cubePos[i], t * reviveSpeed);
				// Reset every cubes rotation to their start rotation.
				cubes[i].transform.rotation = Quaternion.Lerp(cubes[i].transform.rotation, Quaternion.identity, t * reviveSpeed);
			}
			
			yield return null;
		}

		// Disable this object.
		gameObject.SetActive(false);
		// Finish zoom effect
		CameraManager.Instance.zoom = false;
		// Enable camera follow.
		CameraManager.Instance.isActive = true;
		CameraManager.Instance.isMoving = true;

		// Call players revive function to start immortality and other things.
		PlayerController.Instance.RevivePlayer();
	}


}
