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

package eu.georgtoth.supu.models;

import android.content.Context;
import android.widget.LinearLayout;
import android.view.View;

import eu.georgtoth.supu.enums.BOARDCOL;
import eu.georgtoth.supu.enums.COLUMN;
import eu.georgtoth.supu.enums.FIELDSTATE;
import eu.georgtoth.supu.enums.GEMCOLORS;
import eu.georgtoth.supu.enums.SUPURULES;
import eu.georgtoth.supu.exceptions.BoardAccessorOutOfBoundsException;

import com.google.gson.annotations.JsonAdapter;

import org.jetbrains.annotations.Nullable;

import java.io.Serializable;

/**
 * BoardField entity
 *
 * BoardField represents a potential field in the board
 *
 */
public class BoardField implements Serializable {

    // implemented without context, because of framework up to androidx.* serializing gson
    // private Context _context;

    public int dimension = 10;
    public int row = -1;
    public COLUMN column = COLUMN.NONE;
    public String fieldName = null;

    /**
     * Default BoardField constructor without any parameters,
     * don't use it!
     */
    public BoardField() { }

    /**
     * constructor
     *
     * @param aContext implemented without application context,
     *                 because of framework up to androidx.* serializing gson
     */
    public BoardField(@Nullable Context aContext) {
        // this._context = aContext;
    }

    /**
     * constructor
     *
     * @param aContext implemented without application context,
     *                 because of framework up to androidx.* serializing gson
     * @param aColumn board column BOARDCOL enum
     * @param aRow board row
     */
    public BoardField(@Nullable Context aContext, int level, COLUMN aColumn, int aRow) {
        // this._context = aContext;
        this.dimension = level;
        this.column = aColumn;
        this.row = aRow;

        try {
            fieldName = this.getName();
        } catch (Exception ex) {
            ex.printStackTrace();
        }
    }

    /**
     * constructor with single BoardField
     *
     * @param aField BoardField
     */
    public BoardField(BoardField aField) {
        if (aField == null)
            return;
        // this._context = aField._context;
        this.dimension = aField.dimension;
        this.row = aField.row;
        this.column = aField.column;
    }


    /**
     * IsValid
     * @return true, if this is a valid field inside the board, otherwise false
     */
    public boolean isValid() {
        return  (column != COLUMN.NONE && row >= 0 && row < dimension);
    }

    /**
     * getName
     * @return 2 character sized String, that contains first column letter and then row number
     * @throws BoardAccessorOutOfBoundsException new BoardAccessorOutOfBoundsException(column, row)
     */
    public String getName() throws BoardAccessorOutOfBoundsException {
        if (!this.isValid()) {
            throw (BoardAccessorOutOfBoundsException)(new BoardAccessorOutOfBoundsException(column, row));
        }
        return (String.valueOf(column.charUpper()) + String.valueOf(row));
    }

    /**
     * getState
     * @return FIELDSTATE
     */
    public FIELDSTATE getState() {
        if (column == COLUMN.NONE || (column.charUpper() == (char)'\0') || row < 0 || row >= dimension) {
            return FIELDSTATE.INVALID;
        }
        return FIELDSTATE.FREE;
    }

}
