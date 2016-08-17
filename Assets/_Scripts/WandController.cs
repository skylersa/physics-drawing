using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour
{

	SteamVR_TrackedController trackedController;
	SteamVR_TrackedObject trackedObject;
	SteamVR_RenderModel renderModel;

	public void Init (int index)
	{
		trackedObject = gameObject.AddComponent<SteamVR_TrackedObject> ();
		trackedObject.SetDeviceIndex (index);
		
		trackedController = gameObject.AddComponent<SteamVR_TrackedController> ();
		trackedController.SetDeviceIndex (index);
		
		GameObject child = new GameObject ();
		child.name = name + " Render Model";
		child.transform.SetParent (transform, false);
		
		renderModel = child.AddComponent<SteamVR_RenderModel> ();
		renderModel.SetDeviceIndex (index);
		
		trackedController.TriggerClicked += OnTriggerClicked;
		trackedController.TriggerUnclicked += OnTriggerUnclicked;
	}

	void OnTriggerClicked (object sender, ClickedEventArgs e)
	{
		throw new System.NotImplementedException ();
	}

	void OnTriggerUnclicked (object sender, ClickedEventArgs e)
	{
		throw new System.NotImplementedException ();
	}
}
