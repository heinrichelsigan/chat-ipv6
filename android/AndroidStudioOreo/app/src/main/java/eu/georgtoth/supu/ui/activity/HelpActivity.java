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
package eu.georgtoth.supu.ui.activity;

import android.media.AudioAttributes;
import android.media.MediaPlayer;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.Button;
import android.content.Intent;
import android.widget.TextView;
import android.view.View;
import android.view.View.OnClickListener;

import androidx.appcompat.app.AppCompatActivity;

import eu.georgtoth.supu.enums.BOARDCOL;
import eu.georgtoth.supu.enums.FIELDSTATE;
import eu.georgtoth.supu.enums.GEMCOLORS;
import eu.georgtoth.supu.R;

/**
 * HelpActivity class implements help text view.
 *
 * @see <a href="https://github.com/heinrichelsigan/georgtoth/wiki/a>
 */
public class HelpActivity extends BaseApp {

    Button backButton, learnMoreButton;
    TextView helpTextView, builtWithTextView;
    Menu myMenu;

    /**
     * Override onCreate
     * @param savedInstanceState Bundle
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_help);

        backButton = (Button) findViewById(R.id.backButton);
        learnMoreButton =  (Button) findViewById(R.id.learnMoreButton);
        helpTextView = (TextView) findViewById(R.id.helpTextView);
        builtWithTextView = (TextView) findViewById(R.id.builtWithTextView);

        addListenerOnClickables();

    }

    /**
     * onCreateOptionsMenu
     * @param menu Menu
     * @return true|false
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        myMenu = menu;
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        if (id >= 0 && item.getItemId() == id) {
            if (id == R.id.action_start     || id == R.id.action_direction ||
                    id == R.id.action_five  || id == R.id.action_six ||
                    id == R.id.action_seven || id == R.id.action_eight ||
                    id == R.id.action_nine  || id == R.id.action_ten ||
                    id == R.id.action_level || id == R.id.action_options ||
                    id == R.id.action_load  || id == R.id.action_save ||
                    id == R.id.action_gameautomation) {
                finish();
                return true;
            } else if (id == R.id.action_exit) {
                exitGame();
                return true;
            } else if (id == R.id.action_about) {
                showAbout();
                return true;
            } else if (id == R.id.action_help) {
                openHelpUrl();
                return true;
            } else if (id == R.id.action_screenshot) {
                onCameraClick(rootView);
                return true;
            }
            return false;
        }
        return super.onOptionsItemSelected(item);
    }


    /**
     * add listeners on all clickables
     */
    public void addListenerOnClickables() {

        backButton = (Button) findViewById(R.id.backButton);
        backButton.setOnClickListener(arg0 -> backButton_Clicked(arg0));

        learnMoreButton = (Button) findViewById(R.id.learnMoreButton);
        learnMoreButton.setOnClickListener(arg0 -> learnMoreButton_Clicked(arg0));
    }


    /**
     * backButton_Clicked finish about activity
     * @param arg0 current View
     */
    public void backButton_Clicked(View arg0) {
        // finish activity
        finish();
    }


    /**
     * learnMoreButton_Clicked_Clicked
     * @param arg0
     */
    public void learnMoreButton_Clicked(View arg0) {

        String url = "https://github.com/heinrichelsigan/Archon/blob/master/app/src/main/res/raw/dropstone.wav?raw=true"; // your URL here
        MediaPlayer mediaPlayer = new MediaPlayer();
        mediaPlayer.setAudioAttributes(
                new AudioAttributes.Builder()
                        .setContentType(AudioAttributes.CONTENT_TYPE_MUSIC)
                        .setUsage(AudioAttributes.USAGE_MEDIA)
                        .build()
        );
        try {
            mediaPlayer.setDataSource(url);
            mediaPlayer.prepare(); // might take long! (for buffering, etc)
            mediaPlayer.start();
        } catch (Exception exi) {
            showError(exi, true);
        }

        helpTextView.setText(R.string.help_text);

        Intent intent = new Intent();
        intent.setAction(Intent.ACTION_VIEW);
        String help_uri = getString(R.string.help_uri);
        intent.setData(android.net.Uri.parse(help_uri));
        startActivity(intent);
    }

}