using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	private Rigidbody m_Rb;
	private Collider m_Col;

	public float m_GroundSpeed = -0.4f;
	public float m_AirSpeed = -0.2f;
	public float m_JumpForce = 80.0f;

	public float m_TerminalVelocity = 2.0f;

	private const float EPSILON_DIST = 0.1f;
	private const float EPSILON_OMEGA = 0.02f;
	private const float QUARTER_ROTATION = 90.0f;

	private Vector3 m_CurrGroundDir;

	private bool m_RotationControlsPressed = false;

	private bool m_AreControlsSuspended = false;

	void Start()
	{  
		m_Rb = GetComponent<Rigidbody>();  
		m_Col = GetComponent<Collider>();
		m_CurrGroundDir = Vector3.down;
	}

	bool IsGrounded(Vector3 groundDir) 
	{
		float distToGround = Mathf.Abs(Vector3.Dot(m_Col.bounds.extents, groundDir));
		return Physics.Raycast(transform.position, groundDir, distToGround + EPSILON_DIST);
	}

	void CorrectOrientation()
	{
		Vector3 currRot = transform.eulerAngles;
		float rotX = Mathf.Round(currRot.x / QUARTER_ROTATION) * QUARTER_ROTATION;
		float rotY = Mathf.Round(currRot.y / QUARTER_ROTATION) * QUARTER_ROTATION;
		float rotZ = Mathf.Round(currRot.z / QUARTER_ROTATION) * QUARTER_ROTATION;

		transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
	}

	Vector3 GetGroundDirection(Vector3 defaultDir)
	{
		Vector3[] dirs = {
				Vector3.up,
				Vector3.down,
				Vector3.right,
				Vector3.left,
				Vector3.forward,
				Vector3.back
		};

		int layerMask = 1 << 6; // Only hit the 6th "The Cube" layer
		foreach (Vector3 dir in dirs) {
			if (Physics.Raycast(transform.position, dir, Mathf.Infinity, layerMask)) {
				return dir;
			}
		}

		return defaultDir;
	}

	void HandleControls() 
	{
		// check if grounded
		bool isGrounded = IsGrounded(m_CurrGroundDir);
		float speed = isGrounded ? m_GroundSpeed : m_AirSpeed;

		// handle jump
		if (Input.GetButton("Jump") && isGrounded) {
			m_Rb.AddForce(transform.up * m_JumpForce, ForceMode.Impulse);
		}

		// handle rotation
		float rotationInput = -Input.GetAxisRaw("Vertical");
		if (rotationInput != 0) {
			if (!m_RotationControlsPressed) {
				float cacheSpeed = Vector3.Dot(m_Rb.velocity, transform.right);
				float cacheAngSpeed = Vector3.Dot(m_Rb.angularVelocity, transform.forward);

				m_Rb.velocity = Vector3.zero;
				m_Rb.angularVelocity = Vector3.zero;

				CorrectOrientation();				
				float rot = rotationInput * QUARTER_ROTATION;
				transform.RotateAround(transform.position, transform.up, rot);
				CorrectOrientation();	

				m_Rb.velocity = transform.right * cacheSpeed;
				m_Rb.angularVelocity = transform.forward * cacheAngSpeed;				

				m_RotationControlsPressed = true;
			} 
		} else {
			m_RotationControlsPressed = false;
		} 

		// handle translation
		Vector3 translation = Input.GetAxis("Horizontal") * transform.right * speed;
		m_Rb.AddForce(translation, ForceMode.VelocityChange);

		if (m_Rb.velocity.magnitude > m_TerminalVelocity) {
			m_Rb.velocity = m_Rb.velocity.normalized * m_TerminalVelocity;
		}
	}

	void FixedUpdate()
	{
		// handle gravity
		Vector3 groundDir = GetGroundDirection(m_CurrGroundDir);
		if (groundDir != m_CurrGroundDir) {
			Physics.gravity = groundDir * Physics.gravity.magnitude;
			m_CurrGroundDir = groundDir;
		}

		if (m_Rb.angularVelocity.magnitude < EPSILON_OMEGA) {
			if (Mathf.Round(Vector3.Dot(transform.up, -m_CurrGroundDir)) != 1.0f) {
				Debug.Log("Player fell down, getting him back up!");
				float angle = Vector3.Angle(transform.up, -m_CurrGroundDir);
				transform.RotateAround(transform.position, -transform.forward, angle);
				CorrectOrientation();
			}
		}

		// handle controls
		if (!m_AreControlsSuspended) {
			HandleControls();
		}
	}
}
