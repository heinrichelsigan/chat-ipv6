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

package eu.georgtoth.supu.enums;

import java.io.Serializable;

import kotlin.NotImplementedError;

/**
 * Enum SUPUMODE XOR combined mode for (@link GAMEMODE) (@link AUTOMAT) (@link boolean) 
 */
public enum SUPUMODE implements Serializable {
    BotUp(0xDB00),
    BotUp_VHlp(0xDB01),
    BotUp_AutM(0xDB10),
    BotUp_AutM_VHlp(0xDB11),
    TopDwn(0xDD00),
    TopDwn_VHlp(0xDD01),
    TopDwn_AutM(0xDD10),
    TopDwn_AutM_VHlp(0xDD11),
    FreeS(0xDF00),
    FreeS_VHlp(0xDF01),
    FreeS_AutM(0xDF10),
    FreeS_AutM_VHlp(0xDF11);

	/**
	 * SUPUMODE Enum constructor
	 * NOTE: Enum constructor must have private or package scope. You can not use the public access modifier.
	 * @param value {@link long}
	 */
	SUPUMODE(long value) {
        this.value = value;
    }

	/**
	 * SUPUMODE Enum constructor
	 * @param direction {@link eu.georgtoth.supu.enums.GAMEMODE}
	 * @param auto {@link eu.georgtoth.supu.enums.AUTOMAT}
	 * @param visualHelp {@link boolean}
	 */
    SUPUMODE(GAMEMODE direction, AUTOMAT auto, boolean visualHelp) {
        SUPUMODE mode = SUPUMODE.setSupuMode(direction, auto, visualHelp);
		this.value = mode.getValue();
    }

    private final long value;

    /**
     * getValue
     * @return (@link long) value
     */
    public long getValue() { return value; }

	/**
	 * toInt
	 * @return int value {@link int}
	 */
	public int toInt() {
		return (int)this.getValue();
	}

    /**
     * toLong
     * @return long value {@link long}
     */
    public long toLong() {
        return this.getValue();
    }


	/**
     * getAutomation - gets AUTOMAT auto
     * @return {@link AUTOMAT} of SUPUMODE (@link SUPUMODE)
     */
    public AUTOMAT getAutomation() {
		long modeLong = (long)this.getValue();
		byte b = (byte)((modeLong) & 0xff);
		switch (b)
		{
			case (byte)0x10:
			case (byte)0x11:
			case (byte)0x1f:
				return AUTOMAT.ION;
			case (byte)0x00:
			case (byte)0x01:
			case (byte)0xff:
			default:
			return AUTOMAT.IOFF;
		}
    }
	
	/**
     * getDirection - gets GAMEMODE direction
     * @return {@link GAMEMODE} of SUPUMODE (@link SUPUMODE)
     */
    public GAMEMODE getDirection() {
		long modeLong = (long)this.getValue();
		byte b = (byte)((modeLong >> 8) & 0xff);
		switch (b)
		{
			case (byte)0xdb: return GAMEMODE.BottomUp;
			case (byte)0xdd: return GAMEMODE.TopDown;
			case (byte)0xdf: return GAMEMODE.FreeStyle;
			default:
				break;
		}
		return GAMEMODE.BottomUp;
    }
	
	/**
     * getVisualHelp - gets visual help
     * @return {@link boolean} true for visual help on, false for visual help off
     */
    public boolean getVisualHelp() {
		long modeLong = (long)this.getValue();
		byte b = (byte)((modeLong) & 0xff);
		switch (b)
		{
			case (byte)0x00:
			case (byte)0x10:
			case (byte)0xf0:
				return false;
			case (byte)0x01:
			case (byte)0x11:
			case (byte)0xf1:
				return true;
			default:
				break;
		}
		return false;
    }


    /**
     * getName
     * @return name {@link String} of SUPUMODE
     */
    public String getName() {
        return (this.toString());
    }

	/**
	 * getEnum - gets supu mode
	 * @param supuModeValue (@link int} SUPUMODE.getValue()
	 * @return the enum {@link eu.georgtoth.supu.enums.SUPUMODE}
	 */
	public static SUPUMODE getEnum(int supuModeValue) {
		switch (supuModeValue)
		{
			case 0xdbff: return SUPUMODE.BotUp;
			case 0xdbf1: return SUPUMODE.BotUp_VHlp;
			case 0xdb1f: return SUPUMODE.BotUp_AutM;
			case 0xfb11: return SUPUMODE.BotUp_AutM_VHlp;
			case 0xddff: return SUPUMODE.TopDwn;
			case 0xddf1: return SUPUMODE.TopDwn_VHlp;
			case 0xdd1f: return SUPUMODE.TopDwn_AutM;
			case 0xdd11: return SUPUMODE.TopDwn_AutM_VHlp;
			case 0xdfff: return SUPUMODE.FreeS;
			case 0xdff1: return SUPUMODE.FreeS_VHlp;
			case 0xdf1f: return SUPUMODE.FreeS_AutM;
			case 0xdf11: return SUPUMODE.FreeS_AutM_VHlp;
			default: break;
		}
		return SUPUMODE.BotUp;
	}
	
	/**
     * getSupuMode - gets SUPUMODE
     * @param supuModeValue (@link long} SUPUMODE.getValue()
     * @return {@link eu.georgtoth.supu.enums.SUPUMODE} for supu mode
     */
    public static SUPUMODE getSupuMode(long supuModeValue) {
        if (supuModeValue == (long)0xdb00)
			return SUPUMODE.BotUp;
		else if (supuModeValue == (long)0xdb01)
			return SUPUMODE.BotUp_VHlp;
		else if (supuModeValue == (long)0xdb10)
			return SUPUMODE.BotUp_AutM;
		else if (supuModeValue == (long)0xfb11)
			return SUPUMODE.BotUp_AutM_VHlp;
		else if (supuModeValue == (long)0xdd00)
			return SUPUMODE.TopDwn;
		else if (supuModeValue == (long)0xdd01)
			return SUPUMODE.TopDwn_VHlp;
		else if (supuModeValue == (long)0xdd10)
			return SUPUMODE.TopDwn_AutM;
		else if (supuModeValue == (long)0xdd11)
			return SUPUMODE.TopDwn_AutM_VHlp;
		else if (supuModeValue == (long)0xdf00)
			return SUPUMODE.FreeS;
		else if (supuModeValue == (long)0xdf01)
			return SUPUMODE.FreeS_VHlp;
		else if (supuModeValue == (long)0xdf10)
			return SUPUMODE.FreeS_AutM;
		else if (supuModeValue == (long)0xdf11)
			return SUPUMODE.FreeS_AutM_VHlp;

		return SUPUMODE.BotUp;
	}

	/**
	 * toogleVisualHelp - toogles visual help
	 * @param sMode long SUPUMODE.getValue()
	 * @return {@link eu.georgtoth.supu.enums.SUPUMODE}
	 */
	public static SUPUMODE toogleVisualHelp(long sMode) {
		if ((sMode % 2) == 0)
			return getSupuMode(sMode + 1);
		if ((sMode %2) == 1)
			return getSupuMode(sMode - 1);
		return getSupuMode(sMode);
	}

	/**
	 * setVisualHelp - set visual help
	 * @param mode {@link eu.georgtoth.supu.enums.SUPUMODE}
	 * @param vHelp boolean true if set visual help, otherwise if unset false
	 * @return {@link eu.georgtoth.supu.enums.SUPUMODE}
	 */
	public static SUPUMODE setVisualHelp(SUPUMODE mode, boolean vHelp) {
		long modeLong = (long)mode.getValue();
		if (vHelp && (modeLong % 2) == 0)
			return getSupuMode(modeLong + 1);
		if (!vHelp && (modeLong %2) == 1)
			return getSupuMode(modeLong - 1);
		return getSupuMode(modeLong);
	}

	/**
	 * setAutomation - sets automation
	 * @param mode {@link eu.georgtoth.supu.enums.SUPUMODE}
	 * @param auto {@link eu.georgtoth.supu.enums.AUTOMAT}
	 * @return {@link eu.georgtoth.supu.enums.SUPUMODE}
	 */
	public static SUPUMODE setAutomation(SUPUMODE mode, AUTOMAT auto) {
		long modeLong = (long)mode.getValue();
		byte b = (byte)((modeLong) & 0xff);
		switch (b)
		{
			case (byte)0x10:
			case (byte)0x11:
			case (byte)0x1f:
				if (auto == AUTOMAT.IOFF)
					modeLong -= 0x10;
				return getSupuMode(modeLong);
			case (byte)0x00:
			case (byte)0x01:
			case (byte)0xff:
				if (auto == AUTOMAT.ION)
					modeLong += 0x10;
				return getSupuMode(modeLong);
			default: // throw NotImplementedError("setAutomation default not implemented for " + modeLong + " " + automat.toString());
				break;
		}
		return getSupuMode(modeLong);
	}


	/**
	 * setDirection - sets direction
	 * @param mode {@link eu.georgtoth.supu.enums.SUPUMODE}
	 * @param direction {@link eu.georgtoth.supu.enums.GAMEMODE}
	 * @return {@link eu.georgtoth.supu.enums.SUPUMODE}
	 */
	public static SUPUMODE setDirection(SUPUMODE mode, GAMEMODE direction) {
		long modeLong = mode.getValue();
		long newMode = modeLong;
		byte b = (byte)((modeLong) & 0xff);
		switch (direction)
		{
			case BottomUp: 	newMode = (long)0xdb00 + (long)b; break;
			case TopDown: 	newMode = (long)0xdd00 + (long)b; break;
			case FreeStyle:
			default:		newMode = (long)0xdf00 + (long)b; break;
		}
		return getSupuMode(newMode);
	}


	/**
     * setSupuMode - sets supu mode
     * @param direction {@link eu.georgtoth.supu.enums.GAMEMODE}
	 * @param auto {@link eu.georgtoth.supu.enums.AUTOMAT}
	 * @param vHelp {@link boolean}
     * @return the enum {@link eu.georgtoth.supu.enums.SUPUMODE}
     */
    public static SUPUMODE setSupuMode(GAMEMODE direction, AUTOMAT auto, boolean vHelp) {
		long modeLong = (long)(direction.getValue());
		long autoLong = (long)(auto.getValue());
		long vHelpLong = (vHelp) ? (long)0x0001 : (long)0x0000;
		long supuModeLong = modeLong + autoLong + vHelpLong;
		SUPUMODE supuMode = SUPUMODE.getSupuMode(supuModeLong);
		return supuMode;
	}

}

