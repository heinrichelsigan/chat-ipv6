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
 * SCOLOR represents the enumerator for stone colors
 */
public enum SCOLOR implements Serializable {
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
	z(24),
    NONE((int)Byte.MAX_VALUE);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    SCOLOR(int value) {
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
            return (char) (((int) a + value));
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
            return (char) (((int) a + value));
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
            return (char) (((int) a + value));
        }
        return '\0';

    }

    /**
     * getName
     * @return upper letter {@link String} or NONE
     */
    public String getName() {
        return (this.getValue() < 0 || this.getValue() >= (int)Byte.MAX_VALUE) ? "NONE": String.valueOf(this.getlChar());
    }


    /**
     * getEnum
     * @param ch column character
     * @return the enum {@link SCOLOR}
     */
    public static SCOLOR getEnum(char ch) {
        char uch = String.valueOf(ch).toLowerCase().charAt(0);
        switch (uch) {
            case 'a': return SCOLOR.a;
            case 'b': return SCOLOR.b;
            case 'c': return SCOLOR.c;
            case 'd': return SCOLOR.d;
            case 'e': return SCOLOR.e;
            case 'f': return SCOLOR.f;
            case 'g': return SCOLOR.g;
            case 'h': return SCOLOR.h;
            case 'i': return SCOLOR.i;
            case 'j': return SCOLOR.j;
            case 'k': return SCOLOR.k;
            case 'l': return SCOLOR.l;
            case 'm': return SCOLOR.m;
            case 'n': return SCOLOR.n;
            case 'o': return SCOLOR.o;
            case 'p': return SCOLOR.p;
            case 'q': return SCOLOR.q;
            case 'r': return SCOLOR.r;
            case 's': return SCOLOR.s;
            case 't': return SCOLOR.t;
            case 'u': return SCOLOR.u;
            case 'v': return SCOLOR.v;
            case 'w': return SCOLOR.w;
			case 'x': return SCOLOR.x;
			case 'y': return SCOLOR.y;
			case 'z': return SCOLOR.z;
            default:  break;
        }
        return SCOLOR.NONE;

    }

}

