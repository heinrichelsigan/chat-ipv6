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

import android.content.ClipData;
import android.graphics.Point;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.view.DragEvent;
import android.view.MotionEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.DragShadowBuilder;
import android.view.View.OnDragListener;
import android.view.View.OnTouchListener;
import android.view.ViewGroup;
import android.widget.GridLayout;
import android.widget.LinearLayout;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.core.content.res.ResourcesCompat;
import androidx.fragment.app.DialogFragment;

import eu.georgtoth.supu.enums.AUTOMAT;
import eu.georgtoth.supu.enums.BOARDS;
import eu.georgtoth.supu.enums.COLUMN;
import eu.georgtoth.supu.enums.DIALOGS;
import eu.georgtoth.supu.enums.DROPSTATE;
import eu.georgtoth.supu.enums.GAMEMODE;
import eu.georgtoth.supu.enums.GEMCOLORS;
import eu.georgtoth.supu.enums.SOUNDS;
import eu.georgtoth.supu.enums.SUPURULES;

import eu.georgtoth.supu.models.*;
import eu.georgtoth.supu.ui.dialogfragment.AboutDialog;
import eu.georgtoth.supu.ui.dialogfragment.FinishedLevel;
import eu.georgtoth.supu.ui.dialogfragment.FinishedLevelPerfect;
import eu.georgtoth.supu.ui.dialogfragment.GameOver;
import eu.georgtoth.supu.ui.dialogfragment.HelpDialog;
import eu.georgtoth.supu.util.Constants;
import eu.georgtoth.supu.R;

import java.util.HashMap;
import java.util.Objects;
import java.util.OptionalInt;

/**
 * Supu extends BaseApp extends AppCompatActivity
 * Activity for SUPU
 * implements FinishedLevel.NoticeDialogListener,
 *  FinishedLevelPerfect.NoticeDialogListener,
 *  GameOver.NoticeDialogListener,
 *  HelpDialog.NoticeDialogListener,
 *  AboutDialog.NoticeDialogListener
 */
public class Supu
        extends BaseApp
        implements GameOver.NoticeDialogListener,
            FinishedLevel.NoticeDialogListener, FinishedLevelPerfect.NoticeDialogListener,
            HelpDialog.NoticeDialogListener, AboutDialog.NoticeDialogListener {

    int level = 5;
    boolean vHelp = false, auto = false;
    volatile Integer syncLocker  = null;
    String boardTag = null;

    Game game;
    Board board;
    SupuStone currentStone;
    volatile BOARDS fromBoard;
    volatile COLUMN fromBoardColumn;
    volatile DIALOGS dialogToOpen = DIALOGS.None;

    ImageView theStone;
    LinearLayout theStoneLayout;
    GridLayout gridLayoutBoard, gridLayoutStones, dropBoardLayout;
    LinearLayout[][] boardLayout;
    LinearLayout[] stoneBoardLayouts;
    LinearLayout[] dropBoardLayouts;
    ImageView[][] gems;
    ImageView currentImage;
    View toDropView;
    TextView motionLabel;

    HashMap<Integer,ImageView> allImages = new HashMap<Integer,ImageView>();
    HashMap<Integer, LinearLayout> allFields = new HashMap<Integer, LinearLayout>();
    HashMap<Integer, LinearLayout> toFields = new HashMap<>();
    HashMap<Integer, LinearLayout> fromFields = new HashMap<>();
    HashMap<Integer, View> movedStones = new HashMap<>();
    HashMap<Integer, Drawable> dragNDropMap = new HashMap<>();
    HashMap<Integer, View> draggedViews = new HashMap<>();
    // ArrayList<Integer> listStreet = new ArrayList<Integer>();

    private final static Handler resetBackgroundHandler = new Handler(Looper.getMainLooper());

    // @SuppressLint("InlinedApi")
    /**
     * runResetBackground new Runnable() -> { resetBackground(); }
     */
    private final Runnable runResetBackground = () -> resetBackground();



    /**
     * Override onCreate
     *
     * @param savedInstanceState Bundle containing savedInstanceState
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Bundle bundle = getIntent().getExtras();
        if (bundle != null) {
            level = bundle.getInt("Level", 10);
            showMessage("Level is " + bundle.getString("Level"));

            if (bundle.getString("VisualHelp") != null) {
                vHelp = bundle.getBoolean("VisualHelp");
                showMessage("VisualHelp = " + String.valueOf(vHelp));
            }
        }

        initSupuLevel(level, vHelp, null);
    }

    //region initMembers
    /**
     * initSupuLevel
     * inits a specific level for supu game
     * @param lvl int to set level
     * @param visualHelp boolean when true to clear all previous settings
     */
    protected void initSupuLevel(int lvl, boolean visualHelp, Game loadedGame) {

        boolean loaded = false;
        dialogToOpen = DIALOGS.None;

        if (loadedGame != null) {
            loaded = true;
            level = loadedGame.dimension;
            game = loadedGame;
            game.supuBoard = loadedGame.supuBoard;
            game.dropBoard = loadedGame.dropBoard;
            game.stoneBoard = loadedGame.stoneBoard;
            vHelp = loadedGame.supuMode.getVisualHelp();
            auto = (loadedGame.supuMode.getAutomation() == AUTOMAT.ION);
            if (auto)
                myMenu.findItem(R.id.action_gameautomation).setChecked(true);
            else
                myMenu.findItem(R.id.action_gameautomation).setChecked(false);
            if (vHelp)
                myMenu.findItem(R.id.action_visualhelp).setChecked(true);
            else myMenu.findItem(R.id.action_visualhelp).setChecked(false);
        }
        else { // if (loadedGame == null) {
            level = (lvl < 5) ? 5 : (lvl > 10) ? 10 : lvl;
            AUTOMAT automat = (auto) ? AUTOMAT.ION : AUTOMAT.IOFF;
            game = new Game(getApplicationContext(), level, GAMEMODE.FreeStyle, automat, vHelp);
            // game.supuBoard = new SupuBoard(getApplicationContext(), level);
            // game.dropBoard = new DropBoard(getApplicationContext(), level);
            // game.stoneBoard = new StoneBoard(getApplicationContext(), level);
            // game.setVisualHelp(vHelp);
        }
        board = game.supuBoard;

        started = true;

        Point size = new Point();
        getWindowManager().getDefaultDisplay().getSize(size);
        int screenWidth = size.x;
        int screenHeight = size.y;
        gridLayoutBoard = (GridLayout) findViewById(R.id.GridLayoutBoard);
        if (gridLayoutBoard != null) {
            // TODO: 05/04/2024 implement it
        }
        gridLayoutStones = (GridLayout) findViewById(R.id.GridLayoutStones);
        if (gridLayoutStones != null) {
            // TODO: 05/04/2024 implement ir
        }
        dropBoardLayout = (GridLayout) findViewById(R.id.dropBoardLayout);
        if (dropBoardLayout != null) {
            // TODO: 05/04/2024 implement ir
        }

        boardLayout = new LinearLayout[level][level];
        gems = new ImageView[level][level];
        stoneBoardLayouts = new LinearLayout[level];
        dropBoardLayouts = new LinearLayout[level];

        movedStones = new HashMap<>();
        allImages = new HashMap<Integer, ImageView>();
        allFields = new HashMap<Integer, LinearLayout>();
        toFields = new HashMap<>();
        fromFields = new HashMap<>();
        movedStones = new HashMap<>();
        dragNDropMap = new HashMap<>();
        draggedViews = new HashMap<>();

        switch (level) {
            case 5:  setContentView(R.layout.activity_5x5); break;
            case 6:  setContentView(R.layout.activity_6x6); break;
            case 7:  setContentView(R.layout.activity_7x7); break;
            case 8:  setContentView(R.layout.activity_8x8); break;
            case 9:  setContentView(R.layout.activity_9x9); break;
            // TODO: case 11:
            case 10:
            default: setContentView(R.layout.activity_10x10); break;
        }

        motionLabel = (TextView) findViewById(R.id.motionLabel);
        motionLabel.setText("Level: " + game.supuLevel + " \tCredits: " + game.credits + " \tRow: 0");

        InitLinearLayoutFields();
        // InitallFieldsMap();
        InitImageFields();
        // InitImagesMap();

        InitImageOnTouchListeners();
        InitLinearLayoutOnDragListeners();

        rootView = getWindow().getDecorView().getRootView();

        // game = new Game(getApplicationContext(), level);
        // game.supuBoard = new Board(getApplicationContext(), level);
        // game.dropBoard = new DropBoard(getApplicationContext(), level);
        // game.stoneBoard = new StoneBoard(getApplicationContext(), level);
        movedStones = new HashMap<>();
        allImages = new HashMap<Integer, ImageView>();
        allFields = new HashMap<Integer, LinearLayout>();
        toFields = new HashMap<Integer, LinearLayout>();
        fromFields = new HashMap<>();
        movedStones = new HashMap<>();
        dragNDropMap = new HashMap<>();
        draggedViews = new HashMap<>();

        ResetBackgroundMap();
        // ResetForegroundMap();

        setLayoutStones(vHelp);

        InitLinearLayoutFields();
        // InitallFieldsMap();
        InitImageFields();
        // InitImagesMap();

        InitLinearLayoutOnDragListeners();
        InitImageOnTouchListeners();

        if (loaded) {
            InitFromLoadedGame(loadedGame);
        } else if (auto) {
            InitFromLoadedGame(game);
        }

        started = true;
        startedTimes++;
        // showMessage(getString(R.string.string_started));
    }

    /**
     * InitLinearLayoutFields assigns LinearLayout fields
     * to local variables boardLayout, stoneBoardLayouts, dropBoardLayouts
     * and fromFields, toFields
     * assigns all LinearLayout supu baord fields e.g. a0, a1, ..., a5, b0, b1, ..
     * to boardLayout[columnIndex][rowIndex]
     * a0 (a => 0) is mapped to boardLayout[0][0]
     * b2 (b => 1) is mapped to boardLayout[1][2]
     */
    private void InitLinearLayoutFields() {

        toFields.clear();
        fromFields.clear();
        allFields.clear();

        // Add all StoneBoard Layout Fields to stoneBoardLayouts
        for (int rc = 0; rc < level; rc++) {
            COLUMN bc = Constants.allCols[rc];

            String rName = "x" + bc.lowerChar() + String.valueOf(rc);
            int rcId = getApplicationContext().getResources().getIdentifier(rName, "id", getApplicationContext().getPackageName());

            stoneBoardLayouts[rc] = (LinearLayout) findViewById(rcId);
            stoneBoardLayouts[rc].setTag(rName);

            if (!fromFields.containsKey(rcId))
                fromFields.put(rcId, stoneBoardLayouts[rc]);
        }

        // supuBoardLayouts mapping
        for (int c = 0; c < level; c++) {
            COLUMN bc = Constants.allCols[c];

            for (int r = 0; r < level; r++) {
                String rName = "";
                try {
                    rName = bc.lowerChar() + String.valueOf(r);

                    int rId = getApplicationContext().getResources().getIdentifier(rName, "id", getApplicationContext().getPackageName());
                    boardLayout[c][r] = findViewById(rId);

                    // Add each supuBoardField Layout to toFields
                    if (!toFields.containsKey(rId))
                        toFields.put(rId, boardLayout[c][r]);
                    // Add each supuBoardField Layout to fromFields
                    if (!fromFields.containsKey(rId))
                        fromFields.put(rId, boardLayout[c][r]);

                    // if (boardLayout[c][r].getTag().toString() != rName.toString()) {
                    //     boardLayout[c][r].setTag(rName);
                    // }

                } catch (Exception e) {
                    showMessage("Exception " + e.getMessage() + "\r\n" + "rname = " + rName + " c=" + c + " r=" + r);
                }
            }
        }


        // map dropBoardLayouts fields
        for (int rc = 0; rc < level; rc++) {
            COLUMN bc = Constants.allCols[rc];

            String rName = "z" + bc.lowerChar() + String.valueOf(rc);
            int rcId = getApplicationContext().getResources().getIdentifier(rName, "id", getApplicationContext().getPackageName());

            dropBoardLayouts[rc] = (LinearLayout) findViewById(rcId);
            dropBoardLayouts[rc].setTag(rName);

            if (!fromFields.containsKey(rcId))
                fromFields.put(rcId, dropBoardLayouts[rc]);
            if (!toFields.containsKey(rcId))
                toFields.put(rcId, stoneBoardLayouts[rc]);
        }

        allFields.putAll(fromFields);
        allFields.putAll(toFields);
    }

    /**
     * Init all allImages = new HashMap<Integer, ImageView>();
     */
    private void InitImageFields() {
        allImages.clear();

        for (int r = 0; r <  level; r++) {

            for (int c = 0; c < level; c++) {
                COLUMN bc = Constants.allCols[c];

                String gemName = "gem" + r + bc.lowerChar();
                int gemId = getApplicationContext().getResources().getIdentifier(gemName, "id", getApplicationContext().getPackageName());

                gems[c][r] = (ImageView) findViewById(gemId);
                if (gems[c][r] != null) {
                    CharSequence tagName = gems[c][r].getContentDescription();
                    gems[c][r].setTag(tagName.toString());
                    // TODO: compare Tag name with matrix row & column

                    if (!allImages.containsKey(gemId))
                        allImages.put(gemId, gems[c][r]);
                    else
                        allImages.replace(gemId, gems[c][r]);
                    // gems[c][r].setVisibility(View.VISIBLE);
                } else {
                    String x = String.valueOf(gemId);
                }
            }
        }
    }

    /**
     * Init all image onTouchListener
     */
    private void InitImageOnTouchListeners() {

        for (HashMap.Entry<Integer, ImageView> entry : allImages.entrySet()) {
            int ressourceKey = entry.getKey();
            final ImageView myImage = entry.getValue();
            findViewById(ressourceKey).setOnTouchListener(new OnTouchListener() {
                @Override
                public boolean onTouch(View view, MotionEvent motionEvent) {
                    return onTouchListener(view, motionEvent, myImage);
                }
            });
        }
    }

    /**
     * Init all linear layout onDragListener
     */
    private void InitLinearLayoutOnDragListeners() {
        for (HashMap.Entry<Integer, LinearLayout> entry : allFields.entrySet()) {
            int ressourceKey = entry.getKey();
            LinearLayout linearLayout = entry.getValue();

            findViewById(ressourceKey).setOnDragListener(new OnDragListener() {
                @Override
                public boolean onDrag(View view, DragEvent dragEvent) {
                    return onDragListener(view, dragEvent);
                }
            });
        }
    }

    /**
     * InitFromLoadedGame init a supu Level from loaded game
     * @param loadedGame {@link eu.georgtoth.supu.models.Game}
     */
    private void InitFromLoadedGame(Game loadedGame) {

        String gemName;
        String stoneBoardName;
        String supuBoardName;

        if (loadedGame.supuBoard == null)
            return;

        int maxRow = -1;
        // supuBoardLayouts mapping
        for (int r = 0; r < level; r++) {
            String rName = "";
            COLUMN rc = Constants.allCols[r];

            for (int c = 0; c < level; c++) {
                COLUMN bc = Constants.allCols[c];

                rName = bc.lowerChar() + String.valueOf(r);

                int rId = getApplicationContext().getResources().getIdentifier(rName, "id", getApplicationContext().getPackageName());
                boardLayout[c][r] = (LinearLayout) findViewById(rId);
                SupuStone myField = loadedGame.supuBoard.getField(bc, r);

                if (myField != null && myField.color.getValue() < GEMCOLORS.X.getValue()) {

                    int colorVal = myField.color.getValue();
                    stoneBoardName = "x" + myField.color.toString() + myField.color.getValue();
                    gemName = "gem" + r + myField.color.getChar();
                    int stoneBoardId = getApplicationContext().getResources().getIdentifier(stoneBoardName, "id", getApplicationContext().getPackageName());
                    int rGemId = getApplicationContext().getResources().getIdentifier(gemName, "id", getApplicationContext().getPackageName());

                    supuBoardName = bc.lowerChar() + String.valueOf(r);
                    // TODO supuBoardId seems equal rId
                    int supuBoardId = getApplicationContext().getResources().getIdentifier(supuBoardName, "id", getApplicationContext().getPackageName());

                    if (stoneBoardId > -1 && rGemId > -1 && supuBoardId > -1) {
                        stoneBoardLayouts[colorVal] = (LinearLayout) findViewById(stoneBoardId);
                        gems[colorVal][r] = (ImageView) findViewById(rGemId);
                        boardLayout[c][r] = findViewById(supuBoardId);
                        stoneBoardLayouts[colorVal].removeView(gems[colorVal][r]);
                        boardLayout[c][r].addView(gems[colorVal][r]);
                        if (gems[colorVal][r].getVisibility() != View.VISIBLE)
                            gems[colorVal][r].setVisibility(View.VISIBLE);

                        if (r > maxRow)
                            maxRow = r;
                    }
                }
            }
            game.stoneBoard.reFill();
            // resetStoneBoard(r);
        }
        if (maxRow > 0) {
            int fullRows = loadedGame.supuBoard.rowsFullyCompleted();
            if ((fullRows) > maxRow)
                maxRow = fullRows;
            resetStoneBoard(maxRow);
        }
    }


    /**
     * startGame() restarts a new game
     */
    public void startGame() {
        initSupuLevel(level, vHelp, null);
    }

    /**
     * startLevel() starts level lvl
     * @param lvl int level
     * @param visualHelp boolean show visual help
     */
    public void startLevel(int lvl, boolean visualHelp, Game aGame) { initSupuLevel(lvl, visualHelp, aGame); }

    //endregion

    //region resetMembers
    /**
     * resetStoneBoard
     *
     * @param sbRow if -1 make all visible, else only StoneBoardRow sbRow
     */
    private void resetStoneBoard(int sbRow) {
        // Never reset allImages map
        // allImages.clear();
        if (sbRow < 0)
            sbRow = 0;
        if (sbRow >= level)
            sbRow = level - 1;

        for (int rr = sbRow; rr < (sbRow + 1); rr++) {

            for (int cx = 0; cx < level; cx++) {
                COLUMN bcx = Constants.allCols[cx];

                String gemName = "gem" + String.valueOf(rr) + bcx.lowerChar();
                int gemId = getApplicationContext().getResources().getIdentifier(gemName, "id", getApplicationContext().getPackageName());
                gems[cx][rr] = (ImageView) findViewById(gemId);
                if (gems[cx][rr] != null && gems[cx][rr].getVisibility() == View.INVISIBLE)
                    gems[cx][rr].setVisibility(View.VISIBLE);
                if (!allImages.containsKey(gemId)) {
                    allImages.put(gemId, gems[cx][rr]);
                }

                String stoneBoardName = "x" + bcx.lowerChar() + cx;
                int stoneBoardId = getApplicationContext().getResources().getIdentifier(stoneBoardName, "id", getApplicationContext().getPackageName());
                stoneBoardLayouts[cx] = (LinearLayout) findViewById(stoneBoardId);
                if (stoneBoardLayouts[cx] != null) {
                    int childCnt = stoneBoardLayouts[cx].getChildCount();
                    for (int childIdx = 0; childIdx < childCnt; childIdx++) {
                        ImageView myStone = (ImageView) stoneBoardLayouts[cx].getChildAt(childIdx);
                        if (myStone != null) {
                            if (childIdx > 0)
                                myStone.setVisibility(View.INVISIBLE);
                            else
                                myStone.setVisibility(View.VISIBLE);
                        }
                    }
                }
            }
        }
    }


    /**
     * Resets board map background color
     */
    private void ResetBackgroundMap() {

        for (int c = 0; c < level; c++) {
            COLUMN bc = Constants.allCols[c];

            for (int r = 0; r < level; r++) {
                String rName = bc.lowerChar() + String.valueOf(r);

                int rId = getApplicationContext().getResources().getIdentifier(rName, "id", getApplicationContext().getPackageName());
                int rDrawId = (((c + r) % 2) == 0) ? R.drawable.stonecolorv : R.drawable.stonecolorw;
                switch (level)
                {
                    case 5:
                        rDrawId = (((c + r) % 2) == 0) ? R.drawable.stone5colorv : R.drawable.stone5colorw;
                        break;
                    case 6:
                        rDrawId = (((c + r) % 2) == 0) ? R.drawable.stone6colorv : R.drawable.stone6colorw;
                        break;
                    case 7:
                        rDrawId = (((c + r) % 2) == 0) ? R.drawable.stone7colorv : R.drawable.stone7colorw;
                        break;
                    case 8:
                    case 9:
                        rDrawId = (((c + r) % 2) == 0) ? R.drawable.stone8colorv : R.drawable.stone8colorw;
                        break;
                    default:
                        if (((c + r) % 2) == 0)
                            rDrawId = R.drawable.stonecolorv;
                        else
                            rDrawId = R.drawable.stonecolorw;
                        break;
                }
                Drawable drawable = ResourcesCompat.getDrawable(getResources(), rDrawId, getTheme());
                findViewById(rId).setBackground(drawable);
            }
        }

        for (int rc = 0; rc < level; rc++) {
            COLUMN bc = Constants.allCols[rc];

            String rName = "x" + bc.lowerChar() + String.valueOf(rc);
            int rcId = getApplicationContext().getResources().getIdentifier(rName, "id", getApplicationContext().getPackageName());
            int rcDrawId = ((rc % 2) == 0) ? R.drawable.stonecolorw : R.drawable.stonecolorv;
            switch (level)
            {
                case 5:
                    rcDrawId = (((rc) % 2) == 0) ? R.drawable.stone5colorv : R.drawable.stone5colorw;
                    break;
                case 6:
                    rcDrawId = (((rc) % 2) == 1) ? R.drawable.stone6colorv : R.drawable.stone6colorw;
                    break;
                case 7:
                    rcDrawId = (((rc) % 2) == 0) ? R.drawable.stone7colorv : R.drawable.stone7colorw;
                    break;
                case 8:
                case 9:
                    rcDrawId = (((rc) % 2) == 0) ? R.drawable.stone8colorv : R.drawable.stone8colorw;
                    break;
                default:
                    rcDrawId = (((rc) % 2) == 0) ? R.drawable.stonecolorv : R.drawable.stonecolorw;
                    if ((rc % 2) == 0)
                        rcDrawId = R.drawable.stonecolorv;
                    else
                        rcDrawId = R.drawable.stonecolorw;
                    break;
            }
            Drawable drawable = ResourcesCompat.getDrawable(getResources(), rcDrawId, getApplicationContext().getTheme());
            // Drawable drawable = getResources().getDrawable(rcDrawId, getApplicationContext().getTheme());

            ((LinearLayout)findViewById(rcId)).setBackground(drawable);
        }

        for (int zc = 0; zc < level; zc++) {
            COLUMN zbc = Constants.allCols[zc];

            String rName = "z" + zbc.lowerChar() + String.valueOf(zc);
            int rcId = getApplicationContext().getResources().getIdentifier(rName, "id", getApplicationContext().getPackageName());
            int rcDrawId = ((zc % 2) == 0) ? R.drawable.stonecolorw : R.drawable.stonecolorv;
            switch (level)
            {
                case 5:
                    rcDrawId = (((zc) % 2) == 1) ? R.drawable.stone5colorv : R.drawable.stone5colorw;
                    break;
                case 6:
                    rcDrawId = (((zc) % 2) == 0) ? R.drawable.stone6colorv : R.drawable.stone6colorw;
                    break;
                case 7:
                    rcDrawId = (((zc) % 2) == 1) ? R.drawable.stone7colorv : R.drawable.stone7colorw;
                    break;
                case 8:
                case 9:
                    rcDrawId = (((zc) % 2) == 1) ? R.drawable.stone8colorv : R.drawable.stone8colorw;
                    break;
                default:
                    rcDrawId = (((zc) % 2) == 1) ? R.drawable.stonecolorv : R.drawable.stonecolorw;
                    if ((zc % 2) == 1)
                        rcDrawId = R.drawable.stonecolorv;
                    else
                        rcDrawId = R.drawable.stonecolorw;
                    break;
            }
            Drawable drawable = ResourcesCompat.getDrawable(getResources(), rcDrawId, getApplicationContext().getTheme());
            // Drawable drawable = getResources().getDrawable(rcDrawId, getApplicationContext().getTheme());

            ((LinearLayout)findViewById(rcId)).setBackground(drawable);
        }
    }

    /**
     * Resets board foreground map
     */
    @Deprecated
    private void ResetForegroundMap() {
        // remove all views from eeach LinearLayout in toFields (= destination layout views)
        for (HashMap.Entry<Integer, LinearLayout> toViewEntry : toFields.entrySet()) {
            int ressourceKey = toViewEntry.getKey();
            LinearLayout toLinearLayout = toViewEntry.getValue();
            toLinearLayout.removeAllViews();
            ((LinearLayout) findViewById(ressourceKey)).removeAllViews();
        }

        // remove all views from eeach LinearLayout in fromFields (= source layout views)
        for (HashMap.Entry<Integer, LinearLayout> fromViewEntry : fromFields.entrySet()) {
            int ressourceKey = fromViewEntry.getKey();
            LinearLayout fromLinearLayout = fromViewEntry.getValue();
            fromLinearLayout.removeAllViews();
            ((LinearLayout) findViewById(ressourceKey)).removeAllViews();
        }

        for (int c = 0; c < level; c++) {
            COLUMN bc = Constants.allCols[c];

            for (int r = 0; r < level; r++) {
                COLUMN br = Constants.allCols[r];

                String gamName = "gem" + r + bc.lowerChar();
                String stoneBoardName = "x" + bc.lowerChar() + c;

                int gemId = getApplicationContext().getResources().getIdentifier(gamName, "id", getApplicationContext().getPackageName());
                int stoneBoardId = getApplicationContext().getResources().getIdentifier(stoneBoardName, "id", getApplicationContext().getPackageName());

                gems[c][r].setVisibility(View.VISIBLE);
                String gemTag = gems[c][r].getContentDescription().toString();
                gems[c][r].setTag(gemTag);

                View alreadyExists0 = stoneBoardLayouts[c].findViewWithTag(stoneBoardName);
                View alreadyExists1 = stoneBoardLayouts[c].findViewById(stoneBoardId);
                View alreadyExists2 = stoneBoardLayouts[c].findViewById(gemId);

                if (alreadyExists0 == null && alreadyExists1 == null  && alreadyExists2 == null && stoneBoardLayouts[r].getChildCount() == 0) {
                    try {
                        stoneBoardLayouts[r].addView(gems[c][r], 0);
                    } catch (Exception ex) {
                        ex.printStackTrace();
                    }
                }
                stoneBoardLayouts[r].setVisibility(View.VISIBLE);

                // ((LinearLayout)findViewById(vId)).addView(((ImageView)findViewById(rId)), 0);
            }
        }
    }

    /**
     * resetBackground resets the background after drag and drop
     */
    public void resetBackground() {
        if (toDropView != null) {
            int toId = toDropView.getId();
            if (dragNDropMap.containsKey(toId)) {
                toDropView.setBackground(dragNDropMap.get(toId));
            }

            try {
                resetBackgroundHandler.removeCallbacks(runResetBackground);
            } catch (Exception removingCallbackDelegateFailedEx) {
                showException(removingCallbackDelegateFailedEx);
            }
        }
    }

    /**
     * resetDraggedEntries resets the background after drag and drop
     * @param view {@Link android.view.View}
     * @param owner {@link ViewGroup}
     * @param container {@link LinearLayout}
     * @return false
     */
    public boolean resetDraggedEntries(View view, ViewGroup owner, LinearLayout container) {

        boolean canDropGem = false;

        owner.removeView(view);
        owner.addView(view, 0);
        view.setVisibility(View.VISIBLE);

        if (dragNDropMap.containsKey(owner.getId())) {
            Drawable dr = dragNDropMap.get(owner.getId());
            if (dr != null)
                owner.setBackground(dr);
        }

        for (HashMap.Entry<Integer, View> entry : draggedViews.entrySet()) {
            entry.getValue().setVisibility(View.VISIBLE);
        }

        return canDropGem;
    }

    //endregion

    //region OnTouchOnDragListeners

    /**
     * onTouchListener fired, when view with image is touched
     * @param view the view which is touched
     * @param motionEvent specific devent
     * @param theImage image in the view, who is touched
     */
    public boolean onTouchListener(View view, MotionEvent motionEvent, ImageView theImage) {
        syncLocker = Integer.valueOf(startedDrag);
        synchronized (syncLocker) {
            startedDrag = 0;
            fromBoard = BOARDS.NOB;
            fromBoardColumn = COLUMN.NONE;
            boardTag = null;
        }

        if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
            currentImage = theImage;
            boolean wellImageSelected = false;
            if (theImage == null) {
                return false;
            }
            if (!IsCorrectStone(theImage)) {
                showMessage("Wrong image " + theImage.getId() + " touched!");
                return false;
            }

            if (!IsCorrectFromView(view)) {
                ViewGroup parent = (ViewGroup) view.getParent();
                if (!IsCorrectFromView((View)parent)) {
                    showMessage("Wrong view " + view.getId() + " / parent " + parent.getId() + " touched!");
                    return false;
                }
            }

            theStone = theImage;
            theStoneLayout = (LinearLayout) view.getParent();

            boardTag = ((View)(view.getParent())).getTag().toString();
            if (boardTag.length() >= 2 && boardTag.charAt(0) == 'x') {
                fromBoard = BOARDS.STONEB;
                fromBoardColumn = COLUMN.getEnum(boardTag.charAt(1));
                currentStone =  game.stoneBoard.getField(fromBoardColumn);
            }
            if (boardTag.length() >= 2 && boardTag.charAt(0) == 'z') {
                fromBoard = BOARDS.DROPB;
                fromBoardColumn = COLUMN.getEnum(boardTag.charAt(1));
                currentStone =  game.dropBoard.getField(fromBoardColumn);
            }
            if (boardTag.length() == 2) {
                fromBoard = BOARDS.SUPUB;
                currentStone = game.supuBoard.getFieldByName(boardTag);
            }

            dragNDropMap = new HashMap<>();
            dragNDropMap.put(view.getId(), view.getBackground());

            draggedViews = new HashMap<>();
            draggedViews.put(view.getId(), view);

            ClipData data = ClipData.newPlainText("", "");
            DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(view);
            view.startDragAndDrop(data, shadowBuilder, view, View.DRAG_FLAG_GLOBAL);
            //view.startDrag(data, shadowBuilder, view, 0);
            view.setVisibility(View.INVISIBLE);

            return true;
        } else {
            return false;
        }
    }

    /**
     * onDragListener fired, when drag and drop event is performed
     * @param v view on which image is dragged or dropped
     * @param event specific drag, drop event
     */
    public boolean onDragListener(View v, DragEvent event) {

        int stopDrag = 0;

        SUPURULES supuRules = SUPURULES.NONE;
        DROPSTATE dropstate = DROPSTATE.GENERICERROR;
        Drawable savedBackground;
        Drawable enterShape = getResources().getDrawable(R.drawable.shape_droptarget, null);
        Drawable normalShape = getResources().getDrawable(R.drawable.shape, null);

        int action = event.getAction();
        switch (event.getAction()) {
            case DragEvent.ACTION_DRAG_STARTED:
                // do nothing
                savedBackground = v.getBackground();
                // TODO 1st add and never remove 1st & add 2nd, 3rd, ...
                if (!dragNDropMap.containsKey(v.getId())) {
                    dragNDropMap.put(v.getId(), savedBackground);
                }
                syncLocker = Integer.valueOf(startedDrag);
                synchronized (syncLocker) {
                    finishedDrop = 0;
                    if (syncLocker.intValue() == 0) {
                        startedDrag++;
                        playL8rHandler.postDelayed(delayPlayTakeStone, 10);
                        // playTakeStone();
                    }
                }
                break;
            case DragEvent.ACTION_DRAG_ENTERED:
            case DragEvent.ACTION_DRAG_LOCATION:
                savedBackground = v.getBackground();
                if (!dragNDropMap.containsKey(v.getId())) {
                    dragNDropMap.put(v.getId(), savedBackground);
                }
                // v.setBackgroundDrawable(enterShape);
                v.setBackground(enterShape);
                break;
            case DragEvent.ACTION_DRAG_ENDED:
                View view2 = (View) event.getLocalState();
                ViewGroup owner2 = (ViewGroup) view2.getParent();
                LinearLayout container2 = (LinearLayout) v;
                if (!toFields.containsKey(container2.getId())) {
                    resetDraggedEntries(view2, owner2, container2);
                }

                if (dragNDropMap.containsKey(v.getId())) {
                    Drawable dr = dragNDropMap.get(v.getId());
                    if (dr != null) {
                        v.setBackground(enterShape);
                        v.setBackground(dr);
                        dragNDropMap.remove(v.getId());
                    }
                }

                startedDrag = 0;
                break;
            case DragEvent.ACTION_DRAG_EXITED:
                dropstate = DROPSTATE.INVALIDDROPVIEW;
                startedDrag = -1;
                View view3 = (View) event.getLocalState();
                ViewGroup owner3 = (ViewGroup) view3.getParent();
                LinearLayout container3 = (LinearLayout) v;
                if (!toFields.containsKey(container3.getId())) {
                    resetDraggedEntries(view3, owner3, container3);
                }

                if (dragNDropMap.containsKey(v.getId())) {
                    Drawable dr = dragNDropMap.get(v.getId());
                    if (dr != null) {
                        v.setBackground(enterShape);
                        v.setBackground(dr);
                        dragNDropMap.remove(v.getId());
                    }
                }
                if (theStone != null && theStoneLayout != null) {

                    if ((owner3 == null || owner3.getTag() == null) ||
                            !(owner3 instanceof LinearLayout)) {
                        View stone2Add = theStoneLayout.findViewWithTag(theStone.getTag());
                        if (stone2Add == null) {
                            showMessage("Add Stone " + theStone.getTag() + " to " + theStoneLayout.getTag());
                            theStoneLayout.addView(theStone);
                        }
                    }

                    View stoneAdded = theStoneLayout.findViewWithTag(theStone.getTag());
                    if (stoneAdded == null) {
                        showMessage("Add stone " + theStone.getTag() + " to " + theStoneLayout.getTag());
                        theStoneLayout.addView(theStone);
                    }
                    else stoneAdded.setVisibility(View.VISIBLE);
                }

                break;
            case DragEvent.ACTION_DROP:
                // Dropped, reassign View to ViewGroup
                View view = (View) event.getLocalState();
                ViewGroup owner = (ViewGroup)view.getParent();
                LinearLayout container = (LinearLayout) v;

                boolean canDropGem = false;
                GEMCOLORS color = GEMCOLORS.Z;
                COLUMN column = COLUMN.NONE;
                int row = 0;
                SupuStone boardIndexer = null;
                // toDropView = (View) v;

                String viewDbgInfo = "";

                // check if stone comes from valid from field
                if (fromFields.containsKey(owner.getId())) {
                    LinearLayout fromLayout = fromFields.get(owner.getId());
                    boardTag = fromLayout.getTag().toString();
                    if (boardTag.length() >= 2 && boardTag.charAt(0) == 'x') {
                        fromBoard = BOARDS.STONEB;
                        fromBoardColumn = COLUMN.getEnum(boardTag.charAt(1));
                    }
                    if (boardTag.length() >= 2 && boardTag.charAt(0) == 'z') {
                        fromBoard = BOARDS.DROPB;
                        fromBoardColumn = COLUMN.getEnum(boardTag.charAt(1));
                        currentStone =  game.dropBoard.getField(fromBoardColumn);
                    }
                    if (boardTag.length() == 2) {
                        fromBoard = BOARDS.SUPUB;
                        currentStone = game.supuBoard.getFieldByName(boardTag);
                    }
                    viewDbgInfo += "from  view (" + boardTag + ") \r\n";
                    canDropGem = true;
                } else {
                    canDropGem = resetDraggedEntries(view, owner, container);
                    return true;
                }

                // check if stone moves to valid to field
                if (toFields.containsKey(container.getId())) {
                    LinearLayout toLayout = toFields.get(container.getId());
                    String toView = toLayout.getTag().toString();
                    try {
                        boardIndexer = game.supuBoard.getFieldByName(toView);
                        column = boardIndexer.column;
                        row = boardIndexer.row;
                        if (fromBoard == BOARDS.SUPUB) {
                            SupuStone boardIdxOld =  game.supuBoard.getFieldByName(boardTag);
                            game.supuBoard.removeStone(boardIdxOld.column, boardIdxOld.row);
                        }

                        canDropGem = true;
                    } catch (Exception exR) {
                        showException(exR);
                    }
                    viewDbgInfo += "to view <" + toView + "> \r\n";

                } else if (fromFields.containsKey(container.getId()))  {
                    canDropGem = resetDraggedEntries(view, owner, container);
                    return true;
                }  else {
                    canDropGem = resetDraggedEntries(view, owner, container);
                    return true;
                }
                if (boardIndexer == null) {
                    canDropGem = resetDraggedEntries(view, owner, container);
                    return true;
                }

                String fieldName = boardIndexer.getName();
                int fieldId = -1;
                try {
                    fieldId = getApplicationContext().getResources().getIdentifier(fieldName, "id", getApplicationContext().getPackageName());
                } catch (Exception invalidFieldIdEx) {
                    // showException(invalidFieldIdEx);
                    fieldId = -1;
                }


                for (String droppedFigure : Constants.figures) {
                    //Do your stuff here
                    int myId = getApplicationContext().getResources().getIdentifier(droppedFigure, "id", getApplicationContext().getPackageName());
                    if (view.getId() == myId) {
                        color = GEMCOLORS.getColorByFigure(droppedFigure);
                        viewDbgInfo += "gemstone [" + droppedFigure + "] with color" + color.getValue() + " \r\n";
                    }
                }

                boolean canSetGem = false;
                try {
                    if (canDropGem) {
                        boardIndexer = game.supuBoard.setGemColor(column, row, color);
                        canSetGem = true;
                    }
                } catch (Exception ex4) {
                    viewDbgInfo += "Cannot set gam, because of Exception " + ex4.getMessage() + " \r\n";
                    canSetGem = false;
                    canDropGem = false;
                }

                if (canSetGem && boardIndexer != null) {
                    canDropGem = true;

                    if (boardIndexer.column != column || boardIndexer.row != row)
                    {

                        canDropGem = false;

                        viewDbgInfo += "nbecause, color already set in " + boardIndexer.column + "" + boardIndexer.row + "!\r\n";
                        // set field for a while alert yellow,
                        fieldId = -1;

                        try {
                            fieldName = boardIndexer.getFieldName();
                            // showMessage("fieldName[" + boardIndexer.column.getName() + "][" + boardIndexer.row + "] => " + fieldName, false);
                            fieldId = getApplicationContext().getResources().getIdentifier(fieldName, "id", getApplicationContext().getPackageName());
                        } catch (Exception alertYellow) {
                            showException(alertYellow);
                            fieldId = -1;
                        }
                        try {
                            toDropView = (View) findViewById(fieldId);
                            syncLocker = Integer.valueOf(finishedDrop);
                            synchronized (syncLocker) {
                                if (syncLocker.intValue() == 0) {
                                    finishedDrop = 1;
                                    game.credits--;
                                    game.perfectLevel = false;
                                    playErrorDoubleColor();
                                }
                            }

                            if (finishedDrop == 1) {
                                motionLabel.setText("Level: " + game.supuLevel + " \tCredits: " + game.credits + " \tRow: " + game.supuBoard.rowsFullyCompleted());
                                playL8rHandler.postDelayed(delayPlayCreditsMinus, 400);
                            }

                            dropstate = DROPSTATE.DOUBLECOLOR;
                            supuRules = SUPURULES.DENY_CROSS_COLORED;
                            if (toDropView != null) {
                                if (dragNDropMap.containsKey(fieldId)) {
                                    dragNDropMap.put(fieldId, toDropView.getBackground());
                                    dragNDropMap.remove(fieldId);
                                }

                                toDropView.setBackground(enterShape);
                                // toDropView.setBackgroundDrawable(enterShape);

                                // findViewById(fieldId).setBackground(getResources().getDrawable(R.drawable.bgyellowalert, getApplicationContext().getTheme()));
                                resetBackgroundHandler.postDelayed(runResetBackground, 2000);
                            }

                        } catch (Exception invalidFieldEx) {
                            showException(invalidFieldEx);
                        }
                    }
                }

                for (int vc = 0; vc < container.getChildCount(); vc++) {
                    if (allImages.containsKey(container.getChildAt(vc).getId())) {
                        String aTag = allImages.get(container.getChildAt(vc).getId()).getTag().toString();
                        viewDbgInfo += "\r\nCan't drop, because containing already {" + aTag + "}\r\n";
                    }
                    for (String containingFigure : Constants.figures) {
                        //Do your stuff here
                        int myId = getApplicationContext().getResources().getIdentifier(
                                containingFigure, "id", getApplicationContext().getPackageName());
                        if (container.getChildAt(vc).getId() == myId) {
                            viewDbgInfo += "\r\nCan't drop, because occuopied by {" + containingFigure + "}\r\n";
                            canDropGem = false;
                            supuRules = SUPURULES.ERR_STONE_ALREADY_SET;
                        }
                    }
                }

                // showMessage(viewDbgInfo, false);

                if (!canDropGem) {  // we can't drop stone
                    // TODO: Define Figure[12] as constant
                    syncLocker = Integer.valueOf(finishedDrop);
                    synchronized(syncLocker) {
                        if (syncLocker.intValue() == 0) {
                            finishedDrop = 1;
                            playErrorForbiddenPattern();
                        } else {
                            if (game.credits < 0) {
                                finishedDrop = 2;
                            }
                        }
                    }
                    if (finishedDrop == 2) {
                        showGameOverDialog();
                    }
                    resetDraggedEntries(view, owner, container);
                    startedDrag = 0;
                } else {  // we can drop stone
                    // remove view from parent owner  and set it to container
                    owner.removeView(view);
                    container.addView(view, 0);

                    // add view Id to movedStones
                    // movedStones.put(view.getId(), view);

                    // container.addView(view);
                    view.setVisibility(View.VISIBLE);
                    try {
                        if (dragNDropMap.containsKey(container.getId())) {
                            Drawable dr = dragNDropMap.get(container.getId());
                            if (dr != null)
                                container.setBackground(dr);
                            dragNDropMap.remove(container.getId());
                        }
                        toDropView = (View) container;
                        dragNDropMap.put(container.getId(), container.getBackground());

                        syncLocker = Integer.valueOf(finishedDrop);
                        synchronized (syncLocker) {
                            if (syncLocker.intValue() == 0) {
                                finishedDrop = 1;
                            }
                            startedDrag = 0;
                            dropstate = DROPSTATE.CANDROP;
                            supuRules = SUPURULES.OK;
                        }

                        if (finishedDrop == 1) {
                            if (fromBoard == BOARDS.STONEB) {
                                // TODO: Add here remove field

                                SupuStone stone = game.stoneBoard.removeStone(fromBoardColumn);
                                if (game.stoneBoard.isEmpty()) {
                                    // TODO:
                                    game.stoneBoard.reFill();
                                    resetStoneBoard(game.stoneBoard.reRow);
                                }
                            }
                            if (game.supuBoard.isGameFinished(level)) {
                                motionLabel.setText("Level: " + game.supuLevel + " \tCredits: " + game.credits + " \tRow: " + game.supuBoard.rowsFullyCompleted());
                                playLevelCompleted();
                            } else {
                                if (game.supuBoard.isRowCompleted(level, row)) {
                                    if (game.stoneBoard.isEmpty()) {
                                        game.stoneBoard.reFill();

                                        int maxRowResetStoneBoard = (game.supuBoard.rowsFullyCompleted() > (int)row) ? (int)game.supuBoard.rowsFullyCompleted() : (int)row;
                                        resetStoneBoard(maxRowResetStoneBoard);
                                    }
                                    playRowCompleted();
                                    if (game.credits < Constants.MAX_CREDITS_PER_ROW) {
                                        game.credits = Constants.MAX_CREDITS_PER_ROW;
                                        playL8rHandler.postDelayed(deplayPlayCreditsFull, 400);
                                    }
                                    motionLabel.setText("Level: " + game.supuLevel + " \tCredits: " + game.credits + " \tRow: " + game.supuBoard.rowsFullyCompleted());
                                } else
                                    playDropStone();
                            }
                        }

                        fromBoard = BOARDS.NOB;
                        fromBoardColumn = COLUMN.NONE;
                        boardTag = null;
                        container.setBackground(ResourcesCompat.getDrawable(getResources(), R.drawable.bggreenok, getTheme()));
                        resetBackgroundHandler.postDelayed(runResetBackground, 1250);
                    } catch (Exception ex) {
                        showException(ex);
                    }
                    if (game.supuBoard.isGameFinished(level)) {
                        if (game.perfectLevel) {
                            sound2Play = SOUNDS.LEVEL_PERFECT;
                            playL8rHandler.postDelayed(delayPlaySound, 333);
                            showFinishedLevelPerfectDialog();
                        } else {
                            showFinishedLevelDialog();
                        }
                    }
                }

                break;
            default:
                break;
        }
        // ResetBackgroundMap();
        return true;
    }

    //endregion

    /**
     * IsCorrectStone
     * @param theImage gemStone image, that is currently moved
     * @return true, if correct
     */
    private boolean IsCorrectStone(ImageView theImage) {

        boolean matchImage = false;

        if (allImages.containsKey(theImage.getId())) {
            matchImage = true;
            // showMessage("Matched image id: " + theImage.getId() + " Name: " + theImage.getTag().toString());
        }
        for (HashMap.Entry<Integer, ImageView> entry : allImages.entrySet()) {
            int ressourceKey = entry.getKey();
            final ImageView myImage = entry.getValue();
            if ((myImage == theImage) || (theImage == ((ImageView)findViewById(ressourceKey)))) {
                matchImage = true;
                // showMessage("Matched image id: " + ressourceKey + " Name: " + myImage.getTransitionName());
                break;
            }
        }

        if (matchImage) {

            View iv = (View) theImage;
            if (movedStones.containsKey(iv.getId())) {
                // showMessage("Stone " + movedStones.get(iv.getId()).getTag().toString() + " already moved!", false);
                return false;
            }
            return true;
        }

        return false;
    }

    /**
     * IsCorrectFromView
     * @param v view to verify, if it's a correct from drag n drop view
     * @return true, if correct
     */
    private boolean IsCorrectFromView(View v) {
        boolean matchToView = false;
        boolean matchFromView = false;

        // for (HashMap.Entry<Integer, LinearLayout> entry : toFields.entrySet()) {
        //     int ressourceKey = entry.getKey();
        //     LinearLayout linearLayout = entry.getValue();
        //     if (v == ((LinearLayout) findViewById(ressourceKey))) {
        //         return false;
        //     }
        // }

        for (HashMap.Entry<Integer, LinearLayout> entry : fromFields.entrySet()) {
            int ressourceKey = entry.getKey();
            LinearLayout linearLayout = entry.getValue();
            if (v == ((LinearLayout) findViewById(ressourceKey))) {
                return true;
            }
        }

        return false;
    }

    //region MenuMembers
    /**
     * onCreateOptionsMenu
     * Override
     * @param menu main menu
     * @return true, if created without errors, otherwise false
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        myMenu = menu;
        int menuId = -1;

        try {
            if ((menuId = getApplicationContext().getResources().getIdentifier("menu_main",
                    "menu", getApplicationContext().getPackageName())) >= 0) {

                getMenuInflater().inflate(menuId, menu);
                return true;
            }
        } catch (Exception menuEx) {
            showException(menuEx);
        }

        return false;
    }

    /**
     * actionMenuItem - you must implement that method for your purpose
     * @param itemId - ressource Id of menu item
     * @param item - MenuItem item entity
     * @param parentMenu - parent Menu instance, where the menu item belongs to
     * @return true --> Event Consumed here, now It should not be forwarded for other event
     * 			false --> Forward for other event to get consumed
     */
    public boolean actionMenuItem(int itemId, MenuItem item, Menu parentMenu) {
        if (itemId >= 0 && item.getItemId() == itemId) {

            if (itemId == R.id.action_load) {
                setSubMenuLoad(myMenu);
            }

            if (itemId == R.id.action_start) {

                startGame();

            } else if (itemId == R.id.action_exit) {

                exitGame();

            } else if (itemId == R.id.action_save) {

                saveGame(game);

            } else if ((itemId == R.id.load_slot0) ||
                    (itemId == R.id.load_slot1) ||
                    (itemId == R.id.load_slot2) ||
                    (itemId == R.id.load_slot3) ||
                    (itemId == R.id.load_slot4) ||
                    (itemId == R.id.load_slot5) ||
                    (itemId == R.id.load_slot6)) {

                if (item.isEnabled()) {
                    Game aGame = loadGame(Objects.requireNonNull(item.getTitle()).toString());
                    // TODO: check start from loaded game
                    startLevel(aGame.supuLevel, false, aGame);

                }
            } else if (itemId == R.id.action_about) {

                showAbout();

            } else if (itemId == R.id.action_help) {

                showHelpDialog();

            } else if (itemId == R.id.action_visualhelp) {

                setLayoutVisualHelp();

            } else if (itemId == R.id.action_gameautomation) {

                setAutomation();

            } else if (itemId == R.id.action_direction) {

                setDirection();

            } else if (itemId == R.id.action_screenshot) {

                onCameraClick(null);

            } else if (itemId == R.id.action_five) {
                startLevel(5, false, null);
            } else if (itemId == R.id.action_six) {
                startLevel(6, false, null);
            } else if (itemId == R.id.action_seven) {
                startLevel(7,false, null);
            } else if (itemId == R.id.action_eight) {
                startLevel(8,false, null);
            } else if (itemId == R.id.action_nine) {
                startLevel(9,false, null);
            } else if (itemId == R.id.action_ten) {
                startLevel(10,false, null);
            } else {
                return false;
            }
            return true;
        }
        // we fall through by default
        return false;
    }

    //endregion

    //region setLayoutDirection
    /**
     * setLayoutVisualHelp
     *  set stones to gems
     */
    public void setLayoutVisualHelp() {
        boolean checked = myMenu.findItem(R.id.action_visualhelp).isChecked();
        vHelp = (!checked);
        myMenu.findItem(R.id.action_visualhelp).setChecked(vHelp);
        game.setVisualHelp(vHelp);
        this.setLayoutStones(vHelp);
    }

    /**
     * setLayoutStones - changes from gems to stones
     *
     * @param visualHelp - boolean true for vHelp support
     */
    public void setLayoutStones(boolean visualHelp) {

        String stoneColorLayer = "stonecolor";
        if (visualHelp) {
            vHelp = true;
            switch (level) {
                case 5: stoneColorLayer = "stone5color"; break;
                case 6: stoneColorLayer = "stone6color"; break;
                case 7: stoneColorLayer = "stone7color"; break;
                case 8:
                case 9: stoneColorLayer = "stone8color"; break;
                case 10:
                default: stoneColorLayer = "stonecolor"; break;
            }
        }
        else {
            vHelp = false;
            switch (level) {
                case 5: stoneColorLayer = "supu5color"; break;
                case 6: stoneColorLayer = "supu6color"; break;
                case 7: stoneColorLayer = "supu7color"; break;
                case 8:
                case 9: stoneColorLayer = "supu8color"; break;
                case 10:
                default: stoneColorLayer = "supucolor"; break;
            }
        }
        game.setVisualHelp(vHelp);
        this.setLayoutImages(stoneColorLayer);
    }

    /**
     * setLayoutImages changes the layout for the colored gems or stones
     * @param gemOrStones prefix for drawables
     *                    valid for now "stonecolor" or "gemcolor"
     */
    public void setLayoutImages(String gemOrStones) {
        for (int r = 0; r < level; r++) {

            for (int c = 0; c < level; c++) {
                COLUMN bc = Constants.allCols[c];

                String gemColorName = gemOrStones + c;
                int gemColorId = getApplicationContext().getResources().getIdentifier(gemColorName, "drawable", getApplicationContext().getPackageName());
                Drawable drawable = ResourcesCompat.getDrawable(getResources(), gemColorId, getTheme());


                String gemName = "gem" + r + bc.lowerChar();
                int gemId = getApplicationContext().getResources().getIdentifier(gemName, "id", getApplicationContext().getPackageName());
                ImageView aGem = (ImageView) findViewById(gemId);
                if (aGem != null) {
                    this.gems[c][r] = aGem;
                }

                if (this.gems[c][r] != null) {
                    this.gems[c][r].setImageDrawable(drawable);
                }

                if (allImages.containsKey(gemId)) {
                    allImages.get(gemId).setImageDrawable(drawable);
                } else {
                    if (this.gems[c][r] != null) {
                        allImages.put(gemId, gems[c][r]);
                    }
                    String x = String.valueOf(gemId);
                }
            }
        }
    }

    /**
     * setAutomation - sets game automation
     */
    public void setAutomation() {
        boolean checked = myMenu.findItem(R.id.action_gameautomation).isChecked();
        auto = !checked;
        myMenu.findItem(R.id.action_gameautomation).setChecked(auto);
        game.setAutomation(auto);
        if (auto) {
            if (game.supuBoard.rowsFullyCompleted() < 1) {
                game = new Game(getApplicationContext(), level, GAMEMODE.FreeStyle, AUTOMAT.ION, vHelp);
                InitFromLoadedGame(game);
            }
        }
    }

    /**
     * setDirection - sets game direction
     */
    public void setDirection() {
        boolean checked = myMenu.findItem(R.id.action_direction).isChecked();
        if (checked) {
            myMenu.findItem(R.id.action_direction).setChecked(false);
        }
        else {
            myMenu.findItem(R.id.action_direction).setChecked(true);
        }
    }

    //endregion

    //region ModalDialogs

    /**
     * showGameOverDialog
     */
    public void showGameOverDialog() {
        // Create an instance of the dialog fragment and show it
        dialogToOpen = DIALOGS.GameOver;
        DialogFragment dialog = new GameOver();
        Bundle dialogArgs = new Bundle();
        dialogArgs.putInt(getString(R.string.msg_perfect), 0);
        dialogArgs.putInt(getString(R.string.level_current), level);
        dialog.setArguments(dialogArgs);
        playL8rHandler.postDelayed(delayPlayGameOver, 1000);
		dialog.setCancelable(false);
        dialog.show(getSupportFragmentManager(), "GameOver");
    }

    /**
     * showFinishedLevelPerfectDialog
     */
    public void showFinishedLevelPerfectDialog() {
        // Create an instance of the dialog fragment and show it
        dialogToOpen = DIALOGS.FinishedLevelPerfect;
        DialogFragment dialog = new FinishedLevelPerfect();
        Bundle dialogArgs = new Bundle();
        dialogArgs.putInt(getString(R.string.msg_perfect), 1);
        dialogArgs.putInt(getString(R.string.level_current), level);
        dialog.setArguments(dialogArgs);
        dialog.setCancelable(false);
        dialog.show(getSupportFragmentManager(), "FinishedLevelPerfect");
    }

    /**
     * showFinishedLevelDialog
     */
    public void showFinishedLevelDialog() {
        // Create an instance of the dialog fragment and show it
        dialogToOpen = DIALOGS.FinishedLevel;
        DialogFragment dialog = FinishedLevel.newInstance(0, level);
		dialog.setCancelable(false);
        dialog.show(getSupportFragmentManager(), "FinishedLevel");
    }


    /**
     * showHelpDialog
     */
    public void showHelpDialog() {
        // Create an instance of the dialog fragment and show it
        dialogToOpen = DIALOGS.Help;
        DialogFragment dialog = new HelpDialog();
        Bundle dialogArgs = new Bundle();
        dialogArgs.putInt(getString(R.string.msg_perfect), 1);
        dialogArgs.putInt(getString(R.string.level_current), level);
        dialog.setArguments(dialogArgs);
        dialog.setCancelable(false);
        dialog.show(getSupportFragmentManager(), "HelpDialog");
    }

    /**
     * showAboutDialog
     */
    public void showAboutDialog() {
        // Create an instance of the dialog fragment and show it
        dialogToOpen = DIALOGS.About;
        DialogFragment dialog = AboutDialog.newInstance(1, level);
        // dialog.setCancelable(false);
        dialog.show(getSupportFragmentManager(), "AboutDialog");
    }


    /**
     * onDialogPositiveClick
     * Override
     * @param dialog {@link DialogFragment}
     */
    @Override
    public void onDialogPositiveClick(DialogFragment dialog) {
        int nextLvl = level;
        if (dialogToOpen == DIALOGS.About || dialogToOpen == DIALOGS.Help) {
            openHelpUrl();
        }
        else if (dialogToOpen == DIALOGS.GameOver ||
                dialogToOpen == DIALOGS.FinishedLevel ||
                dialogToOpen == DIALOGS.FinishedLevelPerfect) {

            if (level < 10) nextLvl++;
            String nxtLvlStr = getString(R.string.action_start) + " " +
                    getString(R.string.next_level) + " \"" + nextLvl + "\".";
            // showMessage(nxtLvlStr);
            startLevel(nextLvl, vHelp, null);
        }
        dialogToOpen = DIALOGS.None;
        dialog.dismiss();
    }

    /**
     * onDialogPositiveClick
     * Override
     * @param dialog {@link DialogFragment}
     */
    @Override
    public void onDialogNegativeClick(DialogFragment dialog) {
        // String replayLvlStr = getString(R.string.replay_current_level) + ": " + level + ".";
        // showMessage(replayLvlStr);
        if (dialogToOpen == DIALOGS.GameOver ||
                dialogToOpen == DIALOGS.FinishedLevel ||
                dialogToOpen == DIALOGS.FinishedLevelPerfect) {

            startLevel(level, vHelp, null);
        }
        dialogToOpen = DIALOGS.None;
        dialog.dismiss();
    }

    /**
     * onDialogPositiveClick
     * Override
     * @param dialog {@link DialogFragment}
     */
    @Override
    public void onCancel(DialogFragment dialog) {
        dialogToOpen = DIALOGS.None;
        dialog.dismiss();
    }

    //endregion

}
