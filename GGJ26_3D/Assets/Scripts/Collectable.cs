using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameObject player;
    public SphereCollider triggerCollider;
    private Vector3 originalRotation;
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockZ;
    [SerializeField] private GameBehaviors gameBehaviors;

    private void OnEnable()
    {
        originalRotation = transform.rotation.eulerAngles;
        gameBehaviors = FindFirstObjectByType<GameBehaviors>();
    }
    void Update()
    {
        transform.LookAt(player.transform);
        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX){rotation.x = originalRotation.x;}
        if (lockZ){rotation.z = originalRotation.z;}
        transform.rotation = Quaternion.Euler(rotation);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameBehaviors.maskScore += 1;
            Debug.Log("Player is within NPC trigger!");
        }
    }
}
