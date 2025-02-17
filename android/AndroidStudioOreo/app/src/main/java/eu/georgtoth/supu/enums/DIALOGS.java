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

/**
 * DIALOGS enum for defined dialog fragments
 */
public enum DIALOGS {
	About('a'),
	Help('h'),
	GameOver('g'),
	FinishedLevel('f'),
	FinishedLevelPerfect('p'),
    None('n');

    /**
     * enum DIALOGS constructor must have private or package scope. You can not use the public access modifier.
     */
    DIALOGS(char dialogCh) {
        this.dialogChar = dialogCh;
    }

    private final char dialogChar;

    /**
     * getChar
     * @return (@link char) dialogChar
     */
    public int getChar() { return dialogChar; }
	
}
