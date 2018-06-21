using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoG.Math2Int {

	#region GRID RENDERER
	public class GridRenderer : MonoBehaviour {

		#region PROPERTIES
		public object Target { get; private set; }
		public Material Material { get; private set; }
		public Dictionary<Vector, GameObject> Fields { get; private set; }
		#endregion

		#region STATIC FUNCTIONS
		public static GridRenderer Create<T> (Grid2D<T> target, Transform parent = null, Material material = null, Func<T, Material> rule = null) where T : new () {
			GameObject obj = new GameObject ();
			if (parent != null)
				obj.transform.SetParent (parent, false);
			GridRenderer component = obj.AddComponent<GridRenderer> ();
			component.Target = target;
			component.Material = material;
			component.Initialize ();
			component.Render (rule);
			return component;
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public void Render<T> (Func<T, Material> rule) where T : new () {
			Grid2D<T> grid = Target as Grid2D<T>;
			if (grid != null)
				Render (grid.Elements, rule);
		}

		public Material GetMaterial () {
			if (Material != null || transform.parent == null)
				return Material;
			return transform.parent.GetComponent<GraphRenderer> ().GetMaterial ();
		}
		#endregion

		#region PRIVATE FUNCTIONS
		private void Initialize () {
			Fields = new Dictionary<Vector, GameObject> ();
			gameObject.name = "Grid";
		}

		private void Render<T> (Dictionary<Vector, T> cells, Func<T, Material> rule) {
			Dictionary<Vector, GameObject> dict = new Dictionary<Vector, GameObject> ();
			foreach (KeyValuePair<Vector, T> cell in cells) {
				if (Fields.ContainsKey (cell.Key)) {
					dict.Add (cell.Key, Fields[cell.Key]);
					Fields.Remove (cell.Key);
				}
				else
					dict.Add (cell.Key, MakeField (cell, rule));
			}
			foreach (GameObject renderer in Fields.Values)
				Destroy (renderer);
			Fields = dict;
		}

		private GameObject MakeField<T> (KeyValuePair <Vector, T> cell, Func<T, Material> rule) {
			if (rule == null)
				rule = x => GetMaterial ();
			GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
			obj.transform.position = (Vector3)cell.Key;
			obj.transform.localScale = new Vector3 (0.9f, 0.1f, 0.9f);
			obj.GetComponent<MeshRenderer> ().material = rule (cell.Value);
			obj.transform.SetParent (transform, false);
			return obj;
		}
		#endregion
	}
	#endregion
}
