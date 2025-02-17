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
 * SUPURULES enum for defined matching and mismatching rules for supu game
 */
public enum SUPURULES implements Serializable {
    OK(0),
    OK_SAMESUPUB(1),
    ROW_COMPLETED(2),
    ROW_RECOMPLETED(3),
    ROW_BROKEN(4),
    ERR_VALIDATION_FAILED(5),
    ERR_ROWPERROW_VIOLATED(6),
    ERR_STONE_ALREADY_SET(7),
    DENY_CROSS_COLORED(8),
    DENY_REPETATIVE_COLORS(9),
    DENY_CHAIN_COLORED(10),
    DENY_BLOCK_BUILDING(11),
    DENY_3x3_REPETATIVE_COLORS(12),
    LEVEL_CHANGED_MARK_DARK(13),
    NONE((int)Byte.MAX_VALUE);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    SUPURULES(int value) {
        this.value = value;
    }

    private final int value;

    /**
     * getValue
     * @return (@link int) value
     */
    public int getValue() { return value; }

}

