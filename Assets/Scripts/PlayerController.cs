using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance { set; get; }

	private LevelManager lvlMgr => LevelManager.Instance;		// Reference to our Level Manager.

	public bool isImmortal;										// To check if we are immortal.
	public float immortalTime = 1f;                             // Immortality length.
	private void Awake()
	{
		Instance = this;
	}

	// Check Collision
	private void OnCollisionEnter(Collision collision)
	{
		// Check if we are immortal.
		if (!isImmortal)
		{
			// Check if collided objects tag is matching.
			if (collision.collider.CompareTag("obstacle"))
			{
				// Call death function while storing killer object on variable to use it in future.
				Death(collision.gameObject);
			}

		}

		// Play bounce on wall collision.
		if (collision.collider.CompareTag("Wall")) {
			AudioManager.instance.Play("Bounce");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		// Check if we reached to end of the level.
		if (other.CompareTag("LevelEnd"))
		{

		}
	}

	private void OnTriggerExit(Collider other)
	{
		// Check if we exited level end object. If we do, change the level.
		if (other.CompareTag("LevelEnd"))
		{
			// Call the next level.
			StartCoroutine(lvlMgr.NextLevel());

			// Disable this object
			other.gameObject.SetActive(false);
		}
	}

	// To kill player
	public void Death(GameObject obstacle)
	{
		// Make sure time is not slowed down.
		TimeManager.Instance.time = true;

		// Make sure dots are cleared after death.
		GetComponent<DragAndShot>().EndLine();

		GetComponent<DragAndShot>().startPointScreen = Vector3.zero;
		// Play Crash Sound
		AudioManager.instance.Play("Crash");

		// Shake the camera.
		CameraManager.Instance.ShakeCamera();
		CameraManager.Instance.isMoving = false;

		// Explode player
		GameObject deathObject = ObjectPooler.Instance.SpawnFromPool("CubeDead", transform.position, Quaternion.identity);
		deathObject.GetComponent<CubeDead>().Explode();

		// Disable player
		gameObject.SetActive(false);
	}

	// To revive player
	public void RevivePlayer()
	{
		// We need to be sure that we are enabled our player before starting coroutines.
		gameObject.SetActive(true);
		// Call revive coroutine.
		StartCoroutine(Revive());
	}

	// To revive player
	public IEnumerator Revive()
	{
		// Make ourself immortal
		isImmortal = true;

		// Reset our velocity.
		GetComponent<Rigidbody>().velocity = Vector3.zero;

		// Reset our rotation
		transform.rotation = Quaternion.identity;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

		// Enable camera movement
		CameraManager.Instance.isMoving = true;

		// Wait for x seconds
		yield return new WaitForSecondsRealtime(immortalTime);

		// Make ourself mortal again
		isImmortal = false;

		yield return null;
	}
}
