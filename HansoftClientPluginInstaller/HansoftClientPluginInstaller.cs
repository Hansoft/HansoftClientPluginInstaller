using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using HPMSdk;
using Hansoft.ObjectWrapper;
using Hansoft.SimpleLogging;

namespace Hansoft.HansoftClientPluginInstaller
{
    class HansoftClientPluginInstaller
    {
        static ILogger logger;

        static bool cOptionFound = false;
        static bool iOptionFound = false;
        static bool uOptionFound = false;

        static string sdkUser;
        static string sdkUserPwd;
        static string server;
        static int portNumber;
        static string databaseName;
        static string pluginName;
        static string x86PluginFileName;
        static string x64PluginFileName;
        static string usage =
@"Usage:
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

";

        static void Main(string[] args)
        {
            try
            {
                logger = new ConsoleLogger();
                logger.Initialize("HansoftClientPluginInstaller");
                if (ParseArguments(args))
                {
                    try
                    {
                        SessionManager.Initialize(sdkUser, sdkUserPwd, server, portNumber, databaseName);
                        try
                        {
                            SessionManager.Instance.Connect();
                        }
                        catch (Exception)
                        {
                            throw new ApplicationException("Could not connect to Hansoft Server with specified parameters.");
                        }

                        if (SessionManager.Instance.Connected)
                        {
                            try
                            {
                                DoInstall();
                            }
                            catch (Exception e)
                            {
                                logger.Exception(e);
                            }
                            finally
                            {
                                SessionManager.Instance.CloseSession();
                            }
                        }
                        else
                        {
                            logger.Warning("Could not open connection to Hansoft");
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Exception(e);
                    }
                }
                else
                    logger.Information(usage);
            }
            catch (Exception e)
            {
                logger.Exception(e);
            }
        }

        static void DoInstall()
        {
            string tempFile;
            try
            {
                tempFile = Path.GetTempFileName();
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed when calling: Path.GetTempFileName()");
            }
            try
            {
                SessionManager.Session.VersionControlInit(tempFile);
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed when calling: SessionManager.Session.VersionControlInit");
            }
            if (uOptionFound)
		        DeleteVersionControlFile("SDK/Plugins/" + pluginName, true);

            if (iOptionFound)
            {
                DeleteVersionControlFile("SDK/Plugins/" + pluginName, false);
                // Add plugins. You need both a x86 and x64 version built when you run the program. Both are required. If you create your own application make sure to use a directory for a reverse domain name that you own.
                AddVersionControlFile("SDK/Plugins/" + pluginName + "/Win32/x86/Plugin.dll", x86PluginFileName);
                AddVersionControlFile("SDK/Plugins/" + pluginName + "/Win32/x64/Plugin.dll", x64PluginFileName);
            }
        }

        static void DeleteVersionControlFile(string hpmFileName, bool logMessages)
	    {
		    HPMVersionControlDeleteFiles deleteFiles = new HPMVersionControlDeleteFiles();
		    HPMVersionControlFileSpec fileSpec = new HPMVersionControlFileSpec();
		    fileSpec.m_Path = hpmFileName;
            deleteFiles.m_Files = new HPMVersionControlFileSpec[1];
		    deleteFiles.m_Files[0] = fileSpec;
		    deleteFiles.m_bDeleteLocally = false;
		    deleteFiles.m_bPermanent = true;
		    deleteFiles.m_Comment =  "Delete of client plugin:" + pluginName;;
		    HPMChangeCallbackData_VersionControlDeleteFilesResponse Response = SessionManager.Session.VersionControlDeleteFilesBlock(deleteFiles);
            if (logMessages)
            {
                if (Response.m_Errors.Length > 0)
                    logger.Error("Error deleting version control file: " + Response.m_Errors[0].m_File + " Error: " + SessionManager.Session.VersionControlErrorToStr(Response.m_Errors[0].m_Error));
                else
                    logger.Information("The version control file " + hpmFileName + " was deleted.");
            }
	    }

        static void AddVersionControlFile(string hpmFileName, string localFileName)
	    {
		    HPMVersionControlAddFiles addFiles = new HPMVersionControlAddFiles();
		    HPMVersionControlLocalFilePair fileSpec = new HPMVersionControlLocalFilePair();
		    fileSpec.m_LocalPath = localFileName;
		    fileSpec.m_FileSpec.m_Path = hpmFileName;
		    addFiles.m_FilesToAdd = new HPMVersionControlLocalFilePair[1];
            addFiles.m_FilesToAdd[0] = fileSpec;
		    addFiles.m_Comment = "Add of client plugin:" + pluginName;
		    addFiles.m_bDeleteSourceFiles = false;

		    HPMChangeCallbackData_VersionControlAddFilesResponse Response = SessionManager.Session.VersionControlAddFilesBlock(addFiles);
            if (Response.m_Errors.Length > 0)
                logger.Error("Error adding version control file: " + Response.m_Errors[0].m_File + " Error: " + SessionManager.Session.VersionControlErrorToStr(Response.m_Errors[0].m_Error));
            else
                logger.Information("The version control file " + hpmFileName + " was added.");
	    }

        static bool ParseArguments(string[] args)
        {
            foreach (string optionString in args)
            {
                string option = optionString.Substring(0, 2);
                string[] pars = optionString.Substring(2).Split(new char[] { ':' });
                for (int i = 0; i < pars.Length; i += 1)
                {
                    if (pars[i].StartsWith("\"") && pars[i].EndsWith("\""))
                        pars[i] = pars[i].Substring(1, pars[i].Length - 2);
                }
                switch (option)
                {
                    case "-c":
                        if (cOptionFound)
                            throw new ArgumentException("The -c option can only be specified once");
                        if (pars.Length != 5)
                            throw new ArgumentException("The -c option was not specified correctly");
                        cOptionFound = true;
                        server = pars[0];
                        portNumber = Int32.Parse(pars[1]);
                        databaseName = pars[2];
                        sdkUser = pars[3];
                        sdkUserPwd = pars[4];
                        break;
                    case "-i":
                        if (iOptionFound)
                            throw new ArgumentException("The -i option can only be specified once");
                        if (uOptionFound)
                            throw new ArgumentException("You can one of the -i and -u options");
                        if (pars.Length != 3)
                            throw new ArgumentException("The -i option was not specified correctly");
                        iOptionFound = true;
                        pluginName = pars[0];
                        x86PluginFileName = pars[1];
                        x64PluginFileName = pars[2];
                        break;
                    case "-u":
                        if (uOptionFound)
                            throw new ArgumentException("The -u option can only be specified once");
                        if (iOptionFound)
                            throw new ArgumentException("You can one of the -i and -u options");
                        if (pars.Length != 1)
                            throw new ArgumentException("The -u option was not specified correctly");
                        uOptionFound = true;
                        pluginName = pars[0];
                        break;
                    default:
                        throw new ArgumentException("An unsupported option was specifed: " + option);
                }
            }
            return (cOptionFound && (iOptionFound || uOptionFound));
        }
    }
}