using UnityEngine;

public class GizmoRenderer : MonoBehaviour {

	#region ENUMS
	public enum GizmoType { None, Sphere, Cube, WireSphere, WireCube }
	#endregion

	#region PUBLIC VARIABLES
	public GizmoType type = GizmoType.None;
	public Color color = Color.cyan;
	#endregion

	#region STATIC FUNCTIONS
	public static GizmoRenderer Create (Transform p, Vector3? pos, Quaternion? rot, Vector3? size, GizmoType t) {
		if (pos == null)
			pos = Vector3.zero;
		if (rot == null)
			rot = Quaternion.identity;
		if (size == null)
			size = Vector3.one;
		GameObject obj = new GameObject ("Gizmo");
		if (p != null)
			obj.transform.SetParent (p);
		obj.transform.localPosition = (Vector3)pos;
		obj.transform.localRotation = (Quaternion)rot;
		obj.transform.localScale = (Vector3)size;
		GizmoRenderer gr = obj.AddComponent<GizmoRenderer> ();
		gr.type = t;
		return gr;
	}

	public static GizmoRenderer CreateSphere (Transform p, Vector3? pos = null, Quaternion? rot = null, Vector3? size = null) {
		return Create (p, pos, rot, size, GizmoType.Sphere);
	}

	public static GizmoRenderer CreateCube (Transform p, Vector3? pos = null, Quaternion? rot = null, Vector3? size = null) {
		return Create (p, pos, rot, size, GizmoType.Cube);
	}

	public static GizmoRenderer CreateWireSphere (Transform p, Vector3? pos = null, Quaternion? rot = null, Vector3? size = null) {
		return Create (p, pos, rot, size, GizmoType.WireSphere);
	}

	public static GizmoRenderer CreateWireCube (Transform p, Vector3? pos = null, Quaternion? rot = null, Vector3? size = null) {
		return Create (p, pos, rot, size, GizmoType.WireCube);
	}

	public static void AddSphereTo (GameObject obj) {
		obj.AddComponent<GizmoRenderer> ().type = GizmoType.Sphere;
	}

	public static void AddCubeTo (GameObject obj) {
		obj.AddComponent<GizmoRenderer> ().type = GizmoType.Cube;
	}

	public static void AddWireSphereTo (GameObject obj) {
		obj.AddComponent<GizmoRenderer> ().type = GizmoType.WireSphere;
	}

	public static void AddWireCubeTo (GameObject obj) {
		obj.AddComponent<GizmoRenderer> ().type = GizmoType.WireCube;
	}
	#endregion

	#region MESSAGES
	private void OnDrawGizmos () {
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = color;
		switch (type) {
			case GizmoType.Sphere:
				Gizmos.DrawSphere (Vector3.zero, 1f);
				break;
			case GizmoType.Cube:
				Gizmos.DrawCube (Vector3.zero, Vector3.one);
				break;
			case GizmoType.WireSphere:
				Gizmos.DrawWireSphere (Vector3.zero, 1f);
				break;
			case GizmoType.WireCube:
				Gizmos.DrawWireCube (Vector3.zero, Vector3.one);
				break;
			default:
				break;
		}
	}
	#endregion
}
