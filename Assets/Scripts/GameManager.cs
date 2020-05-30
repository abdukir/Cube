using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }                                    // Singleton
    private CameraManager cam => CameraManager.Instance;                                // Reference to our camera
    private PlayerController playerController => PlayerController.Instance;             // Reference to our Player Controller
    public GameObject player;                                                           // Reference to our Player

    public int curLevelID;                                                              // To store Current level.

    public Transform[] startEnvironment;                                               // Array of the objects that visible at the start of the game. 0 and 1 is walls, 2 is death area.

    private void Awake()
    {
        // To make sure we have only one instance of this script running
        Instance = this;

        curLevelID = PlayerPrefs.GetInt("curLevel");
    }

    // To start our game
    [ContextMenu("Start Game")]
    public void StartGame()
    {
        StartCoroutine(StartGameCourotine());
    }

    private IEnumerator StartGameCourotine()
    {
        // LevelGenerator Setup
        LevelGenerator.Instance.isStarted = true;

        // Camera Setup
        cam.isActive = true;
        cam.isMoving = true;

        // Environment Setup
        for (float t = 0.0f; t < 1; t += Time.deltaTime)
        {
            // Move the left wall to it's starting position
            startEnvironment[0].localPosition = Vector3.Lerp(startEnvironment[0].localPosition, new Vector3(-6.65f, 0, 0), t * 0.2f);
            // Move the right wall to it's starting position
            startEnvironment[1].localPosition = Vector3.Lerp(startEnvironment[1].localPosition, new Vector3(6.65f, 0, 0), t * 0.2f);

            // Move the death area to it's starting position
            startEnvironment[2].position = Vector3.Lerp(startEnvironment[2].position, new Vector3(0, -5, 0), t * 0.2f);

            // Move the background to it's starting position
            startEnvironment[3].localScale = Vector3.Lerp(startEnvironment[3].localScale, new Vector3(1, 1, 1), t * 0.2f);

            yield return null;
        }
        DeathArea.Instance.isFollowing = true;

        // Player Setup
        player.transform.position = new Vector3(0, -10f, 0);
        player.GetComponent<BoxCollider>().enabled = false;

        player.GetComponent<DragAndShot>().enabled = false;

        player.SetActive(true);
        // We can put enter animation here instead of directly enabling it.
        for (float t = 0.0f; t < 1; t += Time.deltaTime)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, Vector3.zero, t * 0.2f);
            yield return null;
        }
        player.GetComponent<BoxCollider>().enabled = true;
        player.GetComponent<DragAndShot>().enabled = true;
        
        LevelManager.Instance.GenerateLevel(curLevelID);

    }

    public void EndGame()
    {
        // Camera Setup
        cam.isActive = false;
        cam.isMoving = false;

        // Player Setup
        player.SetActive(false);
    }

    [ContextMenu("Delete Saves")]
    public void DeleteSaves()
    {
        PlayerPrefs.DeleteAll();
        curLevelID = PlayerPrefs.GetInt("curLevel");
        PlayerPrefs.SetFloat("volume", 1f);
    }
}
