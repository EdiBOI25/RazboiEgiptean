using UnityEngine;

namespace Camera
{
    public class CameraParallaxEffect : MonoBehaviour
    {
        [Header("Camera Settings")]
        public float moveSpeed = 5f; // Speed at which the camera moves
        public Vector2 moveLimitX = new Vector2(-10f, 10f); // X-axis movement limits
        public Vector2 moveLimitY = new Vector2(-5f, 5f); // Y-axis movement limits

        private Vector3 _targetPosition;
        private Vector3 _velocity = Vector3.zero;
        private bool _isWindowFocused = true;

        private void Update()
        {
            // Check if the window is focused
            _isWindowFocused = Application.isFocused;

            if (_isWindowFocused)
            {
                // Get the cursor's position in screen coordinates
                Vector3 cursorScreenPosition = Input.mousePosition;

                // Calculate the target position based on cursor position
                CalculateTargetPosition(cursorScreenPosition);
            }
            else
            {
                // Reset the target position to (0, 0) when the window is out of focus
                _targetPosition = new Vector3(0f, 0f, transform.position.z);
            }

            // Smoothly move the camera toward the target position
            MoveCamera();
        }

        private void CalculateTargetPosition(Vector3 cursorScreenPosition)
        {
            // Normalize the cursor position to a range of 0 to 1
            float normalizedX = cursorScreenPosition.x / Screen.width;
            float normalizedY = cursorScreenPosition.y / Screen.height;

            // Map the normalized position to the camera's movement limits
            float targetX = Mathf.Lerp(moveLimitX.x, moveLimitX.y, normalizedX);
            float targetY = Mathf.Lerp(moveLimitY.x, moveLimitY.y, normalizedY);

            // Set the target position
            _targetPosition = new Vector3(targetX, targetY, transform.position.z);
        }

        private void MoveCamera()
        {
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, 0.1f);
        }
    }
}
