using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_Menu : MonoBehaviour
{
    //To store the information once the game has started
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private TMP_Dropdown locationDropDown;
    [SerializeField] private TMP_Dropdown ageDropdown;
    [SerializeField] private GameObject errorMessage;

    //To Gather the Information Post Game
    private string scoreFile = "ScoreResultFileSave.txt";
    private int score;
    private string contaminationFile = "ContaminationResultFileSave.txt";
    private float contamination;
    private string nicknameFile = "NicknameResultFileSave.txt";
    private string nickname = "";
    private string ageFile = "AgeResultFileSave.txt";
    private int age;

    //To post the information post game.
    [SerializeField] private TMP_Text ageText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text contaminationText;
    [SerializeField] private TMP_Text finalScoreText;

    //for the Loading screen and toggling Set Actives
    public static bool resultScreen;
    [SerializeField] private GameObject resultMenu;
    [SerializeField] private GameObject letsgoMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject loadingSlider;
    [SerializeField] private Scrollbar loadingSliderValue;

    //For File Reading
    private string fileContents;

    // Start is called before the first frame update
    void Start()
    {
        //Ensuring the Result menu is Turned off by Default
        resultMenu.SetActive(false);
        //Will only execute if the Bool is Toggle once the game has ended
        if (resultScreen == true)
        {
            //To load the files and store them
            StreamReader scoreReader = new StreamReader(scoreFile);
            fileContents = scoreReader.ReadLine();
            score = int.Parse(fileContents);
            scoreReader.Close();
            Debug.Log(score);

            StreamReader contaminationReader = new StreamReader(contaminationFile);
            fileContents = contaminationReader.ReadLine();
            contamination = int.Parse(fileContents);
            contaminationReader.Close();
            Debug.Log(contamination);

            using (StreamReader nameReader = new StreamReader(nicknameFile))
            {
                nickname = nameReader.ReadToEnd();
            }
            Debug.Log(nickname);

            StreamReader ageReader = new StreamReader(ageFile);
            fileContents = ageReader.ReadLine();
            age = int.Parse(fileContents);
            ageReader.Close();
            Debug.Log(age);

            if (age == 1)
            {
                ageText.text = "1-8"; 
            }
            else if (age == 2)
            {
                ageText.text = "9-12";
            }
            else
            {
                ageText.text = "12+";
            }
            nameText.text = nickname;
            scoreText.text = score.ToString();
            resultMenu.SetActive (true);

            if(contamination >= 50 && contamination < 100)
            {
                score = (int)(score * 0.5);
                contaminationText.text = contamination.ToString() + "% = 50% score";
                finalScoreText.text = score.ToString() + "/ 300";
            }
            else if (contamination >= 100)
            {
                contaminationText.text = contamination.ToString() + "% = 0% score";
                finalScoreText.text = "0 / 300";
            }
            else
            {
                contaminationText.text = contamination.ToString() + "% = 100% score";
                finalScoreText.text = score.ToString() + "/ 300";
            }
        }
    }
    //Once the Let's Go Button is clicked, it will run this code
    public void OnLetsGoButtonClick()
    {
        // This will gather the name from the Input field
        string playerName = nicknameInputField.text;
        // This will gather the location from the DropDown box
        int playerLocation = locationDropDown.value;
        //This will gather the age from the DropDown box
        int playerAge = ageDropdown.value;

        // This will check if the Text Fields and DropDown boxes are not set to default
        if (!string.IsNullOrEmpty(nicknameInputField.text) && playerLocation != 0 && playerAge != 0)
        {

            // Create a StreamWriter object to write to the file
            StreamWriter nicknameResultFileSave = new StreamWriter("NicknameResultFileSave.txt");
            // Write the player's name to the file
            nicknameResultFileSave.WriteLine(playerName);
            // Close the StreamWriter object to save the file
            nicknameResultFileSave.Close();


            StreamWriter locationResultFileSave = new StreamWriter("LocationResultFileSave.txt");
            locationResultFileSave.WriteLine(playerLocation);

            locationResultFileSave.Close();

            StreamWriter ageResultFileSave = new StreamWriter("AgeResultFileSave.txt");
            ageResultFileSave.WriteLine(playerAge);
            ageResultFileSave.Close();

            Debug.Log("Saved!");

            //Will activate the Loading Screen and toggle items in the scene
            mainMenu.SetActive(false);
            letsgoMenu.SetActive(false);
            loadingSlider.SetActive(true);
            StartCoroutine(LoadLevelASync("L_Game"));
        }
        else
        {
            Debug.Log("Empty");
            errorMessage.SetActive(true);
        }
    }
    // This will Load the Level
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
