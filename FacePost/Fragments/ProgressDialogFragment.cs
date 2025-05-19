using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Runtime;
using Android.Util;
using AndroidX.Fragment.App;

namespace FacePost.Fragments
{
    public class ProgressDialogFragment : DialogFragment
    {
        string status;
        public ProgressDialogFragment(string _status)
        {
            status = _status;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.progress, container, false);
            TextView statusTextView = (TextView)view.FindViewById(Resource.Id.progressStatus);
            statusTextView.Text = status;
            return view;
        }
    }
}