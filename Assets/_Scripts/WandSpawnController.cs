using UnityEngine;
using System.Collections;
using Valve.VR;

public class WandSpawnController : MonoBehaviour
{

	public GameObject wandPrefab;

	void OnEnable ()
	{
		SteamVR_Utils.Event.Listen ("device_connected", DeviceConnected);
	}

	void DeviceConnected (params object[] args)
	{
//		Debug.Log (name + ": OnDeviceConnected" + args [0] + " " + args [1]);
		int index = (int)args [0];
		bool activate = (bool)args [1];
		if (SteamVR.instance.hmd.GetTrackedDeviceClass ((uint)index) != ETrackedDeviceClass.Controller) {
			return;
		}
		Debug.Log (name + ": OnDeviceConnected" + args [0] + " " + args [1]);
		if (activate) {
			GameObject wand = Instantiate (wandPrefab);
			wand.name = "Wand" + index;
			wand.transform.SetParent (transform, false);
			WandController wandController = wand.GetComponentInChildren<WandController> ();
			wandController.Init (index);
		}
	}

}
