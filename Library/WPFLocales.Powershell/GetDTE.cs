using System;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using Process = System.Diagnostics.Process;
using Thread = System.Threading.Thread;

namespace WPFLocales.Powershell
{
    [Cmdlet(VerbsCommon.Get, "DTE")]
    public class GetDTE : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Path to Visual Studio", Position = 0)]
        public string VSPath { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Path to project", Position = 1)]
        public string ProjPath { get; set; }

        protected override void ProcessRecord()
        {
            MessageFilter.Register();

            const string envVariableName = "devenvPID";
            int devenvPid;
            Process devenvProcess = null;
            DTE dte;

            if (int.TryParse(Environment.GetEnvironmentVariable(envVariableName, EnvironmentVariableTarget.User), out devenvPid))
            {
                devenvProcess = Process.GetProcesses().FirstOrDefault(x => x.Id == devenvPid);
            }

            if (devenvProcess == null)
            {
                devenvPid = Process.Start(VSPath).Id;
            }

            while (true)
            {
                dte = VSAutomationHelper.GetDTE(devenvPid);

                if (dte == null)
                    Thread.Sleep(1000);
                else
                    break;
            }

            Environment.SetEnvironmentVariable(envVariableName, devenvPid.ToString(), EnvironmentVariableTarget.User);

            WriteObject(dte);

            MessageFilter.Revoke();
        }
    }

    public class VSAutomationHelper
    {
        public static DTE GetDTE(int processId)
        {
            string progId = "!VisualStudio.DTE.12.0:" + processId;
            object runningObject = null;

            IBindCtx bindCtx = null;
            IRunningObjectTable runningObjects = null;
            IEnumMoniker enumMonikers = null;

            try
            {
                Marshal.ThrowExceptionForHR(CreateBindCtx(0, out bindCtx));
                bindCtx.GetRunningObjectTable(out runningObjects);
                runningObjects.EnumRunning(out enumMonikers);

                IMoniker[] moniker = new IMoniker[1];
                IntPtr numberFetched = IntPtr.Zero;
                while (enumMonikers.Next(1, moniker, numberFetched) == 0)
                {
                    IMoniker runningObjectMoniker = moniker[0];

                    string name = null;

                    try
                    {
                        if (runningObjectMoniker != null)
                        {
                            runningObjectMoniker.GetDisplayName(bindCtx, null, out name);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Do nothing, there is something in the ROT that we do not have access to.
                    }

                    if (!string.IsNullOrEmpty(name) && string.Equals(name, progId, StringComparison.Ordinal))
                    {
                        Marshal.ThrowExceptionForHR(runningObjects.GetObject(runningObjectMoniker, out runningObject));
                        break;
                    }
                }
            }
            catch (Exception e) {}
            finally
            {
                if (enumMonikers != null)
                {
                    Marshal.ReleaseComObject(enumMonikers);
                }

                if (runningObjects != null)
                {
                    Marshal.ReleaseComObject(runningObjects);
                }

                if (bindCtx != null)
                {
                    Marshal.ReleaseComObject(bindCtx);
                }
            }
            return (DTE) runningObject;
        }

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);
    }

    public class MessageFilter : IOleMessageFilter
    {
        // Class containing the IOleMessageFilter
        // thread error-handling functions.
        // Start the filter.
        public static void Register()
        {
            IOleMessageFilter newFilter = new MessageFilter();
            IOleMessageFilter oldFilter = null;
            CoRegisterMessageFilter(newFilter, out oldFilter);
        }

        // Done with the filter, close it.
        public static void Revoke()
        {
            IOleMessageFilter oldFilter = null;
            CoRegisterMessageFilter(null, out oldFilter);
        }

        // IOleMessageFilter functions.
        // Handle incoming thread requests.
        int IOleMessageFilter.HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo)
        {
            //Return the flag SERVERCALL_ISHANDLED.
            return 0;
        }

        int IOleMessageFilter.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
        {
            //Return the flag PENDINGMSG_WAITDEFPROCESS.
            return 2;
        }

        // Thread call was rejected, so try again.
        int IOleMessageFilter.RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
        {
            if (dwRejectType == 2)

                // flag = SERVERCALL_RETRYLATER.
            {
                // Retry the thread call immediately if return >=0 & 
                // <100.
                return 99;
            }

            // Too busy; cancel call.
            return -1;
        }

        // Implement the IOleMessageFilter interface.
        [DllImport("Ole32.dll")]
        private static extern int
            CoRegisterMessageFilter(IOleMessageFilter newFilter, out IOleMessageFilter oldFilter);
    }

    [ComImport(), Guid("00000016-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOleMessageFilter
    {
        [PreserveSig]
        int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo);

        [PreserveSig]
        int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);

        [PreserveSig]
        int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
    }
}