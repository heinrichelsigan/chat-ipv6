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
public class StoneBoard extends Board implements Serializable {

    // Already implemented in Board
    // private Context context;

    /**
     * field always indexes field[column][row]
     */
    public SupuStone[] stoneFields;

    // dimension already implemented in Board
    //public int dimension = 10;
    public int reRow = 0;

    /**
     * constructor
     *  initialize all fields of the board
     *
     * @param aContext (required Nullable) to avoid gson serialization of androidx framework over context
     */
    public StoneBoard(@Nullable Context aContext) {
        this(aContext, 10);
    }

    /**
     * constructor
     *  initialize all fields of the board
     *
     * @param aContext (required Nullable) to avoid gson serialization of androidx framework over context
     * @param level (required NonNull) application context
     */
    public StoneBoard(@Nullable Context aContext, int level) {
        super(aContext);

        // this.context = aContext;
        dimension = level;
        reRow = 0;
        fieldIdCnt = 0;
        stoneFields = new SupuStone[dimension];
        for (int stnCol = 0; stnCol < dimension; stnCol++) {
            char gCh = (char) (((int) 'a') + stnCol);
            String figureString = "gam" + String.valueOf(gCh) + String.valueOf(stnCol);
            stoneFields[stnCol] = new SupuStone(aContext, level, Constants.allCols[stnCol], 0,
                    GEMCOLORS.getColorByFigure(figureString), fieldIdCnt++);
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

        if (stoneFields[sCol] == null)
            return null;
        return stoneFields[sCol];
    }

    public SupuStone removeStone(COLUMN idxColumn) throws BoardAccessorOutOfBoundsException {
        int sCol = idxColumn.getValue();
        if (idxColumn == COLUMN.NONE || idxColumn.charUpper() == (char)'\0' || (int)sCol >= dimension) {
            throw (BoardAccessorOutOfBoundsException)(new Throwable("Column \'" + idxColumn.getName() + "\' failed ti set!"));
        }

        if (stoneFields[sCol] == null)
            return null;

        SupuStone retStone = new SupuStone(null, stoneFields[sCol]);
        stoneFields[sCol] = null;

        return retStone;
    }


    /**
     * Checks if StoneBoard is empty
     * @return true, when true, otherwise false
     */
    public boolean isEmpty() {
        for (int fidx = 0; fidx < dimension; fidx++) {
            if (stoneFields[fidx] != null)
                return false;
        }
        return true;
    }

    /**
     *  reFill - refills StoneBoard
     */
    public void reFill() {
        reRow++;
        if (reRow >= dimension) {
            reRow--;
            // return;
        }

        stoneFields = new SupuStone[dimension];
        for (int stnCol = 0; stnCol < dimension; stnCol++) {
            char gCh = (char) (((int) 'a') + stnCol);
            String figureString = "gam" + String.valueOf(gCh) + String.valueOf(stnCol);
            stoneFields[stnCol] = new SupuStone(null, dimension, Constants.allCols[stnCol], 0,
                    GEMCOLORS.getColorByFigure(figureString), fieldIdCnt++);
        }
    }

    /**
     * setGemColor
     * @param sColumn (link @COLUMN) the column indexer of our game board, e.g. A, B, C, D, E, F, G, ...
     * @param setStone refStone as reference for SupuStone to set
     * @return refField SupuStone field reference
     *                 null, if basic set operation failed
     *                 a field with same color inside cross row or cross column
     *                 the field, after the new gemColor has been set there
     * @throws IllegalArgumentException
     */
    public SupuStone setField(COLUMN sColumn, SupuStone setStone) throws RuntimeException {
        SupuStone retField = null;
		int sCol = sColumn.getValue();
        if (sColumn == COLUMN.NONE || sColumn.charUpper() == (char)'\0' || (int)sCol >= dimension) {
            throw (BoardAccessorOutOfBoundsException)(new Throwable("Column \'" + sColumn.getName() + "\' failed ti set!"));
        }
        if (setStone == null) {
            throw (RuntimeException)(new Throwable("Field can't be NULL"));
		}
        if (stoneFields[sCol] != null) {
            throw (RuntimeException)(new Throwable(
                    "Field at " + sColumn.getName() + " already contains stone: " +  stoneFields[sCol].getName()));
        }

        stoneFields[sCol] = setStone;
		retField = setStone;
        return retField;
    }

}
