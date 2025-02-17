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
import eu.georgtoth.supu.enums.GEMCOLORS;
import eu.georgtoth.supu.enums.FIELDSTATE;
import eu.georgtoth.supu.enums.SUPURULES;
import eu.georgtoth.supu.exceptions.BoardAccessorOutOfBoundsException;
import eu.georgtoth.supu.util.Constants;

/**
 * Board entity
 *
 * Board represents the data abstraction of Supu board
 * see BoardField
 */
public class Board implements Serializable {

    // protected Context context;

    public int dimension = 10;
    public int fieldIdCnt = 0;
    public int maxFields = dimension * dimension;
    public BoardField[] fields;
    public BoardField lastField = null;
    public SupuStone lastStone  = null;

    public static GEMCOLORS[][] supuPerm5 =
    {
        { GEMCOLORS.a, GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.e },
        { GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.a, GEMCOLORS.e, GEMCOLORS.d }
    };
    public static GEMCOLORS[][] supuPerm6 =
    {
        { GEMCOLORS.a, GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.e, GEMCOLORS.f },
        { GEMCOLORS.c, GEMCOLORS.e, GEMCOLORS.d, GEMCOLORS.f, GEMCOLORS.b, GEMCOLORS.a },
        { GEMCOLORS.f, GEMCOLORS.c, GEMCOLORS.b, GEMCOLORS.e, GEMCOLORS.a, GEMCOLORS.d }
    };
    public static GEMCOLORS[][] supuPerm7 =
    {
        { GEMCOLORS.a, GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.e, GEMCOLORS.f, GEMCOLORS.g },
        { GEMCOLORS.c, GEMCOLORS.e, GEMCOLORS.d, GEMCOLORS.g, GEMCOLORS.f, GEMCOLORS.b, GEMCOLORS.a },
        { GEMCOLORS.f, GEMCOLORS.c, GEMCOLORS.g, GEMCOLORS.b, GEMCOLORS.a, GEMCOLORS.e, GEMCOLORS.d }
    };
    public static GEMCOLORS[][] supuPerm8 =
    {
        { GEMCOLORS.a, GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.e, GEMCOLORS.f, GEMCOLORS.g, GEMCOLORS.h },
        { GEMCOLORS.e, GEMCOLORS.f, GEMCOLORS.g, GEMCOLORS.h, GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.a },
        { GEMCOLORS.b, GEMCOLORS.a, GEMCOLORS.h, GEMCOLORS.g, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.e, GEMCOLORS.f },
        { GEMCOLORS.g, GEMCOLORS.d, GEMCOLORS.e, GEMCOLORS.c, GEMCOLORS.a, GEMCOLORS.h, GEMCOLORS.f, GEMCOLORS.b }
    };
    public static GEMCOLORS[][] supuPerm9 =
    {
        {GEMCOLORS.a, GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.e, GEMCOLORS.f, GEMCOLORS.g, GEMCOLORS.h, GEMCOLORS.i},
        {GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.a, GEMCOLORS.f, GEMCOLORS.e, GEMCOLORS.i, GEMCOLORS.g, GEMCOLORS.h},
        {GEMCOLORS.e, GEMCOLORS.d, GEMCOLORS.a, GEMCOLORS.f, GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.h, GEMCOLORS.i, GEMCOLORS.g},
        {GEMCOLORS.i, GEMCOLORS.h, GEMCOLORS.g, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.b, GEMCOLORS.e, GEMCOLORS.f, GEMCOLORS.a}
    };
    public static GEMCOLORS[][] supuPerm10 =
    {
            {GEMCOLORS.a, GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.e, GEMCOLORS.f, GEMCOLORS.g, GEMCOLORS.h, GEMCOLORS.i, GEMCOLORS.j},
            {GEMCOLORS.b, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.a, GEMCOLORS.f, GEMCOLORS.e, GEMCOLORS.j, GEMCOLORS.i, GEMCOLORS.g, GEMCOLORS.h},
            {GEMCOLORS.e, GEMCOLORS.d, GEMCOLORS.a, GEMCOLORS.f, GEMCOLORS.b, GEMCOLORS.i, GEMCOLORS.h, GEMCOLORS.j, GEMCOLORS.c, GEMCOLORS.g},
            {GEMCOLORS.i, GEMCOLORS.h, GEMCOLORS.g, GEMCOLORS.c, GEMCOLORS.d, GEMCOLORS.j, GEMCOLORS.b, GEMCOLORS.f, GEMCOLORS.e, GEMCOLORS.a},
            {GEMCOLORS.j, GEMCOLORS.a, GEMCOLORS.f, GEMCOLORS.b, GEMCOLORS.g, GEMCOLORS.d, GEMCOLORS.i, GEMCOLORS.e, GEMCOLORS.h, GEMCOLORS.c},
    };




    /**
     * constructor
     *  initialize all fields of the board
     *
     * @param aContext (required Nullable) to avoid gson reflected framework serialization
     */
    public Board(@Nullable Context aContext) {
        this(aContext, 10);
    }

    /**
     * constructor
     *  initialize all fields of the board
     *
     * @param aContext (required Nullable) to avoid gson serialization of entire reflected androidx framework
     * @param level (required NonNull) application context
     */
    public Board(@Nullable Context aContext, int level) {
        // this.context = aContext;
        this.dimension = level;

        maxFields = dimension;
        if (this instanceof SupuBoard) {
            maxFields = dimension * dimension;
        } else if (this instanceof DropBoard) {
            maxFields = dimension * 2;
        }
        fields =  new BoardField[maxFields];

        lastStone  = null;
        lastField = null;

        int aRow = 0;
        for (int fId = 0; fId < maxFields; fId++) {
            COLUMN aCol = Constants.allCols[fId % dimension];
            if (fId >  0 && ((fId % dimension) == 0))
                aRow++;
            fields[fId] = new BoardField(aContext, level, aCol, aRow);
        }
    }



    /**
     * getFieldByToView
     *  static method, that returns a field, that was identified by view short bane
     * @param toView view String identifier
     * @return BoardField, that represents column and row
     */
    @Deprecated
    public static BoardField getFieldIndexer(@Nullable Context aContext, String toView) {

        if (toView == null || toView.length() < 2)
            return null;

        int row4View  =  (int) Integer.parseInt(String.valueOf(toView.charAt(1)));
        COLUMN column4View = COLUMN.getEnum(toView.charAt(0));

        return new BoardField(aContext, 10, column4View, row4View);
    }
}
