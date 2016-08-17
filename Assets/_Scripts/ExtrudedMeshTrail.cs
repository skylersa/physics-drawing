using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof(MeshFilter))]
public class ExtrudedMeshTrail : MonoBehaviour
{
	public Transform controllerTransform;

	class ExtrudedTrailSection
	{
		public Vector3 point;
		public Matrix4x4 matrix;
		public float time;
	}

	// Generates an extrusion trail from the attached mesh
	// Uses the MeshExtrusion algorithm in MeshExtrusion.cs to generate and preprocess the mesh.
	float time = 2f;
	bool autoCalculateOrientation = true;
	float minDistance = .01f;
	bool invertFaces = false;
	private Mesh srcMesh;
	private MeshExtrusion.Edge[] precomputedEdges;
	private List<ExtrudedTrailSection> sections = new List<ExtrudedTrailSection> ();

	void Start ()
	{
		srcMesh = GetComponent <MeshFilter> ().sharedMesh;
		precomputedEdges = MeshExtrusion.BuildManifoldEdges (srcMesh);
	}

	void Update ()
	{
		transform.position = controllerTransform.position;
		transform.rotation = controllerTransform.rotation;
		Vector3 position = transform.position;
		Vector3 scale = transform.localScale;
		float now = Time.time;
	
		// Remove old sections
		while (sections.Count > 0 && now > sections [sections.Count - 1].time + time) {
			sections.RemoveAt (sections.Count - 1);
		}

		// Add a new trail section to beginning of array
		if (sections.Count == 0 || (sections [0].point - position).sqrMagnitude > minDistance * minDistance) {
			ExtrudedTrailSection section = new ExtrudedTrailSection ();
			section.point = position;
			section.matrix = transform.localToWorldMatrix;
			section.time = now;
			sections.Insert (0, section);
		}
	
		// We need at least 2 sections to create the line
		if (sections.Count < 2)
			return;

		Matrix4x4 worldToLocal = transform.worldToLocalMatrix;
		Matrix4x4[] finalSections = new Matrix4x4[sections.Count + 1];
		//Quaternion previousRotation;
		Vector3 direction;
		Quaternion rotation;

		for (var i = -1; i < sections.Count; i++) {
			if (autoCalculateOrientation) {
				if (i == -1) {
					direction = sections [0].point - sections [1].point;
					rotation = Quaternion.LookRotation (direction, Vector3.up);
					//previousRotation = rotation;
					finalSections [i + 1] = worldToLocal * Matrix4x4.TRS (position, rotation, scale);
				}
			// all elements get the direction by looking up the next section
				else if (i != sections.Count - 1) {	
					direction = sections [i].point - sections [i + 1].point;
					rotation = Quaternion.LookRotation (direction, Vector3.up);
				
					// When the angle of the rotation compared to the last segment is too high
					// smooth the rotation a little bit. Optimally we would smooth the entire sections array.
//				if (Quaternion.Angle (previousRotation, rotation) > 20)
//					rotation = Quaternion.Slerp(previousRotation, rotation, 0.5);
					
					//previousRotation = rotation;
					finalSections [i + 1] = worldToLocal * Matrix4x4.TRS (sections [i].point, rotation, scale);
				}
			// except the last one, which just copies the previous one
			else {
					finalSections [i + 1] = finalSections [i - 1];
				}
			} else {
				if (i == -1) {
					finalSections [i + 1] = Matrix4x4.identity;
				} else {
					finalSections [i + 1] = worldToLocal * sections [i].matrix;
				}
			}
		}
	
		// Rebuild the extrusion mesh	
		MeshExtrusion.ExtrudeMesh (srcMesh, GetComponent <MeshFilter> ().mesh, finalSections, precomputedEdges, invertFaces);
	}

}
