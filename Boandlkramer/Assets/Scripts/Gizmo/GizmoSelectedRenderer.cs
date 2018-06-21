using UnityEngine;

public class GizmoSelectedRenderer : MonoBehaviour {

	#region ENUMS
	public enum GizmoType { None, Sphere, Cube, WireSphere, WireCube }
	#endregion

	#region PUBLIC VARIABLES
	public GizmoType type = GizmoType.None;
	public Color color = Color.cyan;
	#endregion

	#region MESSAGES
	private void OnDrawGizmosSelected () {
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
