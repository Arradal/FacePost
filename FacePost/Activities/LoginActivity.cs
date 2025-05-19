using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using FacePost.Activities;
using FacePost.EventListeners;
using FacePost.Fragments;
using FacePost.Helpers;
using Firebase.Auth;
using System;

namespace FacePost
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class LoginActivity : AppCompatActivity
    {
        EditText emailText, passwordText;
        Button loginButton;
        FirebaseAuth mAuth;
        TaskCompletionListeners taskCompletionListeners = new TaskCompletionListeners();
        ProgressDialogFragment progressDialogue;
        TextView clickToRegister;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);

            emailText = (EditText)FindViewById(Resource.Id.emailLoginText);
            passwordText = (EditText)FindViewById(Resource.Id.passwordLoginText);
            loginButton = (Button)FindViewById(Resource.Id.loginButton);
            loginButton.Click += LoginButton_Click;
            clickToRegister = (TextView)FindViewById(Resource.Id.clickToRegister);
            clickToRegister.Click += ClickToRegister_Click;
            mAuth = AppDataHelper.GetFirebaseAuth();
        }

        private void ClickToRegister_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
            Finish();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string email, password;
            email = emailText.Text;
            password = passwordText.Text;

            if (!email.Contains("@"))
            {
                Toast.MakeText(this, "Please provide a valid email address", ToastLength.Short).Show();
                return;
            }
            else if(password.Length < 8)
            {
                Toast.MakeText(this, "Please provide a valid password", ToastLength.Short).Show();
                return;
            }

            ShowProgressDialogue("Verifying...");
            mAuth.SignInWithEmailAndPassword(email, password).AddOnSuccessListener(taskCompletionListeners)
            .AddOnFailureListener(taskCompletionListeners);

            taskCompletionListeners.Success += (success, args) =>
            {
                CloseProgressDialogue();
                StartActivity(typeof(MainActivity));
                Toast.MakeText(this, "Login was successfull", ToastLength.Short).Show();
            };

            taskCompletionListeners.Failure += (success, args) =>
            {
                CloseProgressDialogue();
                Toast.MakeText(this, "Login Failed : " + args.Cause, ToastLength.Short).Show();
            };

        }

        void ShowProgressDialogue(string status)
        {
            progressDialogue = new ProgressDialogFragment(status);
            var trans = SupportFragmentManager.BeginTransaction();
            progressDialogue.Cancelable = false;
            progressDialogue.Show(trans, "Progress");
        }

        void CloseProgressDialogue()
        {
            if(progressDialogue != null)
            {
                progressDialogue.Dismiss();
                progressDialogue = null;
            }
        }
    }
}