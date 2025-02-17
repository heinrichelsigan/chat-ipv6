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
 * BOARDS represents from board
 * <code>NOB(0), STONEB(1), SUPUB(2), DROPB(3)</code>
 */
public enum BOARDS implements Serializable {
    NOB(0),
	STONEB(1),
    SUPUB(2),
    DROPB(3);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    BOARDS(int value) { this.value = value; };

    private final int value;

    /**
     * getValue
     * @return {@link int} value
     */
    public int getValue() {
        return value;
    }

    /**
     * isValid
     * @return true if state not IMVALID
     */
    public boolean isValid() { return (this.getValue() != 0); }

    /**
     * fromStoneBoard
     * @return true if state is STONEB 
     */
    public boolean fromStoneBoard() { return (this.getValue() == 1); }

    /**
     * fromSupuBoard
     * @return true if state is SUPUB
     */
    public boolean fromSupuBoard() { return (this.getValue() == 2); }

    /**
     * fromDropBoard
     * @return true if state is DROPB
     */
    public boolean fromDropBoard() { return (this.getValue() == 3); }
}
