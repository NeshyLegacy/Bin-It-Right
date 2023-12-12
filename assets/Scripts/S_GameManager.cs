using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class S_GameManager : MonoBehaviour
{
    //Getting Other scripts
    public S_VehicleMovement vehicleMovement;

    //A bool to check if the player is in range of the bins
    public bool garbagePickupRange = false;

    //To Gather the Information menu and Set the Difficulty
    private string ageFile = "AgeResultFileSave.txt";
    public int age;

    //This is the item list for the UI on the Top right
    public GameObject[] itemListLeft;
    public GameObject[] itemListMid;
    public GameObject[] itemListRight;

    //This is the items when approaching the bins for the player to collect
    public GameObject[] collectListTop;
    public GameObject[] collectListMid;
    public GameObject[] collectListBottom;

    //This is the Ticks and Cross if the Player makes the correct or incorrect choices
    public GameObject tickTop;
    public GameObject tickMid;
    public GameObject tickBottom;
    public GameObject crossTop;
    public GameObject crossMid;
    public GameObject crossBottom;

    //To play the animation when the collection menu pops up
    public Animator collectionDisplay;

    //Random rolls for the Items for the UI on the Top Right
    public int itemFirst;
    public int itemSecond;
    public int itemThird;
    public int itemFourth;
    public int itemFifth;
    public int itemSixth;

    //Random Rolls for the Collection Items when approaching the bins for the player to collect
    public int collectFirst;
    public int collectSecond;
    public int collectThird;

    //Score for the UI on the Top Left of the Game
    public TMP_Text scoreText;
    public int score;
    public GameObject scoreTick;

    //This is for the Contamination bar in the game
    public Slider contaminationSlider;
    public TMP_Text contaminationText;

    //If the player wants to get back to the menu
    public Button homeButton;
    public Button pausePlayButton;
    public GameObject pauseImage;
    public GameObject playImage;
    public GameObject pauseText;
    public GameObject pauseBackground;
    [SerializeField] private GameObject menuBackground;
    [SerializeField] private GameObject loadingSlider;
    [SerializeField] private Scrollbar loadingSliderValue; 

    private bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        //Resetting the Stats and score
        score = 0;
        scoreText.text = score.ToString() + "/300";
        contaminationSlider.minValue = 0;
        contaminationSlider.maxValue = 100;
        contaminationText.text = "0%";
        isPaused = false;
        scoreTick.SetActive(false);
        pauseText.SetActive(false);
        pauseBackground.SetActive(false);

        //Resetting the Tick and Cross
        tickTop.SetActive(false);
        tickMid.SetActive(false);
        tickBottom.SetActive(false);
        crossTop.SetActive(false);
        crossMid.SetActive(false);
        crossBottom.SetActive(false);

        //Adding a listening to the Home Button
        homeButton.onClick.AddListener(OnHomeButtonClick);
        pausePlayButton.onClick.AddListener(OnPauseButtonClick);

        //To load the files and store them in a variable
        StreamReader ageReader = new StreamReader(ageFile);
        string fileContents = ageReader.ReadLine();
        age = int.Parse(fileContents);
        ageReader.Close();
        Debug.Log(age);

        //Depending what age the player selected, will determine the difficulty
        if(age == 1)
        {
            vehicleMovement.speed = 6f;
        }
        else if (age == 2)
        {
            vehicleMovement.speed = 10f;
        }
        else
        {
            vehicleMovement.speed = 14f;
        }
        //RNG for the items on the Top right (If adding any new items for future additions)
        itemFirst = Random.Range(0, 3);
        itemSecond = Random.Range(0, 3);
        //Ensuring no duplicate Items
        while(itemSecond == itemFirst)
        {
            itemSecond = Random.Range(0, 3);
        }
        itemThird = Random.Range(0,3);
        //Ensuring no duplicate Items
        while (itemThird == itemFirst || itemThird == itemSecond)
        {
            itemThird = Random.Range(0, 3);
        }
        itemFourth = 3;
        itemFifth = 4;
        itemSixth = 5;
        //Diplay Items on Top left of UI
        itemListLeft[itemFirst].SetActive(true);
        itemListMid[itemSecond].SetActive(true);
        itemListRight[itemThird].SetActive(true);
        Debug.Log("Base: " + collectionDisplay.GetLayerIndex("TopBin")); //results in 0
    }

    // Update is called once per frame
    void Update()
    {
        //Execute code when player press Up or W / Down or S
        if (Input.GetKey("up") || Input.GetKey(KeyCode.W))
        {
            OnReject();
        }
        if (Input.GetKey("down") || Input.GetKey(KeyCode.S))
        {
            OnCollect();
        }
        // Checking to see if the Contamination score is under or equal to 100
        if(contaminationSlider.value >= 100)
        {
            vehicleMovement.speed = 0;
            // Create a StreamWriter object to write to the file
            StreamWriter scoreResultFileSave = new StreamWriter("ScoreResultFileSave.txt");
            // Write the player's name to the file
            scoreResultFileSave.WriteLine(score);
            // Close the StreamWriter object to save the file
            scoreResultFileSave.Close();

            // Create a StreamWriter object to write to the file
            StreamWriter contaminationResultFileSave = new StreamWriter("ContaminationResultFileSave.txt");
            // Write the player's name to the file
            contaminationResultFileSave.WriteLine(contaminationSlider.value);
            // Close the StreamWriter object to save the file
            contaminationResultFileSave.Close();
            S_Menu.resultScreen = true;
            menuBackground.SetActive(true);
            loadingSlider.SetActive(true);
            StartCoroutine(LoadLevelASync("L_Menu"));
            Debug.Log("Game OVer");
        }
    }

    public void OnCollect()
    {
        if(garbagePickupRange == true)
        {


            if (collectFirst == itemFirst || collectFirst == itemSecond || collectFirst == itemThird)
            {
                score = score + 10;
               tickTop.SetActive(true);
                Debug.Log("Top Accept Correct!");
                collectionDisplay.SetBool("TopTrigger", true);
            }
            else
            {
                score = score - 5;
                scoreText.text = score.ToString() + "/300";
                contaminationSlider.value = contaminationSlider.value + 10;
                contaminationText.text = contaminationSlider.value.ToString() + "%";
                crossTop.SetActive(true);
                Debug.Log("Top Accept Incorrect!");
            }

            if (collectSecond == itemFirst || collectSecond == itemSecond || collectSecond == itemThird)
            {
                score = score + 10;
                scoreText.text = score.ToString() + "/300";
                tickMid.SetActive(true);
                Debug.Log("Mid Accept Correct!");
                collectionDisplay.SetBool("MidTrigger", true);
            }
            else
            {
                score = score - 5;
                scoreText.text = score.ToString() + "/300";
                contaminationSlider.value = contaminationSlider.value + 10;
                contaminationText.text = contaminationSlider.value.ToString() + "%"; 
                crossMid.SetActive(true);
                Debug.Log("Mid Accept Incorrect!");
            }

            if (collectThird == itemFirst || collectThird == itemSecond || collectThird == itemThird)
            {
                score = score + 10;
                scoreText.text = score.ToString() + "/300";
                tickBottom.SetActive(true);
                Debug.Log("Bot Accept Correct!");
                collectionDisplay.SetBool("BotTrigger", true);
            }
            else
            {
                score = score - 5;
                scoreText.text = score.ToString() + "/300";
                contaminationSlider.value = contaminationSlider.value + 10;
                contaminationText.text = contaminationSlider.value.ToString() + "%";
                crossBottom.SetActive(true);  
                Debug.Log("Bot Accept Incorrect!");
            }

            if (age == 1)
            {
                vehicleMovement.speed = 6f;
            }
            else if (age == 2)
            {
                vehicleMovement.speed = 10f;
            }
            else
            {
                vehicleMovement.speed = 14f;
            }

            garbagePickupRange = false;


        }

        if (score > 300)
        {
            score = 300;
            scoreText.text = score.ToString() + "/300";
            scoreTick.SetActive(true);
        }
        else if (score == 300)
        {
            scoreTick.SetActive(true);
        }
        else if (score < 300)
        {
            scoreTick.SetActive(false);
        }
        collectionDisplay.Play("A_CollectionDisplayReverseCollect");
        StartCoroutine(TurnOff());

    }
    public void OnReject()
    {

        if (garbagePickupRange == true)
        {

            if (collectFirst == itemFourth || collectFirst == itemFifth || collectFirst == itemSixth)
            {
                score = score + 10;
                tickTop.SetActive(true);
                Debug.Log("Top Decline Correct!");
            }
            if (collectSecond == itemFourth || collectSecond == itemFifth || collectSecond == itemSixth)
            {
                score = score + 10;
                scoreText.text = score.ToString() + "/300";
                tickMid.SetActive(true);
                Debug.Log("Mid Decline Correct!");

            }
            if (collectThird == itemFourth || collectThird == itemFifth || collectThird == itemSixth)
            {
                score = score + 10;
                scoreText.text = score.ToString() + "/300";
                tickBottom.SetActive(true);
                Debug.Log("Bot Decline Correct!");

            }

            if (age == 1)
            {
                vehicleMovement.speed = 6f;
            }
            else if (age == 2)
            {
                vehicleMovement.speed = 10f;
            }
            else
            {
                vehicleMovement.speed = 14f;
            }
            garbagePickupRange = false;

        }
        if (score > 300)
        {
            score = 300;
            scoreText.text = score.ToString() + "/300";
            scoreTick.SetActive(true);
        }
        else if(score == 300)
        {
            scoreTick.SetActive(true);
        }
        else if(score < 300)
        {
            scoreTick.SetActive(false);
        }
        collectionDisplay.Play("A_CollectionDisplayReverseReject");
        StartCoroutine(TurnOff());

    }

    public void OnHomeButtonClick()
    {

        menuBackground.SetActive(true);
        loadingSlider.SetActive(true);
        StartCoroutine(LoadLevelASync("L_Menu"));

    }

    public void OnPauseButtonClick()
    {
        if(isPaused == false)
        {
            isPaused = true;
            Time.timeScale = 0;
            pauseImage.SetActive(false);
            playImage.SetActive(true);
            pauseText.SetActive(true);
            pauseBackground.SetActive(true);
        }
        else if (isPaused == true)
        {
            isPaused = false;
            Time.timeScale = 1;
            pauseImage.SetActive(true);
            playImage.SetActive(false);
            pauseText.SetActive(false);
            pauseBackground.SetActive(false);
        }
    }

    IEnumerator TurnOff()
    {

        yield return new WaitForSeconds(1.6f);

        collectListTop[collectFirst].SetActive(false);
        collectListMid[collectSecond].SetActive(false);
        collectListBottom[collectThird].SetActive(false);
        tickTop.SetActive(false);
        tickMid.SetActive(false);
        tickBottom.SetActive(false);
        crossTop.SetActive(false);
        crossMid.SetActive(false);
        crossBottom.SetActive(false);
        collectionDisplay.SetBool("TopTrigger", true);
        collectionDisplay.SetBool("TopTrigger", true);

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
