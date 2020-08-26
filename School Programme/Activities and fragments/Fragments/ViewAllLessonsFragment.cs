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
using School_Programme.Query;

namespace School_Programme
{
    public class ViewAllLessonsFragment : Fragment, SortByFragment.OnInputSelectedI
    {
        private Button _sortBtn;
        private ListView _list;
        private List<Lesson> _lessons;
        private SQLite_db db;
    
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _list = Activity.FindViewById(Resource.Id.ViewAllFragment_ListView) as ListView;
            _sortBtn = Activity.FindViewById(Resource.Id.ViewAllFragment_SortButton) as Button;

            db = new SQLite_db(Activity, 1);
            _lessons = db.getallLessons();

            _list.Adapter = new LessonAdapter(Activity, _lessons);
            _sortBtn.Click += _sortBtn_Click;
        }

        private void _sortBtn_Click(object sender, EventArgs e)
        {
            FragmentTransaction trans = FragmentManager.BeginTransaction();
            SortByFragment sortby = new SortByFragment();
            sortby.SetTargetFragment(this, 105);
            sortby.Show(trans, "Sort_By");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.ViewAllLessonsFragment, container, false);
        }

        public void SendData(int sortMethod)
        {
            Sortingitems.Sort(Activity, sortMethod, _list, ref _lessons);
        }
    }
}