using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttackEntry : PlayerAttackState
{
	public override void EnterState(PlayerStateManager state)
	{
		base.EnterState(state);
		
		attackIndex = 1;
		duration = 0.5f;
		anim.SetTrigger("Attack" + attackIndex);
		Debug.Log("Player Attack " + attackIndex + " Fired!");
	}

	public override void UpdateState(PlayerStateManager state)
	{
		base.UpdateState(state);
		if (fixedTime >= duration)
		{
			if (shouldCombo)
			{
				
				state.SwitchState(new PlayerMeleeAttackCombo());
			}
			else
			{
				state.SwitchState(state.idleState);
			}
		}
	}

	public override void FixedUpdateState(PlayerStateManager state)
	{
		
		base.FixedUpdateState(state);
	}

	public override void OnTriggerEnter(PlayerStateManager state)
	{
		base.OnTriggerEnter(state);
	}
}
