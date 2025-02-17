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

import android.os.IBinder;

import androidx.annotation.NonNull;

import org.jetbrains.annotations.Contract;

import java.io.Serializable;
import java.lang.String;

/**
 * GEMCOLORS represents the different colors for the gem stones
 * <code>INVALID(0), FREE(1), COLORSET(2)</code>
 */
public enum GEMCOLORS implements Serializable {
    a(0),
    b(1),
    c(2),
    d(3),
    e(4),
    f(5),
    g(6),
    h(7),
    i(8),
    j(9),
    k(10),
    l(11),
    m(12),
    n(13),
    o(14),
    p(15),
    q(16),
    r(17),
    s(18),
    t(19),
    u(20),
    v(21),
    w(22),
    x(23),
    y(24),
    z(25),
    X(31),
    Y(127),
    Z(Byte.MAX_VALUE);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    GEMCOLORS(int value) { this.value = value; }

    private final int value;

    /**
     * getValue
     * @return (@link int) value
     */
    public int getValue() {
        return value;
    }

    /**
     * getUChar
     * @return color number as digit character (@link char)
     */
    @Deprecated
    public char getChar() {
        if (this.getValue() >= 0 && this.getValue() <= 25)
            return (char)((int)'a' + this.getValue());
        if (this.getValue() == 31)
            return 'X';
        // if (this.getValue() == 127)  return 'Y';
        return (char)'Z';
    }

    /**
     * getName
     * @return color enum name (color0, color1, ..., NONE)
     */
    public String getName() {
        return String.valueOf(getChar());
    }


    /**
     * IsValidColor - validates, if that color appears inside current SUPU level
     *
     * @param level int - current SUPU level
     * @return true, if color is appearing inside current SUPU level, otherwise false
     */
    public boolean IsValidColor(int level) {
        return (getValue() < level);
    }


    /**
     * getColorByFigure
     * @param figureString gem Image identifier
     * @return unique color, that figureString image represents
     */
    public static GEMCOLORS getColorByFigure(String figureString) {
        int cvalue = -1;
        if (figureString != null && figureString.length() > 3) {
            char colorCh = figureString.charAt(4);
            cvalue = ((int)colorCh - (int)'a');
        }

        if (cvalue > -1 && cvalue <= GEMCOLORS.z.getValue()) {
            for (GEMCOLORS gemEnum : GEMCOLORS.values()) {
                if (gemEnum.getValue() == cvalue)
                    return gemEnum;
            }
        }

        return GEMCOLORS.Z;
    }
}
