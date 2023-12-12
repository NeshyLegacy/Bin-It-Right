using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class S_VehicleMovement : MonoBehaviour
{
    //public S_Menu menu;
    public S_GameManager gameManager;

    public Transform[] waypoints;
    public float speed = 10f;
    private int currentWaypoint = 0;

    [SerializeField] private GameObject menuBackground;
    [SerializeField] private GameObject loadingSlider;
    [SerializeField] private Scrollbar loadingSliderValue;

    private void Update()
    {
        // If there are still waypoints to visit
        if (currentWaypoint < waypoints.Length) 
        {
            // Move towards the current waypoint
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, speed * Time.deltaTime);
            transform.LookAt(waypoints[currentWaypoint].position);
            // If the vehicle has reached the current waypoint
            if (transform.position == waypoints[currentWaypoint].position)
            {
                // Move to the next waypoint
                currentWaypoint++; 
            }
        }
        else
        {
            // Create a StreamWriter object to write to the file
            StreamWriter scoreResultFileSave = new StreamWriter("ScoreResultFileSave.txt");
            // Write the player's name to the file
            scoreResultFileSave.WriteLine(gameManager.score);
            // Close the StreamWriter object to save the file
            scoreResultFileSave.Close();

            // Create a StreamWriter object to write to the file
            StreamWriter contaminationResultFileSave = new StreamWriter("ContaminationResultFileSave.txt");
            // Write the player's name to the file
            contaminationResultFileSave.WriteLine(gameManager.contaminationSlider.value);
            // Close the StreamWriter object to save the file
            contaminationResultFileSave.Close();

            menuBackground.SetActive(true);
            loadingSlider.SetActive(true);
            StartCoroutine(LoadLevelASync("L_Menu"));

            S_Menu.resultScreen = true;
            Debug.Log("Game OVer");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (gameManager.garbagePickupRange == false)
        {
            gameManager.garbagePickupRange = true;

            gameManager.collectionDisplay.Play("A_CollectionDisplay");

            gameManager.collectFirst = Random.Range(0, 6);
            gameManager.collectSecond = Random.Range(0, 6);
            gameManager.collectThird = Random.Range(0, 6);

            gameManager.tickTop.SetActive(false);
            gameManager.tickMid.SetActive(false);
            gameManager.tickBottom.SetActive(false);
            gameManager.crossTop.SetActive(false);
            gameManager.crossMid.SetActive(false);
            gameManager.crossBottom.SetActive(false);

            gameManager.collectListTop[gameManager.collectFirst].SetActive(true);
            gameManager.collectListMid[gameManager.collectSecond].SetActive(true);
            gameManager.collectListBottom[gameManager.collectThird].SetActive(true);

            if (other.tag == "Garbage")
            {
                if (gameManager.age == 1)
                {
                    speed = 3f;
                }
                else if (gameManager.age == 2)
                {
                    speed = 5f;
                }
                else
                {
                    speed = 7f;
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(gameManager.garbagePickupRange == true)
        {
            gameManager.garbagePickupRange = false;
            gameManager.collectionDisplay.Play("A_CollectionDisplayReverse");

            gameManager.collectListTop[gameManager.collectFirst].SetActive(false);
            gameManager.collectListMid[gameManager.collectSecond].SetActive(false);
            gameManager.collectListBottom[gameManager.collectThird].SetActive(false);

            gameManager.score = gameManager.score - 20;
            gameManager.scoreText.text = gameManager.score.ToString() + "/300";
            Debug.Log("didn't answer in time!");

            if (gameManager.age == 1)
            {
                speed = 6f;
            }
            else if (gameManager.age == 2)
            {
                speed = 10f;
            }
            else
            {
                speed = 14f;
            }
        }

    }

    IEnumerator LoadLevelASync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSliderValue.value = progressValue;
            yield return null;

        }

    }
}
