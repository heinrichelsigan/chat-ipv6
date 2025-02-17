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
import android.widget.ImageView;
import android.widget.TextView;
import android.view.View;
import android.view.View.OnClickListener;

import androidx.activity.OnBackPressedDispatcher;
import androidx.appcompat.app.AppCompatActivity;

import eu.georgtoth.supu.enums.BOARDCOL;
import eu.georgtoth.supu.enums.GEMCOLORS;
import eu.georgtoth.supu.enums.FIELDSTATE;
import eu.georgtoth.supu.R;

/**
 * AboutActivity class implements help text view.
 *
 * @see <a href="https://github.com/heinrichelsigan/archon/wiki/a>
 */
public class AboutActivity extends BaseApp {

    Button backButton;
    TextView builtWithTextView;
    ImageView appHeader, appHeaderClose, appIdea, appCode, appBottom;
    Menu myMenu;

    /**
     * Override onCreate
     * @param savedInstanceState Bundle
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_about);

        backButton = (Button) findViewById(R.id.backButton);

        builtWithTextView = (TextView) findViewById(R.id.builtWithTextView);
        appHeaderClose = (ImageView) findViewById(R.id.appHeaderClose);
        // appHeader = (ImageView) findViewById(R.id.appHeader);
        appIdea = (ImageView) findViewById(R.id.appIdea);
        appCode = (ImageView) findViewById(R.id.appCode);
        appBottom = (ImageView) findViewById(R.id.appBottom);

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

        if (id == R.id.action_start     || id == R.id.action_direction ||
                id == R.id.action_five  || id == R.id.action_six ||
                id == R.id.action_seven || id == R.id.action_eight ||
                id == R.id.action_nine  || id == R.id.action_ten ||
                id == R.id.action_level || id == R.id.action_options ||
                id == R.id.action_load  || id == R.id.action_save ||
                id == R.id.action_gameautomation) {
            finish();
            return true;
        }
        if (id == R.id.action_exit) {
            exitGame();
            return true;
        }
        if (id == R.id.action_help) {
            showHelp();
            return true;
        } if (id == R.id.action_about) {
            openHelpUrl();
            return true;
        }
        if (id == R.id.action_screenshot) {
            onCameraClick(rootView);
            return true;
        }

        return super.onOptionsItemSelected(item);
    }


    /**
     * add listeners on all clickables
     */
    public void addListenerOnClickables() {

        appHeaderClose = (ImageView) findViewById(R.id.appHeaderClose);
        appHeaderClose.setOnClickListener(arg0 -> backButton_Clicked(arg0));

        appIdea = (ImageView) findViewById(R.id.appIdea);
        appIdea.setOnClickListener(arg0 -> idea_Clicked(arg0));

        appCode = (ImageView) findViewById(R.id.appCode);
        appCode.setOnClickListener(arg0 -> code_Clicked(arg0));

        backButton = (Button) findViewById(R.id.backButton);
        backButton.setOnClickListener(arg0 -> backButton_Clicked(arg0));
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
     * @param arg0 View on which was clicked
     */
    public void learnMoreButton_Clicked(View arg0) {

        playLevelCompleted();

        Intent intent = new Intent();
        intent.setAction(Intent.ACTION_VIEW);
        String help_uri = getString(R.string.help_uri);
        intent.setData(android.net.Uri.parse(help_uri));
        startActivity(intent);
    }

    /**
     * idea_Clicked
     * @param arg0 View on which was clicked
     */
    public void idea_Clicked(View arg0) {

        playSupuMaster();

        Intent intent = new Intent();
        intent.setAction(Intent.ACTION_VIEW);
        String idea_uri = getString(R.string.app_idea_url);
        intent.setData(android.net.Uri.parse(idea_uri));
        startActivity(intent);
    }

    /**
     * code_Clicked
     * @param arg0 View on which was clicked
     */
    public void code_Clicked(View arg0) {

        playRowCompleted();

        Intent intent = new Intent();
        intent.setAction(Intent.ACTION_VIEW);
        String code_uri = getString(R.string.app_code_url);
        intent.setData(android.net.Uri.parse(code_uri));
        startActivity(intent);
    }

}