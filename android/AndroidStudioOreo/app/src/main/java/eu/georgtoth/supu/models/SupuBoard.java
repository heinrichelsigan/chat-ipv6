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
import java.util.HashSet;
// import eu.georgtoth.supu.enums.BOARDCOL;
import eu.georgtoth.supu.enums.COLUMN;
import eu.georgtoth.supu.enums.GEMCOLORS;
import eu.georgtoth.supu.enums.FIELDSTATE;
import eu.georgtoth.supu.enums.SUPURULES;
import eu.georgtoth.supu.exceptions.BoardAccessorOutOfBoundsException;
import eu.georgtoth.supu.util.Constants;

/**
 * SupuBoard entity
 *
 * SupuBoard represents concrete data abstraction of SupuBoard
 * see BoardField 
 *
 */
public class SupuBoard extends Board implements Serializable {

    // Already implemented in Board
    // private Context context;

    public int _rowsFullyCompleted = 0;
	public int currentRow = 0;
    public int rowCounter = 0;
	/**
     * field always indexes field[column][row]
     */
    public SupuStone[][] supuFields;

    public HashSet<SupuStone> _colorChains3x3Container = new HashSet<SupuStone>();
    public HashSet<SupuStone> _currentRowS = new HashSet<SupuStone>();
    public HashSet<SupuStone> _currentColumnS = new HashSet<SupuStone>();

    /**
     * constructor
     *  initialize all fields of the board
     *
     * @param aContext (required Nullable) to avoid gson androidx serialization
     */
    public SupuBoard(@Nullable Context aContext) {
        this(aContext, 10);
    }

    /**
     * constructor
     *  initialize all fields of the board
     *
     * @param aContext (required Nullable) to avoid gson androidx serialization
     * @param level (required NonNull) application context
     */
    public SupuBoard(@Nullable Context aContext, int level) {
        super(aContext, level);
        // this.context = aContext;
        this.dimension = level;

        supuFields= new SupuStone[dimension][dimension];
        for (int rowC = 0; rowC < dimension; rowC++) {
            for (int colC = 0; colC < dimension; colC++) {
                COLUMN aCol = Constants.allCols[colC];
                supuFields[colC][rowC] = new SupuStone(aContext, level, aCol, rowC, GEMCOLORS.Z);
            }
        }
    }

    /**
     * rowsFullyCompleted
     * method, that checks, if game is finished and all board fields are filled correctly
     * @return return number of rows fully completed
     */
	public int rowsFullyCompleted() {
        _rowsFullyCompleted = 0;
        for (int rx = 0; rx < this.dimension; rx++) {
			if (isRowCompleted(rx)) {
                _rowsFullyCompleted++;
			}                
        }
        return _rowsFullyCompleted;
	}

    /**
     * isGameFinished
     * method, that checks, if game is finished and all board fields are filled correctly
     * @return returns true, if game finished, otherwise false
     */
    public boolean isGameFinished() {
        int level = 10;
        for (int rowC = 0; rowC < level; rowC++) {
            for (int colC = 0; colC < level; colC++) {
                COLUMN aCol = Constants.allCols[colC];
                if (supuFields[colC][rowC] == null || supuFields[colC][rowC].getState() != FIELDSTATE.COLORSET)
                    return false;
            }
        }

        return true;
    }


    /**
     * isGameFinished
     * method, that checks, if game is finished and all board fields are filled correctly
     * @param level - the level
     * @return returns true, if game finished, otherwise false
     */
    public boolean isGameFinished(int level) {

        for (int rowC = 0; rowC < level; rowC++) {
            for (int colC = 0; colC < level; colC++) {
                COLUMN aCol = Constants.allCols[colC];
                if (supuFields[colC][rowC] == null || supuFields[colC][rowC].getState() != FIELDSTATE.COLORSET)
                    return false;
            }
        }

        return true;
    }

    /**
     * isRowCompleted
     * method, that checks, if row is completed
     * @param level - the level
     * @param rowC - int row number 
     * @return returns true, if game finished, otherwise false
     */
    public boolean isRowCompleted(int level, int rowC) {
        for (int colC = 0; colC < level; colC++) {
            COLUMN aCol = Constants.allCols[colC];
            if (supuFields[colC][rowC] == null || supuFields[colC][rowC].getState() != FIELDSTATE.COLORSET ||
                    supuFields[colC][rowC].color == GEMCOLORS.Z)
                return false;
        }

        return true;
    }

    /**
     * isRowCompleted
     * method, that checks, if row is completed
     * @param rowNumber - int row number
     * @return returns true, if game finished, otherwise false
     */
    public boolean isRowCompleted(int rowNumber) {
        return isRowCompleted(this.dimension, rowNumber);
    }

    /**
     * getField
     *
     * @param idxColumn the column indexer of our game board, e.g. A, B, C, D, E, F, G, ...
     * @param idxRow the row indexer of our game board, e.g. 0, 1, 2, 3, 4, 5, 6, 7, ...
     * @return field reference, that is in the board at col,row index
     * @throws BoardAccessorOutOfBoundsException new Throwable("Column \'" + idxColumn.getName() + "\' failed to set!")
     *         new Throwable("Row \'" + String.valueOf(idxRow) + "\' failed for set!")
     */
    public SupuStone getField(COLUMN idxColumn, int idxRow) throws BoardAccessorOutOfBoundsException {

        if (idxColumn == COLUMN.NONE || idxColumn.charUpper() == (char)'\0') {
            throw (BoardAccessorOutOfBoundsException)(new Throwable("Column \'" + idxColumn.getName() + "\' failed to set!"));
        }
        if (idxRow < 0 || idxRow >= dimension) {
            throw (BoardAccessorOutOfBoundsException)(new Throwable("Row \'" + String.valueOf(idxRow) + "\' failed for set!"));
        }

        int iCol = idxColumn.getValue();
        return  supuFields[iCol][idxRow];
    }

    /**
     * removeStone - removes stone from supuboard
     *
     * @param idxColumn the column indexer of our game board, e.g. A, B, C, D, E, F, G, ...
     * @param idxRow the row indexer of our game board, e.g. 0, 1, 2, 3, 4, 5, 6, 7, ...
     * @return field reference, that is in the board at col,row index
     * @throws BoardAccessorOutOfBoundsException new Throwable("Column \'" + idxColumn.getName() + "\' failed to set!")
     *         new Throwable("Row \'" + String.valueOf(idxRow) + "\' failed for set!")
     */
    public SupuStone removeStone(COLUMN idxColumn, int idxRow) throws BoardAccessorOutOfBoundsException {

        if (idxColumn == COLUMN.NONE || idxColumn.charUpper() == (char)'\0') {
            throw (BoardAccessorOutOfBoundsException)(new Throwable("Column \'" + idxColumn.getName() + "\' failed to set!"));
        }
        if (idxRow < 0 || idxRow >= dimension) {
            throw (BoardAccessorOutOfBoundsException)(new Throwable("Row \'" + String.valueOf(idxRow) + "\' failed for set!"));
        }

        int iCol = idxColumn.getValue();
        SupuStone retField = null;
        if (supuFields[iCol][idxRow] != null) {
            retField = new SupuStone(null, supuFields[iCol][idxRow]);
            // set field on
            supuFields[iCol][idxRow] = new SupuStone(null,
                    dimension, idxColumn, idxRow, GEMCOLORS.Z);
        }

        return retField;
    }

    /**
     * setGemColor
     * @param sColumn the column indexer of our game board, e.g. A, B, C, D, E, F, G, ...
     * @param sRow the row indexer of our game board, e.g. 0, 1, 2, 3, 4, 5, 6, 7, ...
     * @param sColor the color of the new gem to set on the indexed field
     * @return refField SupuStone field reference
     *                 null, if basic set operation failed
     *                 a field with same color inside cross row or cross column
     *                 the field, after the new gemColor has been set there
     * @throws RuntimeException
     */
    public SupuStone setGemColor(COLUMN sColumn, int sRow, GEMCOLORS sColor) throws RuntimeException {
        SupuStone retField = null;

        try {
            retField = trySetGemColor(sColumn, sRow, sColor);
        } catch (Exception ex) {
            retField = null;
            return retField;
        }

        int sCol = sColumn.getValue();

        SupuStone[] rowFields = getRow(sRow);
        for (SupuStone rowField : rowFields) {
            if (rowField.color != GEMCOLORS.Z && rowField.color == sColor) {
                retField = rowField;
                retField.supuRules = SUPURULES.DENY_CROSS_COLORED;
                return retField;
            }
        }

        SupuStone[] columnFields = getColumn(sColumn);
        for (SupuStone colField : columnFields) {
            if (colField.color != GEMCOLORS.Z && colField.color == sColor) {
                retField = colField;
                retField.supuRules = SUPURULES.DENY_CROSS_COLORED;
                return retField;
            }
        }

        retField = supuFields[sCol][sRow];
        retField.color = sColor;
		
		if (isRowCompleted(dimension, sRow)) {
			retField.supuRules = SUPURULES.ROW_COMPLETED;
			if (retField.row == currentRow)
				currentRow++;
		}

        return retField;
    }

    /**
     * canSetGemColor
     * @param column the column indexer of our game board, e.g. A, B, C, D, E, F, G, ...
     * @param row the row indexer of our game board, e.g. 0, 1, 2, 3, 4, 5, 6, 7, ...
     * @param gemColor the color of the new gem to set on the indexed field
     * @return (BoardField) null, when any Exception occurrs, otherwise the addressable field
     * @throws RuntimeException
     */
    public SupuStone trySetGemColor(COLUMN column, int row, GEMCOLORS gemColor) throws RuntimeException {
        SupuStone try2Set = null;

        try {
            if (gemColor == GEMCOLORS.Z) {
                throw (IllegalArgumentException) (new Throwable("Color can't be " + gemColor.getName()));
            }
            try2Set = this.getField(column, row);
        } catch (RuntimeException runEx) {
            throw runEx;
        }

        if (try2Set != null && try2Set.color != GEMCOLORS.Z) {
            throw (RuntimeException) (new Throwable("field " + try2Set.getName() + " already set color=" + gemColor.getName()));
        }

        return try2Set;
    }
	
   /**
     * Clear all stones in board
     */
    public void ClearStones() {
        supuFields = new SupuStone[dimension][dimension];
        for (int rowC = 0; rowC < dimension; rowC++) {
            for (int colC = 0; colC < dimension; colC++) {
                COLUMN aCol = Constants.allCols[colC];
                supuFields[colC][rowC] = new SupuStone(null, dimension, aCol, rowC, GEMCOLORS.Z);
            }
        }
    }

    /**
     * getRow
     * @param row the row indexer of our game board, e.g. 0, 1, 2, 3, 4, 5, 6, 7, ...
     * @return an array of BoardField[], that contains all columns for the indexed row
     *         e.g. if paran row = 1, then it returns an array containing A1, B1, C1, D1, E1, ...
     */
    public SupuStone[] getRow(int row) {
        SupuStone[] retFields = new SupuStone[dimension];

        int coi = 0;
        for (coi = 0; coi  < dimension; coi++) {
            retFields[coi] = supuFields[coi][row];
        }
        return retFields;
    }

    /**
     * getColumn
     * @param xColumn the column indexer of our game board, e.g. A, B, C, D, E, F, G, H, ...
     * @return an array of SupuStone[], that contains all rows for the indexed column
     *         e.g. if paran column = C, then it returns an array containing C0, C1, C2, C3, C4, ...
     */
    public SupuStone[] getColumn(COLUMN xColumn) {
        // ArrayList<BoardField> retFieldList = new ArrayList<BoardField>();
        SupuStone[] retFields = new SupuStone[dimension];
        int col = xColumn.getValue();

        int row = 0;
        for (row = 0; row  < dimension; row++) {
            retFields[row] = supuFields[col][row];
            // retFieldList.add(field[col][row]);
        }

        // SupuStone[] retFields = new SupuStone[retFieldList.size()];
        // return retFieldList.toArray(retFields);
        return retFields;
    }

    /**
     * getFieldByToView
     *  static method, that returns a field, that was identified by view short bane
     * @param nName view String identifier
     * @throws {@link RuntimeException}
     * @return SupuStone, that represents column and row
     */
    public SupuStone getFieldByName(String nName) throws RuntimeException {
        if (nName == null || nName.length() < 2) {
            throw (RuntimeException)(new Throwable("failed to extract field from view name = \'" + nName + "\'."));
        }

        int nRow  =  (int) Integer.parseInt(String.valueOf(nName.charAt(1)));
        COLUMN nColumn = COLUMN.getEnum(nName.charAt(0));

        return this.getField(nColumn, nRow);
    }

}
