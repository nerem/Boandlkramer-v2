using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoG.Math2Int {

	#region GRAPH RENDERER
	public class GraphRenderer : MonoBehaviour {

		#region PROPERTIES
		public object Target { get; private set; }
		public Material Material { get; private set; }
		public Dictionary<Vector, GraphRenderer> Points { get; private set; }
		public Dictionary<Edge, GraphRenderer> Edges { get; private set; }
		#endregion

		#region STATIC FUNCTIONS
		public static GraphRenderer Create<T> (T target, Transform parent = null, Material material = null) {
			GameObject obj = new GameObject ();
			if (parent != null)
				obj.transform.SetParent (parent, false);
			GraphRenderer component = obj.AddComponent<GraphRenderer> ();
			component.Target = target;
			component.Material = material;
			component.Initialize ();
			component.Render ();
			return component;
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public void Render () {
			InitializeRender ();
			Edge? edge = Target as Edge?;
			if (edge != null)
				Render ((Edge)edge);
			Vector? point = Target as Vector?;
			if (point != null)
				Render ((Vector)point);
			if (Points != null)
				foreach (GraphRenderer renderer in Points.Values)
					renderer.Render ();
			if (Edges != null)
				foreach (GraphRenderer renderer in Edges.Values)
					renderer.Render ();
		}

		public Material GetMaterial () {
			if (Material != null || transform.parent == null)
				return Material;
			return transform.parent.GetComponent<GraphRenderer> ().GetMaterial ();
		}
		#endregion

		#region PRIVATE FUNCTIONS
		private void Initialize () {
			Points = new Dictionary<Vector, GraphRenderer> ();
			Edges = new Dictionary<Edge, GraphRenderer> ();
			Graph graph = Target as Graph;
			if (graph != null) {
				Initialize (graph);
				return;
			}
			Triangle? triangle = Target as Triangle?;
			if (triangle != null) {
				Initialize ((Triangle)triangle);
				return;
			}
			Edge? edge = Target as Edge?;
			if (edge != null) {
				Initialize ((Edge)edge);
				return;
			}
			Vector? point = Target as Vector?;
			if (point != null) {
				Initialize ((Vector)point);
				return;
			}
		}

		private void Initialize (Graph target) {
			gameObject.name = "Graph";
			foreach (Vector point in target.Vertices)
				Points.Add (point, Create (point, transform));
			foreach (Edge edge in target.Edges)
				Edges.Add (edge, Create (edge, transform));
		}

		private void Initialize (Triangle target) {
			gameObject.name = "Triangle";
			transform.position = (Vector3)target.A;
			foreach (Vector point in target.Points)
				Points.Add (point, Create (point, transform));
			foreach (Edge edge in target.Edges)
				Edges.Add (edge, Create (edge, transform));
		}

		private void Initialize (Edge target) {
			gameObject.name = "Edge";
			transform.position = (Vector3)target.Start;
		}

		private void Initialize (Vector target) {
			gameObject.name = "Point";
			transform.position = (Vector3)target;
		}

		private void InitializeRender () {
			Graph graph = Target as Graph;
			if (graph != null) {
				InitializeRender (graph.Edges);
				InitializeRender (graph.Vertices);
				return;
			}
			Triangle? triangle = Target as Triangle?;
			if (triangle != null) {
				InitializeRender (((Triangle)triangle).Edges);
				InitializeRender (((Triangle)triangle).Points);
				return;
			}
		}

		private void InitializeRender (List<Edge> list) {
			Dictionary<Edge, GraphRenderer> dict = new Dictionary<Edge, GraphRenderer> ();
			foreach (Edge edge in list) {
				if (Edges.ContainsKey (edge)) {
					dict.Add (edge, Edges[edge]);
					Edges.Remove (edge);
				}
				else
					dict.Add (edge, Create (edge, parent: transform));
			}
			foreach (GraphRenderer renderer in Edges.Values)
				Destroy (renderer.gameObject);
			Edges = dict;
		}

		private void InitializeRender (List<Vector> list) {
			Dictionary<Vector, GraphRenderer> dict = new Dictionary<Vector, GraphRenderer> ();
			foreach (Vector point in list) {
				if (Points.ContainsKey (point)) {
					dict.Add (point, Points[point]);
					Points.Remove (point);
				}
				else
					dict.Add (point, Create (point, parent: transform));
			}
			foreach (GraphRenderer renderer in Points.Values)
				Destroy (renderer.gameObject);
			Points = dict;
		}

		private void Render (Edge target) {
			GameObject instance = new GameObject ("Line");
			instance.transform.SetParent (transform, false);
			LineRenderer line = instance.AddComponent<LineRenderer> ();
			line.useWorldSpace = false;
			line.SetPositions (new Vector3[] { Vector3.zero, (Vector3)(target.End - target.Start) });
			line.material = GetMaterial ();
			line.widthMultiplier = 0.3f;
		}

		private void Render (Vector target) {
			GameObject instance = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			instance.transform.SetParent (transform, false);
			instance.GetComponent<MeshRenderer> ().material = GetMaterial ();
			instance.transform.localScale = 0.7f * Vector3.one;
		}
		#endregion
	}
	#endregion
}
