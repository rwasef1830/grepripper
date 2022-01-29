FunkyGrep
=========
What is FunkyGrep ?
-------------------
FunkyGrep is a free Windows grep utility that aims to provide fast regular expression search with
a friendly UI similar to the popular (but out-of-date) BareGrep utility.

It is written in C# and requires the .NET 6.0 to build and run.

It is preferable to compile from a git working copy to get changeset ids embedded into the version number.

FAQ
---
### Why "FunkyGrep" ?
It comes from "Functional Grep", but FuncGrep would have been hard to pronounce, so I settled on FunkyGrep.

### What is this project licensed under ?
This project is licensed under the [MIT license](http://opensource.org/licenses/MIT). You are free to use it for personal or commercial use, though it would be nice if you contributed new features or bug fixes if it helped you out during your day :-).

Features
--------
1. Accepts a single command-line parameter to launch the UI with the cursor ready in the "Text" field and search path pre-input, just type to search and press ENTER. This is useful to facilitate setting up FunkyGrep as an external tool in other apps like Visual Studio or WebStorm.
2. Supports multiple space-separated DOS filename inclusion patterns.
3. Right-click on found matches and copy useful information such as path as well as "Copy as File" (can be pasted in shell or other programs).
4. Drag a match and drop it in any external application to instantly open the it in your favorite editor.
