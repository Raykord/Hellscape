using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
	// How long this state should be active for before moving on
	public float duration;
	// Cached animator component
	protected Animator anim;
	// bool to check whether or not the next attack in the sequence should be played or not
	protected bool shouldCombo;
	// The attack index in the sequence of attacks
	protected int attackIndex;
	protected float fixedTime;

	PlayerController controller;
	PlayerInput input;
	
	private Vector3 worldPos = new Vector3(0, 0);
	private float AttackPressedTimer = 0;

	public override void EnterState(PlayerStateManager state)
	{
		controller = state.GetComponent<PlayerController>();
		input = state.GetComponent<PlayerInput>();
		anim = state.GetComponent<Animator>();
		fixedTime = 0;
	}
	public override void UpdateState(PlayerStateManager state)
	{
		
		controller.GetDirection();

		//if (!input.isInput())
		//{
		//	state.SwitchState(state.idleState);
		//}
		//else if ((input.GetHorizontalInput() != 0 || input.GetVerticalInput() != 0) && !input.GetLeftMouseButton())
		//{
		//	state.SwitchState(state.runState);
		//}

		AttackPressedTimer -= Time.deltaTime;

		if (anim.GetFloat("Weapon.Active") > 0f)
		{
			Attack();
		}


		if (Input.GetMouseButtonDown(0))
		{
			AttackPressedTimer = 2;
		}

		if (anim.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0)
		{
			shouldCombo = true;
		}
	}

	public override void FixedUpdateState(PlayerStateManager state)
	{
		fixedTime += Time.deltaTime;
		controller.SlowMove();
	}
	public override void OnTriggerEnter(PlayerStateManager state)
	{

	}

	public void Attack()
	{
		//Получаем позицию курсора в мировом пространстве
		worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float dx = controller.gameObject.transform.position.x - worldPos.x;
		float dy = controller.gameObject.transform.position.y - worldPos.y;

		// Вычисляем угол между объектами «Персонаж» и «Указатель»
		float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

		//print(angle);
		

		if (Input.GetMouseButton(0))
		{
			
			//if (angle >= -45 && angle <= 45) anim.AttackLeft();
			//else if (angle >= 45 && angle <= 135) anim.AttackDown();
			//else if (angle >= 135 && angle <= 180 || angle <= -135 && angle >= -180) anim.AttackRight();
			//else if (angle <= -45 && angle >= -135) anim.AttackUp();
		}
	}
}
