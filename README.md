FunkyGrep, formerly FastGrep
============================
What is FunkyGrep ?
-------------------
FunkyGrep is a free Windows grep utility that aims to provide fast regular expression search with
a friendly UI similar to the popular (but out-of-date) BareGrep utility.

It is written in C# and requires the .NET Framework 4.0 to run.
This utility can be compiled with MSBUILD from a command line or with Visual Studio 2010 or higher.
Visual Studio Express Edition 2010 and higher should also work, but hasn't been tested.

It is preferable to compile from a hg working copy to get changeset ids embedded into the version number.

FAQ
---
### Why "FunkyGrep" ?
It comes from "Functional Grep", but FuncGrep would have been hard to pronounce, so I settled on FunkyGrep.

### Why was the project renamed ?
Because I discovered that [the  "FastGrep" name was already taken by a commercial application](http://www.fastgrep.com/), so I renamed this one to avoid confusion. This utility is *not* in anyway affiliated or connected with the one being sold there. *Please do not contact the owners of that site about this utility.*

### What is this project licensed under ?
This project is licensed under the [MIT license](http://opensource.org/licenses/MIT). You are free to use it for personal or commercial use, though it would be nice if you contributed new features or bug fixes if it helped you out during your day :-).

Features
--------
1. Accepts a single command-line parameter to launch the UI with the cursor ready in the "Text" field and search path pre-input, just type to search and press ENTER. This is useful to facilitate setting up FunkyGrep as an external tool in other apps like Visual Studio or WebStorm.
2. Supports multiple space-separated DOS filename inclusion patterns.
3. Right-click on found matches and copy useful information such as absolute and relative path as well as "Copy as File" (can be pasted in shell or other programs).
4. Drag a match and drop it in any external application to instantly open the it in your favorite editor.
