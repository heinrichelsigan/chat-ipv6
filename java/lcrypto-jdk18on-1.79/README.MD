# The Bouncy Castle Crypto Package

The Bouncy Castle Crypto package is a Java implementation of 
cryptographic algorithms, it was developed by the Legion of the
Bouncy Castle, a registered Australian Charity, with a little help! The Legion, and the latest
goings on with this package, can be found at <a href=https://www.bouncycastle.org>https://www.bouncycastle.org</a>.

The Legion also gratefully acknowledges the contributions made to this
package by others (see <a href=CONTRIBUTORS.html>here</a> for the current list). 
 If you would like to contribute to our efforts please feel free to get in touch with us or visit our <a href="https://www.bouncycastle.org/donate">donations page</a>, sponsor some specific work, or purchase a support contract through <a href="https://www.cryptoworkshop.com">Crypto Workshop</a>.

The package is organised so that it 
contains a light-weight API suitable for use in any environment
(including the newly released J2ME) with the additional infrastructure
to conform the algorithms to the JCE framework.

Except where otherwise stated, this software is distributed under a license based on the MIT X Consortium license.  To view the license, <a href="./LICENSE.html">see here</a>. The OpenPGP library also includes a modified BZIP2 library which is licensed under the <a href="https://www.apache.org/licenses/">Apache Software License, Version 2.0</a>.

The current release notes for this package are <a href="docs/releasenotes.html">here</a>.

The current specifications for this package are <a href="docs/specifications.html">here</a>.

The current JavaDoc for the Bouncy Castle APIs can be found <a href="javadoc/index.html">here</a>.

Additional documentation on use of the classes can also be found in the <a href="docs/index.html">docs</a> directory.

## Examples and Tests

To view some examples, look at the test programs in the packages:
		
- <b>org.bouncycastle.crypto.test</b>
- <b>org.bouncycastle.jce.provider.test</b>
- <b>org.bouncycastle.cms.test</b>
- <b>org.bouncycastle.mail.smime.test</b>
 -<b>org.bouncycastle.openpgp.test</b>
- <b>org.bouncycastle.tsp.test</b>
                   
There are also some specific example programs for dealing with Attribute Certificates, PKCS12, SMIME and OpenPGP. They can be found in:
- <b>org.bouncycastle.jce.examples</b>
- <b>org.bouncycastle.mail.smime.examples</b>
- <b>org.bouncycastle.openpgp.examples</b>
- <b>org.bouncycastle.x509.examples</b>

Finally there are also code <a href="https://www.wiley.com/WileyCDA/WileyAncillary/productCd-0764596330.html">examples</a> from <a href="https://www.amazon.com/exec/obidos/redirect?path=ASIN/0764596330&amp;link_code=as2&amp;camp=1789&amp;tag=bouncycastleo-20&amp;creative=9325">Beginning Cryptography with Java</a> which demonstrate both the use of the JCE/JCA and also some of the Bouncy Castle APIs.</p>
<b>Note 1:</b>The JCE classes are only distributed with the JDK 1.1, JDK 1.2, and JDK 1.3 JCE releases. The
JDK 1.0, J2ME, and the JDK 1.1, JDK 1.2, JDK 1.3, JDK 1.4, and JDK 1.5 lightweight releases only include the
Bouncy Castle lightweight cryptography API.<br>
<b>Note 2:</b>The regression test for Diffie-Hellman is quite slow.

The clean room JCE, which will compile with everything from JDK 1.1 and up is in the jce/src/main/java directory.

The build scripts that come with the full distribution allow creation of the different releases by using the tree under src and test/src, excluding classes that are not appropriate and copying in the required compatibility classes from the directories containing compatibility classes appropriate for the distribution.

If you want to try create a build for yourself, using your own environment, the best way to do it is to start with the build for the distribution you are interested in, make sure that builds, and then modify your build scripts to do the required exclusions and file copies for your setup, otherwise you are likely to get class not found exceptions. The final caveat to this is that as the j2me distribution includes some compatibility classes starting in the java package, you need to use an obfuscator to change the package names before attempting to import a midlet using the BC API.

## Mailing Lists

For those who are interested, there are 2 mailing lists
for participation in this project.  To subscribe use the
links below and include the word subscribe in the message body.  (To unsubscribe, replace <b>subscribe</b> with <b>unsubscribe</b> in the message body)

- <a href="mailto:announce-crypto-request@bouncycastle.org">announce-crypto-request@bouncycastle.org</a>
  This mailing list is for new release announcements only, general subscribers cannot post to it.
- <a href="mailto:dev-crypto-request@bouncycastle.org">dev-crypto-request@bouncycastle.org</a>
  This mailing list is for discussion of development of the package.  This includes bugs, comments, requests for enhancements, questions about  use or operation.

<b>NOTE:</b>You need to be subscribed to send mail to the above mailing list.

If you want to provide feedback, directly to the members of <b>The Legion</b> then please use <a href="mailto:feedback-crypto@bouncycastle.org">feedback-crypto@bouncycastle.org</a>, if you want to help this project survive please consider <a href="https://www.bouncycastle.org/donate">donating</a> or getting a <a href="https://www.cryptoworkshop.com">support contract</a>.

Enjoy!
