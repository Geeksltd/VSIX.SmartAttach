using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;

namespace Geeks.VSIX.SmartAttach.Base
{
    public static class ErrorList
    {
        static ErrorListProvider errorListProvider;
        //static Dictionary<string, Microsoft.VisualStudio.Shell.Task> ListOfErrors = new Dictionary<string, Microsoft.VisualStudio.Shell.Task>();

        //public static void AddOrOverrideError(string key, Microsoft.VisualStudio.Shell.Task task)
        //{
        //    if (ListOfErrors.ContainsKey(key))
        //    {
        //        ListOfErrors[key] = task;
        //    }
        //    else
        //    {
        //        ListOfErrors.Add(key, task);
        //    }

        //    if (task is ErrorTask)
        //    {
        //        WriteVisualStudioErrorList(task as ErrorTask);
        //    }
        //}

        //public static void RemoveError(string key)
        //{
        //    if (string.IsNullOrEmpty(key))
        //        return;

        //    if (ListOfErrors.ContainsKey(key))
        //    {
        //        var task = ListOfErrors[key];
        //        if (task == null)
        //            return;

        //        if (task is ErrorTask)
        //        {
        //            errorListProvider.Tasks.Remove(task);
        //        }

        //        ListOfErrors.Remove(key);
        //    }
        //}

        /// <summary>
        /// Write an entry to the Visual Studio Error List.
        /// </summary>
        //static void WriteVisualStudioErrorList(ErrorTask errorTask)
        //{
        //    if (errorListProvider == null)
        //    {
        //        errorListProvider = new ErrorListProvider(SmartAttachPackage.Instance);
        //    }

        //    // Check if this error is already in the error list, don't report more than once  
        //    var alreadyReported = false;
        //    foreach (ErrorTask task in errorListProvider.Tasks)
        //    {
        //        if (task.ErrorCategory == errorTask.ErrorCategory &&
        //            task.Document == errorTask.Document &&
        //            task.Line == errorTask.Line &&
        //            task.Column == errorTask.Column &&
        //            task.Text == errorTask.Text)
        //        {
        //            alreadyReported = true;
        //            break;
        //        }
        //    }

        //    if (!alreadyReported)
        //    {
        //        // Add error to task list
        //        errorListProvider.Tasks.Add(errorTask);
        //    }
        //}
    }
}
