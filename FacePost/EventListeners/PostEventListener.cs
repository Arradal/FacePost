using Android.Gms.Tasks;
using Android.Runtime;
using FacePost.DataModels;
using FacePost.Helpers;
using Firebase.Firestore;
using System;
using System.Collections.Generic;

namespace FacePost.EventListeners
{
    public class PostEventListener : Java.Lang.Object, IOnSuccessListener, IEventListener
    {
        public List<Post> ListOfPost = new List<Post>();

        public event EventHandler<PostEventArgs> OnPostRetrieved;

        public class PostEventArgs : EventArgs
        {
            public List<Post> Posts { get; set; }
        }

        public void FetchPost()
        {
            //Retrieve Only Once

            //AppDataHelper.GetFirestore().Collection("posts").Get()
            //    .AddOnSuccessListener(this);

            AppDataHelper.GetFirestore().Collection("posts").AddSnapshotListener(this);
        }

        public void RemoveListener()
        {
            var listener = AppDataHelper.GetFirestore().Collection("posts").AddSnapshotListener(this);
            listener.Remove();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            OrganizeData(result);
        }

        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            OrganizeData(value);
        }

        void OrganizeData(Java.Lang.Object Value)
        {
            var snapshot = (QuerySnapshot)Value;

            //Checks the entire list of posts through the database
            if (!snapshot.IsEmpty)
            {
                //Make sure post arent duplicated
                if(ListOfPost.Count > 0)
                {
                    ListOfPost.Clear();
                }

                foreach (DocumentSnapshot item in snapshot.Documents)
                {
                    Post post = new Post();
                    post.ID = item.Id;

                    //If a user is missing one or more parameters for a post, we can risc the app crashing
                    //We can handle it like this:
                    post.PostBody = item.Get("post_body") != null ? item.Get("post_body").ToString() : "";

                    post.Author = item.Get("author") != null ? item.Get("author").ToString() : "";
                    post.ImageId = item.Get("image_id") != null ? item.Get("image_id").ToString() : "";
                    post.OwnerId = item.Get("owner_id") != null ? item.Get("owner_id").ToString() : "";
                    post.DownloadUrl = item.Get("download_url") != null ? item.Get("download_url").ToString() : "";
                    string datestring = item.Get("post_date") != null ? item.Get("post_date").ToString() : "";
                    post.PostDate = DateTime.Parse(datestring);

                    //Figures out which posts have been liked and how many have liked a post
                    var data = item.Get("likes") != null ? item.Get("likes") : null;
                    if (data != null)
                    {
                        var dictionaryFromHashMap = new Android.Runtime.JavaDictionary<string, string>(data.Handle, JniHandleOwnership.DoNotRegister);

                        string uid = AppDataHelper.GetFirebaseAuth().CurrentUser.Uid;

                        post.LikeCount = dictionaryFromHashMap.Count;
                        if (dictionaryFromHashMap.Contains(uid))
                        {
                            post.Liked = true;
                        }
                    }

                    //After all data has been fetched, we insert it to a list

                    ListOfPost.Add(post);
                }

                OnPostRetrieved?.Invoke(this, new PostEventArgs { Posts = ListOfPost });

            }
        }

    }
}