using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace School_Programme
{
    [Activity(Label = "School Programme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ReviewLessons : Activity
    {
        private ActionBar bar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ReviewLessons);

            bar = this.ActionBar;
            bar.NavigationMode = ActionBarNavigationMode.Tabs;
            bar.SetTitle(Resource.String.ApplicationName);
            bar.SetIcon(Resource.Drawable.logo);
            bar.SetHomeButtonEnabled(true);
            bar.SetDisplayHomeAsUpEnabled(true);

            AddTab("Add Lessons", Resource.Drawable.AddLessonTab, new AddLessonsFragment());
            AddTab("Remove Lessons", Resource.Drawable.RemoveLessonTab, new RemoveLessonsFragment());
            AddTab("Postpone Lesson", Resource.Drawable.PostponeLessonTab, new PostponeLessonsFragment());
            AddTab("Modify Lesson", Resource.Drawable.ModifyDataTab, new ModifyLessonFragment());
            AddTab("View All Lessons", Resource.Drawable.ViewList, new ViewAllLessonsFragment());
        }

        private void AddTab(string tabName, int iconRes, Fragment view)
        {
            var tab = bar.NewTab();
            tab.SetText(tabName);
            tab.SetIcon(iconRes);

            tab.TabSelected += delegate (object sender, ActionBar.TabEventArgs e)
            {
                var fragment = this.FragmentManager.FindFragmentById(Resource.Id.FrameContainer);
                if (fragment != null)
                    e.FragmentTransaction.Remove(fragment);
                e.FragmentTransaction.Add(Resource.Id.FrameContainer, view);
            };
            tab.TabUnselected += delegate (object sender, ActionBar.TabEventArgs e)
            {
                e.FragmentTransaction.Remove(view);
            };
            this.ActionBar.AddTab(tab);
        }

        public override bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            if (menuItem.ItemId != Android.Resource.Id.Home)
                return base.OnOptionsItemSelected(menuItem);
            this.Finish();
            return true;
        }
    }
}