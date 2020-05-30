using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour {
	public static TimeManager Instance { set; get; }

	public float minTime = 0.01f;
	public float fixedUpdateMultiplier = 0.05f;
	
	public float lerpFactor = 0.9f;

	public bool time;

	public float deatTime = 0.02f;
	public float waitTime = 0.5f;
	public bool wait;

	private void Awake()
	{
		Application.targetFrameRate = 60;
		Instance = this;
	}

	void Update () {
		if (time)
		{
			Time.timeScale = 1;
			Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
			// normal camera zoom
		
			if (Time.timeScale == 1)
			{
				Time.fixedDeltaTime = 0.02f;
				time = false;
			}

		}
		if (Time.timeScale < deatTime)
		{
			wait = true;
			if (wait)
			{
				waitTime -= Time.unscaledDeltaTime;
				if (waitTime <= 0)
				{
					//PlayerController.Instance.Death(null);
					waitTime = 0.5f;
					wait = false;
				}
			}
		}
		
	}

	public void DoSlowMotion()
	{
		Time.timeScale = Mathf.Lerp(minTime, Time.timeScale, lerpFactor);
		Time.fixedDeltaTime = Time.timeScale * fixedUpdateMultiplier;
	}

}
