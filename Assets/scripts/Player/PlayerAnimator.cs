using UnityEngine;


[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private PlayerController playerController;
    private Animator anim;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        if (playerController != null)
        {
            // Use the obtained value of someVariable as needed
        }
        else
        {
            Debug.LogError("PlayerController not found on the PlayerAnimator object");
        }
    }

    public void AttackLeft()
    {
        //anim.Play("AttackLeft");
        Debug.Log("AttackLeft");
	}

	public void AttackDown()
	{
		//anim.Play("AttackDown");
		Debug.Log("AttackDown");
	}

	public void AttackRight()
	{
		//anim.Play("AttackRight");
		Debug.Log("AttackRight");
	}

	public void AttackUp()
	{
		//anim.Play("AttackUp");
		Debug.Log("AttackUp");
	}
}
