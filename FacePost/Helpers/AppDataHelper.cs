﻿using Android.App;
using Android.Content;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

namespace FacePost.Helpers
{
    public static class AppDataHelper
    {

        static ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        static ISharedPreferencesEditor editor;

        public static FirebaseFirestore GetFirestore()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseFirestore database;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()

                    .SetProjectId("Removed for showcasing")
                    .SetApplicationId("Removed for showcasing")
                    .SetApiKey("Removed for showcasing")
                    .SetStorageBucket("Removed for showcasing")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                database = FirebaseFirestore.GetInstance(app);

            }
            else
            {
                database = FirebaseFirestore.GetInstance(app);
            }

            return database;
        }

        public static FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth mAuth;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()

                    .SetProjectId("Removed for showcasing")
                    .SetApplicationId("Removed for showcasing")
                    .SetApiKey("Removed for showcasing")
                    .SetStorageBucket("Removed for showcasing")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                mAuth = FirebaseAuth.Instance;

            }
            else
            {
                mAuth = FirebaseAuth.Instance;
            }

            return mAuth;
        }

        public static void SaveFullName(string fullname)
        {
            editor = preferences.Edit();
            editor.PutString("fullname", fullname);
            editor.Apply();
        }

        public static string GetFullName()
        {
            string fullname = "";
            fullname = preferences.GetString("fullname", "");
            return fullname;
        }
    }
}