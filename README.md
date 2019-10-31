# wgetxpath
Downloads a web page and then executes an XPath expression on it in order to grab only parts of the page of interest.
Outputs on stdout.

Useful tool to monitor parts of static HTML pages.
You can pipe the output of `wgetxpath` to `diff` to see changes with previous version of the same page.

Code is in .NET Core 3.0, works fine on a Linux box with Mono 6.4.0.198.


---

## License ##
|![http://i.imgur.com/oGGeSQP.png](http://i.imgur.com/oGGeSQP.png)|The license for wgetxpath is the [WTFPL](http://www.wtfpl.net/): _Do What the Fuck You Want to Public License_.|
|:----------------------------------------------------------------|:--------------------------------------------------------------------------------------------------------------------|