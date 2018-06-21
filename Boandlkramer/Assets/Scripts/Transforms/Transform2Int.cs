using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoG.Math2Int;

namespace LoG.Transforms {

	#region TRANSFORM2INT
	public class Transform2Int {

		#region PRIVATE VARIABLES
		private Transform2Int _parent;
		#endregion

		#region PROPERTIES
		public object Object { get; private set; }
		public Transform2Int Parent {
			get {
				if (_parent == null)
					return new Transform2Int ();
				return _parent;
			}
			set {
				value = this == value ? null : value;
				if (value == _parent || (value != null && Descendants.Contains (value)))
					return;
				if (_parent != null)
					_parent.Children.Remove (this);
				if (value != null)
					value.Children.Add (this);
				_parent = value;
			}
		}
		public List<Transform2Int> Children { get; private set; }
		public List<Transform2Int> Descendants {
			get {
				List<Transform2Int> children = new List<Transform2Int> ();
				children.AddRange (Children);
				foreach (Transform2Int t in Children)
					children.AddRange (t.Descendants);
				return children;
			}
		}
		public Transform2Int Root {
			get {
				if (_parent == null)
					return this;
				return Parent.Root;
			}
		}
		public Vector LocalPosition { get; set; }
		public Rotation LocalRotation { get; set; }
		public Vector GlobalPosition {
			get {
				if (_parent == null)
					return LocalPosition;
				return Parent.GlobalPosition + Parent.GlobalRotation * LocalPosition;
			}
			set {
				LocalPosition = (-Parent.GlobalRotation) * (value - Parent.GlobalPosition);
			}
		}
		public Rotation GlobalRotation {
			get {
				if (_parent == null)
					return LocalRotation;
				return Parent.GlobalRotation + LocalRotation;
			}
			set {
				LocalRotation = value - Parent.GlobalRotation;
			}
		}
		#endregion

		#region CONSTRUCTOR
		public Transform2Int () {
			Initialize (null, null, Vector.Zero, Rotation.Up, false);
		}

		public Transform2Int (Vector pos, Rotation rot) {
			Initialize (null, null, pos, rot, false);
		}

		public Transform2Int (Transform2Int parent, bool global = false) {
			Initialize (null, parent, Vector.Zero, Rotation.Up, global);
		}

		public Transform2Int (Transform2Int parent, Vector pos, Rotation rot, bool global = false) {
			Initialize (null, parent, pos, rot, global);
		}

		public Transform2Int (object obj) {
			Initialize (obj, null, Vector.Zero, Rotation.Up, false);
		}

		public Transform2Int (object obj, Vector pos, Rotation rot) {
			Initialize (obj, null, pos, rot, false);
		}

		public Transform2Int (object obj, Transform2Int parent, bool global = false) {
			Initialize (obj, parent, Vector.Zero, Rotation.Up, global);
		}

		public Transform2Int (object obj, Transform2Int parent, Vector pos, Rotation rot, bool global = false) {
			Initialize (obj, parent, pos, rot, global);
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public T GetAttachedObject<T> () where T : class {
			return Object as T;
		}

		public void SetParent (Transform2Int parent, bool local = false) {
			if (local) {
				Parent = parent;
				return;
			}
			Vector globalPosition = GlobalPosition;
			Rotation globalRotation = GlobalRotation;
			Parent = parent;
			GlobalPosition = globalPosition;
			GlobalRotation = globalRotation;
		}

		public void MakeRoot () {
			SetParent (null);
		}

		public void RemoveFromHierarchy () {
			SetParent (null, true);
			foreach (Transform2Int child in Children)
				child.SetParent (_parent, true);
			Children = new List<Transform2Int> ();
		}
		#endregion

		#region PRIVATE FUNCTIONS
		private void Initialize (object obj, Transform2Int parent, Vector pos, Rotation rot, bool global) {
			Object = obj;
			Children = new List<Transform2Int> ();
			Parent = parent;
			if (global) {
				GlobalPosition = pos;
				GlobalRotation = rot;
			}
			else {
				LocalPosition = pos;
				LocalRotation = rot;
			}
		}
		#endregion

		#region OPERATORS
		public static Transform2Int operator + (Transform2Int transform, Vector vector) {
			transform.LocalPosition += vector;
			return transform;
		}

		public static Transform2Int operator - (Transform2Int transform, Vector vector) {
			transform.LocalPosition -= vector;
			return transform;
		}

		public static Transform2Int operator + (Transform2Int transform, Rotation rotation) {
			transform.LocalRotation += rotation;
			return transform;
		}

		public static Transform2Int operator - (Transform2Int transform, Rotation rotation) {
			transform.LocalRotation -= rotation;
			return transform;
		}
		#endregion
	}
	#endregion
}