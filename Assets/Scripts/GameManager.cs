using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //Singleton instance
    
    private int score = 0; //Current score of the game

    const string DIR_DATA = "/Data/"; //Directory path for data
    const string FILE_HIGH_SCORE = "highScore.txt"; //File name for high score
    string PATH_HIGH_SCORE; //Full path for high score file

    public const string PREF_HIGH_SCORE = "hsScore"; //Player preferences key for high score

    public List<Cell> allCell = new List<Cell>(); //List of all the cells in the game

    public List<Cell> freeCell = new List<Cell>(); //List of all the empty cells in the game

    public GameObject itemPerfab; //Prefab for item

    // Generates a new item in a random empty cell
    public void CreatItem()
    {
        if (freeCell.Count > 0)
        {
            var index = UnityEngine.Random.Range(0, freeCell.Count);
            var go = GameObject.Instantiate(itemPerfab, freeCell[index].transform.position, Quaternion.identity, freeCell[index].transform);
            go.transform.parent.GetComponent<Cell>().curDrag = go.GetComponent<Drag>();
            go.transform.GetComponent<Drag>().ShowLevel();
            go.transform.SetSiblingIndex(1);
            freeCell.RemoveAt(index);
        }
    }

    // Adds a cell to the list of empty cells
    public void ReMoveCell(Cell cell)
    {
        freeCell.Add(cell);
    }

    // Getter and setter for score
    public int Score
    {
        get { return score; }
        set
        {
            score = value; 
            Debug.Log("THE SCORE CHANGED!!!");

            if (score > HighScore)
            {
                HighScore = score;
            }
        }
    }

    int highScore = 2; //Initial high score

    // Getter and setter for high score
    public int HighScore
    {
        get
        {
            return highScore;
        }
        set
        {
            highScore = value;
            Directory.CreateDirectory(Application.dataPath + DIR_DATA); //Creates the directory if it doesn't exist
            File.WriteAllText(PATH_HIGH_SCORE, "" + highScore); //Writes the high score to the file
        }
    }

    public Text text; //UI text object to display the score

    void Awake()
    {
        //Singleton instance check
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        freeCell = allCell;

        PATH_HIGH_SCORE = Application.dataPath + DIR_DATA + FILE_HIGH_SCORE; //Creates the full path for the high score file

        if (File.Exists(PATH_HIGH_SCORE)) //Checks if the high score file exists
        {
            HighScore = Int32.Parse(File.ReadAllText(PATH_HIGH_SCORE)); //Reads the high score from the file
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            File.Delete(PATH_HIGH_SCORE); //Deletes the high score file
        }

        text.text = "Score: " + score + "\n" + "High Score: " + HighScore; //Updates the score display UI
    }
}

