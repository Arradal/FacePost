using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using FacePost.Helpers;
using Firebase.Auth;

namespace FacePost.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        protected override void OnResume()
        {
            base.OnResume();

            FirebaseUser currentUser = AppDataHelper.GetFirebaseAuth().CurrentUser;

            if(currentUser != null)
            {
                StartActivity(typeof(MainActivity));
                Finish();
            }
            else
            {
                StartActivity(typeof(LoginActivity));
            }
        }

    }
}