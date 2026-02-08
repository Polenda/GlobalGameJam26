using UnityEngine;
using System.Collections;

public class NPCBehavior : MonoBehaviour
{
    public GameObject player;
    public BoxCollider triggerCollider;
    private Vector3 originalRotation;
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockZ;
    public int hits = 1;
    public float walkAwaySpeed = 2f;
    public SpriteRenderer spriteRenderer;
    public Sprite deadSprite;
    [SerializeField] private bool DEAD = false;
    [SerializeField] private GameBehaviors gameBehaviors;
    private Rigidbody rb;

    private void Awake()
    {
        originalRotation = transform.rotation.eulerAngles;
        gameBehaviors = FindFirstObjectByType<GameBehaviors>();
    }

    void Update()
    {
        if (DEAD) return;
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distToPlayer < 3f)
        {
            // Move away from player, only on XZ plane
            Vector3 awayDir = transform.position - player.transform.position;
            awayDir.y = 0f;
            awayDir = awayDir.normalized;
            transform.position += awayDir * walkAwaySpeed * Time.deltaTime;
        }
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX){rotation.x = originalRotation.x;}
        if (lockZ){rotation.z = originalRotation.z;}
        transform.rotation = Quaternion.Euler(rotation);
        if (hits <= 0 && !DEAD)
        {
            StartCoroutine(DieSequence());
        }
    }


    private IEnumerator DieSequence()
    {
        DEAD = true;
        gameBehaviors.peasentScore += 1;
        if (spriteRenderer != null && deadSprite != null)
            spriteRenderer.sprite = deadSprite;
        // Disable all colliders
        foreach (var col in GetComponents<Collider>())
            col.enabled = false;
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;
        // Linear rotate to fall down and lower by 1 in Y
        float elapsed = 0f;
        float duration = 1f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(90, 0, 0));
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, -1f, 0);
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
    }
}


