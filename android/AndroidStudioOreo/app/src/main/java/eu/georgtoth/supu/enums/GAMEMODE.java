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

/**
 * GAMEMODE represents enumerator for direction game mode
 */
public enum GAMEMODE implements Serializable {
    BottomUp(0xDB00),
    TopDown(0xDD00),
    FreeStyle(0xDF00);
	

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    GAMEMODE(int value) {
        this.value = value;
    }

    private final int value;

    /**
     * getValue
     * @return (@link int) value
     */
    public int getValue() { return value; }


}

