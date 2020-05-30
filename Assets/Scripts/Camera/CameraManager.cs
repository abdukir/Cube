using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance { set; get; }          // Singleton; to access this script from everywhere.

	public Vector3 defaultCameraPos;                            // Default camera position.

	public float speed;											// Camera move speed.
	public float speedMultiple = .05f;							// Speed multiplier

	private Vector3 targetPosition;								// To store targets position.
	public Transform target;									// To store target.

	private float distance;										// To store distance between camera and the target.

	public float startLimmit = 7f;								// Amount of units we need to travel to start the camera move

	public bool zoom;                                           // To trigger zoom effect.
	public float zoomAmount = 1f;                               // Amount of the zoom that'll be applied.
	public float zoomSpeed = 1f;                                // Zoom speed.

	public bool isMoving = true;

	public bool isActive = true;                                // To check if the camera is active or not.

	public float reviveDistance;                                // Distance camera will take while reviving.

	public float shakeTime;                                     // To store how long we'll be shaking.

	private void Awake()
	{
		seed = Random.value;
		Instance = this;
	}

	void FixedUpdate()
	{
		// Check if our camera is active.
		if (isActive)
		{
			// Calculate distance between camera and target.
			distance = target.position.y - transform.position.y;

			// If we are zooming add X amount of zoom to our Z position. If not return it to it's default value.
			if (zoom)
			{
				// We can lerp to that position to give it better feeling.
				transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, defaultCameraPos.z + zoomAmount), zoomSpeed);
			}
			else
			{
				// Return it to original position.
				transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, defaultCameraPos.z), zoomSpeed);
			}

			// If our targets position is not bigger than what we set, don't go further in this script.
			if (target.position.y < startLimmit)
				return;
			
			// If our distance to target is bigger than 1 then start lerping to targets Y position.
			if (distance > 1)
			{
				// We are lerping with the speed of distance that decreases while we are lerping. So it'll look better.
				targetPosition = new Vector3(0, target.position.y, transform.position.z);
				// Set our position to lerped position.
				transform.position = Vector3.Lerp(transform.position, targetPosition, distance * Time.unscaledDeltaTime * 2);
			}
			else
			{
				// Check if we want to move the camera upwards.
				if (isMoving)
				{
					// If our distance is not bigger than what we set, keep adding (speed) to move our camera upwards. This speed can be changed for each level.
					targetPosition = new Vector3(0, transform.position.y + speed, transform.position.z);
					// Set our position to lerped position.
					transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime);
				}
			}
		}
	}

	public IEnumerator OnRevive(System.Action<Vector3> callback)
	{
		// Stop all other camera actions to start this event.
		isActive = false;
		isMoving = false;
		zoom = false;

		// Store the camera component to use it later.
		Camera cam = GetComponent<Camera>();

		// Calculate the position we want to move the camera.
		Vector3 movePos = new Vector3(0, transform.position.y - reviveDistance, 0);
		// Callback to send the vector3 value.
		callback(movePos);

		for (float t = 0.0f; t < 2; t += Time.deltaTime)
		{
			transform.position = Vector3.Lerp(transform.position, new Vector3(0, movePos.y, transform.position.z), t * speed * speedMultiple);
			yield return null;
		}
	}

	#region Camera Shake

	[SerializeField]
	Vector3 maximumAngularShake = Vector3.one * 15;
	[SerializeField]
	float frequency = 25;
	[SerializeField]
	float traumaExponent = 1;
	[SerializeField]
	float recoverySpeed = 1;
	private float trauma;
	private float seed;
	// Amount of the stress we are going to apply.
	public float stressAmount = 10f;


	public void ShakeCamera()
	{
		StartCoroutine(ShakeCameraCour());
	}
	private IEnumerator ShakeCameraCour()
	{
		trauma = Mathf.Clamp01(trauma + stressAmount);
		for (float t = 0.0f; t < shakeTime; t += Time.deltaTime)
		{
			// Taking trauma to an exponent allows the ability to smoothen
			// out the transition from shaking to being static.
			float shake = Mathf.Pow(trauma, traumaExponent);

			// This x value of each Perlin noise sample is fixed,
			// allowing a vertical strip of noise to be sampled by each dimension
			// of the translational and rotational shake.
			// PerlinNoise returns a value in the 0...1 range; this is transformed to
			// be in the -1...1 range to ensure the shake travels in all directions.

			transform.localRotation = Quaternion.Euler(new Vector3(
				maximumAngularShake.x * (Mathf.PerlinNoise(seed + 3, Time.time * frequency) * 2 - 1),
				maximumAngularShake.y * (Mathf.PerlinNoise(seed + 4, Time.time * frequency) * 2 - 1),
				maximumAngularShake.z * (Mathf.PerlinNoise(seed + 5, Time.time * frequency) * 2 - 1)
			) * shake);

			trauma = Mathf.Clamp01(trauma - recoverySpeed * Time.deltaTime);
			yield return null;
		}

	}
	#endregion
}
