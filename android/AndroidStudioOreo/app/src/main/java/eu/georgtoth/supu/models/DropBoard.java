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

import com.google.gson.annotations.JsonAdapter;

import org.jetbrains.annotations.Nullable;

import java.io.Serializable;
import java.security.InvalidAlgorithmParameterException;
import java.util.ArrayList;
// import eu.georgtoth.supu.enums.BOARDCOL;
import eu.georgtoth.supu.enums.COLUMN;
import eu.georgtoth.supu.enums.FIELDSTATE;
import eu.georgtoth.supu.enums.GEMCOLORS;
import eu.georgtoth.supu.exceptions.BoardAccessorOutOfBoundsException;
import eu.georgtoth.supu.util.Constants;

/**
 * DropBoard entity
 *
 * DropBoard represents the data abstraction of Supu drop board
 *
 */
public class DropBoard extends Board implements Serializable {

    // Already implemented in Board
    // private Context context;

    /**
     * field always indexes field[column][row]
     */
    public SupuStone[][] dropStones;
	public int[] dropCount;

    // dimension already implemented in Board
    // public int dimension = 10;

    /**
     * constructor
     *  initialize all fields of the board
     *
     * @param aContext (required Nullable) to avoid gson androidx serialization
     */
    public DropBoard(@Nullable Context aContext) {
        this(aContext, 10);
    }

    /**
     * constructor
     *  initialize all fields of the board
     *
     * @param aContext (required Nullable) to avoid gson androidx serialization
     * @param level (required NonNull) application context
     */
    public DropBoard(@Nullable Context aContext, int level) {
        super(aContext, level);

        dropStones = new SupuStone[dimension][dimension];
        dropCount = new int[dimension];
		for (int ix = 0; ix < dimension; ix++) {
			dropCount[ix] = 0;
		}
        
    }

    /**
     * getField
     *
     * @param idxColumn the column indexer of our game board, e.g. A, B, C, D, E, F, G, ...
     * @throws BoardAccessorOutOfBoundsException
     * @return SupuStone field reference, that is in the board at col,row index
     */
    public SupuStone getField(COLUMN idxColumn) throws BoardAccessorOutOfBoundsException {
		int sCol = idxColumn.getValue();
        if (idxColumn == COLUMN.NONE || idxColumn.charUpper() == (char)'\0' || (int)sCol >= dimension) {
            throw (BoardAccessorOutOfBoundsException)(new Throwable("Column \'" + idxColumn.getName() + "\' failed ti set!"));
        }

        int iCol = idxColumn.getValue();
		int iStack = dropCount[sCol];
		dropCount[sCol]--;
        return  dropStones[iCol][iStack];
    }

    /**
     * setStone
     * @param sColumn (link @COLUMN) the column indexer of our game board, e.g. A, B, C, D, E, F, G, ...
     * @param setStone field to set
     * @return refField SupuStone field reference
     *                 null, if basic set operation failed
     *                 a field with same color inside cross row or cross column
     *                 the field, after the new gemColor has been set there
     * @throws IllegalArgumentException
     */
    public SupuStone setStone(COLUMN sColumn, SupuStone setStone) throws RuntimeException {
        SupuStone retField = null;
		int sCol = sColumn.getValue();
		if (sColumn == COLUMN.NONE || setStone == null  || (int)sCol >= dimension) {
			throw (IllegalArgumentException) (new Throwable("Column or field can't be NULL"));
		}

		dropStones[sCol][dropCount[sCol]] = setStone;
		dropCount[sCol]++;
		retField = setStone;
        return retField;
    }


}
