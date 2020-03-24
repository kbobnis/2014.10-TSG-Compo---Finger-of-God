using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum Side {
	Up, Down, Left, Right, Center
}

public static class SideMethods {

	public static int DeltaX(this Side s) {
		switch (s) {
			case Side.Left: return -1;
			case Side.Right: return 1;
			default: return 0;
		}
	}

	public static int DeltaY(this Side s) {
		switch (s) {
			case Side.Up: return -1;
			case Side.Down: return 1;
			default: return 0;
		}
	}
}