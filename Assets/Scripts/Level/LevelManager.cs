using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable] public class Obstacle
{
	// Name of the obstacle to use it with Object Pooler.
	public string obstacleName;

	// Color of the object to add some style to our obstacles.
	public Color obstacleColor;

	// Type of the obstacle. We'll use this to create cases for each type.
	public ObstacleType ObstacleType;

	// Constructor.
	public Obstacle(string obstacleName, Color obstacleColor, ObstacleType obstacleType)
	{
		this.obstacleName = obstacleName;
		this.obstacleColor = obstacleColor;
		ObstacleType = obstacleType;
	}
}
[System.Serializable] public enum ObstacleType
{
	Platform,
	Standalone,
	Side,
	Dynamic,
	LevelEnd
}
[System.Serializable] public class Level
{
	// Lenght of the each level.
	public float levelLength;

	// Distance between each obstacle.
	[Range(8, 17)] public float obstacleProximity;

	// Speed of the level.
	[Range(2, 6)] public float levelSpeed;

	// Color of the obstacle for each level.
	public Color obstacleColor;

	// Color of the wall for each level.
	public Color wallColor;

	// List of obstacles.
	public Obstacle[] obstacles;

	// Properties specific to each obstacle.
	public ObstacleProperties ObstacleProperties;
}
[System.Serializable] public class ObstacleProperties
{
	public PlatfromProperties PlatfromProperties;
	public StandaloneProperties StandaloneProperties;
	public SideProperties SideProperties;
	public DynamicProperties DynamicProperties;
}					// Reference to all obstacle properties.
[System.Serializable] public class PlatfromProperties
{
	[Range(7, 10)] 
	public float minSpaceSize;
	[Range(7, 10)]
	public float maxSpaceSize;
}					// Properties for platform type obstacles.
[System.Serializable] public class StandaloneProperties
{
	public Vector2 positionOffset;
	public float scale;

	public StandaloneProperties(Vector2 positionOffset, float scale)
	{
		this.positionOffset = positionOffset;
		this.scale = scale;
	}
}					// Properties for standalone type obstacles.
[System.Serializable] public class SideProperties
{
	public float scale;

	public SideProperties(float scale)
	{
		this.scale = scale;
	}
}						// Properties for side type obstacles.
[System.Serializable] public class DynamicProperties
{
	public float speed;
}                     // Properties for dynamic type obstacles.

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance { set; get; }                       // Singleton
	public Level[] levels;													// Reference to our levels.
	private CameraManager cam => CameraManager.Instance;                    // Reference to our camera.
	private LevelGenerator lvlGen => LevelGenerator.Instance;               // Reference to our Level Generator.
	private GameManager GM => GameManager.Instance;							// Reference to our Game Manager

	public List<Obstacle> obstaclesToSpawn;									// To store current levels obstacles that we will be spawning.
	public Level curLevel;                                                  // To store current level.

	public GameObject player;                                               // Reference to our player.
	public UIController levelPassUI;                                        // Reference to our Level Pass UI.

	public TextMeshProUGUI curLevelText;
	private void Awake()
	{
		Instance = this;
	}

	[ContextMenu("Generate Level")]
	public void GenLevTest()
	{
		GenerateLevel(0);
	}

	public void GenerateLevel(int levelID)
	{
		// Get the level we want to work on.
		curLevel = levels[levelID];
		// Set our cameras speed to our levels speed.
		cam.speed = curLevel.levelSpeed;
		// Calculate obtacles we'll be spawning throughout the level. (Level Length) / (Obstacle Proximity)
		int obstacleCount = Mathf.CeilToInt(curLevel.levelLength / curLevel.obstacleProximity);
		// Store generated list of obstacle names to use it with object pooler.
		obstaclesToSpawn = GenerateObstacleList(curLevel.obstacles,obstacleCount,curLevel.obstacleColor);

		// Calculate level ending position for progress bar
		lvlGen.levelEndPosition = obstacleCount * curLevel.obstacleProximity + lvlGen.obstacleSpawnY;

		// Update the cur level text in the hud
		curLevelText.text = "Level " + (GM.curLevelID + 1);
	}

	private List<Obstacle> GenerateObstacleList(Obstacle[] obstacles, int _obstacleCount, Color _obstacleColor)
	{
		// Create empty list to store obstacle names.
		List<Obstacle> obsList = new List<Obstacle>();
		for (int i = 0; i < _obstacleCount; i++)
		{
			// In each run pick random obstacle from obstacle list that sent to this function. And store it in Obstacle object.
			int rand = Random.Range(0, obstacles.Length);
			Obstacle obs = new Obstacle(obstacles[rand].obstacleName, _obstacleColor,obstacles[rand].ObstacleType);
			// Add the name of the obstacle to list we've created before to return it.
			obsList.Add(obs);
		}
		// Create level end object
		Obstacle levelEnd = new Obstacle("LevelEnd", _obstacleColor, ObstacleType.LevelEnd);
		// Add the level end object to the obstacle list. 
		obsList.Add(levelEnd);
		
		// Return the list.
		return obsList;
	}

	[ContextMenu("Next Level Test")]
	public void NextLevelTest()
	{
		StartCoroutine(NextLevel());
	}

	public IEnumerator NextLevel()
	{
		// When we change the level we should move the player to start position. Play an animation that covers the screen.
		// Then generate next levels obstacle list. Then start the game.

		// Disable all player control & rigidbody.
		player.GetComponent<DragAndShot>().enabled = false;
		player.GetComponent<Rigidbody>().useGravity = false;

		// Disable Level Generation
		lvlGen.isStarted = false;

		// Disable Camera follow
		cam.isActive = false;
		cam.isMoving = false;

		// Create a Vector3 to store where we are going to move our player.
		Vector3 movePos = player.transform.position + new Vector3(0, 30, 0);
		// Make sure we move the player to center of the screen.
		movePos.x = 0f;

		// Create a Vector3 to store torque.
		Vector3 torqVector = Vector3.zero;
		// Create random value to give some rotation in Y axis
		torqVector.y = 40f;
		// Add the torque force to players rigidbody
		player.GetComponent<Rigidbody>().AddTorque(torqVector);

		for (float t = 0.0f; t < 2; t += Time.deltaTime)
		{
			player.transform.position = Vector3.Lerp(player.transform.position, movePos, t * 0.05f);
			yield return null;
		}

		// Stop the player when we are out of the screen.
		player.GetComponent<Rigidbody>().isKinematic = true;

		// Update the UI text.
		levelPassUI.GetComponent<Text>().text = "Level " + (GM.curLevelID + 1) + "\n Completed";
		// Show the next level UI here.
		levelPassUI.Show();

		// Move the camera to it's starting position.
		cam.transform.localPosition = new Vector3(0, 6, -20);
		// Move the player below screen.
		player.transform.position = new Vector3(0, -10, 0);
		// Reset players rotation.
		player.transform.rotation = Quaternion.identity;

		// Reset Level Generation values.
		lvlGen.isStarted = true;
		lvlGen.ResetLevelGenerator();

		// Wait 3 seconds then disable the UI
		yield return new WaitForSecondsRealtime(3);
		// Disable UI
		levelPassUI.Hide();

		// Move the player slowly to it's starting position.
		for (float t = 0.0f; t < 2; t += Time.deltaTime)
		{
			player.transform.position = Vector3.Lerp(player.transform.position, Vector3.zero, t * 0.05f);
			yield return null;
		}

		// Enable Camera follow.
		cam.isActive = true;
		cam.isMoving = true;

		// Enable the player control.
		player.GetComponent<DragAndShot>().enabled = true;
		player.GetComponent<DragAndShot>().isFirstStart = true;
		player.GetComponent<Rigidbody>().isKinematic = false;

		GM.curLevelID++;

		PlayerPrefs.SetInt("curLevel", GM.curLevelID);
		
		GenerateLevel(GM.curLevelID);
	}
}
