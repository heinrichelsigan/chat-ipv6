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
import androidx.annotation.NonNull;

import com.google.gson.annotations.JsonAdapter;

import eu.georgtoth.supu.enums.BOARDCOL;
import eu.georgtoth.supu.enums.COLUMN;
import eu.georgtoth.supu.enums.FIELDSTATE;
import eu.georgtoth.supu.enums.GEMCOLORS;
import eu.georgtoth.supu.enums.SUPURULES;
import eu.georgtoth.supu.exceptions.BoardAccessorOutOfBoundsException;

import org.jetbrains.annotations.Nullable;

import java.io.Serializable;

/**
 * BoardField entity
 *
 * BoardField represents a potential field in the board
 *
 */
public class SupuStone extends BoardField implements Serializable {

    // Already implemented in BoardField
    // private Context context;

    public GEMCOLORS color = GEMCOLORS.Z;
    public int fieldId = -1;
    public SUPURULES supuRules = SUPURULES.OK;

    /**
     * constructor
     *
     * @param aContext application context is @Nullable to avoid gson serializing
     *                 framework by reflection up to androidx.* serializing
     * @param aColumn board column BOARDCOL enum
     * @param aRow board row
     */
    public SupuStone(@Nullable Context aContext, int level, COLUMN aColumn, int aRow) {
        super(aContext, level, aColumn, aRow);
        // fieldId = context.getResources().getIdentifier(fieldName, "id", context.getPackageName());
        fieldId = aRow * level + (aColumn.getValue()); // logical fieldId
    }

    /**
     * constructor with additional color (calles default construct)
     *
     * @param aContext application context is @Nullable to avoid gson serializing
     *                 framework by reflection up to androidx.* serializing
     * @param aColumn board column BOARDCOL enum
     * @param aRow board row
     * @param aColor gem or stone color
     */
    public SupuStone(@Nullable Context aContext, int level, COLUMN aColumn, int aRow, GEMCOLORS aColor) {
        super(aContext, level, aColumn, aRow);
        this.color = aColor;
        this.fieldId = aRow * level + (aColor.getValue()); // logical fieldId
    }


    /**
     * constructor with additional color (calles default construct)
     *
     * @param aContext application context is @Nullable to avoid gson serializing
     *                 framework by reflection up to androidx.* serializing
     * @param aColumn board column BOARDCOL enum
     * @param aRow board row
     * @param aColor gem or stone color
	 * @param aFieldId unique incremental fieldId
     */
    public SupuStone(@Nullable Context aContext, int level, COLUMN aColumn, int aRow, GEMCOLORS aColor, int aFieldId) {
        super(aContext, level, aColumn, aRow);
        this.color = aColor;
        this.fieldId = aFieldId;
    }
	
	/**
     * constructor with single SupuStone
     *
     * @param sStone SupuStone
     */
    public SupuStone(Context aContext, SupuStone sStone) {
        super(aContext);
        // this.context = aContext;
        if (sStone != null) {
            // this.context = sStone.context;
            this.dimension = sStone.dimension;
            this.row = sStone.row;
            this.column = sStone.column;
            this.color = sStone.color;
            this.fieldId = sStone.fieldId;
            this.supuRules = sStone.supuRules;
        }
    }


    /**
     * IsValid
     * @return true, if this is a valid field inside the board, otherwise false
     */
    public boolean isValid() {
        return  (column != COLUMN.NONE && row >= 0 && row < dimension &&
                column.isValidColumn(dimension) && color.IsValidColor(dimension));
    }

    /**
     * getFieldName
     * @return 2 character sized String, that contains first column letter and then row number
     * @throws BoardAccessorOutOfBoundsException new BoardAccessorOutOfBoundsException(column, row)
     */
    public String getFieldName() throws BoardAccessorOutOfBoundsException {
        return super.getName();
    }

    /**
     * getName
     * @return String with full field name including color
     */
	@Override
    public String getName() {
        try {
            return getFieldName() + (String)((color != GEMCOLORS.Z) ? "_" + color.getName() : "");
        } catch (Exception ex) {
            ex.printStackTrace();
        }
         return null;
    }


    /**
     * getState
     * @return FIELDSTATE
     */
	@Override
    public FIELDSTATE getState() {
        if (column == COLUMN.NONE || (column.charUpper() == (char)'\0') || row < 0 || row >= dimension) {
            return FIELDSTATE.INVALID;
        } else if (color == GEMCOLORS.Z) {
            return FIELDSTATE.FREE;
        } else {
            return FIELDSTATE.COLORSET;
        }
    }


}
