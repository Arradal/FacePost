using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;
using System;
using Java.Util;
using Firebase.Auth;
using FacePost.EventListeners;
using FacePost.Helpers;
using FacePost.Fragments;

namespace FacePost.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class RegistrationActivity : AppCompatActivity
    {

        Button registerButton;
        EditText fullnameText, emailText, passwordText, confirmPasswordText;
        FirebaseFirestore database;
        FirebaseAuth mAuth;
        string fullname, email, password, confirm;
        TaskCompletionListeners taskCompletionListeners = new TaskCompletionListeners();
        ProgressDialogFragment progressDialogue;
        TextView clickHereToLogin;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.register);
            fullnameText = (EditText)FindViewById(Resource.Id.fullNameRegText);
            emailText = (EditText)FindViewById(Resource.Id.emailRegText);
            passwordText = (EditText)FindViewById(Resource.Id.passwordRegText);
            confirmPasswordText = (EditText)FindViewById(Resource.Id.confirmPasswordRegText);
            clickHereToLogin = (TextView)FindViewById(Resource.Id.clickToLogin);
            clickHereToLogin.Click += clickHereToLogin_Click;

            registerButton = FindViewById<Button>(Resource.Id.registerButton);
            registerButton.Click += RegisterButton_Click;
            database = AppDataHelper.GetFirestore();
            mAuth = AppDataHelper.GetFirebaseAuth();

        }

        private void clickHereToLogin_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LoginActivity));
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            fullname = fullnameText.Text;
            email = emailText.Text;
            password = passwordText.Text;
            confirm = confirmPasswordText.Text;

            if (fullname.Length < 4)
            {
                Toast.MakeText(this, "Please enter a valid name", ToastLength.Short).Show();
                return;
            }
            else if (!email.Contains("@"))
            {
                Toast.MakeText(this, "Please enter a valid email address", ToastLength.Short).Show();
                return;
            }
            else if (password.Length < 8)
            {
                Toast.MakeText(this, "Password must be atleast 8 characters", ToastLength.Short).Show();
                return;
            }
            else if (password != confirm)
            {
                Toast.MakeText(this, "Password does not match", ToastLength.Short).Show();
                return;
            }

            //Perform Registration
            ShowProgressDialogue("Registering...");
            mAuth.CreateUserWithEmailAndPassword(email, password).AddOnSuccessListener(this, taskCompletionListeners)
                .AddOnFailureListener(this, taskCompletionListeners);

            //Lambda expression used to implement eventhandler
            //Registration Success Callback
            taskCompletionListeners.Success += (success, args) =>
            {
                HashMap userMap = new HashMap();
                userMap.Put("email", email);
                userMap.Put("fullname", fullname);

                DocumentReference userReference = database.Collection("users").Document(mAuth.CurrentUser.Uid);
                userReference.Set(userMap);
                CloseProgressDialogue();
                StartActivity(typeof(MainActivity));
                Finish();
            };

            //Registration Failure Callback
            taskCompletionListeners.Failure += (failure, args) =>
           {
               CloseProgressDialogue();
               Toast.MakeText(this, "Registration Failed : " + args.Cause, ToastLength.Short).Show();
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
            if (progressDialogue != null)
            {
                progressDialogue.Dismiss();
                progressDialogue = null;
            }
        }

    }
}