using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttackExit : PlayerAttackState
{
	public override void EnterState(PlayerStateManager state)
	{
		base.EnterState(state);
		attackIndex = 3;
		duration = 0.5f;
		anim.SetTrigger("Attack" + attackIndex);
		Debug.Log("Player Attack " + attackIndex + " Fired!");
	}

	public override void UpdateState(PlayerStateManager state)
	{
		base.UpdateState(state);
		if (fixedTime >= duration)
		{
			state.SwitchState(state.idleState);
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
