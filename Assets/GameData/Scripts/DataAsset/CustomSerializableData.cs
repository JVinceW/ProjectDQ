using System;

namespace GameData.DataAsset
{
	/*
	 * We are using the JsonDotnet to serialize Json data. Some may say why not use the JsonUtility to archive it.
	 * Well JsonUtility is good but it's not flexible enough to handle some complex data structure. But JsonDotnet can.
	 * But in the other hands, JsonDotnet is not as fast as JsonUtility, and have some problem with some Looping handling when serialize some Unity structure.
	 * These file is for handle that problem.
	 */
	[Serializable]
	public struct Vector3Serializable : IEquatable<Vector3Serializable>
	{
		public float X;
		public float Y;
		public float Z;
		
		public static Vector3Serializable zero => new Vector3Serializable() {
			X = 0,
			Y = 0,
			Z = 0
		};

		public static Vector3Serializable one => new Vector3Serializable() {
			X = 1,
			Y = 1,
			Z = 1
		};

		public bool Equals(Vector3Serializable other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
		}

		public override bool Equals(object obj)
		{
			return obj is Vector3Serializable other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y, Z);
		}
	}

	[Serializable]
	public struct Vector3IntSerializable : IEquatable<Vector3IntSerializable>
	{
		public int X;
		public int Y;
		public int Z;

		public Vector3IntSerializable(int inX, int inY, int inZ)
		{
			X = inX;
			Y = inY;
			Z = inZ;
		}

		public static Vector3IntSerializable zero => new Vector3IntSerializable() {
			X = 0,
			Y = 0,
			Z = 0
		};

		public static Vector3IntSerializable one => new Vector3IntSerializable() {
			X = 1,
			Y = 1,
			Z = 1
		};

		public bool Equals(Vector3IntSerializable other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public override bool Equals(object obj)
		{
			return obj is Vector3IntSerializable other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y, Z);
		}
	}
}