using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private string level;
    public TextMeshProUGUI selectedLevelText;

    public void PlayGame()
    {
        SceneManager.LoadScene(level);
    }
    
    public void SetLevel(string toSet)
    {
        level = toSet;
    }

    // Start is called before the first frame update
    void Start()
    {
        level = "Level1";
        selectedLevelText.text = "Square Grid";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
