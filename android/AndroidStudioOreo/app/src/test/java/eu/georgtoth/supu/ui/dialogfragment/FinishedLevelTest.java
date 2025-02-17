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
package eu.georgtoth.supu.ui.dialogfragment;

import static org.junit.Assert.assertEquals;

import android.content.Context;
import android.content.ContextWrapper;
import android.os.Bundle;

import androidx.fragment.app.DialogFragment;

import junit.framework.TestCase;

import org.junit.Test;

import eu.georgtoth.supu.R;
import eu.georgtoth.supu.enums.DIALOGS;
import eu.georgtoth.supu.util.ContextLazy;

public class FinishedLevelTest
        extends TestCase {

    @Test
    public void testNewInstance() {
        DialogFragment newDialogInstance = null;
        try {
            newDialogInstance = FinishedLevel.newInstance(0, 5);
        } catch (Exception ex) {
            ex.printStackTrace();
        }
        assertNotNull(newDialogInstance);
    }

    // public void testOnAttach() { }

    @Test
    public void testOnCreateDialog() {
        DialogFragment finishedLevelPerfectDialog = new FinishedLevelPerfect();
        Bundle dialogArgs = new Bundle();
        dialogArgs.putInt("Perfect", 1);
        dialogArgs.putInt("LevelCurrent", 5);
        finishedLevelPerfectDialog.setArguments(dialogArgs);
        android.app.Dialog dialog = finishedLevelPerfectDialog.onCreateDialog(dialogArgs);
        assertNotNull(dialog);
    }

    // public void testOnCreateView() { }

    // public void testShow() { }

    // public void testOnStart() { }

}