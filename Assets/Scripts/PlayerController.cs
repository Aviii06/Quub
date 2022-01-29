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

	private const float EPSILON = 0.1f;
	private const float QUARTER_ROTATION = 90.0f;

	private Vector3 m_CurrGroundDir;

	private bool m_RotationControlsPressed = false;

	void Start()
	{  
		m_Rb = GetComponent<Rigidbody>();  
		m_Col = GetComponent<Collider>();
		m_CurrGroundDir = Vector3.down;
	}

	bool IsGrounded(Vector3 groundDir) 
	{
		float distToGround = Mathf.Abs(Vector3.Dot(m_Col.bounds.extents, groundDir));
		return Physics.Raycast(transform.position, groundDir, distToGround + EPSILON);
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

		foreach (Vector3 dir in dirs)
		{
			if (Physics.Raycast(transform.position, dir)) 
			{
				return dir;
			}
		}

		return defaultDir;
	}

	void FixedUpdate()
	{
		// handle gravity
		Vector3 groundDir = GetGroundDirection(m_CurrGroundDir);
		if (groundDir != m_CurrGroundDir) {
			Physics.gravity = groundDir * Physics.gravity.magnitude;
			m_CurrGroundDir = groundDir;
		}


		// check if grounded
		bool isGrounded = IsGrounded(groundDir);
		float speed = isGrounded ? m_GroundSpeed : m_AirSpeed;


		// handle jump
		if (Input.GetButton("Jump") && isGrounded)
		{
			m_Rb.AddForce(transform.up * m_JumpForce, ForceMode.Impulse);
		}


		// handle rotation
		float rotationInput = Input.GetAxisRaw("Vertical");
		if (rotationInput != 0)
		{
			if (!m_RotationControlsPressed)
			{
				Vector3 currRot = transform.rotation.eulerAngles;
				currRot += rotationInput * QUARTER_ROTATION * transform.up;

				// correct rotation
				float rotX = Mathf.Round(currRot.x / QUARTER_ROTATION) * QUARTER_ROTATION;
				float rotY = Mathf.Round(currRot.y / QUARTER_ROTATION) * QUARTER_ROTATION;
				float rotZ = Mathf.Round(currRot.z / QUARTER_ROTATION) * QUARTER_ROTATION;
				transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);

				m_RotationControlsPressed = true;
			} 
		} 
		else 
		{
			m_RotationControlsPressed = false;
		} 


		// handle translation
		Vector3 translation = Input.GetAxis("Horizontal") * transform.right * speed;
		m_Rb.AddForce(translation, ForceMode.VelocityChange);

		if (m_Rb.velocity.magnitude > m_TerminalVelocity) 
		{
			m_Rb.velocity = m_Rb.velocity.normalized * m_TerminalVelocity;
		}


		// Debug Logs
		Debug.Log(Physics.gravity.normalized);
	}
}
