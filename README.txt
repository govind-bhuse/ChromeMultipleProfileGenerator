Chrome Profile Generator – English rebuild

What this version does
- English Windows Forms interface.
- Finds Google Chrome in common installation locations.
- Creates a "Chrome Profiles" folder on the current user's Desktop.
- Places all generated .lnk shortcuts inside that folder instead of directly on the Desktop.
- Keeps the original naming pattern and profile-directory argument found in the supplied EXE:
  shortcut: <prefix><number padded to 3 digits>.lnk
  Chrome argument: --profile-directory="HENG_<prefix><number padded to 3 digits>"

Important behaviour
This utility creates Chrome shortcuts. Chrome initializes the corresponding profile directory
when each shortcut is opened for the first time. It does not copy or create full browser profile
data immediately when the shortcuts are generated.

Build on Windows 11
1. Install the .NET 8 SDK (not just the runtime):
   https://dotnet.microsoft.com/download/dotnet/8.0
2. Extract this ZIP to a normal folder.
3. Double-click build-windows.bat.
4. When it finishes, find ChromeProfileGenerator.exe in the publish folder.

The published EXE is self-contained for Windows x64. The original EXE was a 32-bit .NET
assembly; this rebuild targets 64-bit Windows 11. Keep the original EXE until you have tested
the replacement.

Note
The original source code was not provided. This is a functional reconstruction based on
the supplied executable's embedded strings and the screenshot. Please test with a small
number of profiles first. If the original app used additional undocumented behaviour,
that may need adjustment after comparing results.


ONLINE BUILD WITHOUT INSTALLING .NET ON YOUR PC (GitHub Actions)
1. Sign in at https://github.com/ and create a new repository.
   For personal use, choose Private if you prefer to keep the source private.
2. Extract this ZIP on your PC.
3. In the new GitHub repository, choose Add file > Upload files.
4. Upload all extracted project files and folders, including the hidden
   .github/workflows/build.yml file. If the browser upload is awkward,
   create the workflow file on GitHub using the exact path:
   .github/workflows/build.yml
   and paste the contents from this package.
5. Commit the files to the main branch.
6. Open the repository's Actions tab.
7. Select "Build Windows EXE" and click "Run workflow".
8. Wait for the run to finish successfully (green check mark).
9. Open the completed run and download the artifact named
   ChromeProfileGenerator-Windows-x64.
10. Extract the downloaded artifact ZIP. It contains the EXE.

The .NET SDK is installed only on GitHub's temporary hosted Windows runner,
not on your computer.
