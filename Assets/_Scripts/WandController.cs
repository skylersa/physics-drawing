using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour
{
	public Transform trailHolder;
	public GameObject trailPrefab;

	SteamVR_TrackedController trackedController;
	SteamVR_TrackedObject trackedObject;
	SteamVR_RenderModel renderModel;
	Transform trail;

	ExtrudedMeshController extrudedMeshController;
	Transform trailContainer;

	void Update ()
	{
		trail.position = trailHolder.transform.position;
		trail.rotation = trailHolder.transform.rotation;
	}

	void OnDestroy ()
	{
		Destroy (trailContainer.gameObject);
	}

	public void Init (int index)
	{
		trailContainer = new GameObject ().transform;
		trailContainer.name = name + " Trail Container";

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

		NewTrail ();
	}

	void NewTrail ()
	{
		trail = Instantiate (trailPrefab).transform;
		trail.SetParent (trailContainer, true);
		extrudedMeshController = trail.GetComponent<ExtrudedMeshController> ();
		trail.localScale = trailHolder.lossyScale;
		trail.name = trailPrefab.name + " " + name;
		trail.GetComponentInChildren<Collider> ().enabled = false;
	}

	void OnTriggerClicked (object sender, ClickedEventArgs e)
	{
		extrudedMeshController.StartExtrusion ();
	}

	void OnTriggerUnclicked (object sender, ClickedEventArgs e)
	{
		extrudedMeshController.StopExtrusion ();
		NewTrail ();
	}
}
