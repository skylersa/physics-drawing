// Generates an extrusion trail from the attached mesh
// Uses the MeshExtrusion algorithm in MeshExtrusion.cs to generate and preprocess the mesh.
var time = 2.0;
var autoCalculateOrientation = true;
var minDistance = 0.1;
var invertFaces = false;
private var srcMesh : Mesh;
private var precomputedEdges : MeshExtrusion.Edge[];

public var controllerTransform : Transform;

class ExtrudedTrailSection
{
	var point : Vector3;
	var matrix : Matrix4x4;
	var time : float;
}

function Start ()
{
	srcMesh = GetComponent(MeshFilter).sharedMesh;
	precomputedEdges = MeshExtrusion.BuildManifoldEdges(srcMesh);
}

private var sections = new Array();

function Update() {
  transform.position = controllerTransform.position;
  transform.rotation = controllerTransform.rotation;
}

function LateUpdate () {
	var position = transform.position;
	var scale = transform.localScale;
	var now = Time.time;
	
	// Remove old sections
	while (sections.length > 0 && now > sections[sections.length - 1].time + time) {
		sections.Pop();
	}

	// Add a new trail section to beginning of array
	if (sections.length == 0 || (sections[0].point - position).sqrMagnitude > minDistance * minDistance)
	{
		var section = ExtrudedTrailSection ();
		section.point = position;
		section.matrix = transform.localToWorldMatrix;
		section.time = now;
		sections.Unshift(section);
	}
	
	// We need at least 2 sections to create the line
	if (sections.length < 2)
		return;

	var worldToLocal = transform.worldToLocalMatrix;
	var finalSections = new Matrix4x4[sections.length + 1];
	var previousRotation : Quaternion;
	
	for (var i=-1;i<sections.length;i++)
	{
		if (autoCalculateOrientation)
		{
			if (i == -1)
			{
				var direction = sections[0].point - sections[1].point;
				var rotation = Quaternion.LookRotation(direction, Vector3.up);
				previousRotation = rotation;
				finalSections[i+1] = worldToLocal * Matrix4x4.TRS(position, rotation, scale);
			}
			// all elements get the direction by looking up the next section
			else if (i != sections.length - 1)
			{	
				direction = sections[i].point - sections[i+1].point;
				rotation = Quaternion.LookRotation(direction, Vector3.up);
				
				// When the angle of the rotation compared to the last segment is too high
				// smooth the rotation a little bit. Optimally we would smooth the entire sections array.
//				if (Quaternion.Angle (previousRotation, rotation) > 20)
//					rotation = Quaternion.Slerp(previousRotation, rotation, 0.5);
					
				previousRotation = rotation;
				finalSections[i+1] = worldToLocal * Matrix4x4.TRS(sections[i].point, rotation, scale);
			}
			// except the last one, which just copies the previous one
			else
			{
				finalSections[i+1] = finalSections[i-1];
			}
		}
		else
		{
			if (i == -1)
			{
				finalSections[i+1] = Matrix4x4.identity;
			}
			else
			{
				finalSections[i+1] = worldToLocal * sections[i].matrix;
			}
		}
	}
	
	// Rebuild the extrusion mesh	
	MeshExtrusion.ExtrudeMesh (srcMesh, GetComponent(MeshFilter).mesh, finalSections, precomputedEdges, invertFaces);
}

@script RequireComponent (MeshFilter)
