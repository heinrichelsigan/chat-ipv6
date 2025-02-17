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

package eu.georgtoth.supu.enums;

import android.graphics.Point;
import android.util.Size;

import java.io.Serializable;

/**
 * XSIZE enum for defined matching and mismatching rules for supu game
 */
public enum XSIZE implements Serializable {
	ATO(1),
	FEMTO(2),
	PICO(3),
	NANO(4),
	MICRO(5),
    TINY(6),
	MILLI(7),
	SMALL(8),
	MEDIUM(9),
	LARGE(10),
	XLARGE(11),
	HUGE(12),
	UHUGE(13),
	ZETTA(14),
	YOTTA(15);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    XSIZE(int value) {
        this.value = value;
    }

    private final int value;

    /**
     * getValue
     * @return (@link int) value
     */
    public int getValue() { return value; }
	
	/**
     * getWidth
     * @return (@link int) value
     */
	public int getWidth() { return ((int)value * 4); }
	
	/**
     * getHeight
     * @return (@link int) value
     */
	public int getHeight() { return ((int)value * 3); }
	
	
	/**
     * getSize
     * @return (@link android.util.Size) size
     */	
	public android.util.Size getSize() { return new android.util.Size(((int)value * 4), ((int)value * 3)); }

	/**
	 * getPoint
	 * @return (@link android.graphics.Point) point
	 */
	public android.graphics.Point getPoint() { return new android.graphics.Point(((int)value * 4), ((int)value * 3)); }

	/*
		public static Size GetSize(this XSIZE xsz) => new Size(xsz.Width(), xsz.Height());

        public static string XSize(this XSIZE xsz) => $"{xsz.Width()}x{xsz.Height()}";

        public static string UXSize(this XSIZE xsz) => ((xsz == XSIZE.MEDIUM) ? "" : $"_{xsz.Width()}x{xsz.Height()}");

        public static long GetArea(this XSIZE xsz) => (long)(xsz.ToInt() * xsz.ToInt() * 12);
	 */
}

