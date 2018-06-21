using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LoG.Math2Int;
using Random = UnityEngine.Random;
using Rect = LoG.Math2Int.Rect;

namespace LoG.Flocking {

	#region FLOCK
	public class Flock {

		#region PROPERTIES
		public List<RectAgent> Agents { get; private set; }
		#endregion

		#region CONSTRUCTOR
		public Flock () {
			Agents = new List<RectAgent> ();
		}

		public Flock (List<Rect> rects) {
			Agents = new List<RectAgent> ();
			foreach (var rect in rects)
				Agents.Add (new RectAgent (rect));
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public void Separate (float speed = 0.1f) {
			for (int i = 0; i < 1000; i++) {
				float c = 0f;
				Dictionary<RectAgent, Vector2> velocities = new Dictionary<RectAgent, Vector2> ();
				foreach (RectAgent agent in Agents)
					velocities.Add (agent, agent.GetSeparationVelocity (Agents, speed));
				foreach (var item in velocities) {
					item.Key.Position += item.Value;
					c += item.Value.sqrMagnitude;
				}
				if (c < 0.001f) {
					Debug.Log ("Separation stopped at " + i.ToString () + " with c = " + c.ToString ());
					return;
				}
					
			}
			Debug.Log ("Separation cancelled.");
		}

		public void LockToGrid () {
			foreach (var agent in Agents)
				agent.LockToGrid ();
		}

		public List<Rect> ToRect () {
			List<Rect> rects = new List<Rect> ();
			foreach (var agent in Agents)
				rects.Add (agent.ToRect ());
			return rects;
		}
		#endregion
	}
	#endregion

	#region RECT AGENT
	public class RectAgent {

		#region PRIVATE VARIABLES
		private readonly Vector _size;
		private readonly float _radius;
		#endregion

		#region PROPERTIES
		public Vector2 Position { get; set; }
		public Vector Size {
			get {
				return _size;
			}
		}
		public float Radius {
			get {
				return _radius;
			}
		}
		public Vector2 Min {
			get {
				return Position - ((Vector2)Size / 2f);
			}
		}
		public Vector2 Max {
			get {
				return Position + ((Vector2)Size / 2f);
			}
		}
		#endregion

		#region CONSTRUCTOR
		public RectAgent (Vector2 pos, Vector size) {
			Position = pos;
			_size = size;
			_radius = (float)_size.Magnitude / 2f;
		}

		public RectAgent (Rect rect) {
			Position = rect.Position;
			_size = rect.Size;
			_radius = (float)_size.Magnitude / 2f;
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public Vector2 GetSeparationVelocity (List<RectAgent> agents, float speed = 0.1f) {
			Vector2 velocity = Vector2.zero;
			foreach (RectAgent agent in agents)
				if (agent != this && IsIntersecting (agent))
					velocity += Position - agent.Position;
			return velocity.normalized * speed;
		}

		public bool IsIntersecting (RectAgent other) {
			/*
			if ((Position - other.Position).magnitude > Radius + other.Radius)
				return false;
			*/
			if (Min.x > other.Max.x || other.Min.x > Max.x)
				return false;
			if (Min.y > other.Max.y || other.Min.y > Max.y)
				return false;
			return true;
		}

		public void LockToGrid () {
			Vector2 min = new Vector2 (Mathf.Floor (Min.x), Mathf.Floor (Min.y));
			Vector2 max = new Vector2 (Mathf.Floor (Max.x), Mathf.Floor (Max.y));
			Position = (min + max) / 2f;
		}

		public Rect ToRect () {
			return new Rect (Position, Size);
		}
		#endregion
	}
	#endregion
}
