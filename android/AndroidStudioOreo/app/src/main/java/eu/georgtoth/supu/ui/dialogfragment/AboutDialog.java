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
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.MediaController;
import android.widget.TextView;
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
 * Use the {@link AboutDialog#newInstance} factory method to
 * create an instance of this fragment.
 */
public class AboutDialog extends DialogFragment {

    /* The activity that creates an instance of this dialog fragment must
     * implement this interface in order to receive event callbacks.
     * Each method passes the DialogFragment in case the host needs to query it. */
    public interface NoticeDialogListener {
        public void onDialogPositiveClick(DialogFragment dialog);
        public void onDialogNegativeClick(DialogFragment dialog);
        public void onCancel(DialogFragment dialog);
    }

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "Perfect";
    private static final String ARG_PARAM2 = "LevelCurrent";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    private int aPerfect = 0;
    private int aCurrentLevel = 5;
    public int perfectGame = 0;

    Context context;
    Button backButton;
    TextView builtWithTextView;
    ImageView appHeader, appHeaderClose, appIdea, appCode, appBottom;
    // Use this instance of the interface to deliver action events
    NoticeDialogListener listener;
    DialogFragment dialogFragment;


    /**
     * AboutDialog
     * parameterless default constructor
     */
    public AboutDialog() {
        // Required empty public constructor
    }

    /**
     * newInstance
	 * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param perfect Parameter 1.
     * @param currentLevel Parameter 2.
     * @return A new instance of fragment About DialogFragment.
     */
    public static AboutDialog newInstance(int perfect, int currentLevel) {
        AboutDialog fragment = new AboutDialog();
        Bundle args = new Bundle();

        args.putInt(ARG_PARAM1, perfect);
        args.putInt(ARG_PARAM2, currentLevel);
        fragment.setArguments(args);
        fragment.perfectGame = perfect;
        return fragment;
    }

    /**
     * onAttach
	 * Override the Fragment.onAttach() method to instantiate the NoticeDialogListener
     *
     * @param c Context
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
        dialogFragment = this;

        if (savedInstanceState != null) {
            perfectGame = savedInstanceState.getInt(getString(R.string.msg_perfect), 0);
        }

        // Inflate and set the layout for the dialog
        // Pass null as the parent view because its going in the dialog layout
        builder.setView(inflater.inflate(R.layout.dialog_fragment_about, null))
                .setNegativeButton(R.string.back_button, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        listener.onDialogNegativeClick(dialogFragment);
                        dialogFragment.getDialog().cancel();
                    }
                })
                .setOnCancelListener(new DialogInterface.OnCancelListener() {
                    @Override
                    public void onCancel(DialogInterface dialog) {
                        listener.onCancel(dialogFragment);
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
        return inflater.inflate(R.layout.dialog_fragment_about, container, false);
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
            dialogFragment = this;
            view = getView();
            appHeaderClose = (ImageView) view.findViewById(R.id.appHeaderClose);
            appIdea = (ImageView) view.findViewById(R.id.appIdea);
            appCode = (ImageView) view.findViewById(R.id.appCode);
            appBottom = (ImageView) view.findViewById(R.id.appBottom);

			builtWithTextView = (TextView) view.findViewById(R.id.builtWithTextView);
            builtWithTextView.setVisibility(View.VISIBLE);

            appHeaderClose.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    listener.onCancel(dialogFragment);
                    dialogFragment.getDialog().cancel();
                }
            });
            /*
                appHeader = (ImageView) view.findViewById(R.id.appHeader);

                appIdea.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        Intent intent = new Intent();
                        intent.setAction(Intent.ACTION_VIEW);
                        String help_uri = getString(R.string.help_uri);
                        intent.setData(android.net.Uri.parse(help_uri));
                        startActivity(intent);
                    }
                });
                appCode.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        Intent intent = new Intent();
                        intent.setAction(Intent.ACTION_VIEW);
                        String help_uri = getString(R.string.app_code_url);
                        intent.setData(android.net.Uri.parse(help_uri));
                        startActivity(intent);
                    }
                });

                backButton = (Button) view.findViewById(R.id.backButton);
                backButton.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        listener.onDialogPositiveClick(dialogFragment);
                        dialogFragment.getDialog().cancel();
                    }
                });
             */

        } catch (Exception exi) {
            exi.printStackTrace();
        }
    }

}
