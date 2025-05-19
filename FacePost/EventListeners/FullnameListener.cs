using Android.Gms.Tasks;
using FacePost.Helpers;
using Firebase.Firestore;
namespace FacePost.EventListeners
{
    public class FullnameListener : Java.Lang.Object, IOnSuccessListener
    {
        public void FetchUser()
        {
            AppDataHelper.GetFirestore().Collection("users").Document(AppDataHelper.GetFirebaseAuth().CurrentUser.Uid).Get()
                .AddOnSuccessListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            DocumentSnapshot snapshot = (DocumentSnapshot)result;
            if (snapshot.Exists())
            {
                string fullname = snapshot.Get("fullname").ToString();
                AppDataHelper.SaveFullName(fullname);
            }
        }
    }
}