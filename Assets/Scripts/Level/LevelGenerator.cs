using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// We are creating new class to be able tidy up the code structure a little more.
[System.Serializable]
public class ObjInfo
{
	public string objName;												// Name of the object. We will use this to call the object from pool.
	public float objSpawnY;												// Start of the spawn position.
	public float objLength;												// Length of the object.
	public int objAmount = 1;											// Amount of the objects.
	public List<GameObject> objList = new List<GameObject>();           // To store objects we spawned.
}

public class LevelGenerator : MonoBehaviour
{
	public static LevelGenerator Instance { set; get; }                 // To setup singleton.

	public Transform obstacleParent;									// To strore obstacle parent transform.

	public Transform playerTransform;                                   // To store current player's transform.
	public bool isStarted;                                              // To check if the game is started.

	public float safeZone;                                              // Area that we don't want to spawn anything.
	public float obstacleSafeZone;                                      // Safe zone for obstacles.
	private ObjectPooler oPool => ObjectPooler.Instance;                // Reference to our Object Pooler.
	private LevelManager lvlMgr => LevelManager.Instance;				// Reference to our Level Manager.

	public ObjInfo[] levelObjects;                                      // List of the objects that we'll be using during game
	public float obstacleSpawnY;

	public float levelEndPosition;										// This where we are going to spawn our level end object.

	private void Awake()
	{
		// For singleton.
		Instance = this;
	}

	// Update is called once per frame.
	void Update()
    {
		// Check if the game is started and we have the player referenced.
		if (isStarted && playerTransform != null)
		{
			BarController.Instance.UpdateValue(100 / levelEndPosition * Mathf.Abs(playerTransform.position.y));
			
			// Check if player is currently in no-generation zone.
			if (playerTransform.position.y > safeZone)
			{
				// Do the following for all the objects in LevelObjects array.
				for (int i = 0; i < levelObjects.Length; i++)
				{
					// Check if we need to spawn new object.
					// (Start of the object pos) - (Amount of the object) * (Object length).
					if (playerTransform.position.y > (levelObjects[i].objSpawnY - levelObjects[i].objAmount * levelObjects[i].objLength))
					{
						// Create a vector for our spawn pos. 
						Vector3 spawnPos = new Vector3(0, 1, 0) * levelObjects[i].objSpawnY;
						// Call the gameObject from pool.
						GameObject obj = oPool.SpawnFromPool(levelObjects[i].objName, spawnPos, Quaternion.identity);
						// Add the object in list to be able to use it in future.
						levelObjects[i].objList.Add(obj);
						// Increase the (Start of the object pos) by (Object Length) to prepare it for the next spawn.
						levelObjects[i].objSpawnY += levelObjects[i].objLength;
					}
				}
			}

			// Check if our player is above where we don't want to spawn obstacles.
			if (playerTransform.position.y > obstacleSafeZone)
			{
				// If we don't have something to spawn, return.
				if (lvlMgr.obstaclesToSpawn.Count == 0) { return; }

				// Check if we need to spawn new obstacle from the list.
				if (playerTransform.position.y > (obstacleSpawnY - 1 * lvlMgr.curLevel.obstacleProximity))
				{
					// Create a vector for our spawn pos. 
					Vector3 spawnPos = new Vector3(0, 1, 0) * obstacleSpawnY;
					// Call the obstacle from object pooler.
					GameObject obj = oPool.SpawnFromPool(lvlMgr.obstaclesToSpawn[0].obstacleName, spawnPos, Quaternion.identity);
					// Increase the (Start of the object pos) by (Object Length) to prepare it for the next spawn.
					obstacleSpawnY += lvlMgr.curLevel.obstacleProximity;
						
					// Do specific actions based on obstacle type.
					switch (lvlMgr.obstaclesToSpawn[0].ObstacleType)
					{
						case ObstacleType.Platform:
							// Create a random float according to what we defined in level creation.
							float randSpace = Random.Range(lvlMgr.curLevel.ObstacleProperties.PlatfromProperties.minSpaceSize, lvlMgr.curLevel.ObstacleProperties.PlatfromProperties.maxSpaceSize);
							// Update the space valueç
							obj.GetComponent<PlatformController>().spaceBetweenPlatforms = randSpace;
							// Update the space itself.
							obj.GetComponent<PlatformController>().CalculateSpace(randSpace);

							break;
						case ObstacleType.Standalone:
							// Offset obstacles position according to levels properties.
							obj.transform.position = transform.position + (Vector3)lvlMgr.curLevel.ObstacleProperties.StandaloneProperties.positionOffset;
							// Todo Scale the standalone obstacles

							break;
						case ObstacleType.Side:
							obj.transform.localScale = transform.localScale * lvlMgr.curLevel.ObstacleProperties.SideProperties.scale;
							break;
						case ObstacleType.Dynamic:
							break;
						case ObstacleType.LevelEnd:
							break;
						default:
							break;
					}
					// Remove the item we used from list.
					lvlMgr.obstaclesToSpawn.RemoveAt(0);
				}
				
			}
		}
	}

	public void ResetLevelGenerator()
	{
		// Reset environment generators.
		// Todo make it prettier :D

		levelObjects[0].objSpawnY = 40f;
		levelObjects[1].objSpawnY = 40f;
		obstacleSpawnY = 25f;

		// Disable all obsolete objects from last level.
		for (int i = 0; i < obstacleParent.transform.childCount; i++)
		{
			obstacleParent.transform.GetChild(i).gameObject.SetActive(false);
		}
	}
}
