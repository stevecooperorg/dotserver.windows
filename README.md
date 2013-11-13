dotserver.windows
=================

A simple C# console application which monitors a directory of .dot (graphviz) files and automatically processes gifs when they change.

I use this to develop DOT files; 

1. Configure the settings in `app.config` or `dotserver.windows.exe.config` to point to the right version of graphviz (defaults to v2.34 on win64) and the right extension for DOT language files (defaults to `.dot`).
1. Run `dotserver.windows`, passing the directory to monitor on the command line;    
    `dotserver.windows "c:\docs"`    
this will monitor the folder for `.dot` files.
1. edit your `.dot` files in a text editor. When you save, `dotserver.windows` will automatically invoke the DOT compiler. It requires graphviz v2.34. If you use a different version, edit the source to change the location of `dot.exe`. It produces a `.gif` file with the same name but the `.gif` extension, in the same directory as the source.
1. You can use this to do a live preview of your files by using [LiveReload](http://livereload.com/);

Live developing with LiveReload
-------------------------------

1. Install [LiveReload](http://livereload.com/).
2. In LiveReload, Add the folder containing your DOT files (the DOT directory) as a site folder.
3. In IIS, map the DOT directory as a virtual directory, eg `http://localhost/mydotfiles`
3. Wrap the GIF you are trying to develop in an HTML page (example html content included below).
4. Open the HTML page in the browser.

Then, you can edit the DOT file, and save it. `dotserver.windows` will detect the change and compile the GIF; [LiveReload](http://livereload.com/) will detect the change and reload the HTML. The end result is that you can have a browser window on one screen and your text editor on another, and they are kept in sync.

Example HTML file
-----------------

    <!DOCTYPE html>
    <html>
    <body>
       <img src="mydotfile.gif" />
    </body>
    </html>