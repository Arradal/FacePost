using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Views;
using FacePost.Helpers;
using FacePost.Adapter;
using System.Collections.Generic;
using FacePost.DataModels;
using AndroidX.RecyclerView.Widget;
using System;
using FacePost.Activities;
using FacePost.EventListeners;
using Android.Support;
using System.Linq;
using FacePost.Fragments;
using Firebase.Storage;

namespace FacePost
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        AndroidX.AppCompat.Widget.Toolbar toolbar;
        RecyclerView postRecyclerView;
        PostAdapter postAdapter;
        List<Post> ListOfPost;
        RelativeLayout layStatus;
        ImageView cameraImage;

        PostEventListener postEventListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            toolbar = (AndroidX.AppCompat.Widget.Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            postRecyclerView = (RecyclerView)FindViewById(Resource.Id.postRecycleView);

            layStatus = (RelativeLayout)FindViewById(Resource.Id.layStatus);
            layStatus.Click += LayStatus_Click;
            cameraImage = (ImageView)FindViewById(Resource.Id.camera);
            cameraImage.Click += LayStatus_Click;

            //Retrieves full name on login
            FullnameListener fullnameListener = new FullnameListener();
            fullnameListener.FetchUser();


            //CreateData();
            FetchPost();
            
        }

        private void LayStatus_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(CreatePostActivity));
        }


        void FetchPost()
        {
            postEventListener = new PostEventListener();
            postEventListener.FetchPost();
            postEventListener.OnPostRetrieved += PostEventListener_OnPostRetrieved;
        }

        //Getting a list from the post which are in the database (The list is made in PostEventListener.cs) and put them in a recyclerview
        private void PostEventListener_OnPostRetrieved(object sender, PostEventListener.PostEventArgs e)
        {
            ListOfPost = new List<Post>();
            ListOfPost = e.Posts;

            //Sorting the list, so the newest post always are on top of the list
            if(ListOfPost != null)
            {
                ListOfPost = ListOfPost.OrderByDescending(o => o.PostDate).ToList();
            }

            SetupRecyclerView();
        }

        //Dummy data
        void CreateData()
        {
            ListOfPost = new List<Post>();
            ListOfPost.Add(new Post { PostBody = "Is this working? Hallo?", Author = "Christian Holfelt", LikeCount = 12});
            ListOfPost.Add(new Post { PostBody = "jlngj wEJFHJ wgljgkl wKJLEGF FGJKWGJ EGJJ GFEF", Author = "Johnny Kirkegaard", LikeCount = 503 });
            ListOfPost.Add(new Post { PostBody = "Yes my boi", Author = "Mufasa Uganda", LikeCount = 7 });
            ListOfPost.Add(new Post { PostBody = "Test. Test! Test? Test...", Author = "Testo Testorino", LikeCount = 23 });
        }

        void SetupRecyclerView()
        {
            postRecyclerView.SetLayoutManager(new AndroidX.RecyclerView.Widget.LinearLayoutManager(postRecyclerView.Context));
            postAdapter = new PostAdapter(ListOfPost);
            postRecyclerView.SetAdapter(postAdapter);
            postAdapter.ItemLongClick += PostAdapter_ItemLongClick;
            postAdapter.LikeClick += PostAdapter_LikeClick;
        }


        private void PostAdapter_LikeClick(object sender, PostAdapterClickEventArgs e)
        {
            Post post = ListOfPost[e.Position];
            LikeEventListener likeEventListener = new LikeEventListener(post.ID);

            if (!post.Liked)
            {
                likeEventListener.LikePost();
            }
            else
            {
                likeEventListener.UnlikePost();
            }
        }

        private void PostAdapter_ItemLongClick(object sender, PostAdapterClickEventArgs e)
        {
            string postID = ListOfPost[e.Position].ID;
            string ownerID = ListOfPost[e.Position].OwnerId;

            if(AppDataHelper.GetFirebaseAuth().CurrentUser.Uid == ownerID)
            {
                AndroidX.AppCompat.App.AlertDialog.Builder alert = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
                alert.SetTitle("Edit or Delete Post");
                alert.SetMessage("Are you sure?");

                //Delete post from Firestore
                alert.SetNegativeButton("Edit Post", (o, args) =>
                {
                    EditPostFragment editPostFragment = new EditPostFragment(ListOfPost[e.Position]);
                    var trans = SupportFragmentManager.BeginTransaction();
                    editPostFragment.Show(trans, "edit");
                });

                alert.SetPositiveButton("Delete", (o, args) =>
                {
                    AppDataHelper.GetFirestore().Collection("posts").Document(postID).Delete();

                    StorageReference storageReference = FirebaseStorage.Instance.GetReference("postsImages/" + postID);
                    storageReference.Delete();
                });

                alert.Show();
            }
        }
            
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.feed_menu, menu);
            return true;
        }

        //Checking which menu the user clicked on
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if(id == Resource.Id.action_logout)
            {
                postEventListener.RemoveListener();
                AppDataHelper.GetFirebaseAuth().SignOut();
                StartActivity(typeof(LoginActivity));
                Finish();
            }

            if(id == Resource.Id.action_refresh)
            {
                Toast.MakeText(this, "Refresh was clicked", ToastLength.Short).Show();
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}