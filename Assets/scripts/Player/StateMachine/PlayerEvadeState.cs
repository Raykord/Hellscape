using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEvadeState : PlayerBaseState
{
	EchoEffect echoEffect;
	PlayerController controller;

	float duration = 0.2f;
	float time;

	public override void EnterState(PlayerStateManager state)
	{
		Debug.Log("Evade!");
		//Логика на включение анимации
		//state.gameObject.GetComponent<PlayerAnimator>().Idle();
		echoEffect = state.GetComponent<EchoEffect>();
		controller = state.GetComponent<PlayerController>();
		
	}
	public override void UpdateState(PlayerStateManager state)
	{
		
		
			
		
		

	}

	public override void FixedUpdateState(PlayerStateManager state)
	{
		if (state.canEvade)
		{
			echoEffect.StartEcho();
			controller.Evade();
			state.canEvade = false;


		}
		else
		{
			if (time >= duration)
			{
				time = 0;
				Debug.Log("I.m work");
				state.SwitchState(state.runState);
			}
			else
			{
				time += Time.deltaTime;
			}
		}
	}
	public override void OnTriggerEnter(PlayerStateManager state)
	{

	}

	
}
