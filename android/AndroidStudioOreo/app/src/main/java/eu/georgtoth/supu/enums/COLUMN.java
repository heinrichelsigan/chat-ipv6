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
import java.lang.String;

/**
 * COLUMN represents the enumerator for columns of the board
 */
public enum COLUMN implements Serializable {
    A(0),
    B(1),
    C(2),
    D(3),
    E(4),
    F(5),
    G(6),
    H(7),
    I(8),
    J(9),
    K(10),
    L(11),
    M(12),
    N(13),
    O(14),
    P(15),
    Q(16),
    R(17),
    S(18),
    T(19),
    NONE((int)Byte.MAX_VALUE);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    COLUMN(int value) {
        this.value = value;
    }

    private final int value;

    /**
     * getValue
     * @return (@link int) value
     */
    public int getValue() { return value; }

    /**
     * charUpper
     * @return upper letter {@link char}
     */
    public char charUpper() {
        int value = this.getValue();
        char a = 'A';
        if (value >= 0 && value <= (int)Byte.MAX_VALUE) {
            return (char) (((int) a + value));
        }
        return '\0';
    }


    /**
     * lowerChar
     * @return lower case letter {@link char}
     */
    public char lowerChar() {
        int value = this.getValue();
        char a = 'a';
        if (value >= 0 && value <= (int)Byte.MAX_VALUE) {
            return (char) (((int) a + value));
        }
        return '\0';
    }



    /**
     * getChar
     * @return (by default upper case) letter of {@link char}
     */
    public char getChar() {
        int value = this.getValue();
        char a = 'A';
        if (value >= 0 && value < 256) {
            return (char) (((int) a + value));
        }
        return '\0';

    }

    /**
     * getName
     * @return upper letter {@link String} or NONE
     */
    public String getName() {
        return (this.getValue() < 0 || this.getValue() >= (int)Byte.MAX_VALUE) ? "NONE": String.valueOf(this.charUpper());
    }

    /**
     * isValidColumn - checks, if that COLUMN appears alreeady in current SUPU level
     * @param level int - current SUPU level
     * @return true, if COLUMN appears already in current SUPU level
     */
    public boolean isValidColumn(int level) {
        return (getValue() < level);
    }

    /**
     * getEnum
     * @param ch column character
     * @return the enum {@link COLUMN}
     */
    public static COLUMN getEnum(char ch) {
        char uch = String.valueOf(ch).toUpperCase().charAt(0);
        for (COLUMN colEnum : COLUMN.values()) {
            if (colEnum.getChar() == uch)
                return colEnum;
        }
        return COLUMN.NONE;
    }

}

