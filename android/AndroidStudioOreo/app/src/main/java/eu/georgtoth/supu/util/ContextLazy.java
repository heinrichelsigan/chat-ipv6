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
package eu.georgtoth.supu.util;

import android.content.Context;
import com.google.gson.annotations.JsonAdapter;
import org.jetbrains.annotations.Nullable;
import java.io.Serializable;
import java.security.InvalidAlgorithmParameterException;
import java.util.ArrayList;
import eu.georgtoth.supu.util.Constants;

/**
  * ContextLazy - provides a lazy singelton for application context
  *
  */
public class ContextLazy {
    private static ContextLazy instance;
    private Context mContext;
	
	public static ContextLazy getInstance(Context context) {
		if (instance == null) {
			// instance = new ContextLazy(context);
			instance = new ContextLazy(context.getApplicationContext());
		}
		return instance;
	}

    public static Context getLastContext() {
        return (instance != null) ? instance.mContext : (Context)null;
    }

    private ContextLazy(Context context) {
        mContext = context;
    }

}