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
 * DROPSTATE represents the possibilites, when dooppimg a color stobe to board
 * <code>GENERICERROR(-1), CANDROP(0)), DOUBLECOLOR(1), FORBIDDENPATERM(2), INVALIDDROPVIEW(3)</code>
 */
public enum DROPSTATE implements Serializable {
	GENERICERROR(-1),
    CANDROP(0),
    DOUBLECOLOR(1),
    FORBIDDENPATERM(2),
	INVALIDDROPVIEW(3),
	STONETAKEN(4);

    /**
     * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
     */
    DROPSTATE(int value) { this.value = value; }

    private final int value;

    /**
     * getValue
     * @return {@link int} value
     */
    public int getValue() {
        return value;
    }

    /**
     * canDrop
     * @return true if we can drop gem
    public boolean canDrop() { return (this.getValue() == 0); }

    /**
     * doubledColor
     * @return true same color aöready exists in curremt row or curremt column
     */
    public boolean doubledColor() { return (this.getValue() == 1); }

    /**
     * forbiddenPatern
     * @return true if a repetable color eow patterm os fprnddem
     */
    public boolean forbiddenPatern() { return (this.getValue() == 2); }


	/**
     * forbiddenPatern
     * @return true if destination view is invalid and doesn't allow DROPS
     */
    public boolean invalidDropView() { return (this.getValue() == 3); }

	/**
     * Generic Error
     * @return true. if any unknown or generic errpr pccurs
     */
    public boolean genericError() { return (this.getValue() == -1); }


}
