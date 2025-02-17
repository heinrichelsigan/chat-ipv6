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

package eu.georgtoth.supu;

import android.content.Intent;
import android.os.Bundle;
import eu.georgtoth.supu.ui.activity.BaseApp;
import eu.georgtoth.supu.ui.activity.Supu;

/**
 * MainActitity for SUPU
 * Don't use MainActivity, use eu.georgtoth.supu.ui.activity.Supu instead
 */
@Deprecated
public class MainActivity extends BaseApp {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Intent intent = new Intent(this, eu.georgtoth.supu.ui.activity.Supu.class);
        startActivity(intent, savedInstanceState);
    }

}
