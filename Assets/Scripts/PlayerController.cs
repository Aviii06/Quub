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

	private Vector3 m_CurrGroundDir;

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
		Vector3 groundDir = GetGroundDirection(m_CurrGroundDir);
		m_Rb.constraints = RigidbodyConstraints.None;
		if (groundDir != m_CurrGroundDir) {
			// Handle Rotation of cube and player
			if (groundDir == Vector3.down || groundDir == Vector3.up)
			{
				m_Rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
			}
			else if (groundDir == Vector3.left || groundDir == Vector3.right)
			{
				m_Rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			}
			else
			{
				m_Rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationY;
			}
			Physics.gravity = groundDir * Physics.gravity.magnitude;
			m_CurrGroundDir = groundDir;
		}

		bool isGrounded = IsGrounded(groundDir);
		float speed = isGrounded ? m_GroundSpeed : m_AirSpeed;

		if (Input.GetButton("Jump") && isGrounded)
		{
			m_Rb.AddForce(transform.up * m_JumpForce, ForceMode.Impulse);
		}

		// change these according to the current orientation of the ground dir
		Vector3 horizontalTranslation = Input.GetAxis("Horizontal") * transform.right * speed;
		Vector3 verticalTranslation = Input.GetAxis("Vertical") * transform.forward * speed;
		Vector3 translation = horizontalTranslation + verticalTranslation;

		m_Rb.AddForce(translation, ForceMode.VelocityChange);
		if (m_Rb.velocity.magnitude > m_TerminalVelocity) 
		{
			m_Rb.velocity = m_Rb.velocity.normalized * m_TerminalVelocity;
		}
	}
}
