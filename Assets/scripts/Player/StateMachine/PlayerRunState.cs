using UnityEngine;
using UnityEngine.Windows;

public class PlayerRunState : PlayerBaseState
{
	PlayerController controller;
	PlayerInput input;

	public override void EnterState(PlayerStateManager state)
	{
		Debug.Log("Run state in Town!");
		controller = state.GetComponent<PlayerController>();
		input = state.GetComponent<PlayerInput>();
	}
	public override void UpdateState(PlayerStateManager state)
	{
		if (UnityEngine.Input.GetMouseButton(0))
		{
			state.SwitchState(state.attackEntryState);
		}
		else if (!input.isInput())
		{
			state.SwitchState(state.idleState);
		}
		else if(input.GetSpaceButton())
		{
			state.SwitchState(state.evadeState);
		}
		
		
		controller.GetDirection();
		
		
	}

	public override void FixedUpdateState(PlayerStateManager state)
	{
		controller.Move();
	}
	public override void OnTriggerEnter(PlayerStateManager state)
	{

	}
}
