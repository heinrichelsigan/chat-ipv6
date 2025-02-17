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

import java.io.Serializable;

import eu.georgtoth.supu.R;

/**
 * BOARDCOL represents the enumerator for columns of the board
 */
public enum SOUNDS implements Serializable  {
    APP_START(0),

    CREDITS_FULL_AGAIN(1),
    CREDITS_MINUS(2),
    DROPBOARD_OPEN(3),

    ERROR_DOUBLE_COLOR(4),
    ERROR_FORBIDDEN_PATTERN(5),
    ERROR_NO_PATTERN(6),
    ERROR_WRONG_ROW(7),

    GAME_OVER(8),
    LEVEL_COMPLETED(9),
    LEVEL_PERFECT(10),
    LEVEL_SUPUMASTER(11),

    MENU_CAMERA_CLICK(12),
    MENU_IMTERACTION(13),
    ROW_COMPLETED(14),
    STONE_DRAG(15),
    STONE_DROP(16),
    STONE_TAKE(17),

    NONE((int)Byte.MAX_VALUE);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    SOUNDS(int value) {
        this.value = value;
    }

    private final int value;

    /**
     * getValue
     * @return (@link int) value
     */
    public int getValue() { return value; }


    /**
     * getName
     * @return upper letter {@link String} or NONE
     */
    public String getName() {
        switch (this.getValue()) {
            case 0:     return "sound_app_start";
            case 1:     return "sound_credits_full_again";
            case 2:     return "sound_credits_minus";
            case 3:     return "sound_dropboard_open";

            case 4:     return "sound_error_double_color";
            case 5:     return "sound_error_forbidden_pattern";
            case 6:     return "sound_error_no_pattern";
            case 7:     return "sound_error_wrong_row";

            case 8:     return "sound_game_over";
            case 9:     return "sound_level_completed";
            case 10:    return "sound_level_perfect";
            case 11:    return "sound_level_supumaster";

            case 12:    return "sound_menu_camera_click";
            case 13:    return "sound_menu_interaction";
            case 14:    return "sound_row_completed";
            case 15:    return "sound_stone_drag";
            case 16:    return "sound_stone_drop";
            case 17:    return "sound_stone_take";
            default:
                return null;
        }
    }

    /**
     * getRId - gets R.raw.RId associated with enum
     *
     * @return int R.raw.RId for raw Id
     */
    public int getRId() {
        switch (this.getValue()) {
            case 0:     return R.raw.sound_app_start;
            case 1:     return R.raw.sound_credits_full_again;
            case 2:     return R.raw.sound_credits_minus;
            case 3:     return R.raw.sound_dropboard_open;

            case 4:     return R.raw.sound_error_double_color;
            case 5:     return R.raw.sound_error_forbidden_pattern;
            case 6:     return R.raw.sound_error_no_pattern;
            case 7:     return R.raw.sound_error_wrong_row;

            case 8:     return R.raw.sound_game_over;
            case 9:     return R.raw.sound_level_completed;
            case 10:    return R.raw.sound_level_perfect;
            case 11:    return R.raw.sound_level_supumaster;

            case 12:    return R.raw.sound_menu_camera_click;
            case 13:    return R.raw.sound_menu_interaction;
            case 14:    return R.raw.sound_row_completed;
            case 15:    return R.raw.sound_stone_drag;
            case 16:    return R.raw.sound_stone_drop;
            case 17:    return R.raw.sound_stone_take;
            default:    break;
        }
        return 0;
    }


    /**
     * getEnum
     * @param idx int
     * @return the enum {@link SOUNDS}
     */
    public static SOUNDS getEnum(int idx) {
        switch (idx) {
            case 0: return SOUNDS.APP_START;
            case 1: return SOUNDS.CREDITS_FULL_AGAIN;
            case 2: return SOUNDS.CREDITS_MINUS;
            case 3: return SOUNDS.DROPBOARD_OPEN;
            case 4: return SOUNDS.ERROR_DOUBLE_COLOR;
            case 5: return SOUNDS.ERROR_FORBIDDEN_PATTERN;
            case 6: return SOUNDS.ERROR_NO_PATTERN;
            case 7: return SOUNDS.ERROR_WRONG_ROW;
            case 8: return SOUNDS.GAME_OVER;
            case 9: return SOUNDS.LEVEL_COMPLETED;
            case 10: return SOUNDS.LEVEL_PERFECT;
            case 11: return SOUNDS.LEVEL_SUPUMASTER;
            case 12: return SOUNDS.MENU_CAMERA_CLICK;
            case 13: return SOUNDS.MENU_IMTERACTION;
            case 14: return SOUNDS.ROW_COMPLETED;
            case 15: return SOUNDS.STONE_DRAG;
            case 16: return SOUNDS.STONE_DROP;
            case 17: return SOUNDS.STONE_TAKE;
            default:  break;
        }
        return SOUNDS.NONE;

    }

    /**
     * getEnum
     * @param ch column character
     * @return the enum {@link SOUNDS}
     */
    public static SOUNDS getEnum(char ch) {
        char uch = String.valueOf(ch).toUpperCase().charAt(0);
        switch (uch) {
            case '0': return SOUNDS.APP_START;
            case '1': return SOUNDS.CREDITS_FULL_AGAIN;
            case '2': return SOUNDS.CREDITS_MINUS;
            case '3': return SOUNDS.DROPBOARD_OPEN;
            case '4': return SOUNDS.ERROR_DOUBLE_COLOR;
            case '5': return SOUNDS.ERROR_FORBIDDEN_PATTERN;
            case '6': return SOUNDS.ERROR_NO_PATTERN;
            case '7': return SOUNDS.ERROR_WRONG_ROW;
            case '8': return SOUNDS.GAME_OVER;
            case '9': return SOUNDS.LEVEL_COMPLETED;
            case 'A': return SOUNDS.LEVEL_PERFECT;
            case 'B': return SOUNDS.LEVEL_SUPUMASTER;
            case 'C': return SOUNDS.MENU_CAMERA_CLICK;
            case 'D': return SOUNDS.MENU_IMTERACTION;
            case 'E': return SOUNDS.ROW_COMPLETED;
            case 'F': return SOUNDS.STONE_DRAG;
            default:  break;
        }
        return SOUNDS.NONE;

    }

}

