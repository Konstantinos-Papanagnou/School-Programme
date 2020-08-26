using Android.App;
using Android.Widget;
using Android.OS;
using School_Programme.Database;
using School_Programme.Model;
using System.Collections.Generic;
using System;
using School_Programme.Adapters;
using Android.Content;

namespace School_Programme
{
    [Activity(Label = "School Programme", MainLauncher = true, Icon = "@drawable/logo")]
    public class MainActivity : Activity
    {
        private SQLite_db db;
        private ListView lv;
        private Button review, about;
        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            db = new SQLite_db(this, 1);
            lv = FindViewById<ListView>(Resource.Id.UpcommingLessonsLV);
            try
            {
                ISharedPreferences shared = GetSharedPreferences(SQLite_db.STORE, FileCreationMode.Private);
                ISharedPreferencesEditor editor = shared.Edit();
                db.UpdateDb(shared.GetInt(SQLite_db.Version, 0));
                editor.PutInt(SQLite_db.Version, shared.GetInt(SQLite_db.Version, 0) + 1);
                editor.Apply();
                Toast.MakeText(this, "Database Configured with update.", ToastLength.Long).Show();
            }
            catch (Exception)
            {
                ISharedPreferences shared = GetSharedPreferences(SQLite_db.STORE, Android.Content.FileCreationMode.Private);
                ISharedPreferencesEditor editor = shared.Edit();
                if (shared.GetInt(SQLite_db.Version, -1) == -1)
                    Toast.MakeText(this, "Unable to configure database (Either contact the developer or reinstall)", ToastLength.Long).Show();
                else
                {
                    editor.PutInt(SQLite_db.Version, (shared.GetInt(SQLite_db.Version, 0))+1);
                }
                shared.Edit().Apply();
            }
            List<Lesson> lessons = db.getUpcommingLessons(this);

            LessonAdapter adapter = new LessonAdapter(this, lessons);
            lv.Adapter = adapter;

            review = FindViewById<Button>(Resource.Id.ReviewLessonsBtn);

            review.Click += Review_Click;
            about = FindViewById<Button>(Resource.Id.About);
            about.Click += (sender, e) => 
            {
                AlertDialog.Builder confirmation = new AlertDialog.Builder(this);
                confirmation.SetMessage("School Programme v1.2\n\nSchool Programme is designed to help you organize your lessons weekly and/or per 2 weeks. The lessons' dates will be updated automatically each time you start the application.\n\nPowered By Pap Industries.\nDeveloper Konstantinos Pap.\nFor any technical issues contact me at papindustries@gmail.com\n\nCopyrights © Konstantinos Pap 2019.");
                confirmation.SetTitle("Information");
                confirmation.SetIcon(Resource.Drawable.About48);
                confirmation.Show();
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            List<Lesson> lessons = db.getUpcommingLessons(this);

            LessonAdapter adapter = new LessonAdapter(this, lessons);
            lv.Adapter = adapter;
        }

        private void Review_Click(object sender, System.EventArgs e)
        {

            StartActivity(typeof(ReviewLessons));
        }
    }
}

