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

import eu.georgtoth.supu.enums.BOARDCOL;
import eu.georgtoth.supu.enums.COLUMN;

import java.io.Serializable;
import java.lang.ArrayIndexOutOfBoundsException;
import java.lang.IndexOutOfBoundsException;

/**
 * BoardAccessorOutOfBoundsException extends {@link java.lang.ArrayIndexOutOfBoundsException}
 * {@link BoardAccessorOutOfBoundsException} is thrown,
 *  when column {@link COLUMN} and row {@link Integer} are out of board bounces!
 */
public class BoardAccessorOutOfBoundsException extends ArrayIndexOutOfBoundsException implements Serializable  {

	/**
	 * constructor
	 * @param sMessage {@link String} for exception sMessage
	 * calling {@link ArrayIndexOutOfBoundsException(String)}
	 */
	public BoardAccessorOutOfBoundsException(String sMessage) {
		super(sMessage);
	}

	/**
	 * constructor
	 * @param message {@link String} for exception
	 * @param cause {@link Throwable} inner exception
	 * calling {@link java.lang.RuntimeException(String, Throwable)}
	 */
	public BoardAccessorOutOfBoundsException(String message, Throwable cause) {
		this(message);
		super.initCause(cause);
	}

	/**
	 * constructor with inner {@link Throwable} exception (@link #java.lang.IndexOutOfBoundsException) 
	 * @param column board {@link COLUMN} column indexer
	 * @param row    board {@link Integer} row indexer
	 */
	public BoardAccessorOutOfBoundsException(COLUMN column, int row) {
		this("board accessor out of bounce at field: " + column.getName() + "" + row + ".",
				((IndexOutOfBoundsException) (new Throwable("out of bounds at colunn=" + column.getValue() + ", row=" + row))));
	}

}
