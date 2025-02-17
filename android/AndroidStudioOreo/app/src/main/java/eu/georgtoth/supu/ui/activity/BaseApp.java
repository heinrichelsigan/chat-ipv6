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

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.OutputStream;

import java.util.Date;
import java.util.Calendar;
import java.util.HashMap;
import java.util.HashSet;

import android.Manifest;
import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.res.AssetFileDescriptor;
import android.content.res.Resources;
import android.graphics.Bitmap;

import android.media.AudioAttributes;
import android.media.MediaPlayer;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.Looper;
import android.provider.MediaStore;
import android.view.Menu;
import android.view.MenuItem;
import android.view.SubMenu;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;

import com.google.gson.Gson;

import eu.georgtoth.supu.R;
import eu.georgtoth.supu.enums.SOUNDS;
import eu.georgtoth.supu.models.Game;

/**
 * BaseAppActivity extends AppCompatActivity
 * inherit all your Activity classes from that class
 * BaseAppActivity provides all lot of framework
 */
public class BaseApp extends AppCompatActivity {

    protected static final int READ_EXTERNAL_STORAGE_PERMISSION_CODE = 101;
    protected static final int WRITE_EXTERNAL_STORAGE_PERMISSION_CODE = 102;
    volatile boolean endRecursion = false;
    protected volatile boolean started = false;
    protected volatile int startedTimes = 0;
    protected volatile int errNum = 0;
    protected volatile int startedDrag = 0;
    protected volatile int finishedDrop = 0;
    // protected String tmp = "";

    protected Menu myMenu;
    protected HashMap<Integer, android.view.View> viewMap;
    protected android.view.View rootView = null;
    // protected android.widget.TextView mSoundName = new android.widget.TextView();
    protected volatile boolean atomicSoundLock = false;
    volatile Integer syncLock;
    protected volatile eu.georgtoth.supu.enums.SOUNDS sound2Play = SOUNDS.NONE;

    protected String[] supuSavedFiles;

    //region HandlerRunnable

    protected final static Handler playL8rHandler = new Handler(Looper.getMainLooper());

    // @SuppressLint("InlinedApi")
    /**
     * delayPlayCreditsMinus new Runnable() -> { playCreditsMinus(); }
     */
    protected final Runnable delayPlayCreditsMinus = () -> playCreditsMinus();

    // @SuppressLint("InlinedApi")
    /**
     * deplayPlayCreditsFull = new Runnable() -> { playCreditsMinus(); }
     */
    protected final Runnable deplayPlayCreditsFull = () -> playCreditsFull();

    // @SuppressLint("InlinedApi")
    /**
     * delayPlayGameOver = new Runnable() -> { playGameOver(); }
     */
    protected final Runnable delayPlayGameOver = () -> playGameOver();

    // @SuppressLint("InlinedApi")
    /**
     * delayPlayTakeStone = new Runnable() -> { playTakeStone(); }
     */
    protected final Runnable delayPlayTakeStone = () -> playTakeStone();

    // @SuppressLint("InlinedApi")
    /**
     * delayPlayScreenshot = new Runnable() -> { playSound(SOUNDS.MENU_CAMERA_CLICK); }
     */
    protected final Runnable delayPlayScreenshot = () -> playSound(SOUNDS.MENU_CAMERA_CLICK);

    /**
     * delayPlayGameOver = new Runnable() -> { playGameOver()(); }
     */
    protected final Runnable delayPlaySound = new Runnable() {
        @Override
        // @SuppressLint("InlinedApi")
        public void run() {
            if (sound2Play != SOUNDS.NONE) {
                playSound(sound2Play);
                syncLock = Integer.valueOf(sound2Play.getValue());
                synchronized (syncLock) {
                    sound2Play = SOUNDS.NONE;
                }
            }
        }
    };

    //endregion

    /**
     * onCreate
     * @param savedInstanceState - Bundle savedInstanceState
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);

        rootView = getWindow().getDecorView().getRootView();
        rootView.setDrawingCacheEnabled(false);
    }

    //region MenuMembers

    /**
     * setSubMenuLoad
     * @param menu mainMenu
     */
    public void setSubMenuLoad(Menu menu) {
        if (menu == null)
            return;

        SubMenu subMenuLoad = null;
        MenuItem slotLoadItem = null;
        int rMenuLoadId = -1;
        // String packName = getApplicationContext().getPackageName();
        supuSavedFiles = getSavedGames();


        if (menu.findItem(R.id.action_load) != null) {
            subMenuLoad = menu.findItem(R.id.action_load).getSubMenu();
        }

        if (subMenuLoad != null) {

            try {
                for (int ssfix = 0; ssfix < supuSavedFiles.length && ssfix < 7; ssfix++) {
                    if ((rMenuLoadId = getApplicationContext().getResources().getIdentifier(
                            "load_slot" + ssfix, "id", getApplicationContext().getPackageName())) > -1) {

                        if (((slotLoadItem = subMenuLoad.findItem(rMenuLoadId)) != null) &&
                                (slotLoadItem.getItemId() > -1) && supuSavedFiles.length > ssfix) {
                            if (!supuSavedFiles[ssfix].isEmpty()) {
                                slotLoadItem.setEnabled(true);
                                slotLoadItem.setTitle(supuSavedFiles[ssfix]);
                            }
                        }
                    }
                }
            } catch (Exception slotLoadExc) {
                slotLoadExc.printStackTrace();
            }

            try {

                if (((slotLoadItem = subMenuLoad.findItem(R.id.load_slot0)) != null) &&
                        (slotLoadItem.getItemId() > -1) && supuSavedFiles.length > 0) {
                    if (!supuSavedFiles[0].isEmpty()) {
                        slotLoadItem.setEnabled(true);
                        slotLoadItem.setTitle(supuSavedFiles[0]);
                    }
                }
                if (((slotLoadItem = subMenuLoad.findItem(R.id.load_slot1)) != null) &&
                        (slotLoadItem.getItemId() > -1) && supuSavedFiles.length > 1) {
                    if (!supuSavedFiles[1].isEmpty()) {
                        slotLoadItem.setEnabled(true);
                        slotLoadItem.setTitle(supuSavedFiles[1]);
                    }
                }
                if (((slotLoadItem = subMenuLoad.findItem(R.id.load_slot2)) != null) &&
                        (slotLoadItem.getItemId() > -1) && supuSavedFiles.length > 2) {
                    if (!supuSavedFiles[2].isEmpty()) {
                        slotLoadItem.setEnabled(true);
                        slotLoadItem.setTitle(supuSavedFiles[2]);
                    }
                }
                if (((slotLoadItem = subMenuLoad.findItem(R.id.load_slot2)) != null) &&
                        (slotLoadItem.getItemId() > -1) && supuSavedFiles.length > 3) {
                    if (!supuSavedFiles[3].isEmpty()) {
                        slotLoadItem.setEnabled(true);
                        slotLoadItem.setTitle(supuSavedFiles[3]);
                    }
                }
                if (((slotLoadItem = subMenuLoad.findItem(R.id.load_slot1)) != null) &&
                        (slotLoadItem.getItemId() > -1) && supuSavedFiles.length > 4) {
                    if (!supuSavedFiles[4].isEmpty()) {
                        slotLoadItem.setEnabled(true);
                        slotLoadItem.setTitle(supuSavedFiles[4]);
                    }
                }
                if (((slotLoadItem = subMenuLoad.findItem(R.id.load_slot2)) != null) &&
                        (slotLoadItem.getItemId() > -1) && supuSavedFiles.length > 5) {
                    if (!supuSavedFiles[5].isEmpty()) {
                        slotLoadItem.setEnabled(true);
                        slotLoadItem.setTitle(supuSavedFiles[5]);
                    }
                }
                if (((slotLoadItem = subMenuLoad.findItem(R.id.load_slot1)) != null) &&
                        (slotLoadItem.getItemId() > -1) && supuSavedFiles.length > 6) {
                    if (!supuSavedFiles[6].isEmpty()) {
                        slotLoadItem.setEnabled(true);
                        slotLoadItem.setTitle(supuSavedFiles[6]);
                    }
                }
            } catch (Exception slotLoadEx) {
                slotLoadEx.printStackTrace();
            }
        }
    }

    /**
     * onCreateOptionsMenu 
     * @param menu - the menu item, that has been selected
	 * @return true, if menu successfully created, otherwise false
     */
    public boolean onCreateOptionsMenu(Menu menu) {
        myMenu = menu;
        int menuId = -1;

        try {
            menuId = getApplicationContext().getResources().getIdentifier(
                    "menu_main",
                    "menu",
                    getApplicationContext().getPackageName());

            if (menuId >= 0) {
                getMenuInflater().inflate(menuId, menu);
                return true;
            }
        } catch (Exception menuEx) {
            showException(menuEx);
        }

        return false;
    }

    /**
     * onOptionsItemSelected
     * @param  item - the menu item, that has been selected
	 * @return false to allow normal menu processing to proceed, true to consume it here.
     */
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int  mItemId = (item != null) ?  item.getItemId() : -1;
		if (mItemId >= 0) {
			boolean consumedNoFwd = actionMenuItem(mItemId, item, myMenu);
			if (consumedNoFwd) 
				return true;
        }
	
        return super.onOptionsItemSelected(item);
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
		
		// we fall through by default
		return false;
	}

    //endregion

    /**
     * showAbout() starts about activity
     */
    public void showAbout() {
        // try {
        //     Thread.currentThread().sleep(10);
        // } catch (Exception exInt) {
        //     errHandler(exInt);
        // }
        // tDbg.setText(R.string.help_text);
        Intent intent = new Intent(this, AboutActivity.class);
        startActivity(intent);
    }

    /**
     * showHelp() prints out help text
     */
    public void showHelp() {
        Intent intent = new Intent(this, HelpActivity.class);
        startActivity(intent);
    }

    /**
     * onCameraClick
     *
     * @param view2Bmp view2Bmp
     */
    public void onCameraClick(View view2Bmp) {
        // if (view2Bmp == null)
        view2Bmp = getWindow().getDecorView().getRootView();
        view2Bmp.buildDrawingCache(false);

        String path = Environment.getExternalStorageDirectory().toString();
        // path = Environment.getStorageDirectory().toString();
        path = getApplicationContext().getExternalFilesDir(null).getAbsolutePath();

        Date currentTime = Calendar.getInstance().getTime();
        String datePartStr =  (Calendar.getInstance().get(Calendar.MONTH) < 10) ?
                "0" : "" + Calendar.getInstance().get(Calendar.MONTH);
        String saveName = Calendar.getInstance().get(Calendar.YEAR) + "-" + datePartStr;
        datePartStr =  (Calendar.getInstance().get(Calendar.DAY_OF_MONTH) < 10) ?
                "0" : "" + Calendar.getInstance().get(Calendar.DAY_OF_MONTH);
        saveName = saveName + datePartStr + "_supu_" + currentTime.getTime() + ".jpg";
        OutputStream fileOutStream = null;
        File file = new File(path, saveName);

        try {
            fileOutStream = new FileOutputStream(file);
            Bitmap pictureBitmap = view2Bmp.getDrawingCache(false); // view2Bmp.getDrawingCache(true);
            pictureBitmap.compress(Bitmap.CompressFormat.JPEG, 92, fileOutStream);
            fileOutStream.flush();
            fileOutStream.close();

            MediaStore.Images.Media.insertImage(getContentResolver(), file.getAbsolutePath(), file.getName(), file.getName());

        } catch (Exception saveExc) {
            showError(saveExc, true);
            saveExc.printStackTrace();
            if (fileOutStream != null) {
                try {
                    fileOutStream.close();
                } catch (Exception closeEx) {
                    closeEx.printStackTrace();
                }
            }
        }
        finally {
            fileOutStream = null;
        }

        try {
            view2Bmp.destroyDrawingCache();
        } catch (Exception destroyCacheExc) {
            destroyCacheExc.printStackTrace();
        }

        playL8rHandler.postDelayed(delayPlayScreenshot, 100);
    }



    /**
     * saveGame - saves current supu game to external storage
     *              requests permission to write access external storage,
     *              if the SUPU app doesn't have that permission until now!
     *
     * @param aGame Game
     */
    public void saveGame(Game aGame) {
        if (aGame == null) {
            showMessage("aGame is empty!");
            return;
        }
        //check for permission
        if (ContextCompat.checkSelfPermission(this,
                Manifest.permission.WRITE_EXTERNAL_STORAGE) == PackageManager.PERMISSION_DENIED){
            //ask for permission
            requestPermissions(new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, WRITE_EXTERNAL_STORAGE_PERMISSION_CODE);
        }

        String fullSaveFileName = getFullSavePathName();
        File outFile = new File(fullSaveFileName);

        OutputStream fsOut = null;
        ObjectOutputStream objOut = null;
        Gson gson = new Gson();
        String jsonStr = "";
        try {
            jsonStr = gson.toJson(aGame, Game.class);
        } catch (Exception exSer) {
            showError(exSer, true);
            exSer.printStackTrace();
            jsonStr = aGame.toString();
        }

        try {
            fsOut = new FileOutputStream(outFile);
            // objOut = new ObjectOutputStream(fileOut);
            // objOut.writeObject(aGame);
            // objOut.flush();
            // objOut.close();
            fsOut.write(jsonStr.getBytes());
            fsOut.flush();
            fsOut.close();
            setSubMenuLoad(myMenu);
        } catch (Exception saveExc) {
            showError(saveExc, true);
            saveExc.printStackTrace();
            if (fsOut != null) {
                try {
                    fsOut.close();
                } catch (Exception closeEx) {
                    closeEx.printStackTrace();
                }
            }
        }
        finally {
            fsOut = null;
            objOut = null;
        }

        playL8rHandler.postDelayed(deplayPlayCreditsFull, 100);
    }

    /**
     * loadGame -   loads a saved supu game from external storage into app
     *      *              requests permission to read access external storage,
     *      *              if the SUPU app doesn't have that permission until now!
     *
     * @param loadFileName String
     */
    public Game loadGame(String loadFileName) {

        Game loadedGame = null;

        if (loadFileName == null || loadFileName.isEmpty()) {
            showMessage("fileName string is empty!");
            return loadedGame;
        }
        //check for permission
        if (ContextCompat.checkSelfPermission(this,
                Manifest.permission.READ_EXTERNAL_STORAGE) == PackageManager.PERMISSION_DENIED){
            //ask for permission
            requestPermissions(new String[]{Manifest.permission.READ_EXTERNAL_STORAGE}, READ_EXTERNAL_STORAGE_PERMISSION_CODE);
        }

        String jsonSerializedGame = "";
        String basePath = Environment.getExternalStorageDirectory().toString();
        // path = Environment.getStorageDirectory().toString();
        basePath = getApplicationContext().getExternalFilesDir(null).getAbsolutePath();
        String dirPath = getFullSavePath();

        File file = new File(dirPath, loadFileName);
        FileInputStream fileIn = null;
        ObjectInputStream objIn = null;

        try {
            long fileLen = file.length();
            fileIn = new FileInputStream(file);
            byte[] bytes = new byte[(int)fileLen];
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
                bytes = fileIn.readAllBytes();
            } else {
                // bytes = fileIn.readNBytes((int)fileLen);
                int rb = fileIn.read(bytes, 0, (int) fileLen);
                if (rb <= 0) {
                    throw new IOException("Error on InputStream read on file: " + file);
                }
            }
            // objIn = new ObjectInputStream(fileIn);
            jsonSerializedGame = new String(bytes);
            // loadedGame = (Game) objIn.readObject();
            // objIn.close();
            fileIn.close();
        } catch (Exception loadGameExc) {
            showError(loadGameExc, true);
            loadGameExc.printStackTrace();

            if (fileIn != null) {
                try {
                    fileIn.close();
                } catch (Exception closeEx) {
                    closeEx.printStackTrace();
                }
            }
        }
        finally {
            objIn = null;
            fileIn = null;
        }

        Gson gson = new Gson();
        loadedGame = (Game)(new Gson()).fromJson(jsonSerializedGame, Game.class);

        playL8rHandler.postDelayed(deplayPlayCreditsFull, 100);

        return loadedGame;
    }


    /**
     * getSavedGames
     *
     * @return String[]
     */
    protected String[] getSavedGames() {

        HashSet<String> supuFiles = new HashSet<>();

        //check for permission
        if (ContextCompat.checkSelfPermission(this,
                Manifest.permission.READ_EXTERNAL_STORAGE) == PackageManager.PERMISSION_DENIED) {
            //ask for permission
            requestPermissions(new String[]{Manifest.permission.READ_EXTERNAL_STORAGE}, READ_EXTERNAL_STORAGE_PERMISSION_CODE);
        }

        String path = Environment.getExternalStorageDirectory().toString();
        // path = Environment.getStorageDirectory().toString();
        path = getApplicationContext().getExternalFilesDir(null).getAbsolutePath();
        // getApplicationContext().getFilesDir()
        String dirPath = getFullSavePath();

        int len = 0;
        File directory = new File(dirPath);
        File[] files = directory.listFiles();
        if (directory.canRead() && files != null) {
            for (File file : files) {
                if (file.getName().endsWith("_supu.json")) {
                    len++;
                    supuFiles.add(file.getName());
                }
            }
        }

        supuSavedFiles = new String[len];

        return (String[])supuFiles.toArray(new String[len]);
    }


    /**
     * getFullSavePath
     * @return full absolute path to saving directory
     */
    protected String getFullSavePath() {
        String basePath = Environment.getExternalStorageDirectory().toString();
        // path = Environment.getStorageDirectory().toString();
        basePath = getApplicationContext().getExternalFilesDir(null).getAbsolutePath();
        String dirPath = basePath;

        File outDir = new File(basePath);
        File[] files = outDir.listFiles();
        if (outDir.canRead() && files != null) {
            for (File file : files) {
                if (file.isDirectory() &&
                        (file.getName().toLowerCase().contains("download") ||
                                file.getName().toLowerCase().contains("supu"))) {
                    dirPath = file.getAbsolutePath();
                    break;
                }
            }
        }
        outDir = new File(dirPath);
        files = outDir.listFiles();
        if (outDir.canRead() && files != null) {
            for (File file : files) {
                if (file.isDirectory() && file.getName().toLowerCase().contains("supu")) {
                    dirPath = file.getAbsolutePath();
                    break;
                }
            }
        }

        return dirPath;
    }

    /**
     * getFullSavePathName
     * @return full absolute path to saving directory + filename
     */
    protected String getFullSavePathName() {
        String saveName = "supu.json";
        String basePath = Environment.getExternalStorageDirectory().toString();
        // path = Environment.getStorageDirectory().toString();
        basePath = getApplicationContext().getExternalFilesDir(null).getAbsolutePath();
        String dirPath = getFullSavePath();
        String fullSavePathName = String.join(basePath, saveName);

        Date currentTime = Calendar.getInstance().getTime();
        int yearInt = Calendar.getInstance().get(Calendar.YEAR);
        int monthInt = Calendar.getInstance().get(Calendar.MONTH) + 1;
        int dayInt = Calendar.getInstance().get(Calendar.DAY_OF_MONTH);
        int hourInt = Calendar.getInstance().get(Calendar.HOUR_OF_DAY);
        int minuteInt = Calendar.getInstance().get(Calendar.MINUTE);

        String yearStr = String.valueOf(yearInt);
        String monthStr =  ((monthInt < 10) ? "0" : "") + monthInt;
        String dayStr =  ((dayInt < 10) ? "0" : "") +  String.valueOf(dayInt);

        String hourStr = ((hourInt < 10) ? "0" : "") + hourInt;
        String minStr =  ((minuteInt < 10) ? "0" : "") + minuteInt;
        String datePartStr = yearStr + monthStr + dayStr + "-" + hourStr + minStr;

        saveName =  datePartStr + "_supu.json";

        fullSavePathName = ((!dirPath.endsWith("/") ? dirPath + "/" : dirPath) + saveName);

        return fullSavePathName;
    }

    /**
     * openHelpUrl
     * opens help url
     */
    public void openHelpUrl() {
        Intent intent = new Intent();
        intent.setAction(Intent.ACTION_VIEW);
        String help_uri = getString(R.string.help_uri);
        intent.setData(android.net.Uri.parse(help_uri));
        startActivity(intent);
    }

    /**
     * exitGame() exit game
     */
    public void exitGame() {
        finishAffinity();
        System.exit(0);
    }

    //region MsgExceptions
    /**
     * showMessage shows a new Toast dynamic message
     * @param text to display
     * @param tooShort if set to yes, message inside toast widget appears only very shortly 
     */
    public void showMessage(CharSequence text, boolean tooShort) {
        if (text != null && text != "") {
            Context context = getApplicationContext();
            Toast toast = Toast.makeText(context, text,
                    tooShort ? Toast.LENGTH_SHORT : Toast.LENGTH_LONG);
            toast.show();
        }
    }
	
	/**
     * showMessage shows a new Toast dynamic message
     * @param text to display
     */
    public void showMessage(CharSequence text) { showMessage(text, false); }

    /**
     * showError simple dummy error handler
     * @param myErr java.lang.Throwable
     * @param showMessage triggers, that a Toast Widget shows the current Error / Exception 	 
     */
    public void showError(java.lang.Throwable myErr, boolean showMessage) {
        if (myErr != null) {
            synchronized(this) {
                ++errNum;
            }
            CharSequence text = "CRITICAL ERROR #" + (errNum) + " " + myErr.getMessage() + "\nMessage: " + myErr.getLocalizedMessage() + "\n";
            if (showMessage)
                showMessage(text);
            myErr.printStackTrace();
        }
    }

    /**
     * showError simple dummy error handler
     * @param myEx - tje exceütion, that has been thrown
     */
    public void showException(java.lang.Exception myEx) {
        showError(myEx, true);
    }

    //endregion

    //region PlaySounds
    /**
     * playMediaFromUri plays any sound media from an internet uri
     * @param url - the full quaöofoed url accessor
     */
    public void playMediaFromUri(String url) {
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
    }

     /**
      * Play sound file stored in res/raw/ directory
      * @param rawName - resource name or resource id
      *   Name
      *     Syntax  :  android.resource://[package]/[res type]/[res name]
      *     Example : @<code>Uri.parse("android.resource://com.my.package/raw/sound1");</code>
      *   Resource id
      *     Syntax  : android.resource://[package]/[resource_id]
      *     Example : @<code>Uri.parse("android.resource://com.my.package/" + R.raw.sound1); </code>
      *
     */
     public void playRawSound(int rId, String rawName) {
        try {
            Resources res = getResources();
            int resId = rId;
            if (rawName != null) {
                resId = getSoundRId(rawName);
            }

            if (resId != rId) {
                String RESOURCE_PATH = ContentResolver.SCHEME_ANDROID_RESOURCE + "://";
                String path = RESOURCE_PATH + getPackageName() + File.separator + resId;
                Uri soundUri = Uri.parse(path);
                showMessage("RawSound: Uri=" + soundUri.toString() + " path=" + path);
            }

            final MediaPlayer mMediaPlayer = new MediaPlayer();
            mMediaPlayer.setVolume(1.0f, 1.0f);
            mMediaPlayer.setLooping(false);
            mMediaPlayer.setOnPreparedListener(mp -> {
                // Toast.makeText(getApplicationContext(),
                //         "start playing sound", Toast.LENGTH_SHORT).show();
                mMediaPlayer.start();
            });
            mMediaPlayer.setOnErrorListener((mp, what, extra) -> {
                // Toast.makeText(getApplicationContext(), String.format(Locale.US,
                //         "Media error what=%d extra=%d", what, extra), Toast.LENGTH_LONG).show();
                return false;
            });


            // 2. Load using content provider, passing file descriptor.
            ContentResolver resolver = getContentResolver();
            AssetFileDescriptor fd = res.openRawResourceFd(resId);
            mMediaPlayer.setDataSource(fd.getFileDescriptor(), fd.getStartOffset(), fd.getLength());
            fd.close();
            mMediaPlayer.prepareAsync();

            // See setOnPreparedListener above
            mMediaPlayer.start();

        } catch (Exception ex) {
            // showException(ex);
            showMessage(String.join("MediaPlayer: " , ex.getMessage()));
            Toast.makeText(this, ex.getMessage(), Toast.LENGTH_LONG).show();
        }
    }

    /**
     * playSound
     *  plays a sound
     * @param sounds - enum SOUNDS unique identifier for sound
     */
    public void playSound(SOUNDS sounds) {
         playRawSound(sounds.getRId(), sounds.getName());
    }

    /**
     * getSoundRId
     * @param rawSoundName - raw sound name
     * @return getRessources.getIdentifier(rawSoundName, ...):
     */
    public int getSoundRId(String rawSoundName) {
        // Build path using resource number
        int rcId = getResources().getIdentifier(rawSoundName, "raw", getPackageName());
        return rcId;
    }

    /**
     * playSound
     *  plays a sound
     * @param rawSoundName - resource name
     *      Syntax  :  android.resource://[package]/[res type]/[res name]
     *      Example : @<code>Uri.parse("android.resource://com.my.package/raw/sound1");</code>
     */
    public void playSound(String rawSoundName) {
        int resID = getSoundRId(rawSoundName);
        playRawSound(resID, rawSoundName);
    }

    public void playTakeStone() { playTakeStone(true); }

    public void playTakeStone(boolean fromRawOrInetUrl) {
         if (fromRawOrInetUrl)
             playSound(SOUNDS.STONE_TAKE);
         else
            playMediaFromUri("https://github.com/heinrichelsigan/Archon/blob/master/app/src/main/res/raw/takestone.wav?raw=true");
    }

    public void playCreditsMinus() {
        playSound(SOUNDS.CREDITS_MINUS);
    }

    public void playCreditsFull() {
        playSound(SOUNDS.CREDITS_FULL_AGAIN);
    }

    public void playDropStone() { playDropStone(true); }

    public void playDropStone(boolean fromRawOrInetUrl) {
        if (fromRawOrInetUrl)
            playSound(SOUNDS.STONE_DROP);
        else
            playMediaFromUri("https://github.com/heinrichelsigan/Archon/blob/master/app/src/main/res/raw/dropstone.wav?raw=true");
    }

    public void playRowCompleted() {
        playSound(SOUNDS.ROW_COMPLETED);
    }

    public void playLevelCompleted() {
        playSound(SOUNDS.LEVEL_COMPLETED);
    }

    public void playLevelPerfect() {
        playSound(SOUNDS.LEVEL_PERFECT);
    }

    public void playErrorDoubleColor() {
        playSound(SOUNDS.ERROR_DOUBLE_COLOR);
    }

    public void playErrorForbiddenPattern() {
        playSound(SOUNDS.ERROR_FORBIDDEN_PATTERN);
    }

    public void playGameOver() {
        playSound(SOUNDS.GAME_OVER);
    }

    public void playSupuMaster() {
        playSound(SOUNDS.LEVEL_SUPUMASTER);
    }

    //endregion
}