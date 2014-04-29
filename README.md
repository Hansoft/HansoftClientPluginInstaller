HansoftClientPluginInstaller
============================

About this program
------------------
This program is an Hansoft SDK client program that connects to the Hansoft server and installs a Hansoft client plugin.
Note that to use this program you need to have the SDK option enabled on your Hansoft Server and also have an SDK user
created in your database.

There is some help displayed if you invoke the program without specifying any parameters. See also below.


Terms and conditions
--------------------
HansoftClientPluginInstaller is licensed under what is known as an MIT License as stated
in the [LICENSE.md](LICENSE.md).

This program is not part of the official Hansoft product or subject to its license agreement.
The program is provided as is and there is no obligation on Hansoft AB to provide support, update or enhance this program.

Building the program
--------------------
The program can be built with the freely available [Visual Studio Express 2012 for Desktop] [1]. 
The program uses the general wrappers [ObjectWrapper] [2] and [SimpleLogging] [3] and you need
to download and build these separately or integrate them into your solution.
You will also need the [Hansoft SDK] [4] to be able to build the program. You will also need to
update the references to the appropriate  Hansoft SDK DLL in the Visual Studio project (e.g.: HPMSdkManaged_4_5.x86.dll)
and make sure that the Hansoft SDK DLL (e.g. HPMSdk.x86.dll) is in the same directory as your executable.

[1]: http://www.microsoft.com/visualstudio/eng/products/visual-studio-express-for-windows-desktop  "Visual Studio Express 2012 for Desktop"
[2]: http://github.com/Hansoft/Hansoft-ObjectWrapper                                               "ObjectWrapper"
[3]: http://github.com/Hansoft/Hansoft-SimpleLogging                                               "SimpleLogging"
[4]: http://hansoft.com/support/downloads/                                                         "Hansoft SDK"

Usage
-----
	HansoftClientPluginInstaller -c<server>:<port>:<database>:<sdk user>:<pwd> -i<plugin_name>:<file_x86>:<file_x64> | -u<plugin_name>

	This utility installs a Hansoft Client Plugin.

	If any parameter values contain spaces, then the parameter value in question need to be double quoted. Colons are not
	allowed in parameter values.

	-c Specifies what hansoft database to connect to and the sdk user to be used
	<server>       : IP or DNS name of the Hansoft server
	<port>         : The listen port of the Hansoft server
	<database>     : Name of the Hansoft Database to get data from
	<sdk user>     : Name of the Hansoft SDK User account
	<pwd>          : Password of the Hansoft SDK User account

	-i Install the specified Hansoft client plugin
	<plugin_name>  : e.g. com.hansoft.safeextension.safeclientplugin
	<file_x86>     :  The 32-bit version of the plugin
	<file_x64>     : The 64-bitr version of the plugin

	-u Uninstall the specified Hansoft client plugin
	<plugin_name>  : e.g. com.hansoft.safeextension.safeclientplugin



















































































