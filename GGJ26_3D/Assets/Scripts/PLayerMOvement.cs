using UnityEngine;

public class PLayerMOvement : MonoBehaviour
{
	public float moveSpeed = 5f;
	public float jumpForce = 6f;
	private Rigidbody rb;

	public bool isGrounded = false;

	[Header("Camera Follow")]
	public Transform cameraTransform;
	public float mouseSensitivity = 2f;
	private float xRotation = 0f;

	// Camera shake variables
	private float shakeTimer = 0f;
	private float shakeTimeRemaining = 0f;
	private Vector3 originalCamLocalPos;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		if (cameraTransform != null)
			originalCamLocalPos = cameraTransform.localPosition;
	}

	private void Update()
	{
		HandleMouseLook();
		HandleJump();
		HandleCameraShake();
		// ...existing code...
	}
		private void HandleCameraShake()
		{
			if (cameraTransform == null)
				return;

			// Only shake if moving (WASD)
			bool isWalking = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f;

			if (!isWalking)
			{
				// Reset camera position if not walking
				if (originalCamLocalPos != Vector3.zero)
					cameraTransform.localPosition = originalCamLocalPos;
				shakeTimer = 0f;
				shakeTimeRemaining = 0f;
				return;
			}

			shakeTimer += Time.deltaTime;
			if (shakeTimer >= 0.8f) // less frequent shake
			{
				shakeTimer = 0f;
				shakeTimeRemaining = 0.15f; // slightly longer shake
				if (originalCamLocalPos == Vector3.zero)
					originalCamLocalPos = cameraTransform.localPosition;
			}

			if (shakeTimeRemaining > 0f)
			{
				shakeTimeRemaining -= Time.deltaTime;
				Vector3 shakeOffset = Random.insideUnitSphere * 0.15f; // more noticeable shake
				cameraTransform.localPosition = originalCamLocalPos + shakeOffset;
				if (shakeTimeRemaining <= 0f)
				{
					cameraTransform.localPosition = originalCamLocalPos;
				}
			}
		}
	private void HandleJump()
	{
		if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
		{
			rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
		}
	}

		private void FixedUpdate()
		{
			float moveX = Input.GetAxisRaw("Horizontal");
			float moveZ = Input.GetAxisRaw("Vertical");
			Vector3 move = Vector3.zero;
			if (cameraTransform != null)
			{
				// Move relative to camera's forward and right
				Vector3 camForward = cameraTransform.forward;
				Vector3 camRight = cameraTransform.right;
				camForward.y = 0f;
				camRight.y = 0f;
				camForward.Normalize();
				camRight.Normalize();
				move = (camForward * moveZ + camRight * moveX).normalized * moveSpeed;
			}
			else
			{
				move = new Vector3(moveX, 0f, moveZ).normalized * moveSpeed;
			}
			Vector3 velocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
			rb.linearVelocity = velocity;
		}

		private void HandleMouseLook()
		{
			float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
			float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

			xRotation -= mouseY;
			xRotation = Mathf.Clamp(xRotation, -80f, 80f);

			// Rotate camera up/down (pitch)
			if (cameraTransform != null)
			{
				cameraTransform.localEulerAngles = new Vector3(xRotation, 0f, 0f);
			}
			// Rotate player left/right (yaw)
			transform.Rotate(Vector3.up * mouseX);
		}


	private void OnTriggerStay(Collider other)
	{
		isGrounded = true;
	}

	private void OnTriggerExit(Collider other)
	{
		isGrounded = false;
	}
}
