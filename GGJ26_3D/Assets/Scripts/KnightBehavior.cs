using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KnightBehavior : MonoBehaviour
{
    public GameObject attackerPrefab;
    public GameObject player;
    public float moveSpeed;
    public Image spriteRenderer;
    public Sprite regularSprite;
    public Sprite attackSprite;
    public Sprite deadSprite;
    public BoxCollider triggerCollider;
    [SerializeField] private GameObject attackEffect;

    private bool canAttack = true;
    public int hits = 2;
    [SerializeField] private bool DEAD = false;
    [SerializeField] private GameBehaviors gameBehaviors;

    private Vector3 originalRotation;
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockZ;

    [SerializeField] private GameObject Collectable;

    private void Awake()
    {
        originalRotation = transform.rotation.eulerAngles;
        gameBehaviors = FindFirstObjectByType<GameBehaviors>();
    }

    void FixedUpdate()
    {
        if (DEAD) return;
        if (player != null)
        {
            Vector3 toPlayer = player.transform.position - transform.position;
            float dist = toPlayer.magnitude;
            
            if (dist > 2f)
            {
                Vector3 direction = toPlayer.normalized;
                direction.y = 0f;
                direction = direction.normalized;
                transform.position += direction * moveSpeed * Time.fixedDeltaTime;
            }
            transform.LookAt(player.transform);
            Vector3 rotation = transform.rotation.eulerAngles;
            if (lockX) { rotation.x = originalRotation.x; }
            if (lockZ) { rotation.z = originalRotation.z; }
            transform.rotation = Quaternion.Euler(rotation);
        }
        if (hits <= 0 && !DEAD)
        {
            StartCoroutine(DieSequence());
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (DEAD) return;
        if (!canAttack) return;
        if (other.transform == player.transform)
        {
            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        canAttack = false;
        Debug.Log("ATTACK");
        spriteRenderer.sprite = attackSprite;
        yield return new WaitForSeconds(1f);
        attackEffect.SetActive(true);
        spriteRenderer.sprite = regularSprite;
        // Only apply damage if player is still in the trigger
        bool playerInTrigger = false;
        foreach (var col in Physics.OverlapBox(triggerCollider.bounds.center, triggerCollider.bounds.extents, triggerCollider.transform.rotation))
        {
            if (col.transform == player.transform)
            {
                playerInTrigger = true;
                break;
            }
        }
        if (playerInTrigger)
        {
            player.GetComponentInChildren<PLayerDamage>().playerHealth -= 1;
            Debug.Log("Player Hit!");
        }
        yield return new WaitForSeconds(0.5f);
        attackEffect.SetActive(false);
        yield return new WaitForSeconds(2f);
        canAttack = true;
    }

    private IEnumerator DieSequence()
    {
        DEAD = true;
        
        gameBehaviors.knightScore += 1;
        if (spriteRenderer != null && deadSprite != null)
            spriteRenderer.sprite = deadSprite;
        // Disable all colliders
        foreach (var col in GetComponents<Collider>())
            col.enabled = false;
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;
        // Disable Rigidbody if present
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        // Linear rotate to fall down and lower by 1.5 in Y
        float elapsed = 0f;
        float duration = 1f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(90, 0, 0));
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, -1.45f, 0);
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRot;
        transform.position = endPos;
        // Generate random number 1-10
            // Spawn attacker prefab at knight position
        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;
        Instantiate(Collectable, spawnPos, Quaternion.identity);
        Debug.Log("Spawned face");

    }
}
