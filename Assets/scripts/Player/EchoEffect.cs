using System.Collections;
using UnityEngine;

public class EchoEffect : MonoBehaviour
{
    private float timeBtwSpawns;
    [SerializeField] private float startTimeBtwSpawns;

    [SerializeField] GameObject echo;
    

	private void Start()
	{
		
	}

	public void StartEcho()
	{
		
		StartCoroutine("Echo");
		
	}

	IEnumerator Echo()
	{
		for (int i = 0; i < 10; i++) {
			

			GameObject instance = (GameObject)Instantiate(echo, transform.position, Quaternion.identity);
			Destroy(instance, 0.2f);
			yield return new WaitForFixedUpdate();
		}

	}
}
