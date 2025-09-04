https://cqrxs.eu/download/EU.CqrXs.Console/README.MD

S:\>EU.CqrXs.Console.exe --help
Usage:  EU.CqrXs.Console.exe
    -i | --inFile= | --inText={string|EnviromentVariable} | --inStd
    -o | --outFile= | --outText=EnviromentVariable | --outStd
    -u | --unzip={gzip|bzip2}
    -z | --zip={gzip|bzip2}
    -d | --decode={raw|hex16|hex32|base32|base64|uu}
    -e | --encode={raw|hex16|hex32|base32|base64|uu}
    -c | --crypt={algo1,algo2,...}
        -k | --key={secret_key}
         algo:
            Aes,AesLight,Rijndael,Des,Des3,Dstu7624,
            Aria,Camellia,CamelliaLight,Cast5,Cast6,
            BlowFish,Fish2,Fish3,ThreeFish256,
            Gost28147,Idea,Noekeon,
            RC2,RC532,RC564,RC6,
            Seed,SkipJack,Serpent,SM4,
            Tea,Tnepres,XTea,
            ZenMatrix,ZenMatrix2
    -D | --decrypt={algo1,algo2,...}
        -k | --key={secret_key}
    -s | --Sum={md5|sha256|sha512}
    -h | --help

Examples:
    EU.CqrXs.Console.exe -i=test.jpg -z=bzip2 -e=base32 -o=test.jpg.bz2.base32
    EU.CqrXs.Console.exe -i=test.jpg.bz2.base32 -d=base32 -u=bzip2 -o=test1.jpg 
    EU.CqrXs.Console.exe -i=test.jpg --zip=gzip --crypt=AesLight,Fish3 -k=MyKey -e=base64 -o=test.jpg.gz.aeslight.fish3.base64 
    EU.CqrXs.Console.exe -i=test.jpg.gz.aeslight.fish3.base64 -d=base64 -D=AesLight,Fish3 -k=MyKey -e=base64 --unzip=gzip -o=test2.jpg