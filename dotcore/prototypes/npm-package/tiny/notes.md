## How to Create a NPM Package 

c:\Dev\jasper\granite-jasper\dotcore\prototypes\npm-package>md tiny

c:\Dev\jasper\granite-jasper\dotcore\prototypes\npm-package>cd tiny

c:\Dev\jasper\granite-jasper\dotcore\prototypes\npm-package\tiny>touch package.json
'touch' is not recognized as an internal or external command,
operable program or batch file.

c:\Dev\jasper\granite-jasper\dotcore\prototypes\npm-package\tiny>dir
 Volume in drive C is OS
 Volume Serial Number is 08D7-8348

 Directory of c:\Dev\jasper\granite-jasper\dotcore\prototypes\npm-package\tiny

10/22/2018  12:45 PM    <DIR>          .
10/22/2018  12:45 PM    <DIR>          ..
10/22/2018  12:45 PM                 0 package.json
               1 File(s)              0 bytes
               2 Dir(s)  44,353,032,192 bytes free

c:\Dev\jasper\granite-jasper\dotcore\prototypes\npm-package\tiny>npm publish
npm ERR! file package.json
npm ERR! code EJSONPARSE
npm ERR! Failed to parse json
npm ERR! Unexpected end of JSON input while parsing near ''
npm ERR! File: package.json
npm ERR! Failed to parse package.json data.
npm ERR! package.json must be actual JSON, not just JavaScript.
npm ERR!
npm ERR! Tell the package author to fix their package.json file. JSON.parse

npm ERR! A complete log of this run can be found in:
npm ERR!     C:\Users\nbontha\AppData\Roaming\npm-cache\_logs\2018-10-22T17_46_21_280Z-debug.log

c:\Dev\jasper\granite-jasper\dotcore\prototypes\npm-package\tiny>npm publish
npm ERR! package.json requires a valid "version" field

npm ERR! A complete log of this run can be found in:
npm ERR!     C:\Users\nbontha\AppData\Roaming\npm-cache\_logs\2018-10-22T17_47_38_463Z-debug.log

c:\Dev\jasper\granite-jasper\dotcore\prototypes\npm-package\tiny>npm publish
npm ERR! publish Failed PUT 402
npm ERR! code E402
npm ERR! You must sign up for private packages : @nat2k5us/tiny

npm ERR! A complete log of this run can be found in:
npm ERR!     C:\Users\nbontha\AppData\Roaming\npm-cache\_logs\2018-10-22T17_48_50_238Z-debug.log

c:\Dev\jasper\granite-jasper\dotcore\prototypes\npm-package\tiny>npm publish --access=public
+ @nat2k5us/tiny@1.0.0

c:\Dev\jasper\granite-jasper\dotcore\prototypes\npm-package\tiny>