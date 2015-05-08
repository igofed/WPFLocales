using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using EnvDTE;
using WPFLocales.Powershell.Utils;
using Process = System.Diagnostics.Process;
using Thread = System.Threading.Thread;

namespace WPFLocales.Powershell
{
    [Cmdlet(VerbsCommon.Get, "DTE")]
    public class GetDTE : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Path to project", Position = 1)]
        public string TargetProjPath { get; set; }

        protected override void BeginProcessing()
        {
            if (!File.Exists(TargetProjPath))
                throw new FileNotFoundException(TargetProjPath + " not found.");
        }

        protected override void ProcessRecord()
        {
            MessageFilter.Register();

            DTE dte;
            if (!GetDte("devenv", out dte))
            {
                Process.Start(TargetProjPath);
                while (!GetDte("devenv", out dte))
                    Thread.Sleep(1000);
            }

            WriteObject(dte);

            MessageFilter.Revoke();
        }

        private bool GetDte(string processName, out DTE dte)
        {
            dte = Process.GetProcessesByName(processName)
                         .Select(x => VSAutomationHelper.GetDTE(x.Id))
                         .FirstOrDefault(dte1 =>
                             dte1 != null && dte1.Solution.GetProjects().Any(x => x.FileName == TargetProjPath));
            return dte != null;
        }
    }

    public class MessageFilter : IOleMessageFilter
    {
        private const int PENDINGMSG_WAITDEFPROCESS = 2;
        private const int SERVERCALL_ISHANDLED = 0;
        private const int SERVERCALL_RETRYLATER = 2;
        private const int TooBusy = -1;

        // Class containing the IOleMessageFilter
        // thread error-handling functions.
        // Start the filter.
        public static void Register()
        {
            IOleMessageFilter newFilter = new MessageFilter();
            IOleMessageFilter oldFilter;
            CoRegisterMessageFilter(newFilter, out oldFilter);
        }

        // Done with the filter, close it.
        public static void Revoke()
        {
            IOleMessageFilter oldFilter;
            CoRegisterMessageFilter(null, out oldFilter);
        }

        // IOleMessageFilter functions.
        // Handle incoming thread requests.
        int IOleMessageFilter.HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo)
        {
            return SERVERCALL_ISHANDLED;
        }

        int IOleMessageFilter.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
        {
            return PENDINGMSG_WAITDEFPROCESS;
        }

        // Thread call was rejected, so try again.
        int IOleMessageFilter.RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
        {
            if (dwRejectType == SERVERCALL_RETRYLATER)
            {
                // Retry the thread call immediately if return >=0 & 
                // <100.
                return 99;
            }

            // Too busy; cancel call.
            return TooBusy;
        }

        // Implement the IOleMessageFilter interface.
        [DllImport("Ole32.dll")]
        private static extern int CoRegisterMessageFilter(IOleMessageFilter newFilter, out IOleMessageFilter oldFilter);
    }

    [ComImport, Guid("00000016-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
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