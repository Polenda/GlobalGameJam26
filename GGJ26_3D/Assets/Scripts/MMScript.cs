using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

// Fade in a black image over everything, then fade out
public class MMScript : MonoBehaviour
{

    [SerializeField] UnityEngine.UI.Image blackImage;
    [SerializeField] GameObject Cavas;
    [SerializeField] GameObject blackCavas;
    [SerializeField] TextMeshProUGUI Narrative;
    [SerializeField] float duration = 1f;
    [SerializeField] TextMeshProUGUI scoreText;
    // Public method for UI button to change scene by index

    private bool listen = false;

    void Start()
    {
        Cavas.SetActive(true);
        blackCavas.SetActive(false);
        Narrative.enabled = false;
        listen = false;


        scoreText.text = "Previous Best Score: \n\n" + PlayerPrefs.GetInt("MaskScore", 0).ToString(); 

    }
    public void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void FadeBlackImage()
    {
        blackCavas.SetActive(true);
        StartCoroutine(FadeBlackImageRoutine(blackImage, duration));
    }

    private IEnumerator FadeBlackImageRoutine(UnityEngine.UI.Image blackImage, float duration)
    {
        if (blackImage == null) yield break;
        blackImage.gameObject.SetActive(true);
        Color c = blackImage.color;
        c.a = 0f;
        blackImage.color = c;
        float t = 0f;
        // Fade in
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / duration);
            blackImage.color = c;
            yield return null;
        }
        c.a = 1f;
        blackImage.color = c;
        // Optionally wait before fading out
        Cavas.SetActive(false);  
        blackImage.gameObject.SetActive(false);     
        Narrative.enabled = true;
        listen = true;

    }
    void Update()
    {
        if (listen && Narrative.enabled && Input.anyKeyDown)
        {
            ChangeSceneByIndex(1);
        }
    }
}
