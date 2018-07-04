using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LoG.Math2Int;
using Random = UnityEngine.Random;

namespace LoG.Map {

	#region MAP NODE
	#region ENUMS
	public enum DecoType { None = 0, FloorDeco, WallDeco }
	#endregion

	public class DecorationNode {

		#region PROPERTIES
		public DecoType Type { get; private set; }
		public Vector Position { get; private set; }
		public Rotation Rotation { get; private set; }
		public Map Map { get; private set; }
		public DecorationObject Object { get; set; }
		#endregion

		#region CONSTRUCTOR
		public DecorationNode (DecoType type, Vector pos, Rotation rot, Map map) {
			Type = type;
			Position = pos;
			Rotation = rot;
			Map = map;
		}
		#endregion

		#region STATIC FUNCTIONS
		public static DecorationNode FloorDeco (Vector pos, Rotation rot, Map map) {
			return new DecorationNode (DecoType.FloorDeco, pos, rot, map);
		}

		public static DecorationNode WallDeco (Vector pos, Rotation rot, Map map) {
			return new DecorationNode (DecoType.WallDeco, pos, rot, map);
		}
		#endregion
	}
	#endregion
}