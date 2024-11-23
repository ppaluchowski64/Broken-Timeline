using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public RectTransform uiElement; // Reference to the UI element (e.g., the slider)
    public float heightOffset = 50f; // The offset height to make the UI element appear above the player

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Cache the main camera
    }

    void Update()
    {
        // Convert the player's world position to screen position
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(player.position);

        // Add an offset to the screen position to make the UI element appear above the player
        screenPosition.y += heightOffset; // Increase the y-coordinate to move it upwards

        // Set the UI element's position to the adjusted screen position
        uiElement.position = screenPosition;
    }
}
