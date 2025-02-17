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

package eu.georgtoth.supu.exceptions;

// import eu.georgtoth.supu.constants.BOARDCOL;
import eu.georgtoth.supu.enums.COLUMN;

import java.io.Serializable;
import java.lang.ArrayIndexOutOfBoundsException;
import java.lang.IndexOutOfBoundsException;

/**
 * BoardIndexOutOfBoundsException extends {@link ArrayIndexOutOfBoundsException}
 * is thrown, when column {@link COLUMN} and row {@link Integer} are out of board bounces!
 */
public class BoardIndexOutOfBoundsException extends ArrayIndexOutOfBoundsException implements Serializable  {

	/**
	 * calling ctor in super {@link ArrayIndexOutOfBoundsException(String)}
	 * @param sMessage {@link String} for exception message
	 */
	public BoardIndexOutOfBoundsException(String sMessage) {
		super(sMessage);
	}

	/**
	 * standard ctor called before {@link #initCause(Throwable throwable)}
	 * @param message {@link String} for exception
	 * @param throwable {@link Throwable} inner exception
	 */
	public BoardIndexOutOfBoundsException(String message, Throwable throwable) {
		this(message);
		super.initCause(throwable);
	}

	/**
	 * constructor with (@link IndexOutOfBoundsException) as {@link Throwable} inner exception
	 * @param column board {@link COLUMN} column indexer
	 * @param row    board {@link Integer} row indexer
	 */
	public BoardIndexOutOfBoundsException(COLUMN column, int row) {
		this("accessor out of board area at field: " + column.getName() + row + ".",
			((IndexOutOfBoundsException) 
				(new Throwable("out of bounds at colunn=" + column.getValue() + ", row=" + row))));
	}
}