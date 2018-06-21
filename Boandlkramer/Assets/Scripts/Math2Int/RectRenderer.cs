using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoG.Math2Int {

	#region RECT RENDERER
	public class RectRenderer : MonoBehaviour {

		#region PRIVATE VARIABLES
		private GameObject _go;
		#endregion

		#region PROPERTIES
		public Rect Target { get; private set; }
		public Material Material { get; private set; }
		#endregion

		#region STATIC FUNCTIONS
		public static List<RectRenderer> Create (List<Rect> targets, Transform parent = null, Material material = null) {
			List<RectRenderer> renderers = new List<RectRenderer> ();
			foreach (Rect target in targets)
				renderers.Add (Create (target, parent, material));
			return renderers;
		}

		public static RectRenderer Create (Rect target, Transform parent = null, Material material = null) {
			GameObject obj = new GameObject ();
			if (parent != null)
				obj.transform.SetParent (parent, false);
			RectRenderer component = obj.AddComponent<RectRenderer> ();
			component.Target = target;
			component.Material = material;
			component.Initialize ();
			component.Render ();
			return component;
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public void Render () {
			if (_go != null)
				Destroy (_go);
			_go = GameObject.CreatePrimitive (PrimitiveType.Cube);
			_go.transform.position = new Vector3 (Target.Position.x, 0f, Target.Position.y);
			_go.transform.localScale = new Vector3 (Target.Size.X - 0.1f, 0.1f, Target.Size.Y - 0.1f);
			_go.GetComponent<MeshRenderer> ().material = GetMaterial ();
			_go.transform.SetParent (transform, false);
		}

		public Material GetMaterial () {
			if (Material != null || transform.parent == null)
				return Material;
			return transform.parent.GetComponent<GraphRenderer> ().GetMaterial ();
		}
		#endregion

		#region PRIVATE FUNCTIONS
		private void Initialize () {
			gameObject.name = "Rect";
		}
		#endregion
	}
	#endregion
}