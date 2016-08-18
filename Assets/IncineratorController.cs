using UnityEngine;
using System.Collections;

public class IncineratorController : MonoBehaviour
{

	void OnTriggerEnter (Collider other)
	{
		Destroy (other.gameObject);
	}
}
