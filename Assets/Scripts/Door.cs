using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    private Transform playerTransform;
    private bool isPlayerNearby = false;

    void Start()
    {
        // Find the player in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Check distance to player
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        isPlayerNearby = distance <= interactionDistance;

        // Load scene when player presses E key while nearby
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name not set on Door script!");
        }
    }

    // Optional: Draw interaction range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
