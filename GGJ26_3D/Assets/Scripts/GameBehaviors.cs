using TMPro;
using UnityEngine;

public class GameBehaviors : MonoBehaviour
{
    public int maskScore;
    public int peasentScore;
    public int knightScore;
    public TextMeshProUGUI maskScoreText;
    private int LastScore = 0;
    public PLayerDamage playerDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Only update mask score if this is the main menu scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (maskScoreText != null)
            {
                int maskScore = PlayerPrefs.GetInt("MaskScore", 0);
                maskScoreText.text = maskScore.ToString();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (maskScore > LastScore)
        {
            LastScore = maskScore;
            maskScoreText.text = maskScore.ToString();
        }

        if (playerDamage != null && playerDamage.isDead)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                // Example: Go to main menu or perform desired action
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
    }
}
