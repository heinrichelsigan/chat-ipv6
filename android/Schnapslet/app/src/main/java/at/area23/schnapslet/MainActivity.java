/*
 *
 * @author           Heinrich Elsigan root@darkstar.work
 * @version          V 1.6.9
 * @since            API 26 Android Oreo 8.1
 *
 */
/*
   Copyright (C) 2000 - 2023 Heinrich Elsigan root@darkstar.work

   Schnapslet java applet is free software; you can redistribute it and/or
   modify it under the terms of the GNU Library General Public License as
   published by the Free Software Foundation; either version 2 of the
   License, or (at your option) any later version.
   See the GNU Library General Public License for more details.

*/
package at.area23.schnapslet;

// import android.app.Activity;
import android.content.ClipData;
import android.content.Context;
import android.content.res.Configuration;
import android.graphics.drawable.AnimationDrawable;
import android.graphics.drawable.Drawable;
import android.graphics.drawable.AnimatedImageDrawable;
import android.graphics.ImageDecoder;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
// import android.support.annotation.DrawableRes;
// import android.support.v4.content.ContextCompat;
// import android.support.v4.content.res.ResourcesCompat;
// import android.support.v13.view.DragAndDropPermissionsCompat;
// import android.support.v13.view.DragStartHelper;
// import android.support.v7.app.AppCompatActivity;
import android.speech.tts.TextToSpeech;
import android.view.DragEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.DragShadowBuilder;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.core.content.res.ResourcesCompat;
import androidx.fragment.app.DialogFragment;

import com.google.gson.Gson;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.HashMap;
import java.util.Locale;

import at.area23.schnapslet.constenum.CARDCOLOR;
import at.area23.schnapslet.constenum.CARDVALUE;
import at.area23.schnapslet.constenum.DIALOGS;
import at.area23.schnapslet.constenum.PLAYERDEF;
import at.area23.schnapslet.constenum.PLAYEROPTIONS;
import at.area23.schnapslet.constenum.SCHNAPSOUNDS;
import at.area23.schnapslet.constenum.SCHNAPSTATE;
import at.area23.schnapslet.models.Card;
import at.area23.schnapslet.models.Game;
import at.area23.schnapslet.models.PairCard;
import at.area23.schnapslet.models.Tournament;
import at.area23.schnapslet.models.TwoCards;

/**
 * MainActivity class implements the MainActivity.
 *
 * @see <a href="https://github.com/heinrichelsigan/schnapslet/wiki</a>
 */
public class MainActivity
        extends BaseAppActivity
        implements Runnable, HelpDialog.NoticeDialogListener {

    private static final String API_KEY = "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDDjZ+QmX6Zi514\nsFbIgT48HFuvXgWnmNbY7aBPW5gWq2kmISwxQcUG/JxdD2VasHiG66QAVgNHjQ8D\nRLyzPSmNUb4QVBMB4WHukqpBW97qG3Uhp4HnHYJ3Tg5XbHmjhFevxISG0ZLEni4C\nJMcNMTug6+VGDeNE/yISN42uhdiPsgTPIaGK/6FeG8KXLB9R501dYhiWprOuwhw5\nTXvAAaLyP+y/3N1/Q/4Po+WSusYqTUl1kNZ6/BvynmK4Bz+Ibakd59eBIn4xMOyK\nOxQuyC5GJbhRYjbcoEvbTzZy7CUk0nzrLunxzIucAr1SuOwJwIDz2yMM5wl/5nXY\nCm2RjzdnAgMBAAECggEAFWc50LTMI3gheyUpynZC3odoDZCn48kZstKHWkg3JDwM\nnSzCTn3ZV8NsRc86k6t+9Z1y7Mp9P2aT/xKV6LRICPyqZdUd43XMpzUMR20Lv+nT\nbySLVkVnkzFK5oyr35bLliRXMP5dJwH9HSTzWGFMGnfXN0yr1FBsZTwJWNGzez6a\nxX3tPFQXd4xwoZev+ZiEuaVgRGl6y1Va83QMw7rKOYA74NSBgMhZyhna+5O1fB3r\nH7mRsaCf+BI9HGYeu+mw9biJRBIHHqBcteT0I8wgXoxMews40elY5UrXYpHyfoV1\nSlYwLRcSaE4ugFO7zJIZGYrxE1Q6we6o6XuHsYCjyQKBgQDj/hOOJ89crQudFzm/\n1t8QHLWntQJzIU9NnazyXXT+coO3AX6qMDCwWy2o4gpku8gP4qqLErRLtCG+3f0T\nC6QHarLDhaONKIweArjJ7la9MsOqpeG9lZdOuzVxUWJCqTb75ykJBi/ickhDketb\nHJiGGTndU6YRIqc4atd4CKiO2wKBgQDbk2T9Nxm4TWvu5NRNYD9eMCVS8hFY5j0D\nU/Z4DDuO0ztktWVu+KQTMaMhn0iX+KjeuKt/ytfex8/uvbGx7cz9sUxP9GIZBKpB\nVTwNVr1Pt76YT5y+ngESlmueCVRQCFUYc//LCGeJh1s6PlmSM0ocV+8WvyrW9AUS\nYUx4g4ABZQKBgD/xyfBL8BfRHPnBQtwwWr29H6Ha3cYGqKRfPdt4JNEcsx6H18vJ\n2k4MNKEyTLH2DOWPsD9zTogRDIno3wsRb774yQyXlciIf8wG/Wb9ZuyHqWNaRRcU\nNqzJSvLuXX3O0fIS4mp6hsGfRe9VpMoYGhs6RgVyaZhSvM3RAX/UBdqTAoGAIC5A\n/c+GiHloWTHWX6S8hMxfnAF4Q2QzCvrSQ5PfYrZYnRDs1c/BFEMRGotis0sxTLsZ\n/3e2HaOBOQc6NM6aXZAPlCRIAEyruzmHvJi61CUk3OPGIDW+CIBdM2NApR4jgpr1\noUcRDZn159pdfEziDrdghh/sYmaPG7uA3qS/LPUCgYADPOzUYG45IPRb42R4qk0E\n5C83ekg5wz9PUsd6aZgRIvHZB3HgZ2p7bnHvMB0DBF+F4WPNB8zsY39lels/lC80\npDcK7XJtcm6ucbWJt0d8eyrxjlwGAzfcvOpubC/McVtW6Atj5+FVTi7dBvhqUSac\nzEXeRxpEeNilJzgNENDtAQ==\n-----END PRIVATE KEY-----\n";
    volatile boolean droppedCard = false, dragged20 = false;
    boolean pSaid = false; // Said something

    volatile byte psaychange = 0;

    volatile int aStage = 4; // Computer show pair stage
    volatile int mStage = 1; // Merge Stage
    int ccard; // Computers Card played
    int phoneDirection = -1;

    Button b20a, b20b,  bChange, bContinue; // bStart, bStop, bHelp;
    ImageView im0,im1,im2, im3, im4,
            imgCOut0, imgCOut1,
            imgComputerStitch0a, imgComputerStitch0b,
            imgPlayerStitch0a, imgPlayerStitch0b,
            imgPlayerStitch1a, imgPlayerStitch1b,
            imgPlayerStitch2a, imgPlayerStitch2b,
            imgPlayerStitch3a, imgPlayerStitch3b,
            imOut0, imOut1, imAtou, imTalon, imMerge;
    TextView tRest, tPoints, tMes, tDbg, tTournamentPoints;
    ConstraintLayout constraintRoot;
    RelativeLayout relativeTop, relativeLayoutStitches;
    // Menu myMenu;

    AnimationDrawable frameAnimation;
    AnimatedImageDrawable animatedGif;

    LinearLayout playerCard0, playerCard1, playerCard2, playerCard3, playerCard4,
            linLayoutCard0, linLayoutCard1,
            playedCard0, playedCard1, atouCard, talonCard;

    RelativeLayout linLayoutComputerStitch0, linLayoutPlayerStitch0, linLayoutPlayerStitch1, linLayoutPlayerStitch2, linLayoutPlayerStitch3;

    volatile Card touchedCard, draggedCard, playedOutCard0, playedOutCard1;
    final Card assignedCard = null;

    static java.lang.Runtime runtime = null;
    HashMap<Integer, Drawable> dragNDropMap = new HashMap<>();
    HashMap<Integer, LinearLayout> toFields = new HashMap<>();
    Game aGame;
    Tournament aTournament;
    // Calling Application class (see application tag in AndroidManifest.xml)
    // GlobalAppSettings globalVariable;

    private final static Handler setComputerPairHandler = new Handler(Looper.getMainLooper());
    private final static Handler showComputer20Handler = new Handler(Looper.getMainLooper());
    private final static Handler mergeByHandler = new Handler(Looper.getMainLooper());

    // @SuppressLint("InlinedApi")
    /**
     * setComputerPair new Runnable() -> { reSetComputerPair(); }
     */
    private final Runnable setComputerPair = () -> reSetComputerPair();

    /**
     * rShowComputer20 new Runnable() -> { showComputer20(playedOutCard1, aStage); }
     */
    private final Runnable rShowComputer20 =  new Runnable() {
        @Override
        // @SuppressLint("InlinedApi")
        public void run() {
            syncLocker = Integer.valueOf(ticks);
            synchronized (syncLocker) {
                ticks++;
                aStage = (aStage < 0) ? 1 : aStage;
            }
            showComputer20(playedOutCard1, aStage);
        }
    };

    // @SuppressLint("InlinedApi")
    /**
     * runMergeByHand new Runnable() -> { mergeByHand(mStage); }
     */
    private final Runnable runMergeByHand = () -> {
        syncLocker = Integer.valueOf(ticks);
        synchronized (syncLocker) {
            ticks++;
            mStage = (mStage < 1 || mStage > 11) ? 1 : mStage;
        }
        mergeByHand(mStage);
    };


    //region onCreateConfigChangedOptions

    /**
     * Override onCreate
     * @param savedInstanceState saved instance state
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        getGlobalAppSettings().initApplication();

        String layoutMes;
        int width = getResources().getDisplayMetrics().widthPixels;
        int height = getResources().getDisplayMetrics().heightPixels;

        Bundle bundle = getIntent().getExtras();
        // if (bundle != null) { }

        if (width > height) {
            phoneDirection = 0;
            layoutMes = getLocaleStringRes(R.string.landscape_mode);
            setContentView(R.layout.activity_main_vertical);
        } else {
            phoneDirection = 1;
            layoutMes = getLocaleStringRes(R.string.portrait_mode);
            if (width >= 392 && width < 500) {
                layoutMes += " " + getLocaleStringRes(R.string.width_400);
                setContentView(R.layout.activity_main_400);
            }
            else {
                setContentView(R.layout.activity_main);
            }
        }

        constraintRoot = (ConstraintLayout) findViewById(R.id.constraintRoot);
        constraintRoot.setDrawingCacheEnabled(false);
        rootViewGroup = (ViewGroup) constraintRoot;
        rootView = constraintRoot.getRootView();
        rootView.setDrawingCacheEnabled(false);

        relativeTop = (RelativeLayout) findViewById(R.id.relativeTop);
        relativeTop.setDrawingCacheEnabled(false);

        relativeLayoutStitches = (RelativeLayout)findViewById(R.id.relativeLayoutStitches);
        linLayoutCard0 = (LinearLayout) findViewById(R.id.linLayoutCard0);
        linLayoutCard1 = (LinearLayout) findViewById(R.id.linLayoutCard1);
        linLayoutComputerStitch0 = (RelativeLayout) findViewById(R.id.linLayoutComputerStitch0);
        linLayoutPlayerStitch0 = (RelativeLayout) findViewById(R.id.linLayoutPlayerStitch0);
        linLayoutPlayerStitch1 = (RelativeLayout) findViewById(R.id.linLayoutPlayerStitch1);
        linLayoutPlayerStitch2 = (RelativeLayout) findViewById(R.id.linLayoutPlayerStitch2);
        linLayoutPlayerStitch3 = (RelativeLayout) findViewById(R.id.linLayoutPlayerStitch3);
        relativeLayoutStitches.setVisibility(View.INVISIBLE);
        linLayoutCard0.setVisibility(View.INVISIBLE);
        linLayoutCard1.setVisibility(View.INVISIBLE);
        linLayoutComputerStitch0.setVisibility(View.INVISIBLE);
        linLayoutPlayerStitch0.setVisibility(View.INVISIBLE);
        linLayoutPlayerStitch1.setVisibility(View.INVISIBLE);
        linLayoutPlayerStitch2.setVisibility(View.INVISIBLE);
        linLayoutPlayerStitch3.setVisibility(View.INVISIBLE);
        playedCard0 = (LinearLayout) findViewById(R.id.playedCard0);
        playedCard1 = (LinearLayout) findViewById(R.id.playedCard1);
        atouCard = (LinearLayout) findViewById(R.id.atouCard);
        talonCard = (LinearLayout) findViewById(R.id.talonCard);
        playerCard0 = (LinearLayout) findViewById(R.id.playerCard0);
        playerCard1 = (LinearLayout) findViewById(R.id.playerCard1);
        playerCard2 = (LinearLayout) findViewById(R.id.playerCard2);
        playerCard3 = (LinearLayout) findViewById(R.id.playerCard3);
        playerCard4 = (LinearLayout) findViewById(R.id.playerCard4);

        im0 = (ImageView) findViewById(R.id.im0);
        im1 = (ImageView) findViewById(R.id.im1);
        im2 = (ImageView) findViewById(R.id.im2);
        im3 = (ImageView) findViewById(R.id.im3);
        im4 = (ImageView) findViewById(R.id.im4);

        imgCOut0 = (ImageView) findViewById(R.id.imgCOut0);
        imgCOut1 = (ImageView) findViewById(R.id.imgCOut1);
        imgComputerStitch0a = (ImageView) findViewById(R.id.imgComputerStitch0a);
        imgComputerStitch0b = (ImageView) findViewById(R.id.imgComputerStitch0b);
        imgPlayerStitch0a = (ImageView) findViewById(R.id.imgPlayerStitch0a);
        imgPlayerStitch0b = (ImageView) findViewById(R.id.imgPlayerStitch0b);
        imgPlayerStitch1a = (ImageView) findViewById(R.id.imgPlayerStitch1a);
        imgPlayerStitch1b = (ImageView) findViewById(R.id.imgPlayerStitch1b);
        imgPlayerStitch2a = (ImageView) findViewById(R.id.imgPlayerStitch2a);
        imgPlayerStitch2b = (ImageView) findViewById(R.id.imgPlayerStitch2b);
        imgPlayerStitch3a = (ImageView) findViewById(R.id.imgPlayerStitch3a);
        imgPlayerStitch3b = (ImageView) findViewById(R.id.imgPlayerStitch3b);
        imOut0 = (ImageView) findViewById(R.id.imOut0);
        imOut1 = (ImageView) findViewById(R.id.imOut1);
        imTalon = (ImageView) findViewById(R.id.imTalon);
        imAtou = (ImageView) findViewById(R.id.imAtou);
        imTalon.setVisibility(View.INVISIBLE);
        imAtou.setVisibility(View.INVISIBLE);

        // bStart = (Button) findViewById(R.id.bStart); bStop = (Button) findViewById(R.id.bStop); bHelp = (Button) findViewById(R.id.bHelp);
        b20a =  (Button) findViewById(R.id.b20a);
        b20b =  (Button) findViewById(R.id.b20b);
        bChange = (Button) findViewById(R.id.bChange);
        bContinue = (Button) findViewById(R.id.bContinue);

        tMes = (TextView) findViewById(R.id.tMes);
        tMes.setVisibility(View.INVISIBLE);
        tPoints = (TextView) findViewById(R.id.tPoints);
        tRest = (TextView) findViewById(R.id.tRest);
        tDbg = (TextView) findViewById(R.id.tDbg);
        // tDbg.setText(layoutMes);
        tMes.setVisibility(View.INVISIBLE);
        tTournamentPoints = (TextView) findViewById(R.id.tTournamentPoints);

        // bStop.setEnabled(false); bContinue.setEnabled(false); bStart.setEnabled(true); bHelp.setEnabled(true);
        toggleEnabled(bChange, false, getLocaleStringRes(R.string.bChange_text),
                getLocaleStringRes(R.string.bChange_text));
        // aGame.bChange = false;

        imMerge = (ImageView) findViewById(R.id.imMerge);
        imMerge.setBackgroundResource(R.drawable.anim_merge);
        mergeCardAnim(true);

        addListenerOnClickables();
        // initURLBase();

        aTournament = getGlobalAppSettings().getTournament();
        tTournamentPoints.setText(aTournament.getTournamentsTable());
        Game bGame = getGlobalAppSettings().getGame();
        if (bGame != null && bGame.isGame &&
            bGame.phoneDirection != phoneDirection)
        {
            aGame = new Game(getApplicationContext(), bGame);
            aGame.phoneDirection = phoneDirection;
            mergeCardAnim(false);

            tPoints.setText(String.valueOf(aGame.gambler.points));
            tRest.setText(String.valueOf((19-aGame.index)));
            if (!aGame.playersTurn) {
                resetButtons(0);
            }
            else {
                if (aGame.index < 18) {
                    if ((aGame.isAtouChangable(aGame.gambler)) && (!pSaid)) {
                        syncLocker = Integer.valueOf(ticks);
                        synchronized (syncLocker) {
                            ticks++;
                            psaychange += 1;
                        }
                        aGame.bChange = true;
                    }
                }
                // Gibts was zum Ansagen ?
                int a20 = aGame.gambler.hasPair();
                if (a20 > 0) {
                    syncLocker = Integer.valueOf(ticks);
                    synchronized (syncLocker) {
                        ticks++;
                        psaychange += 2;
                    }
                    aGame.a20 = true;
                    aGame.sayMarriage20 = aGame.printColor(aGame.gambler.handpairs[0]) + " " +
                            getLocaleStringRes(R.string.say_pair);

                    if (a20 > 1) {
                        syncLocker = Integer.valueOf(ticks);
                        synchronized (syncLocker) {
                            ticks++;
                            psaychange += 4;
                        }
                        aGame.b20 = true;
                        aGame.sayMarriage40 = aGame.printColor(aGame.gambler.handpairs[1]) + " " +
                                getLocaleStringRes(R.string.say_pair);
                    }
                }
            }
            toggleEnabled(bContinue, aGame.shouldContinue, getLocaleStringRes(R.string.bContinue_text),
                    getLocaleStringRes(R.string.bContinue_text));
            toggleEnabled(bChange, aGame.bChange, getLocaleStringRes(R.string.bChange_text),
                    getLocaleStringRes(R.string.bChange_text));
            toggleEnabled(b20a, aGame.a20, aGame.sayMarriage20, aGame.sayMarriage20);
            toggleEnabled(b20b, aGame.b20, aGame.sayMarriage40, aGame.sayMarriage40);
            setTextMessage(aGame.statusMessage);
            printMes();

            showTalonCard(aGame.schnapState);
            showAtouCard(aGame.schnapState);
            playedOutCard0 = aGame.playedOut0;
            playedOutCard1 = aGame.playedOut1;
            showStitches(-2);
            showPlayersCards();
            showPlayedOutCards();
            getGlobalAppSettings().setTournamentGame(aTournament, aGame);
        }
        else {
            resetButtons(0);
        }
    }

    /**
     * Override onConfigurationChanged
     * @param newConfig the new configuration
     */
    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        String layoutMes = "";
        int width = getResources().getDisplayMetrics().widthPixels;
        int height = getResources().getDisplayMetrics().heightPixels;

        super.onConfigurationChanged(newConfig);
        getGlobalAppSettings().initApplication();

        if (newConfig.orientation == Configuration.ORIENTATION_LANDSCAPE)
        {
            phoneDirection = 0;
            layoutMes = getLocaleStringRes(R.string.landscape_mode);
            setContentView(R.layout.activity_main_vertical);
        }
        if (newConfig.orientation == Configuration.ORIENTATION_PORTRAIT)
        {
            phoneDirection = 1;
            layoutMes = getLocaleStringRes(R.string.portrait_mode);
            if (width >= 392 && width < 500) {
                layoutMes += " " + getLocaleStringRes(R.string.width_400);
                setContentView(R.layout.activity_main_400);
            }
            else {
                setContentView(R.layout.activity_main);
            }
        }

        if (tDbg != null) {
            tDbg.setText(layoutMes);
        }
    }

    //endregion

    //region menuCreateItemSelected

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        myMenu = menu;
        if (myMenu != null) {
            menuResetLanguageCheckboxes(myMenu);
            setCardDeckFromSystemLocale();
            if (getGlobalAppSettings().getSound())
                checkSoundMenuItem(true);
        }
        return true;
    }

    /**
     * Handles action bar item clicks in option menu.
     * @param item in options menu, that has been clicked
     */
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        int mItemId = (item != null) ?  item.getItemId() : -1;

        if (mItemId == R.id.id_action_start) {
            if (aGame == null || !aGame.isGame)
                startGame();
            return true;
        }
        if (mItemId == R.id.id_action_restart) {
            if (aGame != null && aGame.isGame) {
                stopGame(7, PLAYERDEF.COMPUTER, 3,
                        getLocaleStringRes(R.string.ending_game));
            }
            return true;
        }
        if (mItemId == R.id.id_action_help) {
            showHelp();
            return true;
        }
        if (mItemId == R.id.id_action_about) {
            startAboutActivity();
            return true;
        }
        if (mItemId == R.id.id_action_screenshot) {
            rootView.setDrawingCacheEnabled(false);
            takeScreenShot(rootView, true);
            return true;
        }
        if (mItemId == R.id.id_action_sound) {
            toggleSoundOnOff();
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    //endregion

    /**
     * setLanguage
     * @param aLocale - a locale
     * @param item - a menu item
     * @return true in case of succcess, otherwise false
     */
    @Override
    protected boolean setLocale(Locale aLocale, MenuItem item) {

        if (getGlobalAppSettings().getLocale().getLanguage() != aLocale.getLanguage()) {

            playL8rHandler.postDelayed(delayPlayScreenshot, 100);

            //Overwrites application locale in GlobalAppSettings with english
            getGlobalAppSettings().setLocale(aLocale);
            // Adjust language for text to speach
            // text2Speach.setLanguage(globalVariable.getLocale());

            if (aGame != null) {
                showAtouCard(aGame.schnapState);
                showPlayersCards();
                showPlayedOutCards();
            }
        }

        if (myMenu != null && item != null) {
            menuResetLanguageCheckboxes(myMenu);
            item.setChecked(true);
            if (getGlobalAppSettings().getSound())
                checkSoundMenuItem(true);
        }

        return true;
    }

    //region ClickDragDrop

    /**
     * add listeners on all clickables
     */
    public void addListenerOnClickables() {

        playedCard0 = (LinearLayout) findViewById(R.id.playedCard0);
        playedCard0.setOnDragListener((view, dragEvent) -> layoutView_OnDragHandler(view, dragEvent, -2));
        playedCard1 = (LinearLayout) findViewById(R.id.playedCard1);
        playedCard1.setOnDragListener((view, dragEvent) -> layoutView_OnDragHandler(view, dragEvent, -2));

        playerCard0 = (LinearLayout) findViewById(R.id.playerCard0);
        playerCard1 = (LinearLayout) findViewById(R.id.playerCard1);
        playerCard2 = (LinearLayout) findViewById(R.id.playerCard2);
        playerCard3 = (LinearLayout) findViewById(R.id.playerCard3);
        playerCard4 = (LinearLayout) findViewById(R.id.playerCard4);

        bChange = (Button) findViewById(R.id.bChange);
        bChange.setOnClickListener(this::bChange_Clicked);
        b20a = (Button) findViewById(R.id.b20a);
        b20a.setOnClickListener(this::b20a_Clicked);

        b20b = (Button) findViewById(R.id.b20b);
        b20b.setOnClickListener(this::b20b_Clicked);
        bContinue = (Button) findViewById(R.id.bContinue);
        bContinue.setOnClickListener(this::bContinue_Clicked);

        imgComputerStitch0a.setOnClickListener(v -> showStitches(-1));
        imgComputerStitch0b.setOnClickListener(v -> showStitches(-1));

        imgPlayerStitch0a.setOnClickListener(v -> showStitches(0));
        imgPlayerStitch0b.setOnClickListener(v -> showStitches(0));
        imgPlayerStitch1a.setOnClickListener(v -> showStitches(1));
        imgPlayerStitch1b.setOnClickListener(v -> showStitches(1));
        imgPlayerStitch2a.setOnClickListener(v -> showStitches(2));
        imgPlayerStitch2b.setOnClickListener(v -> showStitches(2));
        imgPlayerStitch3a.setOnClickListener(v -> showStitches(3));
        imgPlayerStitch3b.setOnClickListener(v -> showStitches(3));

        im0 = (ImageView) findViewById(R.id.im0);
        im0.setOnTouchListener((view, motionEvent) -> image_OnTouchListener(view, motionEvent, 0));

        im1 = (ImageView) findViewById(R.id.im1);
        im1.setOnTouchListener((view, motionEvent) -> image_OnTouchListener(view, motionEvent, 1));

        im2 = (ImageView) findViewById(R.id.im2);
        im2.setOnTouchListener((view, motionEvent) -> image_OnTouchListener(view, motionEvent, 2));

        im3 = (ImageView) findViewById(R.id.im3);
        im3.setOnTouchListener((view, motionEvent) -> image_OnTouchListener(view, motionEvent, 3));

        im4 = (ImageView) findViewById(R.id.im4);
        im4.setOnTouchListener((view, motionEvent) -> image_OnTouchListener(view, motionEvent, 4));

        imAtou = (ImageView) findViewById(R.id.imAtou);
        imAtou.setOnClickListener(arg0 -> imageView_ClickEventHandler(arg0, 10));

        imgCOut0 = (ImageView) findViewById(R.id.imgCOut0);
        imgCOut1 = (ImageView) findViewById(R.id.imgCOut1);
        imOut0 = (ImageView) findViewById(R.id.imOut0);
        imOut0.setOnClickListener(arg0 -> imOut_Click(arg0));
        imOut1 = (ImageView) findViewById(R.id.imOut1);
        imOut1.setOnClickListener(arg0 -> imOut_Click(arg0));
        imMerge = (ImageView) findViewById(R.id.imMerge);
        imMerge.setOnClickListener(arg0 -> imMerge_Click(arg0));
    }

    /**
     * imOut_Click at reset game
     * @param arg0 View
     */
    public void imOut_Click(View arg0) {
        if (arg0 == null || arg0.getVisibility() == View.INVISIBLE)
            return;
        // TODO: What shell we do with the drunken sailor?
        /*
        if (!bContinue.isEnabled()) {
            if (((playedOutCard0 == null || playedOutCard0.getCardColor() == CARDCOLOR.EMPTY) &&
                    (arg0.getId() == R.id.imOut0 || arg0.getTag() == "imOut0_tag")) ||
                ((playedOutCard1 == null || playedOutCard1.getCardColor() == CARDCOLOR.EMPTY) &&
                        (arg0.getId() == R.id.imOut1 || arg0.getTag() == "imOut1_tag"))) {

                stopGame(3, getString(R.string.ending_game));
            }
        }
         */
    }

    /**
     * imMerge_Click
     * @param arg0 view that was clicked
     */
    public void imMerge_Click(View arg0) {
        if (arg0 == null && arg0.getVisibility() == View.VISIBLE)
            gameStartAnimation(true);
    }

    /**
     * b20a_Clicked
     * @param arg0 view that was clicked
     */
    public void bChange_Clicked(View arg0) {
        try {
            aGame.changeAtou(aGame.gambler);

            sayText(getLocaleStringRes(R.string.bChange_text));

            aGame.bChange = false;
            toggleEnabled(bChange, aGame.bChange, getLocaleStringRes(R.string.bChange_text),
                    getLocaleStringRes(R.string.bChange_text));

            showAtouCard(aGame.schnapState);
            showPlayersCards();

            getGlobalAppSettings().setTournamentGame(aTournament, aGame);

            gameTurn(1);
        } catch (Exception e) {
            this.errHandler(e);
        }
    }

    /**
     * b20a_Clicked
     * @param arg0 view that was clicked
     */
    public void b20a_Clicked(View arg0) {
        try {
            if ((pSaid) || (aGame.gambler.handpairs[0] == 'n')) {
                return;
            }
            String sayPair;
            aGame.said = aGame.gambler.handpairs[0];
            if (aGame.gambler.handpairs[0] == aGame.atouInGame()) {
                aGame.gambler.points += 40;
                sayPair = getLocaleStringRes(
                            R.string.fourty_in_color) +  " " + aGame.printColor(aGame.said);
            } else {
                aGame.gambler.points += 20;
                sayPair = getLocaleStringRes(
                            R.string.twenty_in_color) + " " + aGame.printColor(aGame.said);
            }
            pSaid = true;
            resetButtons(0);

            setTextMessage(String.format(getLocaleStringRes(R.string.you_say_pair),
                    aGame.printColor(aGame.said)));
            // setTextMessage(getString(
            //    getLocaleStringRes(R.string.you_say_pair), aGame.printColor(aGame.said)));
            sayText(sayPair);
            aGame.insertMsg(String.format(getLocaleStringRes(R.string.you_say_pair),
                    aGame.printColor(aGame.said)));
            // aGame.insertMsg(getString(
            //        getLocaleStringRes(R.string.you_say_pair), aGame.printColor(aGame.said)));
            ;
            printMes();

            tPoints.setText(String.valueOf(aGame.gambler.points));
            if (aGame.gambler.points > 65) {
                twentyEnough(true);
            }
        } catch (Exception e) {
            this.errHandler(e);
        }
    }

    /**
     * b20b_Clicked
     * @param arg0 view that was clicked
     */
    public void b20b_Clicked(View arg0) {
        try {
            if ((pSaid) || (aGame.gambler.handpairs[1]=='n')) {
                return;
            }
            String sayPair;
            aGame.said = aGame.gambler.handpairs[1];
            if (aGame.gambler.handpairs[1] == aGame.atouInGame()) {
                aGame.gambler.points += 40;
                // sayPair = getString(R.string.fourty_in_color) + " " + aGame.printColor(aGame.said);
                sayPair = getLocaleStringRes(R.string.fourty_in_color) + " " + aGame.printColor(aGame.said);
            }
            else {
                aGame.gambler.points += 20;
                // sayPair = getString(R.string.twenty_in_color) + " " + aGame.printColor(aGame.said);
                sayPair = getLocaleStringRes(R.string.twenty_in_color) + " " + aGame.printColor(aGame.said);
            }
            pSaid = true;
            resetButtons(0);

            setTextMessage(String.format(getLocaleStringRes(R.string.you_say_pair),
                    aGame.printColor(aGame.said)));
            // setTextMessage(
            //         getLocaleStringRes(R.string.you_say_pair), aGame.printColor(aGame.said));
            sayText(sayPair);

            aGame.insertMsg(String.format(getLocaleStringRes(R.string.you_say_pair),
                    aGame.printColor(aGame.said)));
            // aGame.insertMsg(
            //         getLocaleStringRes(R.string.you_say_pair), aGame.printColor(aGame.said));
            printMes();

            tPoints.setText(String.valueOf(aGame.gambler.points));
            if (aGame.gambler.points > 65) {
                twentyEnough(true);
            }
        } catch (Exception e) {
            this.errHandler(e);
        }
    }

    /**
     * bContinue_Clicked
     * @param arg0 view that was clicked
     */
    public void bContinue_Clicked(View arg0) {
        continueTurn();
        imTalon.setOnClickListener(null);
    }

    /**
     * OnTouchListener
     * @param view current view
     * @param motionEvent the motion Event
     * @param ic additional indexer
     * @return true|false
     */
    protected boolean image_OnTouchListener(View view, MotionEvent motionEvent, int ic) {

        if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {

            if (!aGame.gambler.hand[ic].isValidCard()) {
                String sNoValidCard = getLocaleStringRes(R.string.this_is_no_valid_card);
                // setTextMessage(getString(R.string.this_is_no_valid_card));
                setTextMessage(sNoValidCard);

                aGame.insertMsg(sNoValidCard);
                printMes();
                return false;
            }

            atouCard = (LinearLayout) findViewById(R.id.atouCard);
            atouCard.setOnDragListener(null);

            ClipData data = ClipData.newPlainText("", "");
            DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(view);

            int viewId = view.getId();
            String tmp;

            for (int i = 0; i < 5; i++){
                tmp = "im" + i;
                int myID = getApplicationContext().getResources().getIdentifier(tmp, "id", getApplicationContext().getPackageName());
                if (viewId == myID) {
                    touchedCard = aGame.gambler.hand[i];
                    break;
                }
            }

            if ((aGame.isAtouChangable(aGame.gambler)) && (!pSaid) &&
                    touchedCard.getCardValue() == CARDVALUE.JACK && touchedCard.isAtou()) {
                atouCard = (LinearLayout) findViewById(R.id.atouCard);
                atouCard.setOnDragListener((view1, dragEvent) -> layoutView_OnDragHandler(view1, dragEvent, -1));
            }

            playerCard0 = (LinearLayout) findViewById(R.id.playerCard0);
            playerCard0.setOnDragListener(null);
            playerCard1 = (LinearLayout) findViewById(R.id.playerCard1);
            playerCard1.setOnDragListener(null);
            playerCard2 = (LinearLayout) findViewById(R.id.playerCard2);
            playerCard2.setOnDragListener(null);
            playerCard3 = (LinearLayout) findViewById(R.id.playerCard3);
            playerCard3.setOnDragListener(null);
            playerCard4 = (LinearLayout) findViewById(R.id.playerCard4);
            playerCard4.setOnDragListener(null);

            if (touchedCard.getCardValue() == CARDVALUE.QUEEN ||
                    touchedCard.getCardValue() == CARDVALUE.KING) {

                for (int idx = 0; idx < 5; idx++) {
                    Card dropCard = aGame.gambler.hand[idx];
                    if (dropCard.isValidCard() &&
                            (dropCard.getCardColor() == touchedCard.getCardColor()) &&
                            (dropCard.getCardValue().getValue() + touchedCard.getCardValue().getValue()
                                    == 7)) {

                        switch (idx) {
                            case 0:
                                playerCard0.setOnDragListener((view12, dragEvent) -> layoutView_OnDragHandler(view12, dragEvent, 0));
                                break;
                            case 1:
                                playerCard1.setOnDragListener((view13, dragEvent) -> layoutView_OnDragHandler(view13, dragEvent, 1));
                                break;
                            case 2:
                                playerCard2.setOnDragListener((view14, dragEvent) -> layoutView_OnDragHandler(view14, dragEvent, 2));
                                break;
                            case 3:
                                playerCard3.setOnDragListener((view15, dragEvent) -> layoutView_OnDragHandler(view15, dragEvent, 3));
                                break;
                            case 4:
                                playerCard4.setOnDragListener((view16, dragEvent) -> layoutView_OnDragHandler(view16, dragEvent, 4));
                                break;
                            default:
                                break;
                        }
                    }
                }

            }

            // view.startDragAndDrop(data, shadowBuilder, view, 0);
            view.startDragAndDrop(data, shadowBuilder, view, 0);
            view.setVisibility(View.INVISIBLE);
            return true;
        } else {
            return false;
        }
    }

    /**
     * OnDragHandler
     * @param view current view
     * @param dragEvent current drag event
     * @param ic additional index to specify event
     * @return true|false
     */
    protected boolean layoutView_OnDragHandler(View view, DragEvent dragEvent, int ic) {

        Drawable enterShape = getResources().getDrawable(R.drawable.shape_droptarget, getApplicationContext().getTheme());
        Drawable normalShape = getResources().getDrawable(R.drawable.shape, getApplicationContext().getTheme());

        if (touchedCard == null)
            return false;

        Drawable savedBackground;
        int action = dragEvent.getAction();
        switch (dragEvent.getAction()) {
            case DragEvent.ACTION_DRAG_STARTED:
                // do nothing
                savedBackground = view.getBackground();
                if (!dragNDropMap.containsKey(view.getId())) {
                    dragNDropMap.put(view.getId(), savedBackground);
                }

                break;
            case DragEvent.ACTION_DRAG_ENTERED:
                savedBackground = view.getBackground();
                if (!dragNDropMap.containsKey(view.getId())) {
                    dragNDropMap.put(view.getId(), savedBackground);
                }
                view.setBackground(enterShape);
                break;
            case DragEvent.ACTION_DRAG_EXITED:

                if (dragNDropMap.containsKey(view.getId())) {
                    Drawable dr = dragNDropMap.get(view.getId());
                    if (dr != null) {
                        view.setBackground(enterShape);
                        view.setBackground(dr);
                        dragNDropMap.remove(view.getId());
                    }
                }

                view.setBackground(normalShape);
                break;
            case DragEvent.ACTION_DROP:
                // Dropped, reassign View to ViewGroup
                View dropView = (View) dragEvent.getLocalState();
                ViewGroup owner = (ViewGroup) dropView.getParent();
                // owner.removeView(dropView);
                LinearLayout container = (LinearLayout) view;

                dragged20 = false;
                droppedCard = true;

                int viewID = dropView.getId();
                int lcId = container.getId();

                if (lcId == R.id.atouCard || ic == -1) {
                    draggedCard = touchedCard;
                    touchedCard = null;
                    // container.addView(view, 0);
                    view.setVisibility(View.VISIBLE);
                    bChange_Clicked(view);
                    showPlayersCards();
                    return true;
                }
                if (lcId == R.id.playedCard0 || lcId == R.id.playedCard1 || ic == -2) {
                    // container.addView(view, 0);
                    view.setVisibility(View.VISIBLE);
                    draggedCard = touchedCard;
                    touchedCard = null;

                    String tmp;
                    for (int i = 0; i < 5; i++){
                        tmp = "im" + i;
                        int myID = getApplicationContext().getResources().getIdentifier(tmp, "id", getApplicationContext().getPackageName());
                        if (viewID == myID) {
                            imageView_ClickEventHandler(view, i);
                            return true;
                        }
                    }
                }

                if (lcId == R.id.playerCard0 || lcId == R.id.playerCard1 || lcId == R.id.playerCard2 ||
                        lcId == R.id.playerCard3 || lcId == R.id.playerCard4) {
                    if (touchedCard.getCardValue() != CARDVALUE.QUEEN &&
                            touchedCard.getCardValue() != CARDVALUE.KING) {
                        view.setVisibility(View.VISIBLE);
                        return false;
                    }
                    Card dropCard = null;
                    String tmp;
                    for (int i = 0; i < 5; i++){
                        tmp = "playerCard" + i;
                        int myID = getApplicationContext().getResources().getIdentifier(tmp, "id", getApplicationContext().getPackageName());
                        if (lcId == myID) {
                            dropCard = aGame.gambler.hand[i];
                        }
                    }
                    switch (lcId) {
                        case R.id.playerCard0:
                            dropCard = aGame.gambler.hand[0];
                            break;
                        case R.id.playerCard1:
                            dropCard = aGame.gambler.hand[1];
                            break;
                        case R.id.playerCard2:
                            dropCard = aGame.gambler.hand[2];
                            break;
                        case R.id.playerCard3:
                            dropCard = aGame.gambler.hand[3];
                            break;
                        case R.id.playerCard4:
                            dropCard = aGame.gambler.hand[4];
                            break;
                        default:
                            break;
                    }
                    if ((dropCard.getCardColor().getChar()
                            == touchedCard.getCardColor().getChar()) &&
                            (dropCard.getCardValue().getValue() +
                                    touchedCard.getCardValue().getValue()
                                    == 7)) {

                        if (dropCard.isAtou()) {
                            // if (aGame.gambler.handpairs[0] == aGame.atouInGame) {
                            aGame.gambler.points += 40;
                        } else {
                            aGame.gambler.points += 20;
                        }
                        pSaid = true;
                        resetButtons(0);
                        aGame.said = dropCard.getCardColor().getChar();

                        String sayMarriage = String.format(
                            getLocaleStringRes(R.string.you_say_pair),  aGame.printColor(aGame.said));
                        setTextMessage(sayMarriage);
                        sayText(sayMarriage);
                        aGame.insertMsg(sayMarriage);
                        printMes();

                        tPoints.setText(String.valueOf(aGame.gambler.points));
                        if (aGame.gambler.points > 65) {
                            twentyEnough(true);
                        }

                        dragged20 = true;

                        switch (viewID) {
                            case R.id.im0:
                                imageView_ClickEventHandler(view, 0);
                                im0.setImageResource(R.drawable.e);
                                break;
                            case R.id.im1:
                                imageView_ClickEventHandler(view, 1);
                                im1.setImageResource(R.drawable.e);
                                break;
                            case R.id.im2:
                                imageView_ClickEventHandler(view, 2);
                                im2.setImageResource(R.drawable.e);
                                break;
                            case R.id.im3:
                                imageView_ClickEventHandler(view, 3);
                                im3.setImageResource(R.drawable.e);
                                break;
                            case R.id.im4:
                                imageView_ClickEventHandler(view, 4);
                                im4.setImageResource(R.drawable.e);
                                break;
                            default:
                                // assert(0);
                                break;
                        }

                        view.setVisibility(View.VISIBLE);
                        return true;
                    }

                    view.setVisibility(View.VISIBLE);
                    showPlayersCards();
                    return false;

                }

                // container.addView(view, 0);
                // container.addView(view);
                view.setVisibility(View.VISIBLE);
                break;
            case DragEvent.ACTION_DRAG_LOCATION:
            case DragEvent.ACTION_DRAG_ENDED:

                if (dragNDropMap.containsKey(view.getId())) {
                    Drawable dr = dragNDropMap.get(view.getId());
                    if (dr != null) {
                        view.setBackground(enterShape);
                        view.setBackground(dr);
                        dragNDropMap.remove(view.getId());
                    }
                }

                if (!dragged20 || !droppedCard)
                    showPlayersCards();
            default:
                break;
        }
        return true;
    }

    /**
     * EventHandler for all ImageViews
     * @param arg0 View, that fired click
     * @param ic image counter, that represents which ImageView is clicked
     */
    protected void imageView_ClickEventHandler(View arg0, int ic) {
        int j;
        // don't let player drag and drop cards, when he shouldn't
        if (aGame != null && !aGame.isReady) {
            return;
        }

        try {
            if (ic == 10) {
                // TODO: more proper states
                if (aGame.playersTurn && (!aGame.isClosed) &&  (!pSaid) && (aGame.index < 16)) {
                    closeGame(PLAYERDEF.HUMAN);
                }
                return;
            }
            if (!aGame.gambler.hand[ic].isValidCard()) {
                String sInvalidCard = getLocaleStringRes(R.string.this_is_no_valid_card);
                setTextMessage(sInvalidCard);
                aGame.insertMsg(sInvalidCard);
                printMes();
                return;
            }
            if (pSaid) {
                if ((aGame.said == aGame.gambler.hand[ic].getCardColor().getChar()) &&
                        (aGame.gambler.hand[ic].getCardValue().getValue() > 2) &&
                        (aGame.gambler.hand[ic].getCardValue().getValue() < 5)) {
                    ; // we can continue
                } else {
                    String sMustPlayOtherCard = getLocaleStringRes(R.string.you_must_play_pair_card);
                    setTextMessage(sMustPlayOtherCard);
                    aGame.insertMsg(sMustPlayOtherCard);
                    printMes();
                    return ;
                }
            }
            if (aGame.colorHitRule && (!aGame.playersTurn)) {
                // CORRECT WAY ?
                if ((!aGame.gambler.isValidInColorHitsContext(ic,aGame.computer.hand[ccard]))) {
                    String sMustColorHitRules = getLocaleStringRes(R.string.you_must_play_color_hit_force_rules);
                    setTextMessage(sMustColorHitRules);
                    aGame.insertMsg(sMustColorHitRules);
                    int tmpint = aGame.gambler.preferedInColorHitsContext(aGame.computer.hand[ccard]);
                    // for (j = 0; j < 5; j++) {
                    //     c_array = c_array + aGame.gambler.colorHitArray[j] + " ";
                    // }
                    // aGame.mqueue.insert(c_array);

                    aGame.insertMsg(
                            String.format(getLocaleStringRes(R.string.best_card_would_be),
                                aGame.gambler.hand[tmpint].getName()));
                    printMes();
                    showPlayersCards();
                    return ;
                }
            }
            if (psaychange > 0) {
                resetButtons(0);
                psaychange = 0;
            }
            aGame.playedOut = aGame.gambler.hand[ic];
            // Besser Cards als Array
            String tmp = "im" + ic;
            int myID = getApplicationContext().getResources().getIdentifier(tmp, "id", getApplicationContext().getPackageName());
            ImageView cardPlayed = (ImageView) findViewById(myID);
            cardPlayed.setImageResource(R.drawable.e);

            switch (ic) {
                case 0:
                    im0.setImageResource(R.drawable.e);
                    break;
                case 1:
                    im1.setImageResource(R.drawable.e);
                    break;
                case 2:
                    im2.setImageResource(R.drawable.e);
                    break;
                case 3:
                    im3.setImageResource(R.drawable.e);
                    break;
                case 4:
                    im4.setImageResource(R.drawable.e);
                    break;
                default: if (tDbg != null) tDbg.append("Assertion !");
            }

            playedOutCard0 = aGame.gambler.hand[ic];
            aGame.playedOut0 = playedOutCard0;
            imOut0.setImageDrawable(aGame.gambler.hand[ic].getDrawable());

        } catch (Exception e) {
            this.errHandler(e);
        }
        aGame.gambler.hand[ic] = getGlobalAppSettings().cardEmpty();
        aGame.isReady = false;

        getGlobalAppSettings().setTournamentGame(aTournament, aGame);

        endTurn();

    }

    //endregion

    /**
     * implements Runnable
     */
    public void run() {

    }

    /**
     * init all ImageView's with default empty values
     */
    @Deprecated
    public void initURLBase() {
        im0.setImageResource(R.drawable.n0);
        im1.setImageResource(R.drawable.n0);
        im2.setImageResource(R.drawable.n0);
        im3.setImageResource(R.drawable.n0);
        im4.setImageResource(R.drawable.n0);
        imAtou.setImageResource(R.drawable.n0);
        imTalon.setImageResource(R.drawable.t);
        imOut0.setImageResource(R.drawable.e);
        imOut1.setImageResource(R.drawable.e);
        imgCOut0.setImageResource(R.drawable.e);
        imgCOut1.setImageResource(R.drawable.e);
        imgComputerStitch0a.setImageResource(R.drawable.n1);
        imgComputerStitch0b.setImageResource(R.drawable.n1);
        imgPlayerStitch0a.setImageResource(R.drawable.n1);
        imgPlayerStitch0b.setImageResource(R.drawable.n1);
        imgPlayerStitch1a.setImageResource(R.drawable.n1);
        imgPlayerStitch1b.setImageResource(R.drawable.n1);
        imgPlayerStitch2a.setImageResource(R.drawable.n1);
        imgPlayerStitch2b.setImageResource(R.drawable.n1);
        imgPlayerStitch3a.setImageResource(R.drawable.n1);
        imgPlayerStitch3b.setImageResource(R.drawable.n1);
        imTalon.setVisibility(View.INVISIBLE);
        imAtou.setVisibility(View.INVISIBLE);
        playedOutCard0 = null;
        playedOutCard1 = null;
    }

    //region showUI

    /**
     * gameStartAnimation - starts game with animation of splitting or knocking
     * @param splitKnock - boolean true for split, false for knock
     */
    protected void gameStartAnimation(boolean splitKnock) {
        startGame();
    }

    /**
     * mergeCardAnim
     * @param startMergeAnim true to start merge anim, false to stop
     */
    protected void mergeCardAnim(boolean startMergeAnim) {
        if (startMergeAnim) {
            // animatedGif = null;
            showAtouCard(SCHNAPSTATE.MERGING_CARDS);
            showTalonCard(SCHNAPSTATE.MERGING_CARDS);
            imMerge.setVisibility(View.VISIBLE);
            if (android.os.Build.VERSION.SDK_INT >= Build.VERSION_CODES.P) {
                try {
                    ImageDecoder.Source source =
                            ImageDecoder.createSource(getResources(), R.drawable.mergecards);
                    Drawable animDrawable = ImageDecoder.decodeDrawable(source);
                    imMerge.setImageDrawable(animDrawable);

                    if (animDrawable instanceof AnimatedImageDrawable) {
                        animatedGif = ((AnimatedImageDrawable)animDrawable);
                        // ((AnimatedImageDrawable) animDrawable).start();
                        ((AnimatedImageDrawable)animatedGif).start();
                    }
                }
                catch (Exception exCraw) {
                    this.errHandler(exCraw);
                }
                // frameAnimation = (AnimationDrawable)imMerge.getBackground();
                // frameAnimation.start();
            }
            else {
                imMerge.setImageResource(R.drawable.merge1);
                mergeByHandler.postDelayed(runMergeByHand, 250);
            }

        }
        if (!startMergeAnim) {
            // frameAnimation.stop();
            if (android.os.Build.VERSION.SDK_INT >= Build.VERSION_CODES.P) {
                if (animatedGif != null)
                    animatedGif.stop();
            }
            else {
                try {
                    mergeByHandler.removeCallbacks(runMergeByHand);
                }
                catch (Exception exMerge) {
                    exMerge.printStackTrace();
                }
            }
            showAtouCard(SCHNAPSTATE.GAME_START);
            showTalonCard(SCHNAPSTATE.GAME_START);
            imMerge.setVisibility(View.INVISIBLE);
        }
    }

    /**
     * mergeByHand merge animation by runnabler handler
     *
     * @param stage - int stage
     */
    protected void mergeByHand(int stage) {
        mStage = (stage > 0 && stage <= 11) ? stage : 1;

        switch (mStage) {
            case 2:
            case 7:
                imMerge.setImageResource(R.drawable.merge2);
                break;
            case 3:
            case 8:
                imMerge.setImageResource(R.drawable.merge3);
                break;
            case 4:
            case 9:
                imMerge.setImageResource(R.drawable.merge4);
                break;
            case 5:
            case 10:
                imMerge.setImageResource(R.drawable.merge5);
                break;
            case 6:
            case 11:
                imMerge.setImageResource(R.drawable.merge6);
                break;
            case 0:
            case 1:
            default:
                imMerge.setImageResource(R.drawable.merge1);
                break;
        }

        syncLocker = Integer.valueOf(ticks);
        synchronized (syncLocker) {
            ticks++;
            ++mStage;
        }
        if (mStage <= 11) {
            mergeByHandler.postDelayed(runMergeByHand, 250);
        }
        else {
            try {
                mergeByHandler.removeCallbacks(runMergeByHand);
            }
            catch (Exception exMerge) {
                exMerge.printStackTrace();
            }
        }
    }

    /**
     * showTalonCard - shows current talon card
     */
    protected void showTalonCard(SCHNAPSTATE gameState) {
        try {
            if (gameState.getValue() > 0 && gameState.getValue() < 6) {
                imTalon.setImageResource(R.drawable.t);
                imTalon.setVisibility(View.VISIBLE);
                talonCard.setVisibility(View.VISIBLE);
            }
            else  {
                imTalon.setImageResource(R.drawable.e1);
                imTalon.setVisibility(View.INVISIBLE);
                talonCard.setVisibility(View.INVISIBLE);
            }
        } catch (Exception imTalonEx) {
            this.errHandler(imTalonEx);
        }
    }

    /**
     * showAtouCard - shows current atou card => needed when changing locale and card set
     */
    protected void showAtouCard(SCHNAPSTATE gameState) {
        try {
            if (gameState.getValue() > 0 && gameState.getValue() < 6) {
                if (gameState == SCHNAPSTATE.GAME_CLOSED)
                    imAtou.setImageResource(R.drawable.n1);
                else
                    imAtou.setImageDrawable(aGame.set[19].getDrawable());
                imAtou.setVisibility(View.VISIBLE);
                atouCard.setVisibility(View.VISIBLE);
            } else {
                imAtou.setImageResource(R.drawable.e1);
                imAtou.setVisibility(View.INVISIBLE);
                atouCard.setVisibility(View.INVISIBLE);
            }
        } catch (Exception exp) {
            this.errHandler(exp);
        }
    }

    /**
     * showPlayedOutCards - shows playedOutCards => needed when changing locale and card deck
     */
    protected  void showPlayedOutCards() {
        if ((aGame != null && aGame.playedOut0 != null && playedOutCard0 != null &&
            !aGame.playedOut0.getColorValue().equals(playedOutCard0.getColorValue())) ||
                (aGame != null && aGame.playedOut0 != null && playedOutCard0 == null)) {
            playedOutCard0 = aGame.playedOut0;
        }
        if (aGame == null && playedOutCard0 == null)
            playedOutCard0 = getGlobalAppSettings().cardEmpty();
        imOut0.setImageDrawable(playedOutCard0.getDrawable());

        if ((aGame != null && aGame.playedOut1 != null && playedOutCard1 != null &&
                !aGame.playedOut1.getColorValue().equals(playedOutCard1.getColorValue())) ||
                (aGame != null && aGame.playedOut1 != null && playedOutCard1 == null)) {
            playedOutCard1 = aGame.playedOut1;
        }
        if (playedOutCard1 == null)
            playedOutCard1 = getGlobalAppSettings().cardEmpty();
        imOut1.setImageDrawable(playedOutCard1.getDrawable());
    }

    /**
     * showPlayersCards
     */
    protected void showPlayersCards() {
        try {
            Drawable normalShape = ResourcesCompat.getDrawable(getResources(), R.drawable.shape, null);

            Card handCard = (aGame.gambler != null && aGame.gambler.hand[0].isValidCard()) ?
                    aGame.gambler.hand[0] : getGlobalAppSettings().cardEmpty();
            playerCard0.setVisibility(View.VISIBLE);
            playerCard0.setBackground(normalShape);
            im0.setBackground(normalShape);
            im0.setImageDrawable(handCard.getDrawable());
            im0.setVisibility(View.VISIBLE);

            handCard =  (aGame.gambler != null && aGame.gambler.hand[1].isValidCard()) ?
                    aGame.gambler.hand[1] : getGlobalAppSettings().cardEmpty();
            playerCard1.setVisibility(View.VISIBLE);
            playerCard1.setBackground(normalShape);
            im1.setBackground(normalShape);
            im1.setImageDrawable(handCard.getDrawable());
            im1.setVisibility(View.VISIBLE);

            handCard = (aGame.gambler != null && aGame.gambler.hand[2].isValidCard()) ?
                aGame.gambler.hand[2] : getGlobalAppSettings().cardEmpty();
            playerCard2.setVisibility(View.VISIBLE);
            playerCard2.setBackground(normalShape);
            im2.setBackground(normalShape);
            im2.setImageDrawable(handCard.getDrawable());
            im2.setVisibility(View.VISIBLE);

            handCard = (aGame.gambler != null && aGame.gambler.hand[3].isValidCard()) ?
                    aGame.gambler.hand[3] : getGlobalAppSettings().cardEmpty();
            playerCard3.setVisibility(View.VISIBLE);
            playerCard3.setBackground(normalShape);
            im3.setBackground(normalShape);
            im3.setImageDrawable(handCard.getDrawable());
            im3.setVisibility(View.VISIBLE);

            handCard = (aGame.gambler != null && aGame.gambler.hand[4].isValidCard()) ?
                 aGame.gambler.hand[4] : getGlobalAppSettings().cardEmpty();
            playerCard4.setVisibility(View.VISIBLE);
            playerCard4.setBackground(normalShape);
            im4.setBackground(normalShape);
            im4.setImageDrawable(handCard.getDrawable());
            im4.setVisibility(View.VISIBLE);

        } catch (Exception exp) {
            this.errHandler(exp);
        }
    }

    /**
     * ShowStitches - shows player and computer stitches
     * @param whichStitch   number >= 0 for player stitch
     *                      -1 for computer stitch,
     *                      -2 for none
     *                      -3 for
     */
    protected void showStitches(int whichStitch) {
        if (aGame != null && aGame.gambler != null && aGame.computer != null) {

            if (whichStitch < -2) {
                imgComputerStitch0a.setImageResource(R.drawable.n1);
                imgComputerStitch0b.setImageResource(R.drawable.n1);
                linLayoutComputerStitch0.setVisibility(View.INVISIBLE);

                imgPlayerStitch0a.setImageResource(R.drawable.n1);
                imgPlayerStitch0b.setImageResource(R.drawable.n1);
                imgPlayerStitch1a.setImageResource(R.drawable.n1);
                imgPlayerStitch1b.setImageResource(R.drawable.n1);
                imgPlayerStitch2a.setImageResource(R.drawable.n1);
                imgPlayerStitch2b.setImageResource(R.drawable.n1);
                imgPlayerStitch3a.setImageResource(R.drawable.n1);
                imgPlayerStitch3b.setImageResource(R.drawable.n1);
                linLayoutPlayerStitch0.setVisibility(View.INVISIBLE);
                linLayoutPlayerStitch1.setVisibility(View.INVISIBLE);
                linLayoutPlayerStitch2.setVisibility(View.INVISIBLE);
                linLayoutPlayerStitch3.setVisibility(View.INVISIBLE);

                relativeLayoutStitches.setVisibility(View.INVISIBLE);
            }
            else if (whichStitch == -2) {
                relativeLayoutStitches.setVisibility(View.VISIBLE);

                imgComputerStitch0a.setImageResource(R.drawable.n1);
                imgComputerStitch0b.setImageResource(R.drawable.n1);

                imgPlayerStitch0a.setImageResource(R.drawable.n1);
                imgPlayerStitch0b.setImageResource(R.drawable.n1);
                imgPlayerStitch1a.setImageResource(R.drawable.n1);
                imgPlayerStitch1b.setImageResource(R.drawable.n1);
                imgPlayerStitch2a.setImageResource(R.drawable.n1);
                imgPlayerStitch2b.setImageResource(R.drawable.n1);
                imgPlayerStitch3a.setImageResource(R.drawable.n1);
                imgPlayerStitch3b.setImageResource(R.drawable.n1);
            }
            else {
                relativeLayoutStitches.setVisibility(View.VISIBLE);
            }

            //region playersStitches
            if (aGame.gambler.stitchCount > 0) {
                linLayoutPlayerStitch0.setVisibility(View.VISIBLE);

                if (aGame.gambler.stitchCount > 1) {
                    linLayoutPlayerStitch1.setVisibility(View.VISIBLE);
                }
                if (aGame.gambler.stitchCount > 2) {
                    linLayoutPlayerStitch2.setVisibility(View.VISIBLE);
                }
                if (aGame.gambler.stitchCount > 3) {
                    linLayoutPlayerStitch3.setVisibility(View.VISIBLE);
                }

                for (HashMap.Entry<Integer, TwoCards> stitchEntry : aGame.gambler.cardStitchs.entrySet()) {
                    if (stitchEntry != null) {
                        if (stitchEntry.getKey() == 0 && whichStitch == 0) {
                            TwoCards playerStitch0 = stitchEntry.getValue();
                            if (playerStitch0 != null && playerStitch0.card1st != null && playerStitch0.card2nd != null) {
                                imgPlayerStitch0a.setImageDrawable(stitchEntry.getValue().card1st.getDrawable());
                                imgPlayerStitch0b.setImageDrawable(stitchEntry.getValue().card2nd.getDrawable());
                            }
                        }
                        if (stitchEntry.getKey() == 1 && whichStitch == 1) {
                            TwoCards playerStitch1 = stitchEntry.getValue();
                            if (playerStitch1 != null && playerStitch1.card1st != null && playerStitch1.card2nd != null) {
                                imgPlayerStitch1a.setImageDrawable(stitchEntry.getValue().card1st.getDrawable());
                                imgPlayerStitch1b.setImageDrawable(stitchEntry.getValue().card2nd.getDrawable());
                            }
                        }
                        if (stitchEntry.getKey() == 2 && whichStitch == 2) {
                            TwoCards playerStitch2 = stitchEntry.getValue();
                            if (playerStitch2 != null && playerStitch2.card1st != null && playerStitch2.card2nd != null) {
                                imgPlayerStitch2a.setImageDrawable(stitchEntry.getValue().card1st.getDrawable());
                                imgPlayerStitch2b.setImageDrawable(stitchEntry.getValue().card2nd.getDrawable());
                            }
                        }
                        if (stitchEntry.getKey() == 3 && whichStitch == 3) {
                            TwoCards playerStitch3 = stitchEntry.getValue();
                            if (playerStitch3 != null && playerStitch3.card1st != null && playerStitch3.card2nd != null) {
                                imgPlayerStitch3a.setImageDrawable(stitchEntry.getValue().card1st.getDrawable());
                                imgPlayerStitch3b.setImageDrawable(stitchEntry.getValue().card2nd.getDrawable());
                            }
                        }
                    }
                }
            }
            //endregion

            //region computerStitches
            if (aGame.computer.stitchCount > 0) {
                linLayoutComputerStitch0.setVisibility(View.VISIBLE);

                for (HashMap.Entry<Integer, TwoCards> stitchEntry : aGame.computer.cardStitchs.entrySet()) {
                    if (stitchEntry != null && stitchEntry.getKey() == 0 && whichStitch == -1) {
                        TwoCards computerStitch0 = stitchEntry.getValue();
                        if (computerStitch0 != null && computerStitch0.card1st != null && computerStitch0.card2nd != null) {
                            imgComputerStitch0a.setImageDrawable(stitchEntry.getValue().card1st.getDrawable());
                            imgComputerStitch0b.setImageDrawable(stitchEntry.getValue().card2nd.getDrawable());
                        }
                    }
                }
            }
            //endregion
        }
    }

    /**
     * showComputer20 shows computer pair, when computer has 20 or 40
     *
     * @param computerPlayedOut - computer played out that Card
     * @param stage - int stage
     */
    protected void showComputer20(Card computerPlayedOut, int stage) {
        syncLocker = Integer.valueOf(ticks);
        synchronized (syncLocker) {
            ticks++;
            aStage = (stage >= 0) ? stage : aStage;
        }
        if (computerPlayedOut == null)
            computerPlayedOut = playedOutCard1;
        ImageView imCOut0 = imgCOut0;
        ImageView imCOut1 = imgCOut1;

        linLayoutCard0.setVisibility(View.INVISIBLE);
        linLayoutCard1.setVisibility(View.INVISIBLE);

        switch (aStage) {
            case 0:
            case 1:
            default:
                linLayoutCard0.setVisibility(View.VISIBLE);
                linLayoutCard1.setVisibility(View.VISIBLE);
                imCOut0 = imgCOut0;
                imCOut1 = imgCOut1;
                break;
        }

        for (int ci = 0; ci < aGame.computer.hand.length; ci++) {
            if (computerPlayedOut.getCardValue().getValue() == 3 &&
                    aGame.computer.hand[ci].getCardColor() == computerPlayedOut.getCardColor() &&
                    aGame.computer.hand[ci].getCardValue().getValue() == 4) {
                imCOut0.setImageDrawable(aGame.computer.hand[ci].getDrawable());
                imCOut1.setImageDrawable(computerPlayedOut.getDrawable());
                break;
            }
            if (computerPlayedOut.getCardValue().getValue() == 4 &&
                    aGame.computer.hand[ci].getCardColor() == computerPlayedOut.getCardColor() &&
                    aGame.computer.hand[ci].getCardValue().getValue() == 3) {
                imCOut0.setImageDrawable(computerPlayedOut.getDrawable());
                imCOut1.setImageDrawable(aGame.computer.hand[ci].getDrawable());
                break;
            }
        }

        syncLocker = Integer.valueOf(ticks);
        synchronized (syncLocker) {
            ticks++;
            aStage--;
        }
        if (aStage > 0) {
            showComputer20Handler.postDelayed(rShowComputer20, 800);
        }
        else {
            aGame.playedOut1 = playedOutCard1;
            imOut1.setImageDrawable(computerPlayedOut.getDrawable());
            setComputerPairHandler.postDelayed(setComputerPair, 750);
        }
    }

    /**
     * reSetComputerPair resets computer pair images and linear layout placeholder
     */
    protected void reSetComputerPair() {
        linLayoutCard0.setVisibility(View.INVISIBLE);
        linLayoutCard1.setVisibility(View.INVISIBLE);
        imgCOut0.setImageResource(R.drawable.e);
        imgCOut1.setImageResource(R.drawable.e);

        aGame.playedOut1 = playedOutCard1;
        imOut1.setImageDrawable(playedOutCard1.getDrawable());
    }

    //endregion

    /**
     * reset Buttons
     * @param level of reset
     *  0 disable Mariage and Change Atou button
     *  1 perform level 0 and enable Continue, reset atou and talon card
     *  2 perform 0 + 1 and reset play out position cards
     */
    protected void resetButtons(int level) {

        if (level >= 0 ) {
            toggleEnabled(b20a, false, getLocaleStringRes(R.string.b20a_text),
                    getLocaleStringRes(R.string.b20a_text));
            if (aGame != null) {
                aGame.a20 = false;
                aGame.b20 = false;
                aGame.bChange = false;
            }
            toggleEnabled(b20b, false, getLocaleStringRes(R.string.b20b_text),
                    getLocaleStringRes(R.string.b20b_text));
            toggleEnabled(bChange, false, getLocaleStringRes(R.string.bChange_text),
                    getLocaleStringRes(R.string.bChange_text));
        }

        if (level >= 1) {
            if (aGame != null) {
                aGame.shouldContinue = false;
            }
            toggleEnabled(bContinue, false, getLocaleStringRes(R.string.bContinue_text),
                    getLocaleStringRes(R.string.bContinue_text));

            showAtouCard(SCHNAPSTATE.GAME_START);
            showTalonCard(SCHNAPSTATE.GAME_START);
        }

        if (level > 2) {
            try {
                imOut0.setImageResource(R.drawable.e);
                imOut1.setImageResource(R.drawable.e);
                playedOutCard0 = getGlobalAppSettings().cardEmpty();
                playedOutCard1 = getGlobalAppSettings().cardEmpty();
                aGame.playedOut0 = playedOutCard0;
                aGame.playedOut1 = playedOutCard1;
            } catch (Exception ex) {
                this.errHandler(ex);
            }
        }
        if (aGame != null) {
            getGlobalAppSettings().setTournamentGame(aTournament, aGame);
        }
    }

    //region gameStartStopEndsTurnAndEnough

    /**
     * start game
     */
    protected void startGame() {

        playL8rHandler.postDelayed(delayPlayScreenshot, 100);
        toggleMenuItem(myMenu, R.id.id_action_start, false);
        aGame = null;

        if (aTournament.getTournamentWinner() != PLAYERDEF.UNKNOWN) {
            aTournament = getGlobalAppSettings().newTournament();
            tTournamentPoints.setText(aTournament.getTournamentsTable());
        }

        // runtime = java.lang.Runtime.getRuntime();
        // runtime.runFinalization();
        // runtime.gc();

        aGame = new Game(getApplicationContext(), aTournament.nextGameGiver);
        aGame.isReady = true;
        tMes.setVisibility(View.INVISIBLE);

        mergeCardAnim(false);

        showStitches(-3);
        showPlayersCards();
        resetButtons(1);

        if (tDbg != null) {
            tDbg.setText("");
        }
        tRest.setText(String.valueOf((19-aGame.index)));
        // emptyTmpCard = new Card(-2, getApplicationContext()); // new Card(this, -1);
        tPoints.setText(String.valueOf(aGame.gambler.points));
        showAtouCard(aGame.schnapState);
        showTalonCard(aGame.schnapState);

        toggleMenuItem(myMenu, R.id.id_action_restart, true);
        toggleMenuItem(myMenu, R.id.id_action_start, false);

        getGlobalAppSettings().setTournamentGame(aTournament, aGame);
        gameTurn(0);
    }

    /**
     * stop current game
     * @param whoWon PLAYERDEF who won game
     * @param levela level of stop
     * @param stopMessage ending game message
     */
    protected void stopGame(int tournamentPoints, PLAYERDEF whoWon, int levela, String stopMessage) {

        if (stopMessage != null && stopMessage.length() > 0) {
            setTextMessage(stopMessage);
            sayText(stopMessage);
        }

        playL8rHandler.postDelayed(delayPlayScreenshot, 100);

        toggleMenuItem(myMenu, R.id.id_action_start, true);
        toggleMenuItem(myMenu, R.id.id_action_restart, true);

        if (aGame.schnapState != SCHNAPSTATE.NONE && aGame.schnapState != SCHNAPSTATE.MERGING_CARDS)
            aGame.stopGame(whoWon);

        resetButtons(levela);
        showStitches(-3);

        toggleEnabled(bContinue, true, getLocaleStringRes(R.string.bContinue_text),
                getLocaleStringRes(R.string.bContinue_text));
        toggleMenuItem(myMenu, R.id.id_action_start, true);

        showPlayersCards();

        playedOutCard0 = getGlobalAppSettings().cardEmpty();
        playedOutCard1 = getGlobalAppSettings().cardEmpty();
        aGame.playedOut0 = playedOutCard0;
        aGame.playedOut1 = playedOutCard1;

        aTournament.addPointsRotateGiver(tournamentPoints, aGame.whoWon);
        tTournamentPoints.setText(aTournament.getTournamentsTable());
        getGlobalAppSettings().setTournamentGame(aTournament, aGame);

        if (aGame.schnapState != SCHNAPSTATE.NONE && aGame.schnapState != SCHNAPSTATE.MERGING_CARDS)
            aGame.destroyGame();

        PLAYERDEF playerWon = aTournament.getTournamentWinner();
        if (playerWon != PLAYERDEF.UNKNOWN) {
            String tournamentWonMsg = "";
            if (playerWon == PLAYERDEF.COMPUTER) {
                tournamentWonMsg = (aTournament.hasTaylor()) ?
                    // getString(R.string.computer_won_taylor) :
                    getLocaleStringRes(R.string.computer_won_taylor) :
                    // getString(R.string.computer_won_tournement);
                    getLocaleStringRes(R.string.computer_won_tournement);
            }
            if (playerWon == PLAYERDEF.HUMAN) {
                tournamentWonMsg = (aTournament.hasTaylor()) ?
                    // getString(R.string.you_won_taylor) :
                    getLocaleStringRes(R.string.you_won_taylor) :
                    // getString(R.string.you_won_tournement);
                    getLocaleStringRes(R.string.you_won_tournement);
            }
            setTextMessage(tournamentWonMsg);
            sayText(tournamentWonMsg);
        }

        if (levela <= 0 || levela >= 3) {
            mergeCardAnim(true);
        }
    }


    /**
     * close that game
     *
     * @param whoCloses PLAYERDEF - player or computer
     */
    protected void closeGame(PLAYERDEF whoCloses) { //	Implementierung des Zudrehens
        if (!aGame.isGame || aGame.gambler == null ||
            aGame.colorHitRule || aGame.isClosed ||
            aGame.schnapState == SCHNAPSTATE.GAME_CLOSED ||
            aGame.schnapState == SCHNAPSTATE.TALON_ONE_REMAINS ||
            aGame.schnapState == SCHNAPSTATE.TALON_CONSUMED ||
            aGame.schnapState == SCHNAPSTATE.NONE) {
            // setTextMessage(getString(R.string.nogame_started));
            setTextMessage(getLocaleStringRes(R.string.nogame_started));
            return;
        }

        aGame.closeGame(whoCloses);

        setTextMessage(aGame.statusMessage);
        sayText(aGame.statusMessage);
        showTalonCard(aGame.schnapState);
        showAtouCard(aGame.schnapState);

        getGlobalAppSettings().setTournamentGame(aTournament, aGame);
        if (whoCloses == PLAYERDEF.HUMAN) {
            gameTurn(0);
        }
    }

    /**
     * a turn in game
     * @param ixlevel level
     */
    protected void gameTurn(int ixlevel) {
        if (ixlevel < 1) {
            try {
                imOut0.setImageResource(R.drawable.e1);
                imOut1.setImageResource(R.drawable.e1);
                playedOutCard0 = getGlobalAppSettings().cardEmpty();
                playedOutCard1 = getGlobalAppSettings().cardEmpty();
                aGame.playedOut0 = playedOutCard0;
                aGame.playedOut1 = playedOutCard1;
            } catch (Exception jbpvex) {
                this.errHandler(jbpvex);
            }
            getGlobalAppSettings().setTournamentGame(aTournament, aGame);
            showPlayersCards();
            pSaid = false;
            aGame.said = 'n';
            aGame.csaid = 'n';
        }

        showStitches(-2);
        aGame.bChange = false;
        aGame.a20 = false;
        aGame.b20 = false;
        aGame.sayMarriage20= getLocaleStringRes(R.string.b20a_text);
        aGame.sayMarriage40 = getLocaleStringRes(R.string.b20a_text);

        if (aGame.playersTurn) {
            // Wann kann man austauschen ?
            if (ixlevel < 1) {
                if ((aGame.isAtouChangable(aGame.gambler)) && (!pSaid)) {
                    syncLocker = Integer.valueOf(ticks);
                    synchronized (syncLocker) {
                        ticks++;
                        psaychange += 1;
                    }
                    aGame.bChange = true;
                }
                toggleEnabled(bChange, (aGame.bChange), getLocaleStringRes(R.string.bChange_text),
                        getLocaleStringRes(R.string.bChange_text));
            }
            // Gibts was zum Ansagen ?
            int a20 = aGame.gambler.hasPair();
            if (a20 > 0) {
                syncLocker = Integer.valueOf(ticks);
                synchronized (syncLocker) {
                    ticks++;
                    psaychange += 2;
                }
                aGame.a20 = true;
                aGame.sayMarriage20 = aGame.printColor(aGame.gambler.handpairs[0]) +
                        " " + getLocaleStringRes(R.string.say_pair);

                if (a20 > 1) {
                    syncLocker = Integer.valueOf(ticks);
                    synchronized (syncLocker) {
                        ticks++;
                        psaychange += 4;
                    }
                    aGame.b20 = true;
                    aGame.sayMarriage40 = aGame.printColor(aGame.gambler.handpairs[1]) +
                        " " + getLocaleStringRes(R.string.say_pair);
                }
                else {
                    aGame.sayMarriage40 = getLocaleStringRes(R.string.no_second_pair);
                }
            }

            toggleEnabled(b20a, aGame.a20, aGame.sayMarriage20, aGame.sayMarriage20);
            toggleEnabled(b20b, aGame.b20, aGame.sayMarriage40, aGame.sayMarriage40);

            // Info
            // setTextMessage(getString(R.string.toplayout_clickon_card));
            setTextMessage(getLocaleStringRes(R.string.toplayout_clickon_card));
        }
        else {
            // COMPUTERS TURN IMPLEMENTIEREN
            String outPutMessage = "";
            // boolean atouNowChanged = aGame.atouChanged;
            // if (aGame.atouIsChangable(aGame.computer))  aGame.changeAtou(aGame.computer); } /* old implementaton */
            ccard = aGame.computerStarts();

            if ((aGame.computer.playerOptions & PLAYEROPTIONS.CHANGEATOU.getValue()) == PLAYEROPTIONS.CHANGEATOU.getValue()) {
                this.showAtouCard(aGame.schnapState);
                // outPutMessage += getString(R.string.computer_changes_atou);
                outPutMessage += getLocaleStringRes(R.string.computer_changes_atou);
            }
            // if (atouNowChanged == false && aGame.atouChanged) { }

            boolean computerSaid20 = false;
            if ((aGame.computer.playerOptions & PLAYEROPTIONS.SAYPAIR.getValue()) == PLAYEROPTIONS.SAYPAIR.getValue()) {
                computerSaid20 = true;
                // String computerSaysPair = getString(R.string.computer_says_pair, aGame.printColor(aGame.csaid));
                String computerSaysPair = String.format(getLocaleStringRes(R.string.computer_says_pair),
                        aGame.printColor(aGame.csaid));
                outPutMessage = outPutMessage + " " + computerSaysPair;
            }
            setTextMessage(outPutMessage);
            sayText(outPutMessage);

            if ((aGame.computer.playerOptions & PLAYEROPTIONS.ANDENOUGH.getValue()) == PLAYEROPTIONS.ANDENOUGH.getValue()) {
                twentyEnough(false);
                aGame.isReady = false;
                getGlobalAppSettings().setTournamentGame(aTournament, aGame);
                return;
            }

            if ((aGame.computer.playerOptions & PLAYEROPTIONS.CLOSESGAME.getValue()) == PLAYEROPTIONS.CLOSESGAME.getValue()) {
                aGame.isClosed = true;
                // outPutMessage += getString(R.string.computer_closed_game);
                outPutMessage += getLocaleStringRes(R.string.computer_closed_game);
                setTextMessage(outPutMessage);
                sayText(outPutMessage);
                closeGame(PLAYERDEF.COMPUTER);
            }

            try {
                playedOutCard1 = aGame.computer.hand[ccard];
                if (computerSaid20)
                    showComputer20(playedOutCard1, 1);
                else
                    imOut1.setImageDrawable(aGame.computer.hand[ccard].getDrawable());
                aGame.playedOut1 = playedOutCard1;
            } catch (Exception jbpvex) {
                this.errHandler(jbpvex);
            }
            // tMes.setVisibility(View.VISIBLE);
            // tMes.setText("Zum Antworten einfach auf die entsprechende Karte klicken");
        }

        aGame.isReady = true;
        printMes();
        getGlobalAppSettings().setTournamentGame(aTournament, aGame);
    }

    /**
     * Continue turn
     */
    protected void continueTurn() {
        try {

            dragged20 = false;      // TODO: make this persistent in game state
            droppedCard = false;    // TODO: make this persistent in game state

            if (aGame == null || !aGame.isGame) {
                startGame();
                return;
            }
            aGame.isReady = true;

            aGame.shouldContinue = false;
            toggleEnabled(bContinue, false, getLocaleStringRes(R.string.bContinue_text),
                    getLocaleStringRes(R.string.bContinue_text));

            tMes.setVisibility(View.INVISIBLE);

            getGlobalAppSettings().setTournamentGame(aTournament, aGame);

            gameTurn(0);
        } catch (Exception e) {
            this.errHandler(e);
        }
    }

    /**
     * end current turn in game
     */
    protected void endTurn() {
        int tmppoints;
        String msgText;

        /* implement computers strategy here */
        if (aGame.playersTurn) {
            ccard = aGame.computersAnswer();
            try {
                playedOutCard1 = aGame.computer.hand[ccard];
                imOut1.setImageDrawable(aGame.computer.hand[ccard].getDrawable());
                aGame.playedOut1 = playedOutCard1;
            } catch (Exception jbpvex) {
                this.errHandler(jbpvex);
            }
        }

        tmppoints = aGame.checkPoints(ccard);
        aGame.computer.hand[ccard] = getGlobalAppSettings().cardEmpty();
        tPoints.setText(String.valueOf(aGame.gambler.points));

        if (tmppoints > 0) {
            // msgText = getString(R.string.your_hit_points, String.valueOf(tmppoints)) + " " + getString(R.string.click_continue);
            msgText = String.format(getLocaleStringRes(R.string.your_hit_points), String.valueOf(tmppoints)) +
                    " " + getLocaleStringRes(R.string.click_continue);
            setTextMessage(msgText);
            // sayText(getString(R.string.your_hit_points, String.valueOf(tmppoints)));
            sayText(String.format(getLocaleStringRes(R.string.your_hit_points),
                    String.valueOf(tmppoints)));

            TwoCards stitchPlayer = new TwoCards(aGame.playedOut, aGame.playedOut1);
            if (!aGame.gambler.cardStitchs.containsKey(aGame.gambler.stitchCount)) {
                aGame.gambler.cardStitchs.put(aGame.gambler.stitchCount, stitchPlayer);
                aGame.gambler.stitchCount++;
            }

            if (aGame.isClosed && (aGame.computer.hasClosed)) {
                int tPts = aGame.getTournamentPoints(PLAYERDEF.HUMAN);
                getGlobalAppSettings().setTournamentGame(aTournament, aGame);
                stopGame(tPts, PLAYERDEF.HUMAN,1, getLocaleStringRes(R.string.computer_closing_failed));
                return;
            }
        } else {
            msgText = String.format(getLocaleStringRes(R.string.computer_hit_points),
                    String.valueOf(-tmppoints)) + " " + getLocaleStringRes(R.string.click_continue);
            setTextMessage(msgText);
            sayText(String.format(getLocaleStringRes(R.string.computer_hit_points),
                    String.valueOf(-tmppoints)));

            TwoCards stitchComputer = new TwoCards(aGame.playedOut, aGame.playedOut1);
            if (!aGame.computer.cardStitchs.containsKey(aGame.computer.stitchCount)) {
                aGame.computer.cardStitchs.put(aGame.computer.stitchCount, stitchComputer);
                aGame.computer.stitchCount++;
            }

            if ((aGame.isClosed) && (aGame.gambler.hasClosed)) {
                int tPts = aGame.getTournamentPoints(PLAYERDEF.HUMAN);
                getGlobalAppSettings().setTournamentGame(aTournament, aGame);
                stopGame(tPts, PLAYERDEF.COMPUTER,1,
                        getLocaleStringRes(R.string.closing_failed));
                return;
            }
        }

        // Assign new cards
        if (aGame.assignNextCard(assignedCard)) {
            /* NOW WE HAVE NO MORE TALON */
            try {
                showTalonCard(aGame.schnapState);
                showAtouCard(aGame.schnapState);
            } catch (Exception jbpvex) {
                this.errHandler(jbpvex);
            }

            setTextMessage(getLocaleStringRes(R.string.color_hit_force_mode));
            sayText(getLocaleStringRes(R.string.color_hit_force_mode));
        }
        tRest.setText(String.valueOf((19-aGame.index)));
        printMes();
        // resetButtons(0);
        pSaid = false;
        aGame.said = 'n';
        aGame.csaid = 'n';

        if (aGame.playersTurn) {
            if (aGame.gambler.points > 65) {
                getGlobalAppSettings().setTournamentGame(aTournament, aGame);
                int tPts = aGame.getTournamentPoints(PLAYERDEF.HUMAN);
                stopGame(tPts, PLAYERDEF.HUMAN,1, String.format(
                        getLocaleStringRes(R.string.you_have_won_points),
                        String.valueOf(aGame.gambler.points)));
                return;
            }
        } else {
            if (aGame.computer.points > 65) {
                getGlobalAppSettings().setTournamentGame(aTournament, aGame);
                int tPts = aGame.getTournamentPoints(PLAYERDEF.HUMAN);
                stopGame(tPts, PLAYERDEF.COMPUTER, 1, String.format(
                        getLocaleStringRes(R.string.computer_has_won_points),
                                String.valueOf(aGame.computer.points)));
                return;
            }
        }

        if (aGame.movs >= 5) {
            if (aGame.isClosed) {
                if (aGame.gambler.hasClosed) {
                    int tPts = aGame.getTournamentPoints(PLAYERDEF.HUMAN);
                    getGlobalAppSettings().setTournamentGame(aTournament, aGame);
                    stopGame(tPts, PLAYERDEF.COMPUTER,1,
                            getLocaleStringRes(R.string.closing_failed));
                }
                try {
                    if (aGame.computer.hasClosed) {
                        int tPts = aGame.getTournamentPoints(PLAYERDEF.HUMAN);
                        getGlobalAppSettings().setTournamentGame(aTournament, aGame);
                        stopGame(tPts, PLAYERDEF.HUMAN, 1,
                                getLocaleStringRes(R.string.computer_closing_failed));
                    }
                } catch (Exception jbpvex) {
                    this.errHandler(jbpvex);
                }
                return ;
            } else {
                if (tmppoints > 0) {
                    int tPts = aGame.getTournamentPoints(PLAYERDEF.HUMAN);
                    getGlobalAppSettings().setTournamentGame(aTournament, aGame);
                    stopGame(tPts, PLAYERDEF.HUMAN,1,
                            getLocaleStringRes(R.string.last_hit_you_have_won));
                } else {
                    int tPts = aGame.getTournamentPoints(PLAYERDEF.HUMAN);
                    getGlobalAppSettings().setTournamentGame(aTournament, aGame);
                    stopGame(tPts, PLAYERDEF.COMPUTER,1,
                            getLocaleStringRes(R.string.computer_wins_last_hit));
                }
                return;
            }
        }

        aGame.shouldContinue = true;
        toggleEnabled(bContinue, true, getLocaleStringRes(R.string.bContinue_text),
                getLocaleStringRes(R.string.bContinue_text));
        imTalon.setOnClickListener(this::bContinue_Clicked);
        aGame.isReady = false;
        getGlobalAppSettings().setTournamentGame(aTournament, aGame);
    }

    /**
     * say 20 or 40 and enough to finish game
     * @param who player or computer
     */
    protected void twentyEnough(boolean who) {
        int xj = 0;
        String andEnough = getLocaleStringRes(R.string.twenty_and_enough);
        aGame.isReady = false;

        if (who) {
            if (aGame.said == aGame.atouInGame()) {
                andEnough = getLocaleStringRes(R.string.fourty_and_enough);
            }
            try {
                for (xj = 0; xj < 5; xj++) {
                    if (aGame.gambler.hand[xj].getCardColor().getChar() == aGame.said &&
                            aGame.gambler.hand[xj].getCardValue().getValue() == 3) {
                        playedOutCard0 = aGame.gambler.hand[xj];
                        aGame.playedOut0 = playedOutCard0;
                        imOut0.setImageDrawable(aGame.gambler.hand[xj].getDrawable());
                    }
                    if (aGame.gambler.hand[xj].getCardColor().getChar() == aGame.said &&
                            aGame.gambler.hand[xj].getCardValue().getValue() == 4) {
                        playedOutCard1 = aGame.gambler.hand[xj];
                        aGame.playedOut1 = playedOutCard1;
                        imOut1.setImageDrawable(aGame.gambler.hand[xj].getDrawable());
                    }
                }
            } catch (Exception jbpvex) {
                this.errHandler(jbpvex);
            }
            int tPts = aGame.getTournamentPoints(PLAYERDEF.HUMAN);
            stopGame(tPts, PLAYERDEF.HUMAN,2, andEnough + " " +
                    String.format(getLocaleStringRes(R.string.you_have_won_points),
                            String.valueOf(aGame.gambler.points)));

        } else {
            if (aGame.csaid == aGame.atouInGame()) {
                andEnough = getLocaleStringRes(R.string.fourty_and_enough);
            }
            try {
                for (xj = 0; xj < 5; xj++) {
                    if (aGame.computer.hand[xj].getCardColor().getChar() == aGame.csaid &&
                            aGame.computer.hand[xj].getCardValue().getValue() == 3) {
                        playedOutCard0 = aGame.computer.hand[xj];
                        aGame.playedOut0 = playedOutCard0;
                        imOut0.setImageDrawable(aGame.computer.hand[xj].getDrawable());
                    }
                    if (aGame.computer.hand[xj].getCardColor().getChar() == aGame.csaid &&
                            aGame.computer.hand[xj].getCardValue().getValue() == 4) {
                        playedOutCard1 = aGame.computer.hand[xj];
                        aGame.playedOut1 = playedOutCard1;
                        imOut1.setImageDrawable(aGame.computer.hand[xj].getDrawable());
                    }
                }
            } catch (Exception jbpvex) {
                this.errHandler(jbpvex);
            }

            printMes();
            String tsEndMes1 = (andEnough + " " +
                    String.format(getLocaleStringRes(R.string.computer_has_won_points),
                            String.valueOf(aGame.computer.points)));
            int tPts = aGame.getTournamentPoints(PLAYERDEF.COMPUTER);
            stopGame(tPts, PLAYERDEF.COMPUTER,2, tsEndMes1);
            // stopGame(1, new String(andEnough + " Computer hat gewonnen mit " + String.valueOf(aGame.computer.points) + " Punkten !"));
        }
        getGlobalAppSettings().setTournamentGame(aTournament, aGame);
        return;
    }

    //endregion

    /**
     * SetImageDrawable - Future design
     *
     * @param imageView ImageView to display
     * @param card Card to display
     */
    protected void SetImageDrawable(ImageView imageView, Card card) {
        if (aGame != null && card == null)
            card = getGlobalAppSettings().cardEmpty();

        String tmp = card.getCardColor().getChar() +
                String.valueOf(card.getCardValue().getValue());
        imageView.setImageDrawable(card.getDrawable());

        imageView.setTag(0, tmp);
        imageView.setTag(1, imageView.getVisibility());
    }

    //region TextMsgError

    /**
     * setTextMessage shows a new Toast dynamic message
     * @param text to display
     */
    private void setTextMessage(CharSequence text) {
        Context context = getApplicationContext();
        String textMsg = (text != null && text.length() > 2) ?
                String.valueOf(text) : getLocaleStringRes(R.string.tDbg_text);

        if (aGame != null) // TOOD:
            aGame.statusMessage = textMsg;

        boolean tMesEna = (textMsg != null && textMsg.length() > 2);
        toggleEnabled(tMes, tMesEna, textMsg);


    }

    /**
     * print message queue
     */
    private void printMes() {
        if (tDbg != null) {
            tDbg.append(aGame.fetchMsg());
        }
    }

    /**
     * Error handler
     * @param myErr java.lang.Throwable
     */
    private void errHandler(java.lang.Throwable myErr) {
        syncLocker = Integer.valueOf(ticks);
        synchronized (syncLocker) {
            ticks++;
            ++errNum;
        }

        String strDbg = "\nCRITICAL ERROR #" + String.valueOf(errNum) + " " + myErr.getMessage();
        strDbg += "\n\t" + myErr.toString();
        strDbg += "\nMessage: "+ myErr.getLocalizedMessage() + "\n";

        if (tDbg != null) {
            tDbg.append(strDbg);
        }
        myErr.printStackTrace();
    }

    /**
     * showHelp() - show help Dialog
     */
    public void showHelp() {
        showHelpDialog();
    }

    //endregion

    //region modalDialogs

    /**
     * showHelpDialog
     */
    public void showHelpDialog() {

        playL8rHandler.postDelayed(delayPlayScreenshot, 100);

        // Create an instance of the dialog fragment and show it
        getGlobalAppSettings().setDialog(DIALOGS.Help);
        int idx = (aGame != null) ? aGame.index : 9;
        int ptx = (aGame != null && aGame.gambler != null) ? aGame.gambler.points : 0;
        DialogFragment dialog = new HelpDialog();
        Bundle dialogArgs = new Bundle();
        dialogArgs.putInt(getString(R.string.param1_index), idx);
        dialogArgs.putInt(getString(R.string.param2_points), ptx);
        dialog.setArguments(dialogArgs);
        dialog.setCancelable(false);
        dialog.show(getSupportFragmentManager(), "HelpDialog");
    }


    @Override
    public void onDialogPositiveClick(DialogFragment dialog) {
        DIALOGS dialogOpen = getGlobalAppSettings().getDialog();
        if (dialogOpen == DIALOGS.About || dialogOpen == DIALOGS.Help) {
            openUrl(getString(R.string.wiki_uri));
        }
        getGlobalAppSettings().setDialog(DIALOGS.None);
        dialog.dismiss();
    }

    @Override
    public void onDialogNegativeClick(DialogFragment dialog) {
        getGlobalAppSettings().setDialog(DIALOGS.None);
        dialog.dismiss();
    }

    //endregion

    protected String SerializeJsom(Game aGame) {
        String gsonStr = new Gson().toJson(aGame);
        return gsonStr;
    }
}