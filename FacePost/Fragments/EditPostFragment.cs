using Android.OS;
using Android.Views;
using Android.Widget;
using FacePost.DataModels;
using FacePost.Helpers;
using FFImageLoading;
using Firebase.Firestore;
using System;

namespace FacePost.Fragments
{
    public class EditPostFragment : AndroidX.AppCompat.App.AppCompatDialogFragment
    {
        Post thisPost;
        EditText posteditText;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public EditPostFragment(Post post)
        {
            thisPost = post;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.editpost, container, false);

            Button editButton = (Button)view.FindViewById(Resource.Id.editButton);
            ImageView postImageView = (ImageView)view.FindViewById(Resource.Id.postImageView);
            posteditText = (EditText)view.FindViewById(Resource.Id.postEditText);
            posteditText.Text = thisPost.PostBody;
            GetImage(thisPost.DownloadUrl, postImageView);
            editButton.Click += EditButton_Click;
            return view;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            DocumentReference reference = AppDataHelper.GetFirestore().Collection("posts").Document(thisPost.ID);
            reference.Update("post_body", posteditText.Text);
            this.Dismiss();
        }

        void GetImage(string url, ImageView imageView)
        {
            ImageService.Instance.LoadUrl(url)
                .Retry(3, 200)
                .DownSample(400, 400)
                .Into(imageView);
        }
    }
}