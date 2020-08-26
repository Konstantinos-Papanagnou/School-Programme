using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using School_Programme.Model;
using School_Programme.Database;
using School_Programme.Adapters;
using School_Programme.Activities_and_fragments.Fragments;
namespace School_Programme
{
    public class RemoveLessonsFragment : Fragment, SortByFragment.OnInputSelectedI
    {
        private SQLite_db db;
        private List<Lesson> lessons;
        private ListView RemoveListView;
        private Button RemoveAllBtn;
        private Button SortBy;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FindViews();
            db = new SQLite_db(Activity, 1);
            lessons = db.getallLessons();
            LessonAdapter adapter = new LessonAdapter(Activity, lessons);
            RemoveListView.Adapter = adapter;
            SortBy.Click += SortBy_Click;
            RemoveListView.ItemLongClick += RemoveListView_ItemLongClick;
            RemoveAllBtn.Click += RemoveAllBtn_Click;
        }

        private void SortBy_Click(object sender, EventArgs e)
        {
            FragmentTransaction trans = FragmentManager.BeginTransaction();
            SortByFragment sortby = new SortByFragment();
            sortby.SetTargetFragment(this, 105);
            sortby.Show(trans, "Sort_By");
        }

        public void SendData(int sortMethod)
        {
            Sortingitems.Sort(Activity, sortMethod, RemoveListView, ref lessons);
        }

        private void RemoveAllBtn_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder confirmation = new AlertDialog.Builder(Activity);
            confirmation.SetMessage("Are you sure you want to delete everything?");
            confirmation.SetTitle("Confirmation");
            confirmation.SetIcon(Resource.Drawable.Question);
            confirmation.SetPositiveButton("Yes",
                delegate
                {
                    foreach (Lesson l in lessons)
                        db.RemoveLesson(l);
                    refreshList();
                });
            confirmation.SetNegativeButton("No", delegate { Toast.MakeText(Activity, "Removal process canceled by user", ToastLength.Long).Show(); });
            confirmation.Show();
        }

        private void refreshList()
        {
            lessons = db.getallLessons();
            LessonAdapter adapter = new LessonAdapter(Activity, lessons);
            RemoveListView.Adapter = adapter;
        }

        private void RemoveListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            AlertDialog.Builder confirmation = new AlertDialog.Builder(Activity);
            confirmation.SetMessage("Are you sure you want to delete the selected item? " + lessons[e.Position].LessonName + " [" + lessons[e.Position].Type + "]");
            confirmation.SetTitle("Confirmation");
            confirmation.SetIcon(Resource.Drawable.Question);
            confirmation.SetPositiveButton("Yes",
                delegate
                {
                    if (db.RemoveLesson(lessons[e.Position]))
                    {
                        Toast.MakeText(Activity, "Item deleted successfully!", ToastLength.Long).Show();
                        refreshList();
                    }
                    else Toast.MakeText(Activity, "Oops, Something went wrong...", ToastLength.Long).Show();
                });
            confirmation.SetNegativeButton("No", delegate { Toast.MakeText(Activity, "Removal process canceled by user", ToastLength.Long).Show(); });
            confirmation.Show();
        }

        public override void OnResume()
        {
            base.OnResume();
            // refresh the list.
            refreshList();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.RemoveLessonFragment, container, false);
        }

        private void FindViews()
        {
            RemoveListView = Activity.FindViewById<ListView>(Resource.Id.RemoveListView);
            RemoveAllBtn = Activity.FindViewById<Button>(Resource.Id.RemoveAllBtn);
            SortBy = Activity.FindViewById<Button>(Resource.Id.RemoveLessonFragment_SortButton);
        }
    }
}