using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;

namespace WPFLocales.Powershell.Utils
{
    public class VSAutomationHelper
    {
        public static DTE GetDTE(int processId)
        {
            var progId = "VisualStudio.DTE";
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

                    if (!string.IsNullOrEmpty(name) && name.Contains(progId))
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