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
import androidx.constraintlayout.core.motion.utils.Utils;

import com.google.gson.annotations.JsonAdapter;
import com.google.gson.annotations.SerializedName;

import eu.georgtoth.supu.enums.AUTOMAT;
import eu.georgtoth.supu.enums.BOARDCOL;
import eu.georgtoth.supu.enums.GAMEMODE;
import eu.georgtoth.supu.enums.GEMCOLORS;
import eu.georgtoth.supu.enums.BOARDS;
import eu.georgtoth.supu.enums.COLUMN;
import eu.georgtoth.supu.enums.FIELDSTATE;
import eu.georgtoth.supu.enums.SUPUMODE;
import eu.georgtoth.supu.enums.SUPURULES;
import eu.georgtoth.supu.exceptions.BoardAccessorOutOfBoundsException;
import eu.georgtoth.supu.util.Constants;

import java.io.Serializable;

import kotlin.jvm.internal.SerializedIr;

/**
 * Game
 *   implements {@link Serializable}
 *
 * Game represents supu game entity
 */
public class Game implements Serializable {

    // Context context commented out (required Nullable) to avoid gson androidx serialization
    // private Context context;

    public int dimension = 10;
    public int supuLevel = 6;
	public int credits  = Constants.MAX_CREDITS_PER_ROW;
	public int stoneCounter  = 0;
    public int fieldIdCnt = 0;
    public boolean perfectLevel = true;
	public boolean onceDragNDropped = false;
	boolean _dropBoardEnabled = false;

    public SUPUMODE supuMode = SUPUMODE.FreeS;

	public SupuBoard supuBoard;
	public DropBoard dropBoard;
    public StoneBoard stoneBoard;

    public SUPURULES supuRules = SUPURULES.OK;

    /**
     * constructor
     *
     * @param aContext (required Nullable) to avoid gson androidx serialization
     * @param level {@link int} dimension = level
     */
    public Game(@NonNull Context aContext, int level) {
        // this.context = aContext;
        dimension = level;
        supuLevel = level - 4;
        supuBoard = new SupuBoard(aContext, level);
        stoneBoard = new StoneBoard(aContext, level);
        dropBoard = new DropBoard(aContext, level);
    }

    /**
     * Game constructor
     * @param aContext (required Nullable) to avoid gson androidx serialization
     * @param level {@link int} dimension = level
     * @param visualHelp {@link boolean} true to set, false to unset
     */
    public Game(@NonNull Context aContext, int level, boolean visualHelp) {
        this(aContext, level);
        setVisualHelp(visualHelp);
    }


    /**
     * Game constructor
     * @param aContext (required Nullable) to avoid gson androidx serialization
     * @param level {@link int} dimension = level
     * @param direction {@link eu.georgtoth.supu.enums.GAMEMODE}
     * @param auto {@link eu.georgtoth.supu.enums.AUTOMAT}
     * @param visualHelp {@link boolean} true to set, false to unset
     */
    public Game(@NonNull Context aContext, int level, GAMEMODE direction, AUTOMAT auto, boolean visualHelp) {
        this(aContext, level);
        supuMode = SUPUMODE.setSupuMode(direction, auto, visualHelp);
        if (auto == AUTOMAT.ION) {
            int maxRow = (level % 2 == 0) ? (int)(level / 2) : (int)((level - 1)/2);
            for (int row = 0; row < maxRow; row++) {
                for (int c = 0; c < level; c++) {
                    COLUMN col = Constants.allCols[c];
                    GEMCOLORS gemColor = GEMCOLORS.Z;
                    switch (level) {
                        case 5:
                            gemColor = Board.supuPerm5[row][c];
                            break;
                        case 6:
                            gemColor = Board.supuPerm6[row][c];
                            break;
                        case 7:
                            gemColor = Board.supuPerm7[row][c];
                            break;
                        case 8:
                            gemColor = Board.supuPerm8[row][c];
                            break;
                        case 9:
                            gemColor = Board.supuPerm9[row][c];
                            break;
                        case 10:
                        default:
                            gemColor = Board.supuPerm10[row][c];
                            break;
                    }
                    supuBoard.setGemColor(col, row, gemColor);
                }
                // stoneBoard.reFill();
            }
            supuBoard.currentRow = maxRow;
            stoneBoard.reRow = maxRow;
        }
    }


    /**
     * setVisualHelp
     * @param vHelp boolean true to set visual help, false to unset visual help
     */
    public void setVisualHelp(boolean vHelp) {
        supuMode = SUPUMODE.setVisualHelp(supuMode, vHelp);
    }

    /**
     * setAutomation - sets or unsets game automation
     * @param auto boolean true to set automation, false to unset automation
     */
    public void setAutomation(boolean auto) {
        supuMode = SUPUMODE.setAutomation(supuMode, (auto) ? AUTOMAT.ION : AUTOMAT.IOFF);
    }


	public boolean isFinished() {
		return supuBoard.isGameFinished();
	}

    protected boolean isDropBoardEnabled() {
		if (!_dropBoardEnabled) {
            if (supuBoard.rowsFullyCompleted() >= Constants.DROPBOARD_ROWENABLED[dimension - 1]) {
                _dropBoardEnabled = true;
            } else if (supuBoard.currentRow >= Constants.DROPBOARD_ROWENABLED[dimension - 1]) {
                _dropBoardEnabled = true;
            }
        }
        return _dropBoardEnabled;
	}

    protected int getFieldIdCnt() {
        return fieldIdCnt;
    }

    protected void setFieldIdCnt(int _fieldIdCnt) {
        this.fieldIdCnt = _fieldIdCnt;
        supuBoard.fieldIdCnt = _fieldIdCnt;
        stoneBoard.fieldIdCnt = _fieldIdCnt;
        dropBoard.fieldIdCnt = _fieldIdCnt;
    }

    /**
     * getName
     * @return String with full field name including color
     */
    protected String getName() {
        try {
            return ("Game_level_" + dimension);
        } catch (Exception ex) {
            ex.printStackTrace();
        }
        return null;
    }

}
