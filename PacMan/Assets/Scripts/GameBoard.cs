using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



/// <summary>
/// The game board class stores data relating to a level, such as the dimensions, total number of pellets, and game objects in the space.
/// </summary>
public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private int boardWidth = 28;
    [SerializeField]
    private int boardHeight = 36;
    public TextMeshProUGUI dimensionsText;

    /// <summary>
    /// The total number of dots or pellets in the maze, which PacMan will need to consume to beat the level.
    /// </summary>
    /// <value>
    /// Count of pellets in the maze.
    /// </value>
    public int totalDots;

    /// <summary>
    /// The player's score for the board, determined by the amount of consumables eaten.
    /// </summary>
    /// <value>
    /// The player's score.
    /// </value>
    private int score = 0;
    public TextMeshProUGUI scoreText;

    private double elapsedTime = 0f;
    public TextMeshProUGUI timerText;

    private int lives = 3;
    public TextMeshProUGUI livesText;
    private bool gameOver = false;

    public GameObject gameOverMenu;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI gameOverScoreText;

    /// An array referencing nodes within the game board.
    private List<GameObject> nodes = new List<GameObject> ();

    // An array referencing the pellets within the maze.
    private List<GameObject> dots = new List<GameObject> ();
    
    // A reference to the player
    private GameObject pacMan;

    // When the ghosts collide with pacman he is invincible for a moment, and can consume ghost if ate super pellet
    private bool pacManInvincible = false, pacManSuperPellet = false;
    private float stateChangeTimeDelay = 3f;

    // A reference to the ghost(s)
    private List<GameObject> ghosts = new List<GameObject> ();

    private const float collisionDistance = 1f;


    /// <summary>
    /// The Start() function is called before the first frame update and is useful for initialization.
    /// The function sets up the GameBoard class by finding objects like pellets and nodes in the maze and places the objects in an array.
    /// The total number of dots found in the maze is calculated and stored.
    /// </summary>
    /// <pre>
    /// The player has started a level and the game has initialized properly. The maze has been populated with its game objects and the dimensions of the board have been indicated.
    /// </pre>
    /// <post>
    /// The board array has been populated with pertinent game objects from the maze.
    /// </post>
    /// <post>
    /// The total number of pellets in the level has been set.
    /// </post>
    void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Dot");
        foreach (GameObject obj in objects) {
            if (obj != null) {
                dots.Add(obj);
            }
        }
        dots = dots.OrderBy(m => m.transform.position.x).ThenBy(m => m.transform.position.y).ToList();
        totalDots = dots.Count;
        score = 0;
        
        objects = GameObject.FindGameObjectsWithTag("Node");
        foreach (GameObject obj in objects) {
            if (obj != null) {
                nodes.Add(obj);
            }
        }
        nodes = nodes.OrderBy(m => m.transform.position.x).ThenBy(m => m.transform.position.y).ToList();

        pacMan = GameObject.FindGameObjectWithTag("PacMan");

        objects = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject obj in objects) {
            if (obj != null) {
                ghosts.Add(obj);
            }
        }

        dimensionsText.text = string.Format("{0} x {1}", boardWidth, boardHeight);
        gameOverMenu.SetActive(false);
        gameOver = false;
    }

    void Update()
    {
        if (!gameOver) {
            UpdateTimer();

            if (dots.Count > 0) {
                ConsumeAtPosition((Vector2)pacMan.transform.position);
            } else {
                GameOver(true);
            }

            foreach (GameObject ghost in ghosts) {
                if (((Vector2)(pacMan.transform.position - ghost.transform.position)).sqrMagnitude < collisionDistance) {
                    if (!pacManInvincible) {
                        LoseLife(ghost);
                        break;
                    }/* else if (pacManSuperPellet) {
                        ConsumeGhost(ghost);
                    }*/
                }
            }
        }
    }

    private void LoseLife(GameObject ghost)
    {
        if (lives <= 0) {
            GameOver(false);
        } else {
            lives -= 1;
            livesText.text = string.Format("{0}", lives);

            StartCoroutine(SetPacManState(true, false, stateChangeTimeDelay));
        }
    }

    private IEnumerator SetPacManState(bool invincible, bool superPellet, float waitTime)
    {
        pacManInvincible = invincible;
        pacManSuperPellet = superPellet;

        yield return new WaitForSeconds(waitTime);

        pacManInvincible = false;
        superPellet = false;
    }

    private void GameOver(bool wonGame)
    {
        Time.timeScale = 0f;
        gameOverMenu.SetActive(true);
        gameOver = true;

        if (!wonGame) {
            gameOverText.text = string.Format("Game Over\nYou lost!");
        } else {
            int scoreBonus = Mathf.RoundToInt((float)Math.Pow(boardWidth * boardHeight / elapsedTime, 2));
            int finalScore = score + scoreBonus;
            gameOverText.text = string.Format("Game Over\nCongrats!");
            gameOverScoreText.text = string.Format("Bonus = (width * height / time)^2\n= ({0} * {1} / {2})^2 = {3}\nscore = {4} + {3}\n= {5}", boardWidth, boardHeight, Mathf.RoundToInt((float)elapsedTime), scoreBonus, score, finalScore);
            score = finalScore;
        }
    }


    void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;
        TimeSpan t = TimeSpan.FromSeconds(elapsedTime);
        timerText.text = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
    }
    
    public void ConsumeGhost(GameObject ghost)
    {
        return;
    }

    /// <summary>
    /// Retrieves the pellet object at a particular position on the game board.
    /// </summary>
    /// <param>
    /// The position within the maze to search for a pellet.
    /// </param>
    /// <return>
    /// The relevant object if a pellet was found, otherwise null.
    /// </return>
    /// <pre>
    /// The GameBoard has initialized and the board was populated with the maze objects.
    /// </pre>
    /// <pre>
    /// The parameter is a valid position on the board.
    /// </pre>
    public bool ConsumeAtPosition(Vector2 pos)
    {
        GameObject obj = dots.FirstOrDefault(m => (pos - (Vector2)m.transform.position).sqrMagnitude < collisionDistance); // = dot[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)];

        if (obj != null) {
            Consumable pellet = obj.GetComponent<Consumable> ();

            if (pellet != null && !pellet.didConsume) {
                pellet.didConsume = true;

                if (pellet.isSuperPellet) {
                    StartCoroutine(SetPacManState(true, true, stateChangeTimeDelay));
                }

                obj.GetComponent<SpriteRenderer> ().enabled = false;
                
                score += pellet.pointsValue;
                scoreText.text = score.ToString();

                dots.Remove(obj);

                return true;
            }
        }

        return false;
    }
}
