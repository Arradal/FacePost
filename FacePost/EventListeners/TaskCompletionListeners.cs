using Android.Gms.Tasks;
using System;

namespace FacePost.EventListeners
{
    public class TaskCompletionListeners : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
    {
        public event EventHandler<TaskSuccessEventArgs> Success;
        public event EventHandler<TaskCompletionFailureEventArgs> Failure;
        public class TaskCompletionFailureEventArgs : EventArgs
        {
            public string Cause { get; set; }
        }

        public class TaskSuccessEventArgs : EventArgs
        {
            public Java.Lang.Object Result { get; set; }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Failure?.Invoke(this, new TaskCompletionFailureEventArgs { Cause = e.Message });
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            Success?.Invoke(this, new TaskSuccessEventArgs { Result = result });
        }
    }
}