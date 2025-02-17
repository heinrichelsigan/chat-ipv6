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
 * FIELDSTATE represents field state enum of a field in the board
 * <code>INVALID(0), FREE(1), COLORSET(2)</code>
 */
public enum FIELDSTATE implements Serializable {
    INVALID(0),
    FREE(1),
    COLORSET(2);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    FIELDSTATE(int value) { this.value = value; }

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
     * isFree
     * @return true if state is FREE and no color set
     */
    public boolean isFree() { return (this.getValue() == 1); }

    /**
     * colorSet
     * @return true if state has a color set
     */
    public boolean colorSet() { return (this.getValue() == 2); }

}
