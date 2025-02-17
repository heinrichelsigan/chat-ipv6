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

import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.net.Uri;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.MediaController;
import android.widget.VideoView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AlertDialog;
import androidx.fragment.app.DialogFragment;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;

import org.jetbrains.annotations.NotNull;

import eu.georgtoth.supu.R;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link GameOver#newInstance} factory method to
 * create an instance of this fragment.
 */
public class GameOver extends DialogFragment {

    /**
     * The activity that creates an instance of this dialog fragment must
     * implement this interface in order to receive event callbacks.
     * Each method passes the DialogFragment in case the host needs to query it
     */
    public interface NoticeDialogListener {
        public void onDialogPositiveClick(DialogFragment dialog);
        public void onDialogNegativeClick(DialogFragment dialog);
    }

    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "Perfect";
    private static final String ARG_PARAM2 = "LevelCurrent";

    // TODO: 01/04/2024 Rename and change types of parameters
    private String mParam1;
    private String mParam2;
    private int aPerfect = 0;
    private int aCurrentLevel = 5;

    Context context;
    // Video view
    ImageView imgViewHeader, imgViewGameOver;

    // Use this instance of the interface to deliver action events
    NoticeDialogListener listener;

    /**
     * GameOver
     * parameterless default constructor
     */
    public GameOver() {
        // Required empty public constructor
    }

    /**
     * newInstance
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param perfect Parameter 1.
     * @param currentLevel Parameter 2.
     * @return A new instance of fragment GameOver DialogFragment.
     */
    public static GameOver newInstance(int perfect, int currentLevel) {
        GameOver fragment = new GameOver();
        Bundle args = new Bundle();

        args.putInt(ARG_PARAM1, perfect);
        args.putInt(ARG_PARAM2, currentLevel);
        fragment.setArguments(args);
        return fragment;
    }

    /**
     * onAttach
     * Override the Fragment.onAttach() method to instantiate the NoticeDialogListener
     *
     * @param c Context
     * @return @NotNull Dialog
     */
    @Override
    public void onAttach(Context c) {
        super.onAttach(c);
        context = c;

        // Verify that the host activity implements the callback interface
        try {
            // Instantiate the NoticeDialogListener so we can send events to the host
            listener = (NoticeDialogListener) context;
        } catch (ClassCastException e) {
            // The activity doesn't implement the interface, throw exception
            throw new ClassCastException("You must implement NoticeDialogListener" + e.getMessage());
        }
    }

    /**
     * onCreateDialog
     *
     * @param savedInstanceState Bundle
     * @return @NotNull Dialog
     */
    @Override
    public @NotNull Dialog onCreateDialog(Bundle savedInstanceState) {
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        // Get the layout inflater
        LayoutInflater inflater = requireActivity().getLayoutInflater();
        final DialogFragment dialogFragment = this;

        // Inflate and set the layout for the dialog
        // Pass null as the parent view because its going in the dialog layout
        builder.setView(inflater.inflate(R.layout.fragment_game_over_dialog, null))
                // Add action buttons
                // .setPositiveButton(R.string.next_level, new DialogInterface.OnClickListener() {
                // @Override
                //  public void onClick(DialogInterface dialog, int id) {
                // listener.onDialogPositiveClick(dialogFragment);
                //        dialogFragment.getDialog().cancel();
                //     }
                // })
                .setNegativeButton(R.string.replay_level, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        listener.onDialogNegativeClick(dialogFragment);
                        dialogFragment.getDialog().cancel();
                    }
                });
        return builder.create();
    }

    /**
     * onCreateView
     * Override onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
     *
     * @param inflater LayoutInflater
     * @param container ViewGroup
     * @param savedInstanceState Bundle
     * @return created View
     */
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_finished_level_dialog, container, false);
    }

    /**
     * show
     * Override show(@NonNull FragmentManager manager, @Nullable String tag)
     *
     * @param manager @NonNull FragmentManager
     * @param tag @Nullable String
     */
    @Override
    public void show(@NonNull FragmentManager manager, @Nullable String tag) {
        super.show(manager, tag);
    }

    /**
     * onStart
     * Override onStart()
     */
    @Override
    public void onStart() {
        super.onStart();

        View view = null;
        try {
            view = getView();
            imgViewHeader = (ImageView) view.findViewById(R.id.imgViewHeader);
            imgViewGameOver = (ImageView) view.findViewById(R.id.imgViewGameOver);           
        } catch (Exception exi) {
            exi.printStackTrace();
        }
    }

}
