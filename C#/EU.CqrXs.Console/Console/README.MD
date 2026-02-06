https://cqrxs.eu/download/EU.CqrXs.Console/README.MD

EU.CqrXs.Console.exe -?
Usage:  EU.CqrXs.Console.exe
        -i  | --inFile= | --inText={string|EnviromentVariable} | --inStd    
        -k  | --key=mykey
        -H  | --hash={Oct|Blake2xs|BCrypt|CShake|Dstu7564|MD5|RipeMD256|SCrypt|Sha1|Sha256|Sha384|Sha512|Whirlpool|TupleHash}
        -z  | --zip={gzip|bzip2|zip}
        -C  | --CipherAlgos={[aes,des3,blowfish,fish2,fish3]|key}
        algo:
            Aes,AesLight,Rijndael,Des,Des3,Dstu7624,
            Aria,Camellia,CamelliaLight,Cast5,Cast6,
            BlowFish,Fish2,Fish3,
            Gost28147,Idea,Noekeon,
            RC2,RC532,RC564,RC6,
            Seed,SkipJack,Serpent,SM4,
            Tea,Tnepres,XTea,
            ZenMatrix,ZenMatrix2
        symmAlgo: 
            Aes,BlowFish,Camellia,Cast6,Des3,Fish2,Fish3,Gost28147,Idea,RC532,Seed,SkipJack,Serpent,Tea,XTea,SM4        
        -S  | --SymmCipher   
        -e  | --encode={raw|hex16|hex32|base32|base64|uu}
        -o  | --outFile= | --outText=EnviromentVariable | --outStd            
        -D  | --Decrypt           
        -Y  | --YankeeTest
        -?  | --gethelp

Examples:
    
    EU.CqrXs.Console.exe -i=.\README.MD -e=base16 -o=.\README_MD.base16
    EU.CqrXs.Console.exe -D  -i=.\README_MD.base16 -e=base16 -o=.\READ_MD.txt

    EU.CqrXs.Console.exe -i=.\README.MD -k=Hallo -z=gzip  -C=BlowFish,Fish2,Fish3 -e=base64 -o=.\README.MD.gz.BfF.base64
    EU.CqrXs.Console.exe -D -i=.\README.MD.gz.BfF.base64 -k=Hallo -e=base64 -C=BlowFish,Fish2,Fish3 -z=gzip -o=.\READ_GUNZIP.txt

    EU.CqrXs.Console.exe -i=.\README.MD -z=bz -k=heinrichelsigan.area23.at -H=Whirlpool -e=hex32 -o=.\README.MD.Whirlpool.bz.Hex32
    EU.CqrXs.Console.exe -D -i=.\README.MD.Whirlpool.bz.Hex32 -e=hex32 -k=heinrichelsigan.area23.at -H=Whirlpool -z=bz -o=.\READ_BUNZIP.txt

    EU.CqrXs.Console.exe -i=.\README.MD  -k=io.cqrxs.eu -H=SCrypt -z=zip -C=Aes,Blowfish,Des3,Fish2,Fish3,Seed,Serpent,SM4  -e=uu -o=.\README.MD.SCrypt.zip.uu
    EU.CqrXs.Console.exe -D -i=.\README.MD.SCrypt.zip.uu -e=uu -k=io.cqrxs.eu -H=SCrypt -z=zip -C=Aes,Blowfish,Des3,Fish2,Fish3,Seed,Serpent,SM4  -o=.\READ_UNZIP.txt

    EU.CqrXs.Console.exe -i=.\README.MD -S -z=zip -k=io.cqrxs.eu -H=BCrypt -e=xx -o=.\README.MD.BCrypt.zip.xx
    EU.CqrXs.Console.exe -D -i=.\README.MD.BCrypt.zip.xx -S -e=xx -k=io.cqrxs.eu -H=BCrypt -z=zip -o=.\README_SYM_BCRYPT_UNZIP.txt


