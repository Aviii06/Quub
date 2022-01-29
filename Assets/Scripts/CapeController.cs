using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapeController : MonoBehaviour
{
	private Rigidbody m_Rb;
	private Collider m_Col;

	public float m_GroundSpeed = -0.4f;
	public float m_AirSpeed = -0.2f;
	public float m_JumpForce = 80.0f;

	public float m_TerminalVelocity = 2.0f;

	private float m_DistToGround;

	private const float EPSILON = 0.1f;

	void Start()
	{  
		m_Rb = GetComponent<Rigidbody>();  
		m_Col = GetComponent<Collider>();
		m_DistToGround = m_Col.bounds.extents.y;
	}

	bool IsGrounded() 
	{
		return Physics.Raycast(transform.position, -Vector3.up, m_DistToGround + EPSILON);
	}

	void FixedUpdate()
	{
		bool isGrounded = IsGrounded();
		float speed = isGrounded ? m_GroundSpeed : m_AirSpeed;

		if (Input.GetButton("Jump") && isGrounded)
		{
			m_Rb.AddForce(transform.up * m_JumpForce, ForceMode.Impulse);
		}
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
