/*
  @author           <a href="mailto:heinrich.elsigan@area23.at">Heinrich Elsigan</a>, <a href="mailto:Eman@gmx.at">Georg Toth</a>
 * @version          V 1.0.1
 * @since            API 27 Oreo 8.1
 *
* <p>SUPU is the idea of  by <a href="mailto:Eman@gmx.at">Georg Toth</a>
 * based Sudoku with colors instead of numbers.</p>
 *
 * <P>Coded 2021-2025 by <a href="mailto:heinrich.elsigan@area23.at">Heinrich Elsigan</a>
 */

package eu.georgtoth.supu.enums;

import java.io.Serializable;

/**
 * AUTOMAT Enum for Automation
 */
public enum AUTOMAT implements Serializable {
    IOFF(0x00F0),
    ION(0x0010);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    AUTOMAT(int value) {
        this.value = value;
    }

    private final int value;

    /**
     * getValue
     * @return (@link int) value
     */
    public int getValue() { return value; }


}

