/**
 * @author           <a href="mailto:heinrich.elsigan@area23.at">Heinrich Elsigan</a>
 * @version          V 1.0.1
 * @since            API 27 Oreo 8.1
 *
 * <p>SUPU is the idea of  by <a href="mailto:Eman@gmx.at">Georg Toth</a>
 * based Sudoku with colors instead of numbers.</p>
 *
 * Coded 2021-2025 by
 * <a href="mailto:he@area23.at">Heinrich.Elsigan</a><a href="https://area23.at">area23.at</a>
 */

package eu.georgtoth.supu.util;

import eu.georgtoth.supu.enums.COLUMN;
import eu.georgtoth.supu.enums.GEMCOLORS;
import eu.georgtoth.supu.enums.SCOLOR;

/**
 * Util provides only static fields
 */
public class Constants {

	//region c constants´
	public final static String STR_NULL = null;
	public final static String STR_EMPTY = "";
	public final static String LF = "\n";
	public final static String CRLF = "\r\n";
	//endregion c constants

	//region application constants
	public final static String APP_NAME = "Supu";
	public final static String SETTINGS_FILENAME = "SupuSets.json";

	public final static int MIN_LEVEL_DEFAULT = 5;
	public final static int MAX_LEVEL_DEFAULT = 10;
	public final static int MAX_CREDITS_PER_ROW = 3;
	public final static int MAX_DROP_FIELDS = 2;

	public final static int ALLOWED_REPETATIVE_PATTERN_FIELDS = 4;
	public final static int ALLOWED_REPETATIVE_PATTERN_AUTO = 3;
	public final static int DENIED_REPETATIVE_PATTERN_3x3 = 9;
	public final static int ALLOWED_COLOR_CHAINED_ROWS = 3;

	public final static int AUTOMATION_DIVISOR = 2;
	public final static int AUTOMATION_SURPLUS_PERFECT = 1;

	public final static boolean ENQ_MSG = false; // false;
	//endregion application constants


	//region predefined arrays
	public static final COLUMN[] allCols = {
			COLUMN.A, COLUMN.B, COLUMN.C, COLUMN.D, COLUMN.E,
			COLUMN.F, COLUMN.G, COLUMN.H, COLUMN.I, COLUMN.J,
			COLUMN.NONE};

	public static final int[] allRows = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, Integer.MAX_VALUE };

	public static final GEMCOLORS[] allColors = {
			GEMCOLORS.a, GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.e,
			GEMCOLORS.f, GEMCOLORS.g, GEMCOLORS.h, GEMCOLORS.i, GEMCOLORS.j,
			GEMCOLORS.k, GEMCOLORS.l, GEMCOLORS.m, GEMCOLORS.n, GEMCOLORS.o,
			GEMCOLORS.p, GEMCOLORS.q, GEMCOLORS.r, GEMCOLORS.s, GEMCOLORS.t,
			GEMCOLORS.u, GEMCOLORS.v, GEMCOLORS.w, GEMCOLORS.x, GEMCOLORS.y,
			GEMCOLORS.z, GEMCOLORS.X, GEMCOLORS.Y, GEMCOLORS.Z
	};

	public static final SCOLOR[] SCOLORS = {
			SCOLOR.a, SCOLOR.b, SCOLOR.c, SCOLOR.d, SCOLOR.e, SCOLOR.f, SCOLOR.g, SCOLOR.h, SCOLOR.i, SCOLOR.j,
			SCOLOR.u, SCOLOR.v, SCOLOR.w, SCOLOR.x, SCOLOR.y, SCOLOR.z, SCOLOR.NONE
	};

	public static final String[] fromViews = {
			"GA0", "GB1", "GC2", "GD3", "GE4", "GF5", "GG6", "GH7", "GI8", "GJ9",
			"ga0", "gb1", "gc2", "gd3", "ge4", "gf5", "gg6", "gh7", "gi8", "gj9",
			"gA0", "gB1", "gC2", "gD3", "gE4", "gF5", "gG6", "gH7", "gI8", "gJ9",
			"Ga0", "Gb1", "Gc2", "Gd3", "Ge4", "Gf5", "Gg6", "Gh7", "Gi8", "Gj9"
	};

	public static final String[] toViews = {
			"a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9",
			"b0", "b1", "b2", "b3", "b4", "b5", "b6", "b7", "b8", "b9",
			"c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9",
			"d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9",
			"e0", "e1", "e2", "e3", "e4", "e5", "e6", "e7", "e8", "e9",
			"f0", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9",
			"g0", "g1", "g2", "g3", "g4", "g5", "g6", "g7", "g8", "g9",
			"h0", "h1", "h2", "h3", "h4", "h5", "h6", "h7", "h8", "h9",
			"i0", "i1", "i2", "i3", "i4", "i5", "i6", "i7", "i8", "i9",
			"j0", "j1", "j2", "j3", "j4", "j5", "j6", "j7", "j8", "j9",
			"A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9",
			"B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9",
			"C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9",
			"D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9",
			"E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9",
			"F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9",
			"G0", "G1", "G2", "G3", "G4", "G5", "G6", "G7", "G8", "G9",
			"H0", "H1", "H2", "H3", "H4", "H5", "H6", "H7", "H8", "H9",
			"I0", "I1", "I2", "I3", "I4", "I5", "I6", "I7", "I8", "I9",
			"J0", "J1", "J2", "J3", "J4", "J5", "J6", "J7", "J8", "J9"
	};

	public static final String[] figures = {
			"gem0a", "gem1a", "gem2a", "gem3a", "gem4a", "gem5a", "gem6a", "gem7a", "gem8a", "gem9a",
			"gem0b", "gem1b", "gem2b", "gem3b", "gem4b", "gem5b", "gem6b", "gem7b", "gem8b", "gem9b",
			"gem0c", "gem1c", "gem2c", "gem3c", "gem4c", "gem5c", "gem6c", "gem7c", "gem8c", "gem9a",
			"gem0d", "gem1d", "gem2d", "gem3d", "gem4d", "gem5d", "gem6d", "gem7d", "gem8d", "gem9d",
			"gem0e", "gem1e", "gem2e", "gem3e", "gem4e", "gem5e", "gem6e", "gem7e", "gem8e", "gem9e",
			"gem0f", "gem1f", "gem2f", "gem3f", "gem4f", "gem5f", "gem6f", "gem7f", "gem8f", "gem9f",
			"gem0g", "gem1g", "gem2g", "gem3g", "gem4g", "gem5g", "gem6g", "gem7g", "gem8g", "gem9g",
			"gem0h", "gem1h", "gem2h", "gem3h", "gem4h", "gem5h", "gem6h", "gem7h", "gem8h", "gem9h",
			"gem0i", "gem1i", "gem2i", "gem3i", "gem4i", "gem5i", "gem6i", "gem7i", "gem8i", "gem9i",
			"gem0j", "gem1j", "gem2j", "gem3j", "gem4j", "gem5j", "gem6j", "gem7j", "gem8j", "gem9j"
	};

	public static final String[] gemIds = {
			"gem0a", "gem1a", "gem2a", "gem3a", "gem4a", "gem5a", "gem6a", "gem7a", "gem8a", "gem9a",
			"gem0b", "gem1b", "gem2b", "gem3b", "gem4b", "gem5b", "gem6b", "gem7b", "gem8b", "gem9b",
			"gem0c", "gem1c", "gem2c", "gem3c", "gem4c", "gem5c", "gem6c", "gem7c", "gem8c", "gem9a",
			"gem0d", "gem1d", "gem2d", "gem3d", "gem4d", "gem5d", "gem6d", "gem7d", "gem8d", "gem9d",
			"gem0e", "gem1e", "gem2e", "gem3e", "gem4e", "gem5e", "gem6e", "gem7e", "gem8e", "gem9e",
			"gem0f", "gem1f", "gem2f", "gem3f", "gem4f", "gem5f", "gem6f", "gem7f", "gem8f", "gem9f",
			"gem0g", "gem1g", "gem2g", "gem3g", "gem4g", "gem5g", "gem6g", "gem7g", "gem8g", "gem9g",
			"gem0h", "gem1h", "gem2h", "gem3h", "gem4h", "gem5h", "gem6h", "gem7h", "gem8h", "gem9h",
			"gem0i", "gem1i", "gem2i", "gem3i", "gem4i", "gem5i", "gem6i", "gem7i", "gem8i", "gem9i",
			"gem0j", "gem1j", "gem2j", "gem3j", "gem4j", "gem5j", "gem6j", "gem7j", "gem8j", "gem9j"
	};

	public static final String[] gemOrders = { "a", "b", "c", "d", "e", "f","g","h", "i", "j" };

	public static final int[] DROPBOARD_ROWENABLED = {
			3,  // Level 1  ( 5x 5) => after completing  3 rows     => 4, 5
			4,  // Level 2  ( 6x 6) => after completing  4 rows     => 5, 6
			4,  // Level 3  ( 7x 7) => after completing  5 rows     => 5, 6, 7
			5,  // Level 4  ( 8x 8) => after completing  5 rows     => 6, 7, 8
			6,  // Level 5  ( 9x 9) => after completing  6 rows     => 7, 8, 9
			7,  // Level 6  (10x10) => after completing  7 rows     => 8, 9, 10
			7,  // Level 7  (11x11) => after completing  7 rows     => 8, 9, 10, 11
			8,  // Level 8  (12x12) => after completing  8 rows     => 9, 10, 11, 12
			9,  // Level 9  (13x13) => after completing  9 rows     => 10, 11, 12, 13
			10, // Level 10 (14x14) => after completing 10 rows     => 11, 12, 13, 14
			11, // Level 11 (15x15) => after completing 11 rows     => 12, 13, 14, 15
			11, // Level 12 (16x16) => after completing 11 rows     => 12, 13, 14, 15, 16
			12, // Level 13 (17x17) => after completing 12 rows     => 13, 14, 15, 16, 17
			13, // Level 14 (18x18) => after completing 13 rows     => 14, 15, 16, 17, 18
			14, // Level 15 (19x19) => after completing 14 rows     => 15, 16, 17, 18, 19
			14, // Level 16 (20x20) => after completing 14 rows     => 15, 16, 17, 18, 19, 20
	};
}

