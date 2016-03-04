using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;

namespace WPFLocales.Powershell.Utils
{
    public class VSAutomationHelper
    {
        public static DTE GetDTE(int processId, VisualStudioVersion vsVersion)
        {
            string progId = null;
            object runningObject = null;

            switch (vsVersion)
            {
                case VisualStudioVersion.VisualStudio2012:
                    progId = "!VisualStudio.DTE.12.0:" + processId;
                    break;
                case VisualStudioVersion.VisualStudio2013:
                    progId = "!VisualStudio.DTE.12.0:" + processId;
                    break;
                case VisualStudioVersion.VisualStudio2015:
                    progId = "!VisualStudio.DTE.14.0:" + processId;
                    break;
                default:
                    throw new NotSupportedException(vsVersion.ToString());
            }

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
            catch (Exception e)
            {
            }
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
}