using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	Rigidbody2D rb;
	PlayerAnimator anim;
	PlayerInput input;

	Vector2 direction;
	[SerializeField, Range(0, 500)] float speed = 10;
	[SerializeField, Range(0, 1)] float speedSlower = 0.5f;
	[SerializeField, Range(0, 50)] float dodgePower;
	

	private float realSpeed;
	



	private void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<PlayerAnimator>();
		input = GetComponent<PlayerInput>();
	}

    public void GetDirection()
    {
		float xInput = input.GetHorizontalInput();
		float yInput = input.GetVerticalInput();

		direction = new Vector3(xInput, yInput).normalized;
	}

	public void Move()
	{
		realSpeed = speed;
		rb.velocity = direction * realSpeed * Time.fixedDeltaTime;
		
	}

	public void SlowMove()
	{
		realSpeed = speed * speedSlower;
		rb.velocity = direction * realSpeed * Time.fixedDeltaTime;
		//rb.AddForce(direction * realSpeed * Time.fixedDeltaTime);
	}

	public void StopMove()
	{
		rb.velocity = new Vector2(0, 0);
		print("Stop");
	}

	public void Evade()
	{
		rb.AddForce(direction * dodgePower, ForceMode2D.Impulse);
	}

	

	
}
