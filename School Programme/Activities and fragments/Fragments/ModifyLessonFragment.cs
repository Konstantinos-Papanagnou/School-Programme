using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using School_Programme.Model;
using School_Programme.Database;
using School_Programme.Animations.Hold_Fading_animation;
using School_Programme.Adapters;
using School_Programme.Activities_and_fragments.Fragments;

namespace School_Programme
{
    public class ModifyLessonFragment : Fragment, SortByFragment.OnInputSelectedI
    {
        //main views
        private ListView list;
        private LinearLayout ll, DateDataLL, TimeDataLL;
        private ScrollView sv;
        
        // views and data holders
        private DatePicker DayP;
        private Switch GenDataSw, Per2WeeksSw, DateDataSw, TimeDataSw;
        private Button Submit, Back, SortBy;
        private TextView ModifySelectedItemTV;
        private EditText BegginingHourET, EndingHourET, ClassroomET;

        private ImageButton _keyboardSelector;

        // General variables
        private Lesson lessonforModification;
        List<Lesson> lessons;
        private SQLite_db db;
        private HoldFadingAnim anim;
        private KeyboardHandeler.KeyboardType type;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FindViews();
            db = new SQLite_db(Activity, 1);
            RefreshListAdapter();
            anim = new HoldFadingAnim();
            

            list.ItemLongClick += List_ItemLongClick;
            GenDataSw.CheckedChange += GenDataSw_CheckedChange;
            DateDataSw.CheckedChange += DateDataSw_CheckedChange;
            TimeDataSw.CheckedChange += TimeDataSw_CheckedChange;
            Submit.Click += Submit_Click;
            Back.Click += Back_Click;
            SortBy.Click += SortBy_Click;

            type = KeyboardHandeler.GetPrefsForKeyboard(Activity);
            SetKeyboards();
            _keyboardSelector.Click += KeyboardSelector_Click;
        }

        private void KeyboardSelector_Click(object sender, EventArgs e)
        {
            if (++type > KeyboardHandeler.KeyboardType.normal)
                type = 0;
            SetKeyboards();
            KeyboardHandeler.SetPrefsForKeyboard(Activity, type);
        }

        private void SetKeyboards()
        {
            KeyboardHandeler handeler = new KeyboardHandeler(_keyboardSelector);
            handeler.ChangeImageButtonIcon(type);
            handeler.ChangeKeyboardRestrictions(BegginingHourET, EndingHourET, type);
        }

        public void SendData(int sortMethod)
        {
            Sortingitems.Sort(Activity, sortMethod, list, ref lessons);
        }

        private void SortBy_Click(object sender, EventArgs e)
        {
            FragmentTransaction trans = FragmentManager.BeginTransaction();
            SortByFragment sortby = new SortByFragment();
            sortby.SetTargetFragment(this, 105);
            sortby.Show(trans, "Sort_By");
        }

        private void Back_Click(object sender, EventArgs e)
        {
            TimeDataSw.Checked = false;
            DateDataSw.Checked = false;
            GenDataSw.Checked = false;
            lessonforModification = new Model.Lesson();
            anim.ControlInput(1, ll, sv);
            RefreshListAdapter();
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            bool update = false;
            Lesson temp = lessonforModification;
            temp.Per2Weeks = Per2WeeksSw.Checked;
            temp.day = DayP.DateTime;
            if (!string.IsNullOrWhiteSpace(ClassroomET.Text))
                temp.Classroom = ClassroomET.Text;
            else temp.Classroom = lessonforModification.Classroom;

            DateTime time;
            if (!string.IsNullOrWhiteSpace(BegginingHourET.Text))
            {   
                if (DateTime.TryParse(BegginingHourET.Text, out time))
                {
                    update = true;
                    temp.BegginingHour = BegginingHourET.Text;
                }
                else Toast.MakeText(Activity, "Wrong Time Input...", ToastLength.Long).Show();
            }
            else
            {
                update = true;
                temp.BegginingHour = lessonforModification.BegginingHour;
            }

            if (!string.IsNullOrWhiteSpace(EndingHourET.Text))
            {
                if (DateTime.TryParse(EndingHourET.Text, out time))
                {
                    temp.EndingHour = EndingHourET.Text;
                }
                else Toast.MakeText(Activity, "Wrong Time Input...", ToastLength.Long).Show();
            }
            else
            {
                update = true;
                temp.EndingHour = lessonforModification.EndingHour;
            }

            if (update)
            {
                AlertDialog.Builder confirmation = new AlertDialog.Builder(Activity);
                confirmation.SetTitle("Confirmation");
                confirmation.SetMessage("Are you sure you want to modify this lesson? " + temp.LessonName);
                confirmation.SetIcon(Resource.Drawable.Question);
                confirmation.SetPositiveButton("Yes", delegate 
                {
                    if (db.ModifyLesson(temp))
                    {
                        TimeDataSw.Checked = false;
                        DateDataSw.Checked = false;
                        GenDataSw.Checked = false;
                        Toast.MakeText(Activity, "Modification Successfull", ToastLength.Long).Show();
                        lessonforModification = new Model.Lesson();
                        anim.ControlInput(1, ll, sv);
                        RefreshListAdapter();
                    }
                });
                confirmation.SetNegativeButton("No", delegate { Toast.MakeText(Activity, "Modification cancelled by user", ToastLength.Long).Show(); });
                confirmation.Show();
            }
        }

        #region Animations

        private void TimeDataSw_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                TimeDataLL.Visibility = ViewStates.Visible;
                GenDataSw.Checked = false;
                DateDataSw.Checked = false;
            }
            else
            {
                TimeDataLL.Visibility = ViewStates.Gone;
            }
        }

        private void DateDataSw_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                DateDataLL.Visibility = ViewStates.Visible;
                GenDataSw.Checked = false;
                TimeDataSw.Checked = false;
            }
            else
            {
                DateDataLL.Visibility = ViewStates.Gone;
            }
        }

        private void GenDataSw_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if(e.IsChecked)
            {
                Per2WeeksSw.Visibility = ViewStates.Visible;
                ClassroomET.Visibility = ViewStates.Visible;
                TimeDataSw.Checked = false;
                DateDataSw.Checked = false;
            }
            else
            {
                Per2WeeksSw.Visibility = ViewStates.Gone;
                ClassroomET.Visibility = ViewStates.Gone;
            }
        }
        #endregion

        private void List_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            anim.ControlInput(0, ll, sv);
            lessonforModification = lessons[e.Position];
            lessons.Clear();
            Setup();
        }   
        
        private void Setup()
        {
            ModifySelectedItemTV.Text = "Modify Selected Item: " + lessonforModification.LessonName;
            if (lessonforModification.Per2Weeks)
                Per2WeeksSw.Checked = true;
            else Per2WeeksSw.Checked = false;
            DayP.DateTime = lessonforModification.day;
            BegginingHourET.Hint = "Current Beggining Hour = " + lessonforModification.BegginingHour;
            EndingHourET.Hint = "Current Ending Hour = " + lessonforModification.EndingHour;
            BegginingHourET.Text = string.Empty;
            EndingHourET.Text = string.Empty;
            ClassroomET.Text = string.Empty;
            ClassroomET.Hint = "Current Classroom: " + lessonforModification.Classroom;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ModifyLessonFragment, container, false);
        }



        private void RefreshListAdapter()
        {
            lessons = db.getallLessons();
            LessonAdapter adapter = new LessonAdapter(Activity, lessons);
            list.Adapter = adapter;
        }

        private void FindViews()
        {
            list = Activity.FindViewById<ListView>(Resource.Id.ModifyListView);
            ll = Activity.FindViewById<LinearLayout>(Resource.Id.ListViewModifyLL);
            sv = Activity.FindViewById<ScrollView>(Resource.Id.ModifyScrollViewHolder);
            DateDataLL = Activity.FindViewById<LinearLayout>(Resource.Id.DateDataLL);
            TimeDataLL = Activity.FindViewById<LinearLayout>(Resource.Id.ModifyTimeSettingsLL);
            DayP = Activity.FindViewById<DatePicker>(Resource.Id.ModifyDayDP);
            GenDataSw = Activity.FindViewById<Switch>(Resource.Id.GeneralDataSwitch);
            Per2WeeksSw = Activity.FindViewById<Switch>(Resource.Id.ModifyPer2Weeks);
            DateDataSw = Activity.FindViewById<Switch>(Resource.Id.DateDataSwitch);
            TimeDataSw = Activity.FindViewById<Switch>(Resource.Id.ModifyTimeSettingsSw);
            ModifySelectedItemTV = Activity.FindViewById<TextView>(Resource.Id.ModifySelectedItemTV);
            BegginingHourET = Activity.FindViewById<EditText>(Resource.Id.ModifyBegginingHourET);
            EndingHourET = Activity.FindViewById<EditText>(Resource.Id.ModifyEndingHourET);
            Submit = Activity.FindViewById<Button>(Resource.Id.SubmitChangesBtn);
            Back = Activity.FindViewById<Button>(Resource.Id.ModifyBackBtn);
            SortBy = Activity.FindViewById<Button>(Resource.Id.ModifyLessonFragment_SortButton);
            _keyboardSelector = Activity.FindViewById<ImageButton>(Resource.Id.ModifyLessonFragment_KeyboardSelection);
            ClassroomET = Activity.FindViewById<EditText>(Resource.Id.ModifyLessonFragment_ClassroomET);
        }
    }
}