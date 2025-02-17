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
 * BOARDCOL represents the enumerator for columns of the board
 */
@Deprecated
public enum BOARDCOL implements Serializable {
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
    U(20),
    V(21),
    W(22),
    NONE((int)Byte.MAX_VALUE);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    BOARDCOL(int value) {
        this.value = value;
    }

    private final int value;

    /**
     * getValue
     * @return (@link int) value
     */
    public int getValue() { return value; }

    /**
     * getUChar
     * @return upper letter {@link char}
     */
    public char getUChar() {
        int value = this.getValue();
        char a = 'A';
        if (value >= 0 && value <= (int)Byte.MAX_VALUE) {
            char chReturn = (char) (((int) a + value));
            return chReturn;
        }
        return '\0';
    }


    /**
     * getUChar
     * @return upper letter {@link char}
     */
    public char getlChar() {
        int value = this.getValue();
        char a = 'a';
        if (value >= 0 && value <= (int)Byte.MAX_VALUE) {
            char chReturn = (char) (((int) a + value));
            return chReturn;
        }
        return '\0';
    }



    /**
     * getUChar
     * @return upper letter {@link char}
     */
    public char getChar() {
        int value = this.getValue();
        char a = 'a';
        if (value >= 0 && value < 256) {
            char chReturn = (char) (((int) a + value));
            return chReturn;
        }
        return '\0';

    }

    /**
     * getName
     * @return upper letter {@link String} or NONE
     */
    public String getName() {
        return (this.getValue() < 0 || this.getValue() >= (int)Byte.MAX_VALUE) ? "NONE": String.valueOf(this.getUChar());
    }


    /**
     * getEnum
     * @param ch column character
     * @return the enum {@link BOARDCOL}
     */
    public static BOARDCOL getEnum(char ch) {
        char uch = String.valueOf(ch).toUpperCase().charAt(0);
        switch (uch) {
            case 'A': return BOARDCOL.A;
            case 'B': return BOARDCOL.B;
            case 'C': return BOARDCOL.C;
            case 'D': return BOARDCOL.D;
            case 'E': return BOARDCOL.E;
            case 'F': return BOARDCOL.F;
            case 'G': return BOARDCOL.G;
            case 'H': return BOARDCOL.H;
            case 'I': return BOARDCOL.I;
            case 'J': return BOARDCOL.J;
            case 'K': return BOARDCOL.K;
            case 'L': return BOARDCOL.L;
            case 'M': return BOARDCOL.M;
            case 'N': return BOARDCOL.N;
            case 'O': return BOARDCOL.O;
            case 'P': return BOARDCOL.P;
            case 'Q': return BOARDCOL.Q;
            case 'R': return BOARDCOL.R;
            case 'S': return BOARDCOL.S;
            case 'T': return BOARDCOL.T;
            case 'U': return BOARDCOL.U;
            case 'V': return BOARDCOL.V;
            case 'W': return BOARDCOL.W;
            default:  break;
        }
        return BOARDCOL.NONE;

    }

}

