using UnityEngine;

using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class PLayerDamage : MonoBehaviour
{
    public GameObject health1;
    public GameObject health2;
    public GameObject health3;
    public int playerHealth = 3;
    [SerializeField] Image attackImage;
    private Coroutine attackImageFadeCoroutine;
    private bool attackLocked = false;
    public BoxCollider attackTrigger;

    public List<Component> enemiesInTrigger = new List<Component>();

    // UI for displaying scores
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject mainUI;
    [SerializeField] private TMPro.TextMeshProUGUI maskScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI peasentScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI knightScoreText;

    // Reference to GameBehaviors for scores
    [SerializeField] private GameBehaviors gameBehaviors;

    // Death state
    public bool isDead;

    void Start()
    {
        health1.SetActive(true);
        health2.SetActive(true);
        health3.SetActive(true);
        attackTrigger.enabled = true;
        deathScreen.SetActive(false);
        mainUI.SetActive(true);
    }

    void Update()
    {
        // Health UI
        switch (playerHealth)
        {
            case 3:
                health1.SetActive(true);
                health2.SetActive(true);
                health3.SetActive(true);
                break;
            case 2:
                health1.SetActive(true);
                health2.SetActive(true);
                health3.SetActive(false);
                Debug.Log(playerHealth);
                break;
            case 1:
                health1.SetActive(true);
                health2.SetActive(false);
                health3.SetActive(false);
                Debug.Log(playerHealth);
                break;
            case 0:
                health1.SetActive(false);
                health2.SetActive(false);
                health3.SetActive(false);
                playerDeath();
                break;
        }


        if (!attackLocked && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Mouse0)))
        {
            attackLocked = true;
            // Apply damage to all enemies currently in trigger
            foreach (var enemy in enemiesInTrigger)
            {
                var knight = enemy as KnightBehavior;
                if (knight != null)
                {
                    knight.hits--;
                }
                var npc = enemy as NPCBehavior;
                if (npc != null)
                {
                    npc.hits--;
                }
            }

            // Show and fade attack image
            if (attackImage != null)
            {
                if (attackImageFadeCoroutine != null)
                {
                    StopCoroutine(attackImageFadeCoroutine);
                }
                attackImage.enabled = true;
                var color = attackImage.color;
                color.a = 1f;
                attackImage.color = color;
                attackImageFadeCoroutine = StartCoroutine(FadeAttackImage(0.5f));
            }

            StartCoroutine(AttackLockout(0.5f));
        }
    }
    
    // Removed ActivateAttackTrigger; not needed
    private IEnumerator AttackLockout(float duration)
    {
        yield return new WaitForSeconds(duration);
        attackLocked = false;
    }

    private IEnumerator FadeAttackImage(float duration)
    {
        float elapsed = 0f;
        Color color = attackImage.color;
        color.a = 1f;
        attackImage.color = color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            attackImage.color = color;
            yield return null;
        }
        color.a = 0f;
        attackImage.color = color;
        attackImage.enabled = false;
        attackImageFadeCoroutine = null;
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered", other);
        if (attackTrigger != null && other != null && other != attackTrigger)
        {
            var knight = other.GetComponent<KnightBehavior>();
            if (knight != null && !enemiesInTrigger.Contains(knight))
            {
                enemiesInTrigger.Add(knight);
            }
            var npc = other.GetComponent<NPCBehavior>();
            if (npc != null && !enemiesInTrigger.Contains(npc))
            {
                enemiesInTrigger.Add(npc);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (attackTrigger != null && other != null && other != attackTrigger)
        {
            var knight = other.GetComponent<KnightBehavior>();
            if (knight != null && enemiesInTrigger.Contains(knight))
            {
                enemiesInTrigger.Remove(knight);
            }
            var npc = other.GetComponent<NPCBehavior>();
            if (npc != null && enemiesInTrigger.Contains(npc))
            {
                enemiesInTrigger.Remove(npc);
            }
        }
    }

    void playerDeath()
    {
        if (isDead) return;
        isDead = true;
        deathScreen.SetActive(true);
        mainUI.SetActive(false);
        // Set score UI
        if (gameBehaviors != null)
        {
            if (maskScoreText != null)
                maskScoreText.text = "Masks: " + gameBehaviors.maskScore.ToString();
            if (peasentScoreText != null)
                peasentScoreText.text = "Peasants: " + gameBehaviors.peasentScore.ToString();
            if (knightScoreText != null)
                knightScoreText.text = "Knights: " + gameBehaviors.knightScore.ToString();

            // Save scores to PlayerPrefs for transfer to main menu
            PlayerPrefs.SetInt("MaskScore", gameBehaviors.maskScore);
            PlayerPrefs.SetInt("PeasentScore", gameBehaviors.peasentScore);
            PlayerPrefs.SetInt("KnightScore", gameBehaviors.knightScore);
            PlayerPrefs.Save();
        }

        // Stop all player controls (disable this script)
        enabled = false;
    }
}
