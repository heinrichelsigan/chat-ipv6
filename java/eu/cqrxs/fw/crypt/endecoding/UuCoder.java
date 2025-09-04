package cqrxs.eu.fw.crypt.endecoding; 

import java.util.*;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.PrintStream;
import java.io.IOException;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.nio.ByteBuffer;
import java.io.PushbackInputStream;


public class UuCoder extends EnDeCoder {

    public UuCoder() {
    }

    public byte[] decodeString(String uuEncString) {
        UUDecoder uud = new UUDecoder();
        /*
            uud.mode = 664;
            uud.bufferName = "aString";
        */
        byte[] decodedBytes = new byte[0];
        try {
            decodedBytes = uud.decodeBuffer(uuEncString);
        } catch (IOException ioEx) {
        }
        return decodedBytes;
    }

    public String encodeBytes(byte[] inBytes) {
        UUEncoder uue = new UUEncoder();
        String uuEncString = "";
        try {
            uuEncString = uue.encode(inBytes);
        } catch (Exception exUue) {
        }
        return uuEncString;
    }
    
    public String decode(String uuEncString) { 
        UUDecoder uud = new UUDecoder();
        /* 
            uud.mode = 664; 
            uud.bufferName = "aString"; 
        */ 
        byte[] uuDecBytes = new byte[0];
        try {
            uuDecBytes = uud.decodeBuffer(uuEncString);
        } catch (IOException ioUuDexEx) {
        }
        String uuDecString = new String(uuDecBytes);
        
        return uuDecString;
    }

    public String encode(String inString) {
        byte[] inBytes = inString.getBytes(StandardCharsets.UTF_8);
        UUEncoder uue = new UUEncoder(); 
        String uuEncString = uue.encode(inBytes); 
        return uuEncString;
    }

/*
 * Copyright (c) 1995, 2004, Oracle and/or its affiliates. All rights reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.  Oracle designates this
 * particular file as subject to the "Classpath" exception as provided
 * by Oracle in the LICENSE file that accompanied this code.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
 * or visit www.oracle.com if you need additional information or have any
 * questions.
 */
 

/**
 * This class implements a Berkeley uu character encoder. This encoder
 * was made famous by uuencode program.
 *
 * The basic character coding is algorithmic, taking 6 bits of binary
 * data and adding it to an ASCII ' ' (space) character. This converts
 * these six bits into a printable representation. Note that it depends
 * on the ASCII character encoding standard for english. Groups of three
 * bytes are converted into 4 characters by treating the three bytes
 * a four 6 bit groups, group 1 is byte 1's most significant six bits,
 * group 2 is byte 1's least significant two bits plus byte 2's four
 * most significant bits. etc.
 *
 * In this encoding, the buffer prefix is:
 * <pre>
 *     begin [mode] [filename]
 * </pre>
 *
 * This is followed by one or more lines of the form:
 * <pre>
 *      (len)(data)(data)(data) ...
 * </pre>
 * where (len) is the number of bytes on this line. Note that groupings
 * are always four characters, even if length is not a multiple of three
 * bytes. When less than three characters are encoded, the values of the
 * last remaining bytes is undefined and should be ignored.
 *
 * The last line of data in a uuencoded file is represented by a single
 * space character. This is translated by the decoding engine to a line
 * length of zero. This is immediately followed by a line which contains
 * the word 'end[newline]'
 *
 * @author      Chuck McManis
 * @see         CharacterEncoder
 * @see         UUDecoder
 */
class UUEncoder extends CharacterEncoder {

    /**
     * This name is stored in the begin line.
     */
    private String bufferName;

    /**
     * Represents UNIX(tm) mode bits. Generally three octal digits representing
     * read, write, and execute permission of the owner, group owner, and
     * others. They should be interpreted as the bit groups:
     * (owner) (group) (others)
     *  rwx      rwx     rwx    (r = read, w = write, x = execute)
     *
     * By default these are set to 644 (UNIX rw-r--r-- permissions).
     */
    private int mode;


    /**
     * Default - buffer begin line will be:
     * <pre>
     *  begin 644 encoder.buf
     * </pre>
     */
    public UUEncoder() {
        bufferName = "encoder.buf";
        mode = 644;
    }

    /**
     * Specifies a name for the encoded buffer, begin line will be:
     * <pre>
     *  begin 644 [FNAME]
     * </pre>
     */
    public UUEncoder(String fname) {
        bufferName = fname;
        mode = 644;
    }

    /**
     * Specifies a name and mode for the encoded buffer, begin line will be:
     * <pre>
     *  begin [MODE] [FNAME]
     * </pre>
     */
    public UUEncoder(String fname, int newMode) {
        bufferName = fname;
        mode = newMode;
    }

    /** number of bytes per atom in uuencoding is 3 */
    protected int bytesPerAtom() {
        return (3);
    }

    /** number of bytes per line in uuencoding is 45 */
    protected int bytesPerLine() {
        return (45);
    }

    /**
     * encodeAtom - take three bytes and encodes them into 4 characters
     * If len is less than 3 then remaining bytes are filled with '1'.
     * This insures that the last line won't end in spaces and potentiallly
     * be truncated.
     */
    protected void encodeAtom(OutputStream outStream, byte data[], int offset, int len)
        throws IOException {
        byte    a, b = 1, c = 1;
        int     c1, c2, c3, c4;

        a = data[offset];
        if (len > 1) {
            b = data[offset+1];
        }
        if (len > 2) {
            c = data[offset+2];
        }

        c1 = (a >>> 2) & 0x3f;
        c2 = ((a << 4) & 0x30) | ((b >>> 4) & 0xf);
        c3 = ((b << 2) & 0x3c) | ((c >>> 6) & 0x3);
        c4 = c & 0x3f;
        outStream.write(c1 + ' ');
        outStream.write(c2 + ' ');
        outStream.write(c3 + ' ');
        outStream.write(c4 + ' ');
        return;
    }

    /**
     * Encode the line prefix which consists of the single character. The
     * lenght is added to the value of ' ' (32 decimal) and printed.
     */
    protected void encodeLinePrefix(OutputStream outStream, int length)
        throws IOException {
        outStream.write((length & 0x3f) + ' ');
    }


    /**
     * The line suffix for uuencoded files is simply a new line.
     */
    protected void encodeLineSuffix(OutputStream outStream) throws IOException {
        pStream.println();
    }

    /**
     * encodeBufferPrefix writes the begin line to the output stream.
     */
    protected void encodeBufferPrefix(OutputStream a) throws IOException {
        super.pStream = new PrintStream(a);
        super.pStream.print("begin "+mode+" ");
        if (bufferName != null) {
            super.pStream.println(bufferName);
        } else {
            super.pStream.println("encoder.bin");
        }
        super.pStream.flush();
    }

    /**
     * encodeBufferSuffix writes the single line containing space (' ') and
     * the line containing the word 'end' to the output stream.
     */
    protected void encodeBufferSuffix(OutputStream a) throws IOException {
        super.pStream.println(" \nend");
        super.pStream.flush();
    }

}

/*
 * Copyright (c) 1995, 2001, Oracle and/or its affiliates. All rights reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.  Oracle designates this
 * particular file as subject to the "Classpath" exception as provided
 * by Oracle in the LICENSE file that accompanied this code.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
 * or visit www.oracle.com if you need additional information or have any
 * questions.
 */

/**
 * This class implements a Berkeley uu character decoder. This decoder
 * was made famous by the uudecode program.
 *
 * The basic character coding is algorithmic, taking 6 bits of binary
 * data and adding it to an ASCII ' ' (space) character. This converts
 * these six bits into a printable representation. Note that it depends
 * on the ASCII character encoding standard for english. Groups of three
 * bytes are converted into 4 characters by treating the three bytes
 * a four 6 bit groups, group 1 is byte 1's most significant six bits,
 * group 2 is byte 1's least significant two bits plus byte 2's four
 * most significant bits. etc.
 *
 * In this encoding, the buffer prefix is:
 * <pre>
 *     begin [mode] [filename]
 * </pre>
 *
 * This is followed by one or more lines of the form:
 * <pre>
 *      (len)(data)(data)(data) ...
 * </pre>
 * where (len) is the number of bytes on this line. Note that groupings
 * are always four characters, even if length is not a multiple of three
 * bytes. When less than three characters are encoded, the values of the
 * last remaining bytes is undefined and should be ignored.
 *
 * The last line of data in a uuencoded buffer is represented by a single
 * space character. This is translated by the decoding engine to a line
 * length of zero. This is immediately followed by a line which contains
 * the word 'end[newline]'
 *
 * If an error is encountered during decoding this class throws a
 * CEFormatException. The specific detail messages are:
 *
 * <pre>
 *      "UUDecoder: No begin line."
 *      "UUDecoder: Malformed begin line."
 *      "UUDecoder: Short Buffer."
 *      "UUDecoder: Bad Line Length."
 *      "UUDecoder: Missing 'end' line."
 * </pre>
 *
 * @author      Chuck McManis
 * @see         CharacterDecoder
 * @see         UUEncoder
 */
class UUDecoder extends CharacterDecoder {
    
    /**
     * This string contains the name that was in the buffer being decoded.
     */
    public String bufferName;

    /**
     * Represents UNIX(tm) mode bits. Generally three octal digits
     * representing read, write, and execute permission of the owner,
     * group owner, and  others. They should be interpreted as the bit groups:
     * <pre>
     * (owner) (group) (others)
     *  rwx      rwx     rwx    (r = read, w = write, x = execute)
     *</pre>
     *
     */
    public int mode;


    /**
     * UU encoding specifies 3 bytes per atom.
     */
    protected int bytesPerAtom() {
        return (3);
    }

    /**
     * All UU lines have 45 bytes on them, for line length of 15*4+1 or 61
     * characters per line.
     */
    protected int bytesPerLine() {
        return (45);
    }

    /** This is used to decode the atoms */
    private byte decoderBuffer[] = new byte[4];

    /**
     * Decode a UU atom. Note that if l is less than 3 we don't write
     * the extra bits, however the encoder always encodes 4 character
     * groups even when they are not needed.
     */
    protected void decodeAtom(PushbackInputStream inStream, OutputStream outStream, int l)
        throws IOException {
        int i, c1, c2, c3, c4;
        int a, b, c;
        StringBuffer x = new StringBuffer();

        for (i = 0; i < 4; i++) {
            c1 = inStream.read();
            if (c1 == -1) {
                throw new CEStreamExhausted();
            }
            x.append((char)c1);
            decoderBuffer[i] = (byte) ((c1 - ' ') & 0x3f);
        }
        a = ((decoderBuffer[0] << 2) & 0xfc) | ((decoderBuffer[1] >>> 4) & 3);
        b = ((decoderBuffer[1] << 4) & 0xf0) | ((decoderBuffer[2] >>> 2) & 0xf);
        c = ((decoderBuffer[2] << 6) & 0xc0) | (decoderBuffer[3] & 0x3f);
        outStream.write((byte)(a & 0xff));
        if (l > 1) {
            outStream.write((byte)( b & 0xff));
        }
        if (l > 2) {
            outStream.write((byte)(c&0xff));
        }
    }

    /**
     * For uuencoded buffers, the data begins with a line of the form:
     *          begin MODE FILENAME
     * This line always starts in column 1.
     */
    protected void decodeBufferPrefix(PushbackInputStream inStream, OutputStream outStream) throws IOException {
        int     c;
        StringBuffer q = new StringBuffer(32);
        String r;
        boolean sawNewLine;

        /*
         * This works by ripping through the buffer until it finds a 'begin'
         * line or the end of the buffer.
         */
        sawNewLine = true;
        while (true) {
            c = inStream.read();
            if (c == -1) {
                throw new CEFormatException("UUDecoder: No begin line.");
            }
            if ((c == 'b')  && sawNewLine){
                c = inStream.read();
                if (c == 'e') {
                    break;
                }
            }
            sawNewLine = (c == '\n') || (c == '\r');
        }

        /*
         * Now we think its begin, (we've seen ^be) so verify it here.
         */
        while ((c != '\n') && (c != '\r')) {
            c = inStream.read();
            if (c == -1) {
                throw new CEFormatException("UUDecoder: No begin line.");
            }
            if ((c != '\n') && (c != '\r')) {
                q.append((char)c);
            }
        }
        r = q.toString();
        if (r.indexOf(' ') != 3) {
                throw new CEFormatException("UUDecoder: Malformed begin line.");
        }
        mode = Integer.parseInt(r.substring(4,7));
        bufferName = r.substring(r.indexOf(' ',6)+1);
        /*
         * Check for \n after \r
         */
        if (c == '\r') {
            c = inStream.read ();
            if ((c != '\n') && (c != -1))
                inStream.unread (c);
        }
    }

    /**
     * In uuencoded buffers, encoded lines start with a character that
     * represents the number of bytes encoded in this line. The last
     * line of input is always a line that starts with a single space
     * character, which would be a zero length line.
     */
    protected int decodeLinePrefix(PushbackInputStream inStream, OutputStream outStream) throws IOException {
        int     c;

        c = inStream.read();
        if (c == ' ') {
            c = inStream.read(); /* discard the (first)trailing CR or LF  */
            c = inStream.read(); /* check for a second one  */
            if ((c != '\n') && (c != -1))
                inStream.unread (c);
            throw new CEStreamExhausted();
        } else if (c == -1) {
            throw new CEFormatException("UUDecoder: Short Buffer.");
        }

        c = (c - ' ') & 0x3f;
        if (c > bytesPerLine()) {
            throw new CEFormatException("UUDecoder: Bad Line Length.");
        }
        return (c);
    }


    /**
     * Find the end of the line for the next operation.
     * The following sequences are recognized as end-of-line
     * CR, CR LF, or LF
     */
    protected void decodeLineSuffix(PushbackInputStream inStream, OutputStream outStream) throws IOException {
        int c;
        while (true) {
            c = inStream.read();
            if (c == -1) {
                throw new CEStreamExhausted();
            }
            if (c == '\n') {
                break;
            }
            if (c == '\r') {
                c = inStream.read();
                if ((c != '\n') && (c != -1)) {
                    inStream.unread (c);
                }
                break;
            }
        }
    }

    /**
     * UUencoded files have a buffer suffix which consists of the word
     * end. This line should immediately follow the line with a single
     * space in it.
     */
    protected void decodeBufferSuffix(PushbackInputStream inStream, OutputStream outStream) throws IOException  {
        int     c;

        c = inStream.read(decoderBuffer);
        if ((decoderBuffer[0] != 'e') || (decoderBuffer[1] != 'n') ||
            (decoderBuffer[2] != 'd')) {
            throw new CEFormatException("UUDecoder: Missing 'end' line.");
        }
    }

}


/*
 * Copyright (c) 1995, 2005, Oracle and/or its affiliates. All rights reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.  Oracle designates this
 * particular file as subject to the "Classpath" exception as provided
 * by Oracle in the LICENSE file that accompanied this code.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
 * or visit www.oracle.com if you need additional information or have any
 * questions.
 */


/**
 * This class defines the encoding half of character encoders.
 * A character encoder is an algorithim for transforming 8 bit binary
 * data into text (generally 7 bit ASCII or 8 bit ISO-Latin-1 text)
 * for transmition over text channels such as e-mail and network news.
 *
 * The character encoders have been structured around a central theme
 * that, in general, the encoded text has the form:
 *
 * <pre>
 *      [Buffer Prefix]
 *      [Line Prefix][encoded data atoms][Line Suffix]
 *      [Buffer Suffix]
 * </pre>
 *
 * In the CharacterEncoder and CharacterDecoder classes, one complete
 * chunk of data is referred to as a <i>buffer</i>. Encoded buffers
 * are all text, and decoded buffers (sometimes just referred to as
 * buffers) are binary octets.
 *
 * To create a custom encoder, you must, at a minimum,  overide three
 * abstract methods in this class.
 * <DL>
 * <DD>bytesPerAtom which tells the encoder how many bytes to
 * send to encodeAtom
 * <DD>encodeAtom which encodes the bytes sent to it as text.
 * <DD>bytesPerLine which tells the encoder the maximum number of
 * bytes per line.
 * </DL>
 *
 * Several useful encoders have already been written and are
 * referenced in the See Also list below.
 *
 * @author      Chuck McManis
 * @see         CharacterDecoder;
 * @see         UCEncoder
 * @see         UUEncoder
 * @see         BASE64Encoder
 */
abstract class CharacterEncoder {

    /** Stream that understands "printing" */
    protected PrintStream pStream;

    /** Return the number of bytes per atom of encoding */
    abstract protected int bytesPerAtom();

    /** Return the number of bytes that can be encoded per line */
    abstract protected int bytesPerLine();

    /**
     * Encode the prefix for the entire buffer. By default is simply
     * opens the PrintStream for use by the other functions.
     */
    protected void encodeBufferPrefix(OutputStream aStream) throws IOException {
        pStream = new PrintStream(aStream);
    }

    /**
     * Encode the suffix for the entire buffer.
     */
    protected void encodeBufferSuffix(OutputStream aStream) throws IOException {
    }

    /**
     * Encode the prefix that starts every output line.
     */
    protected void encodeLinePrefix(OutputStream aStream, int aLength)
    throws IOException {
    }

    /**
     * Encode the suffix that ends every output line. By default
     * this method just prints a <newline> into the output stream.
     */
    protected void encodeLineSuffix(OutputStream aStream) throws IOException {
        pStream.println();
    }

    /** Encode one "atom" of information into characters. */
    abstract protected void encodeAtom(OutputStream aStream, byte someBytes[],
                int anOffset, int aLength) throws IOException;

    /**
     * This method works around the bizarre semantics of BufferedInputStream's
     * read method.
     */
    protected int readFully(InputStream in, byte buffer[])
        throws java.io.IOException {
        for (int i = 0; i < buffer.length; i++) {
            int q = in.read();
            if (q == -1)
                return i;
            buffer[i] = (byte)q;
        }
        return buffer.length;
    }

    /**
     * Encode bytes from the input stream, and write them as text characters
     * to the output stream. This method will run until it exhausts the
     * input stream, but does not print the line suffix for a final
     * line that is shorter than bytesPerLine().
     */
    public void encode(InputStream inStream, OutputStream outStream)
        throws IOException {
        int     j;
        int     numBytes;
        byte    tmpbuffer[] = new byte[bytesPerLine()];

        encodeBufferPrefix(outStream);

        while (true) {
            numBytes = readFully(inStream, tmpbuffer);
            if (numBytes == 0) {
                break;
            }
            encodeLinePrefix(outStream, numBytes);
            for (j = 0; j < numBytes; j += bytesPerAtom()) {

                if ((j + bytesPerAtom()) <= numBytes) {
                    encodeAtom(outStream, tmpbuffer, j, bytesPerAtom());
                } else {
                    encodeAtom(outStream, tmpbuffer, j, (numBytes)- j);
                }
            }
            if (numBytes < bytesPerLine()) {
                break;
            } else {
                encodeLineSuffix(outStream);
            }
        }
        encodeBufferSuffix(outStream);
    }

    /**
     * Encode the buffer in <i>aBuffer</i> and write the encoded
     * result to the OutputStream <i>aStream</i>.
     */
    public void encode(byte aBuffer[], OutputStream aStream)
    throws IOException {
        ByteArrayInputStream inStream = new ByteArrayInputStream(aBuffer);
        encode(inStream, aStream);
    }

    /**
     * A 'streamless' version of encode that simply takes a buffer of
     * bytes and returns a string containing the encoded buffer.
     */
    public String encode(byte aBuffer[]) {
        ByteArrayOutputStream   outStream = new ByteArrayOutputStream();
        ByteArrayInputStream    inStream = new ByteArrayInputStream(aBuffer);
        String retVal = null;
        try {
            encode(inStream, outStream);
            // explicit ascii->unicode conversion
            retVal = outStream.toString("8859_1");
        } catch (Exception IOException) {
            // This should never happen.
            throw new Error("CharacterEncoder.encode internal error");
        }
        return (retVal);
    }

    /**
     * Return a byte array from the remaining bytes in this ByteBuffer.
     * <P>
     * The ByteBuffer's position will be advanced to ByteBuffer's limit.
     * <P>
     * To avoid an extra copy, the implementation will attempt to return the
     * byte array backing the ByteBuffer.  If this is not possible, a
     * new byte array will be created.
     */
    private byte [] getBytes(ByteBuffer bb) {
        /*
         * This should never return a BufferOverflowException, as we're
         * careful to allocate just the right amount.
         */
        byte [] buf = null;

        /*
         * If it has a usable backing byte buffer, use it.  Use only
         * if the array exactly represents the current ByteBuffer.
         */
        if (bb.hasArray()) {
            byte [] tmp = bb.array();
            if ((tmp.length == bb.capacity()) &&
                    (tmp.length == bb.remaining())) {
                buf = tmp;
                bb.position(bb.limit());
            }
        }

        if (buf == null) {
            /*
             * This class doesn't have a concept of encode(buf, len, off),
             * so if we have a partial buffer, we must reallocate
             * space.
             */
            buf = new byte[bb.remaining()];

            /*
             * position() automatically updated
             */
            bb.get(buf);
        }

        return buf;
    }

    /**
     * Encode the <i>aBuffer</i> ByteBuffer and write the encoded
     * result to the OutputStream <i>aStream</i>.
     * <P>
     * The ByteBuffer's position will be advanced to ByteBuffer's limit.
     */
    public void encode(ByteBuffer aBuffer, OutputStream aStream)
        throws IOException {
        byte [] buf = getBytes(aBuffer);
        encode(buf, aStream);
    }

    /**
     * A 'streamless' version of encode that simply takes a ByteBuffer
     * and returns a string containing the encoded buffer.
     * <P>
     * The ByteBuffer's position will be advanced to ByteBuffer's limit.
     */
    public String encode(ByteBuffer aBuffer) {
        byte [] buf = getBytes(aBuffer);
        return encode(buf);
    }

    /**
     * Encode bytes from the input stream, and write them as text characters
     * to the output stream. This method will run until it exhausts the
     * input stream. It differs from encode in that it will add the
     * line at the end of a final line that is shorter than bytesPerLine().
     */
    public void encodeBuffer(InputStream inStream, OutputStream outStream)
        throws IOException {
        int     j;
        int     numBytes;
        byte    tmpbuffer[] = new byte[bytesPerLine()];

        encodeBufferPrefix(outStream);

        while (true) {
            numBytes = readFully(inStream, tmpbuffer);
            if (numBytes == 0) {
                break;
            }
            encodeLinePrefix(outStream, numBytes);
            for (j = 0; j < numBytes; j += bytesPerAtom()) {
                if ((j + bytesPerAtom()) <= numBytes) {
                    encodeAtom(outStream, tmpbuffer, j, bytesPerAtom());
                } else {
                    encodeAtom(outStream, tmpbuffer, j, (numBytes)- j);
                }
            }
            encodeLineSuffix(outStream);
            if (numBytes < bytesPerLine()) {
                break;
            }
        }
        encodeBufferSuffix(outStream);
    }

    /**
     * Encode the buffer in <i>aBuffer</i> and write the encoded
     * result to the OutputStream <i>aStream</i>.
     */
    public void encodeBuffer(byte aBuffer[], OutputStream aStream)
    throws IOException {
        ByteArrayInputStream inStream = new ByteArrayInputStream(aBuffer);
        encodeBuffer(inStream, aStream);
    }

    /**
     * A 'streamless' version of encode that simply takes a buffer of
     * bytes and returns a string containing the encoded buffer.
     */
    public String encodeBuffer(byte aBuffer[]) {
        ByteArrayOutputStream   outStream = new ByteArrayOutputStream();
        ByteArrayInputStream    inStream = new ByteArrayInputStream(aBuffer);
        try {
            encodeBuffer(inStream, outStream);
        } catch (Exception IOException) {
            // This should never happen.
            throw new Error("CharacterEncoder.encodeBuffer internal error");
        }
        return (outStream.toString());
    }

    /**
     * Encode the <i>aBuffer</i> ByteBuffer and write the encoded
     * result to the OutputStream <i>aStream</i>.
     * <P>
     * The ByteBuffer's position will be advanced to ByteBuffer's limit.
     */
    public void encodeBuffer(ByteBuffer aBuffer, OutputStream aStream)
        throws IOException {
        byte [] buf = getBytes(aBuffer);
        encodeBuffer(buf, aStream);
    }

    /**
     * A 'streamless' version of encode that simply takes a ByteBuffer
     * and returns a string containing the encoded buffer.
     * <P>
     * The ByteBuffer's position will be advanced to ByteBuffer's limit.
     */
    public String encodeBuffer(ByteBuffer aBuffer) {
        byte [] buf = getBytes(aBuffer);
        return encodeBuffer(buf);
    }

}

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.  Oracle designates this
 * particular file as subject to the "Classpath" exception as provided
 * by Oracle in the LICENSE file that accompanied this code.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
 * or visit www.oracle.com if you need additional information or have any
 * questions.
 */


/**
 * This class defines the decoding half of character encoders.
 * A character decoder is an algorithim for transforming 8 bit
 * binary data that has been encoded into text by a character
 * encoder, back into original binary form.
 *
 * The character encoders, in general, have been structured
 * around a central theme that binary data can be encoded into
 * text that has the form:
 *
 * <pre>
 *      [Buffer Prefix]
 *      [Line Prefix][encoded data atoms][Line Suffix]
 *      [Buffer Suffix]
 * </pre>
 *
 * Of course in the simplest encoding schemes, the buffer has no
 * distinct prefix of suffix, however all have some fixed relationship
 * between the text in an 'atom' and the binary data itself.
 *
 * In the CharacterEncoder and CharacterDecoder classes, one complete
 * chunk of data is referred to as a <i>buffer</i>. Encoded buffers
 * are all text, and decoded buffers (sometimes just referred to as
 * buffers) are binary octets.
 *
 * To create a custom decoder, you must, at a minimum,  overide three
 * abstract methods in this class.
 * <DL>
 * <DD>bytesPerAtom which tells the decoder how many bytes to
 * expect from decodeAtom
 * <DD>decodeAtom which decodes the bytes sent to it as text.
 * <DD>bytesPerLine which tells the encoder the maximum number of
 * bytes per line.
 * </DL>
 *
 * In general, the character decoders return error in the form of a
 * CEFormatException. The syntax of the detail string is
 * <pre>
 *      DecoderClassName: Error message.
 * </pre>
 *
 * Several useful decoders have already been written and are
 * referenced in the See Also list below.
 *
 * @author      Chuck McManis
 * @see         CEFormatException
 * @see         CharacterEncoder
 * @see         UCDecoder
 * @see         UUDecoder
 * @see         BASE64Decoder
 */

abstract class CharacterDecoder {

    /** Return the number of bytes per atom of decoding */
    abstract protected int bytesPerAtom();

    /** Return the maximum number of bytes that can be encoded per line */
    abstract protected int bytesPerLine();

    /** decode the beginning of the buffer, by default this is a NOP. */
    protected void decodeBufferPrefix(PushbackInputStream aStream, OutputStream bStream) throws IOException { }

    /** decode the buffer suffix, again by default it is a NOP. */
    protected void decodeBufferSuffix(PushbackInputStream aStream, OutputStream bStream) throws IOException { }

    /**
     * This method should return, if it knows, the number of bytes
     * that will be decoded. Many formats such as uuencoding provide
     * this information. By default we return the maximum bytes that
     * could have been encoded on the line.
     */
    protected int decodeLinePrefix(PushbackInputStream aStream, OutputStream bStream) throws IOException {
        return (bytesPerLine());
    }

    /**
     * This method post processes the line, if there are error detection
     * or correction codes in a line, they are generally processed by
     * this method. The simplest version of this method looks for the
     * (newline) character.
     */
    protected void decodeLineSuffix(PushbackInputStream aStream, OutputStream bStream) throws IOException { }

    /**
     * This method does an actual decode. It takes the decoded bytes and
     * writes them to the OutputStream. The integer <i>l</i> tells the
     * method how many bytes are required. This is always <= bytesPerAtom().
     */
    protected void decodeAtom(PushbackInputStream aStream, OutputStream bStream, int l) throws IOException {
        throw new CEStreamExhausted();
    }

    /**
     * This method works around the bizarre semantics of BufferedInputStream's
     * read method.
     */
    protected int readFully(InputStream in, byte buffer[], int offset, int len)
        throws java.io.IOException {
        for (int i = 0; i < len; i++) {
            int q = in.read();
            if (q == -1)
                return ((i == 0) ? -1 : i);
            buffer[i+offset] = (byte)q;
        }
        return len;
    }

    /**
     * Decode the text from the InputStream and write the decoded
     * octets to the OutputStream. This method runs until the stream
     * is exhausted.
     * @exception CEFormatException An error has occurred while decoding
     * @exception CEStreamExhausted The input stream is unexpectedly out of data
     */
    public void decodeBuffer(InputStream aStream, OutputStream bStream) throws IOException {
        int     i;
        int     totalBytes = 0;

        PushbackInputStream ps = new PushbackInputStream (aStream);
        decodeBufferPrefix(ps, bStream);
        while (true) {
            int length;

            try {
                length = decodeLinePrefix(ps, bStream);
                for (i = 0; (i+bytesPerAtom()) < length; i += bytesPerAtom()) {
                    decodeAtom(ps, bStream, bytesPerAtom());
                    totalBytes += bytesPerAtom();
                }
                if ((i + bytesPerAtom()) == length) {
                    decodeAtom(ps, bStream, bytesPerAtom());
                    totalBytes += bytesPerAtom();
                } else {
                    decodeAtom(ps, bStream, length - i);
                    totalBytes += (length - i);
                }
                decodeLineSuffix(ps, bStream);
            } catch (CEStreamExhausted e) {
                break;
            }
        }
        decodeBufferSuffix(ps, bStream);
    }

    /**
     * Alternate decode interface that takes a String containing the encoded
     * buffer and returns a byte array containing the data.
     * @exception CEFormatException An error has occurred while decoding
     */
    public byte[] decodeBuffer(String inputString) throws IOException { 
        byte inputBuffer[] = inputString.getBytes(); 
        ByteArrayInputStream inStream = new ByteArrayInputStream(inputBuffer); 
        ByteArrayOutputStream outStream = new ByteArrayOutputStream();
        decodeBuffer(inStream, outStream);
        return outStream.toByteArray();
    }

    /**
     * Decode the contents of the inputstream into a buffer.
     */
    public byte decodeBuffer(InputStream in)[] throws IOException {
        ByteArrayOutputStream outStream = new ByteArrayOutputStream();
        decodeBuffer(in, outStream);
        return (outStream.toByteArray());
    }

    /**
     * Decode the contents of the String into a ByteBuffer.
     */
    public ByteBuffer decodeBufferToByteBuffer(String inputString)
        throws IOException {
        return ByteBuffer.wrap(decodeBuffer(inputString));
    }

    /**
     * Decode the contents of the inputStream into a ByteBuffer.
     */
    public ByteBuffer decodeBufferToByteBuffer(InputStream in)
        throws IOException {
        return ByteBuffer.wrap(decodeBuffer(in));
    }
}



/*
 * Copyright (c) 1995, 2011, Oracle and/or its affiliates. All rights reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.  Oracle designates this
 * particular file as subject to the "Classpath" exception as provided
 * by Oracle in the LICENSE file that accompanied this code.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
 * or visit www.oracle.com if you need additional information or have any
 * questions.
 */

class CEFormatException extends java.io.IOException {
    static final long serialVersionUID = -7139121221067081482L;
    public CEFormatException(String s) {
        super(s);
    }
}

/*
 * Copyright (c) 1995, 2011, Oracle and/or its affiliates. All rights reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.  Oracle designates this
 * particular file as subject to the "Classpath" exception as provided
 * by Oracle in the LICENSE file that accompanied this code.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
 * or visit www.oracle.com if you need additional information or have any
 * questions.
 */

/** This exception is thrown when EOF is reached */
class CEStreamExhausted extends java.io.IOException {
    static final long serialVersionUID = -5889118049525891904L;
}

}
