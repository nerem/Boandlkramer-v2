using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LoG.Math2Int {

	#region GRID
	public class Grid2D<T> where T : new () {

		#region PROPERTIES
		public Dictionary<Vector, T> Elements { get; private set; }
		public Vector Min {
			get {
				if (Elements == null || Elements.Count == 0)
					return Vector.Zero;
				Vector m = Elements.First ().Key;
				foreach (Vector v in Elements.Keys)
					m = Vector.Min (m, v);
				return m;
			}
		}
		public Vector Max {
			get {
				if (Elements == null || Elements.Count == 0)
					return Vector.Zero;
				Vector m = Elements.First ().Key;
				foreach (Vector v in Elements.Keys)
					m = Vector.Max (m, v);
				return m;
			}
		}
		public Vector Center {
			get {
				return new Vector ((int)Elements.Keys.Select (v => v.X).Average (), (int)Elements.Keys.Select (v => v.Y).Average ());
			}
		}
		#endregion

		#region CONSTRUCTOR
		public Grid2D () {
			Elements = new Dictionary<Vector, T> ();
		}

		public Grid2D (List<Vector> list, T d = default(T)) {
			Elements = new Dictionary<Vector, T> ();
			foreach (Vector vector in list)
				Elements[vector] = d;
		}

		public Grid2D (Dictionary<Vector, T> dict) {
			Elements = new Dictionary<Vector, T> ();
			foreach (KeyValuePair<Vector, T> item in dict)
				Elements[item.Key] = item.Value;
		}
		#endregion

		#region STATIC FUNCTIONS
		public static Grid2D<T> Create (List<Vector> points, int width = 3, Func<Vector, T> construct = null) {
			int e = (int)((width - 1) / 2f);
			if (construct == null)
				construct = x => new T ();
			Grid2D<T> grid = new Grid2D<T> ();
			Vector vector;
			Rotation? rot;
			Vector key;
			for (int i = 0; i < points.Count - 1; i++) {
				vector = points[i + 1] - points[i];
				rot = vector.Orientation;
				if (rot == null)
					return null;
				for (int j = -e; j < 1 + e; j++)
					for (int k = -e; k <= (int)vector.Magnitude + e; k++) {
						key = points[i] + j * (Vector)(Rotation.Right + (Rotation)rot) + k * (Vector)rot;
						grid.Add (key, construct (key));
					}
			}
			return grid;
		}

		public static Grid2D<T> Merge (List<Grid2D<T>> grids, Func<T, T, T> rule = null) {
			Grid2D<T> grid = new Grid2D<T> ();
			foreach (Grid2D<T> g in grids)
				grid.MergeWith (g, rule: rule, inplace: true);
			return grid;
		}

		public static Grid2D<T> Merge (Grid2D<T> first, Grid2D<T> second, Func<T, T, T> rule = null) {
			return first.MergeWith (second, rule, inplace: false);
		}

		public static List<Grid2D<T>> GetConnectedComponents (Grid2D<T> grid, Func<Dictionary<Vector, T>, Vector, T, bool> rule = null) {
			List<Grid2D<T>> grids = new List<Grid2D<T>> ();
			for (int i = 0; i < 1000; i++) {
				if (grid.Elements.Count == 0)
					return grids;
				grids.Add (grid.ExtractConnected (grid.Elements.Keys.First (), rule));
			}
			return grids;
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public T Get (Vector key, T d = default(T)) {
			try {
				return Elements[key];
			}
			catch (KeyNotFoundException) { }
			catch (ArgumentException) { }
			return d;
		}

		public void Add (Vector key, T value) {
			Elements[key] = value;
		}

		public void Remove (Vector key) {
			if (Elements.ContainsKey (key))
				Elements.Remove (key);
		}

		public Grid2D<T> MergeWith (Grid2D<T> other, Func<T, T, T> rule = null, bool inplace = true) {
			if (rule == null)
				rule = (t1, t2) => t1;
			Grid2D<T> grid;
			if (inplace)
				grid = this;
			else
				grid = new Grid2D<T> (Elements);
			foreach (KeyValuePair<Vector, T> item in other.Elements) {
				if (Elements.ContainsKey (item.Key))
					grid.Elements[item.Key] = rule (Elements[item.Key], item.Value);
				else
					grid.Elements[item.Key] = item.Value;
			}
			return grid;
		}

		public void Remove (Grid2D<T> other) {
			foreach (Vector key in other.Elements.Keys)
				Remove (key);
		}

		public Grid2D<T> GetConnected (Vector key, Func<Dictionary<Vector, T>, Vector, T, bool> rule = null) {
			T value = Elements[key];
			Dictionary<Vector, T> dict = new Dictionary<Vector, T> (Elements);
			dict.Remove (key);
			Grid2D<T> grid = new Grid2D<T> ();
			grid.Add (key, value);
			if (rule == null)
				rule = (e, k, v) => e.ContainsKey (k);
			List<Vector> vectors = new List<Vector> () { key };
			Vector[] keys;
			for (int i = 0; i < 1000; i++) {
				if (vectors.Count == 0)
					return grid;
				keys = vectors.ToArray ();
				foreach (var v in keys) {
					foreach (var d in Vector.Directions)
						if (rule (dict, v + d, value)) {
							vectors.Add (v + d);
							grid.Add (v + d, dict[v + d]);
							dict.Remove (v + d);
						}
					vectors.Remove (v);
				}

			}
			return grid;
		}

		public Grid2D<T> ExtractConnected (Vector key, Func<Dictionary<Vector, T>, Vector, T, bool> rule = null) {
			T value = Elements[key];
			Elements.Remove (key);
			Grid2D<T> grid = new Grid2D<T> ();
			grid.Add (key, value);
			if (rule == null)
				rule = (e, k, v) => e.ContainsKey (k);
			List<Vector> vectors = new List<Vector> () { key };
			Vector[] keys;
			for (int i = 0; i < 1000; i++) {
				if (vectors.Count == 0)
					return grid;
				keys = vectors.ToArray ();
				foreach (var v in keys) { 
					foreach (var d in Vector.Directions) 
						if (rule (Elements, v + d, value)) {
							vectors.Add (v + d);
							grid.Add (v + d, Elements[v + d]);
							Elements.Remove (v + d);
						}
					vectors.Remove (v);
				}

			}
			return grid;
		}

		public List<Vector> IntersectWith (Grid2D<T> other, Func<T, T, bool> rule = null) {
			if (rule == null)
				rule = (x, y) => true;
			List<Vector> vectors = new List<Vector> ();
			foreach (Vector k in Elements.Keys)
				if (other.Elements.Keys.Contains (k) && rule (Elements[k], other.Elements[k]))
					vectors.Add (k);
			return vectors;
		}
		#endregion
	}

	public class Grid2DList<T> : Grid2D<List<T>> {

		#region PUBLIC FUNCTIONS
		public T Get (Vector key, int index, T d = default (T)) {
			try {
				return Elements[key][index];
			}
			catch (ArgumentException) { }
			catch (IndexOutOfRangeException) { }
			return d;
		}

		public void Add (Vector key, T value) {
			if (Elements.ContainsKey (key))
				Elements[key].Add (value);
			else
				Elements.Add (key, new List<T> () { value });
		}
		#endregion
	}
	#endregion

	#region GRAPH
	public class Graph {

		#region PROPERTIES
		public List<Vector> Vertices { get; private set; }
		public List<Edge> Edges { get; private set; }
		#endregion

		#region CONSTRUCTOR
		public Graph () {
			Vertices = new List<Vector> ();
			Edges = new List<Edge> ();
		}

		public Graph (List<Vector> vertices, List<Edge> edges) {
			Vertices = vertices.Distinct ().ToList ();
			Edges = edges.Distinct ().ToList ();
		}

		public Graph (List<Triangle> triangles) {
			Vertices = new List<Vector> ();
			Edges = new List<Edge> ();
			foreach (Triangle t in triangles) {
				Vertices.Add (t.A);
				Vertices.Add (t.B);
				Vertices.Add (t.C);
				Edges.Add (new Edge (t.A, t.B));
				Edges.Add (new Edge (t.B, t.C));
				Edges.Add (new Edge (t.C, t.A));
			}
			Vertices = Vertices.Distinct ().ToList ();
			Edges = Edges.Distinct ().ToList ();
		}
		#endregion

		#region STATIC FUNCTIONS
		public static List<Triangle> Delaunay (List<Vector> points) {

			if (points.Count < 3) {
				throw new ArgumentException ("Cannot triangulate less than 3 vertices!");
			}

			List<Triangle> triangles = new List<Triangle> ();
			Triangle super = SuperTriangle (points);
			triangles.Add (super);

			List<Edge> t_edges;
			for (int i = 0; i < points.Count; i++) {
				t_edges = new List<Edge> ();
				for (int j = triangles.Count - 1; j >= 0; j--) {
					if (triangles[j].ContainsInCircumcircle (points[i])) {
						t_edges.Add (new Edge (triangles[j].A, triangles[j].B));
						t_edges.Add (new Edge (triangles[j].B, triangles[j].C));
						t_edges.Add (new Edge (triangles[j].C, triangles[j].A));
						triangles.RemoveAt (j);
					}
				}

				for (int j = t_edges.Count - 2; j >= 0; j--) {
					for (int k = t_edges.Count - 1; k >= j + 1; k--) {
						if (t_edges[j] == t_edges[k]) {
							t_edges.RemoveAt (k);
							t_edges.RemoveAt (j);
							k--;
							continue;
						}
					}
				}

				for (int j = 0; j < t_edges.Count; j++) {
					triangles.Add (new Triangle (t_edges[j].Start, t_edges[j].End, points[i]));
				}
			}

			for (int i = triangles.Count - 1; i >= 0; i--) {
				if (triangles[i].SharesVertexWith (super)) triangles.RemoveAt (i);
			}

			return triangles;
		}

		public static Triangle SuperTriangle (List<Vector> points) {
			int m = Mathf.Max (points.Max (p => p.X), points.Max (p => p.Y));
			return new Triangle (new Vector (10 * m, 0), new Vector (0, 10 * m), new Vector (-10 * m, -10 * m));
		}


		public static Graph Kruskal (Graph graph, float percent = 0f) {

			graph.OrderEdges ();
			List<Edge> edges = new List<Edge> ();
			List<Edge> unused = new List<Edge> ();

			DisjointSetTree tree = new DisjointSetTree ();
			tree.MakeSets (graph.Vertices);

			foreach (Edge e in graph.Edges) {
				if (tree.Get (e.Start).Union (tree.Get (e.End)))
					edges.Add (e);
				else
					unused.Add (e);
			}

			edges.AddRange (unused.OrderBy (x => UnityEngine.Random.value).Take ((int)(percent * unused.Count)));


			return new Graph (graph.Vertices, edges);
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public void OrderEdges () {
			Edges = Edges.OrderBy (e => e.Length).ToList ();
		}
		#endregion
	}
	#endregion

	#region DISJOINT SET
	public class DisjointSet {

		#region PROPERTIES
		public Vector Point { get; private set; }
		public DisjointSet Parent { get; private set; }
		public DisjointSet Root {
			get {
				if (Parent != this)
					return Parent.Root;
				return this;
			}
		}
		#endregion

		#region CONSTRUCTOR
		public DisjointSet (Vector point) {
			Point = point;
			Parent = this;
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public bool Union (DisjointSet other) {
			DisjointSet root = Root;
			DisjointSet otherRoot = other.Root;
			if (root != otherRoot) {
				otherRoot.Parent = root;
				return true;
			}
			return false;
		}
		#endregion

		#region OVERRIDE
		public override int GetHashCode () {
			return base.GetHashCode ();
		}

		public override bool Equals (object other) {
			return this == (DisjointSet)other;
		}
		#endregion

		#region OPERATORS
		public static bool operator == (DisjointSet left, DisjointSet right) {
			if (((object)left) == ((object)right))
				return true;

			if ((((object)left) == null) || (((object)right) == null))
				return false;

			if (left.Point == right.Point)
				return true;

			return false;
		}

		public static bool operator != (DisjointSet left, DisjointSet right) {
			return !(left == right);
		}
		#endregion
	}

	public class DisjointSetTree {

		#region PUBLIC VARIABLES
		public Dictionary<Vector, DisjointSet> Sets;
		#endregion

		#region CONSTRUCTOR
		public DisjointSetTree () {
			Sets = new Dictionary<Vector, DisjointSet> ();
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public void MakeSet (Vector point) {
			DisjointSet set = new DisjointSet (point);
			if (!Sets.ContainsKey (set.Point))
				Sets.Add (set.Point, set);
		}

		public void MakeSets (List<Vector> points) {
			foreach (Vector point in points)
				MakeSet (point);
		}

		public DisjointSet Get (Vector point) {
			try {
				return Sets[point];
			}
			catch (ArgumentException) { }
			finally { }
			return null;
		}
		#endregion
	}
	#endregion

	#region TRIANGLE
	public struct Triangle {

		#region PROPERTIES
		public Vector A { get; private set; }
		public Vector B { get; private set; }
		public Vector C { get; private set; }
		public List<Vector> Points {
			get {
				return new List<Vector> () { A, B, C };
			}
		}
		public List<Edge> Edges {
			get {
				return new List<Edge> () { new Edge (A, B), new Edge (B, C), new Edge (C, A) };
			}
		}
		#endregion

		#region CONSTRUCTOR
		public Triangle (Vector a, Vector b, Vector c) {
			// Counterclockwise order.
			A = a;
			if ((b - a).Cross (c - a) > 0) {
				B = b;
				C = c;
			}
			else {
				B = c;
				C = b;
			}
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public bool ContainsInCircumcircle (Vector p) {
			Vector ap = A - p;
			Vector bp = B - p;
			Vector cp = C - p;

			long abc = (long)ap.X * (long)bp.Y * cp.SquareMagnitude;
			long cab = (long)cp.X * (long)ap.Y * bp.SquareMagnitude;
			long bca = (long)bp.X * (long)cp.Y * ap.SquareMagnitude;

			long cba = (long)cp.X * (long)bp.Y * ap.SquareMagnitude;
			long acb = (long)ap.X * (long)cp.Y * bp.SquareMagnitude;
			long bac = (long)bp.X * (long)ap.Y * cp.SquareMagnitude;

			return abc + cab + bca - cba - acb - bac > 0;
		}

		public bool SharesVertexWith (Triangle other) {
			if (A == other.A || A == other.B || A == other.C)
				return true;
			if (B == other.A || B == other.B || B == other.C)
				return true;
			if (C == other.A || C == other.B || C == other.C)
				return true;
			return false;
		}
		#endregion
	}
	#endregion

	#region EDGE
	public struct Edge {

		#region PROPERTIES
		public Vector Start { get; private set; }
		public Vector End { get; private set; }
		public List<Vector> Points {
			get {
				return new List<Vector> () { Start, End };
			}
		}
		public double Length {
			get {
				return (End - Start).Magnitude;
			}
		}
		#endregion

		#region CONSTRUCTOR
		public Edge (Vector start, Vector end) {
			Start = start;
			End = end;
		}
		#endregion

		#region OVERRIDE
		public override int GetHashCode () {
			return base.GetHashCode ();
		}

		public override bool Equals (object obj) {
			return this == (Edge)obj;
		}
		#endregion

		#region OPERATORS
		public static bool operator == (Edge left, Edge right) {
			if (((object)left) == ((object)right)) {
				return true;
			}

			if ((((object)left) == null) || (((object)right) == null)) {
				return false;
			}

			return ((left.Start == right.Start && left.End == right.End) ||
					 (left.Start == right.End && left.End == right.Start));
		}

		public static bool operator != (Edge left, Edge right) {
			return left != right;
		}
		#endregion
	}
	#endregion

	#region RECT
	#region STRUCT
	public struct Rect {

		#region PROPERTIES
		public Vector2 Position { get; private set; }
		public Vector Size { get; private set; }
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
		public int Area {
			get {
				return Size.X * Size.Y;
			}
		}
		#endregion

		#region CONSTRUCTOR
		public Rect (Vector min, Vector max) {
			Position = (Vector2)(min + max) / 2f;
			Size = max - min;
		}

		public Rect (Vector2 pos, Vector size) {
			Position = pos;
			Size = size;
		}
		#endregion

		#region STATIC FUNCTIONS
		public static Rect GetRandom (Func<Vector2> pos = null, Func<Vector> size = null) {
			if (pos == null)
				pos = () => (Vector2)Vector.GetRandom (-5, 5);
			if (size == null)
				size = () => Vector.GetRandom (2, 5);
			return new Rect (pos (), size ());
		}
		#endregion
	}
	#endregion
	#endregion

	#region VECTOR
	#region STRUCT
	[Serializable]
	public struct Vector {

		#region PROPERTIES
		public int X { get; private set; }
		public int Y { get; private set; }
		public Rotation? Orientation {
			get {
				if (X == 0) {
					if (Y > 0)
						return Rotation.Up;
					else
						return Rotation.Down;
				}
				if (Y == 0) {
					if (X > 0)
						return Rotation.Right;
					else
						return Rotation.Left;
				}
				return null;
			}
		}
		public long SquareMagnitude {
			get {
				return Dot (this, this);
			}
		}
		public double Magnitude {
			get {
				return Mathf.Sqrt (SquareMagnitude);
			}
		}
		#endregion

		#region STATIC PROPERTIES
		public static Vector Zero {
			get {
				return new Vector (0, 0);
			}
		}
		public static Vector One {
			get {
				return new Vector (1, 1);
			}
		}
		public static Vector Up {
			get {
				return new Vector (0, 1);
			}
		}
		public static Vector Right {
			get {
				return new Vector (1, 0);
			}
		}
		public static Vector Down {
			get {
				return new Vector (0, -1);
			}
		}
		public static Vector Left {
			get {
				return new Vector (-1, 0);
			}
		}
		public static List<Vector> Directions {
			get {
				return new List<Vector> () { Up, Right, Down, Left };
			}
		}
		public static List<Vector> Neighbors {
			get {
				return new List<Vector> () { Up, Up + Right, Right, Right + Down, Down, Down + Left, Left, Left + Up };
			}
		}
		#endregion

		#region CONSTRUCTOR
		public Vector (int x, int y) {
			X = x;
			Y = y;
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public Vector Min (Vector other) {
			return new Vector (Math.Min (X, other.X), Math.Min (Y, other.Y));
		}

		public Vector Max (Vector other) {
			return new Vector (Math.Max (X, other.X), Math.Max (Y, other.Y));
		}

		public Vector Min (int other) {
			return new Vector (Math.Min (X, other), Math.Min (Y, other));
		}

		public Vector Max (int other) {
			return new Vector (Math.Max (X, other), Math.Max (Y, other));
		}

		public long Dot (Vector other) {
			return (long)X * (long)other.X + (long)Y * (long)other.Y;
		}

		public long Cross (Vector other) {
			return (long)X * (long)other.Y - (long)Y * (long)other.X;
		}
		#endregion

		#region STATIC FUNCTIONS
		public static Vector Min (Vector left, Vector right) {
			return left.Min (right);
		}

		public static Vector Max (Vector left, Vector right) {
			return left.Max (right);
		}

		public static Vector Min (Vector vector, int i) {
			return vector.Min (i);
		}

		public static Vector Max (Vector vector, int i) {
			return vector.Max (i);
		}

		public static long Dot (Vector left, Vector right) {
			return left.Dot (right);
		}

		public static long Cross (Vector left, Vector right) {
			return left.Cross (right);
		}

		public static Vector GetRandom (int range) {
			return new Vector (Random.Range (0, range), Random.Range (0, range));
		}

		public static Vector GetRandom (int min, int max) {
			return new Vector (Random.Range (min, max), Random.Range (min, max));
		}

		public static Vector GetRandom (Vector min, Vector max) {
			return new Vector (Random.Range (min.X, max.X), Random.Range (min.Y, max.Y));
		}

		public static List<Vector> GetRandom (int min, int max, int amount) {
			List<Vector> vectors = new List<Vector> ();
			for (int i = 0; i < amount; i++)
				vectors.Add (GetRandom (min, max));
			return vectors;
		}

		public static List<Vector> GetRandom (Vector min, Vector max, int amount) {
			List<Vector> vectors = new List<Vector> ();
			for (int i = 0; i < amount; i++)
				vectors.Add (GetRandom (min, max));
			return vectors;
		}

		public static Vector GetRandomInCircle (Func<float> radius = null) {
			if (radius == null)
				radius = () => Random.Range (0f, 5f);
			float r = radius ();
			float a = Random.Range (0f, 2f * Mathf.PI);
			return new Vector ((int)(r * Mathf.Cos (a)), (int)(r * Mathf.Sin (a)));
		}

		public static List<Vector> Rect (Vector pos, Vector size) {
			List<Vector> vectors = new List<Vector> ();
			for (int i = 0; i < size.X; i++)
				for (int j = 0; j < size.Y; j++)
					vectors.Add (pos + new Vector (i, j));
			return vectors;
		}
		#endregion

		#region OVERRIDE
		public override bool Equals (object obj) {
			if (obj.GetType () == typeof (Vector))
				return this == (Vector)obj;
			return false;
		}

		public override int GetHashCode () {
			return base.GetHashCode ();
		}
		#endregion

		#region OPERATORS
		public static bool operator == (Vector left, Vector right) {
			if (((object)left) == null || ((object)right) == null)
				return false;
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator != (Vector left, Vector right) {
			return !(left == right);
		}

		public static bool operator < (Vector left, Vector right) {
			return (left.X < right.X && left.Y < Right.Y);
		}

		public static bool operator > (Vector left, Vector right) {
			return right < left;
		}

		public static bool operator <= (Vector left, Vector right) {
			return (left.X <= right.X && left.Y <= Right.Y);
		}

		public static bool operator >= (Vector left, Vector right) {
			return right <= left;
		}

		public static bool operator < (Vector left, int right) {
			return (left.X < right && left.Y < right);
		}

		public static bool operator < (int left, Vector right) {
			return (left < right.X && left < right.Y);
		}

		public static bool operator > (Vector left, int right) {
			return right < left;
		}

		public static bool operator > (int left, Vector right) {
			return right < left;
		}

		public static bool operator <= (Vector left, int right) {
			return (left.X <= right && left.Y <= right);
		}

		public static bool operator <= (int left, Vector right) {
			return (left <= right.X && left <= right.Y);
		}

		public static bool operator >= (Vector left, int right) {
			return right <= left;
		}

		public static bool operator >= (int left, Vector right) {
			return right <= left;
		}

		public static Vector operator + (Vector left, Vector right) {
			return new Vector (left.X + right.X, left.Y + right.Y);
		}

		public static Vector operator - (Vector vector) {
			return new Vector (-vector.X, -vector.Y);
		}

		public static Vector operator - (Vector left, Vector right) {
			return new Vector (left.X - right.X, left.Y - right.Y);
		}

		public static Vector operator * (Vector left, Vector right) {
			return new Vector (left.X * right.X, left.Y * right.Y);
		}

		public static Vector operator * (int scalar, Vector vector) {
			return new Vector (scalar * vector.X, scalar * vector.Y);
		}

		public static Vector operator * (Vector vector, int scalar) {
			return new Vector (scalar * vector.X, scalar * vector.Y);
		}

		public static Vector operator * (Rotation rotation, Vector vector) {
			switch (rotation.Direction) {
				case Direction.Right:
					return new Vector (vector.Y, -vector.X);
				case Direction.Down:
					return -vector;
				case Direction.Left:
					return new Vector (-vector.Y, vector.X);
				default:
					return vector;
			}
		}

		public static explicit operator Vector2 (Vector vector) {
			return new Vector2 (vector.X, vector.Y);
		}

		public static explicit operator Vector3 (Vector vector) {
			return new Vector3 (vector.X, 0, vector.Y);
		}

		public static implicit operator Vector2Int (Vector vector) {
			return new Vector2Int (vector.X, vector.Y);
		}

		public static implicit operator Vector (Vector2Int vector) {
			return new Vector (vector.x, vector.y);
		}
		#endregion
		#endregion
	}
	#endregion

	#region ROTATION
	#region ENUMS
	public enum Direction { Up = 0, Right, Down, Left }
	#endregion

	#region STRUCT
	[Serializable]
	public struct Rotation {

		#region PROPERTIES
		public Direction Direction { get; private set; }
		public Vector Vector {
			get {
				switch (Direction) {
					case Direction.Right:
						return Vector.Right;
					case Direction.Down:
						return Vector.Down;
					case Direction.Left:
						return Vector.Left;
					default:
						return Vector.Up;
				}
			}
		}
		public Quaternion Quaternion {
			get {
				return Quaternion.Euler (0, 90 * (int)Direction, 0);
			}
		}
		public Rotation Upward {
			get {
				return this + Up;
			}
		}
		public Rotation Rightward {
			get {
				return this + Right;
			}
		}
		public Rotation Downward {
			get {
				return this + Down;
			}
		}
		public Rotation Leftward {
			get {
				return this + Left;
			}
		}
		#endregion

		#region STATIC PROPERTIES
		public static Rotation Up {
			get {
				return new Rotation (Direction.Up);
			}
		}
		public static Rotation Right {
			get {
				return new Rotation (Direction.Right);
			}
		}
		public static Rotation Down {
			get {
				return new Rotation (Direction.Down);
			}
		}
		public static Rotation Left {
			get {
				return new Rotation (Direction.Left);
			}
		}
		public static Rotation[] All {
			get {
				return new Rotation[] { Up, Right, Down, Left };
			}
		}
		public static Rotation Random {
			get {
				return new Rotation (UnityEngine.Random.Range (0, 4));
			}
		}
		#endregion

		#region CONSTRUCTOR
		public Rotation (Direction direction) {
			Direction = direction;
		}

		public Rotation (int direction) {
			Direction = (Direction)Mod (direction, 4);
		}
		#endregion

		#region PRIVATE STATIC FUNCTIONS
		private static int Mod (int x, int m) {
			if (m < 0)
				m = -m;
			return ((x % m) + m) % m;
		}
		#endregion

		#region OVERRIDE
		public override bool Equals (object obj) {
			if (obj.GetType () == typeof (Rotation))
				return this == (Rotation)obj;
			return false;
		}

		public override int GetHashCode () {
			return base.GetHashCode ();
		}
		#endregion

		#region OPERATORS
		public static bool operator == (Rotation left, Rotation right) {
			if (((object)left) == null || ((object)right) == null)
				return false;
			return left.Direction == right.Direction;
		}

		public static bool operator != (Rotation left, Rotation right) {
			return !(left == right);
		}

		public static Rotation operator + (Rotation left, Rotation right) {
			return new Rotation ((int)left.Direction + (int)right.Direction);
		}

		public static Rotation operator - (Rotation rotation) {
			return new Rotation (-(int)rotation.Direction);
		}

		public static Rotation operator - (Rotation left, Rotation right) {
			return new Rotation ((int)left.Direction - (int)right.Direction);
		}

		public static explicit operator Vector (Rotation rotation) {
			return rotation.Vector;
		}

		public static explicit operator Vector2 (Rotation rotation) {
			return (Vector2)rotation.Vector;
		}

		public static explicit operator Vector3 (Rotation rotation) {
			return (Vector3)rotation.Vector;
		}

		public static explicit operator Quaternion (Rotation rotation) {
			return rotation.Quaternion;
		}
		#endregion
	}
	#endregion
	#endregion

	#region EXTENSIONS
	public static class RandomVector2 {

		public static Vector2 GetRandom (float min, float max) {
			return new Vector2 (Random.Range (min, max), Random.Range (min, max));
		}

		public static Vector2 GetRandom (Vector2 min, Vector2 max) {
			return new Vector2 (Random.Range (min.x, max.x), Random.Range (min.y, max.y));
		}

		public static Vector2 GetRandomInCircle (Func<float> radius) {
			if (radius == null)
				radius = () => Random.Range (0f, 5f);
			float r = radius ();
			float a = Random.Range (0f, 2f * Mathf.PI);
			return new Vector2 (r * Mathf.Cos (a), r * Mathf.Sin (a));
		}

	}
	#endregion
}
