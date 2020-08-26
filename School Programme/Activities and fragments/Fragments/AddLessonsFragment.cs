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

namespace School_Programme
{
    public class AddLessonsFragment : Fragment
    {
        private SQLite_db db;

        private LinearLayout IdentificationLayout, DateLayout, TimeLayout;
        private Switch postponedSwitch, P2WSWitch;
        private TextView postponedForTV, ChosenTypeTV, PostponedTimeTextOutputTV;
        private DatePicker postponedforPicker, dayP;
        private Button nextBtn, prevBtn;
        private EditText LessonName, Semester, BegginingHourET, EndingHourET, PostponedStartingHourET, PostponedEndingHourET, ClassroomET;
        private SeekBar Typebar;
        private ImageButton keyboardSelect;

        private short page = 0;
        private Lesson lesson;
        private bool isPostponed = false;
        KeyboardHandeler.KeyboardType type;
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            db = new SQLite_db(Activity, 1);
            FindViews();
            nextBtn.Click += NextBtn_Click;
            prevBtn.Click += PrevBtn_Click;
            lesson = new Lesson();
            ChosenTypeTV.Text = "Lesson Type: " + ChooseType(0);
            lesson.Type = ChooseType(0);
            //handle events
            postponedSwitch.CheckedChange += PostponedSwitch_CheckedChange;
            Typebar.ProgressChanged += Typebar_ProgressChanged;
            P2WSWitch.CheckedChange += P2WSWitch_CheckedChange;

            // Load and subscribe.
            type = KeyboardHandeler.GetPrefsForKeyboard(Activity);
            SetKeyboards();
            keyboardSelect.Click += KeyboardSelect_Click;
        }


        private void KeyboardSelect_Click(object sender, EventArgs e)
        {
            if (++type > KeyboardHandeler.KeyboardType.normal)
                type = 0;
            SetKeyboards();
            KeyboardHandeler.SetPrefsForKeyboard(Activity, type);
        }

        private void SetKeyboards()
        {
            KeyboardHandeler handeler = new KeyboardHandeler(keyboardSelect);
            handeler.ChangeImageButtonIcon(type);
            handeler.ChangeKeyboardRestrictions(BegginingHourET, EndingHourET, type);
            handeler.ChangeKeyboardRestrictions(PostponedStartingHourET, PostponedEndingHourET, type);
        }

        private void P2WSWitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            lesson.Per2Weeks = e.IsChecked;
        }

        private void Typebar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            ChosenTypeTV.Text = "Lesson Type: " + ChooseType(e.Progress);
        }

        private void PrevBtn_Click(object sender, EventArgs e)
        {
            nextBtn.Text = "Next";
            page--;
            ControlInput();
        }

        private bool ConfirmData()
        {
            if (page == 0)
            {
                if (!string.IsNullOrWhiteSpace(LessonName.Text) && !string.IsNullOrWhiteSpace(Semester.Text))
                {
                    lesson.LessonName = LessonName.Text;
                    lesson.Semester = Convert.ToInt32(Semester.Text);
                    lesson.Type = ChooseType(Typebar.Progress);
                    lesson.Classroom = ClassroomET.Text;
                    return true;
                }
                else return false;
            }
            else if (page == 1)
            {
                lesson.day = dayP.DateTime;
                if (!isPostponed)
                {
                    lesson.Postponed = false;
                }
                else
                {
                    lesson.Postponed = true;
                    lesson.Postponedfor = postponedforPicker.DateTime;
                }
                return true;
            }
            else
            {
                if(isPostponed)
                {
                    if (!isTimeInputCorrect(PostponedStartingHourET.Text, PostponedEndingHourET.Text)) return false;
                    lesson.PostponedStartingHour = PostponedStartingHourET.Text;
                    lesson.PostponedEndingHour = PostponedEndingHourET.Text;
                }
                if (!isTimeInputCorrect(BegginingHourET.Text, EndingHourET.Text)) return false;
                lesson.BegginingHour = BegginingHourET.Text;
                lesson.EndingHour = EndingHourET.Text;
                return true;
            }
        }

        private bool isTimeInputCorrect(string startingHour, string endingHour)
        {
            if (!string.IsNullOrWhiteSpace(startingHour) && !string.IsNullOrWhiteSpace(endingHour))
            {
                DateTime time;
                if (DateTime.TryParse(BegginingHourET.Text, out time) && DateTime.TryParse(EndingHourET.Text, out time)) return true;
                else return false;
            }
            return false;
        }

        private string ChooseType(int progress)
        {
            if (progress == 0) return "Theory";
            else if (progress == 1) return "Excercise/Practice";
            else return "Lab";
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            if (ConfirmData())
            {
                if (page < 2)
                {
                    page++;
                    ControlInput();
                }
                else
                {
                    if (db.insertLesson(lesson))
                    {
                        ClearForm();
                        Toast.MakeText(Activity, "Sumbited", ToastLength.Long).Show();
                    }
                    else { Toast.MakeText(Activity, "Oops, something went wrong...", ToastLength.Long).Show();
                    }
                }
            }
            else
            {
                Toast.MakeText(Activity, "Oops, it seems like you missed a field or your inputs are not valid.", ToastLength.Long).Show();
            }
        }

        private void ClearForm()
        {
            if (isPostponed)
            {
                PostponedTimeTextOutputTV.Visibility = ViewStates.Gone;
                PostponedStartingHourET.Visibility = ViewStates.Gone;
                PostponedEndingHourET.Visibility = ViewStates.Gone;
                PostponedEndingHourET.Text = string.Empty;
                PostponedStartingHourET.Text = string.Empty;
            }
            lesson = new Lesson();
            LessonName.Text = string.Empty;
            Semester.Text = string.Empty;
            BegginingHourET.Text = string.Empty;
            EndingHourET.Text = string.Empty;
            Typebar.Progress = 0;
            ChosenTypeTV.Text = "Lesson Type: " + ChooseType(Typebar.Progress);
            isPostponed = false;
            postponedSwitch.Checked = false;
            P2WSWitch.Checked = false;
            nextBtn.Text = "Next";
            page = 0;
            ControlInput();
            ClassroomET.Text = string.Empty;
        }

        private void ControlInput()
        {
            switch (page)
            {
                case 0:
                    prevBtn.Visibility = ViewStates.Gone;
                    IdentificationLayout.Visibility = ViewStates.Visible;
                    DateLayout.Visibility = ViewStates.Gone;
                    TimeLayout.Visibility = ViewStates.Gone;
                    break;
                case 1:
                    prevBtn.Visibility = ViewStates.Visible;
                    IdentificationLayout.Visibility = ViewStates.Gone;
                    DateLayout.Visibility = ViewStates.Visible;
                    TimeLayout.Visibility = ViewStates.Gone;
                    break;
                case 2:
                    if (isPostponed)
                    {
                        PostponedTimeTextOutputTV.Visibility = ViewStates.Visible;
                        PostponedStartingHourET.Visibility = ViewStates.Visible;
                        PostponedEndingHourET.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        PostponedTimeTextOutputTV.Visibility = ViewStates.Gone;
                        PostponedStartingHourET.Visibility = ViewStates.Gone;
                        PostponedEndingHourET.Visibility = ViewStates.Gone;
                    }
                    nextBtn.Text = "Submit";
                    prevBtn.Visibility = ViewStates.Visible;
                    IdentificationLayout.Visibility = ViewStates.Gone;
                    DateLayout.Visibility = ViewStates.Gone;
                    TimeLayout.Visibility = ViewStates.Visible;
                    break;
            }
        }

        private void FindViews()
        {
            IdentificationLayout = Activity.FindViewById<LinearLayout>(Resource.Id.IdentificationData);
            DateLayout = Activity.FindViewById<LinearLayout>(Resource.Id.DateData);
            TimeLayout = Activity.FindViewById<LinearLayout>(Resource.Id.TimeData);
            postponedSwitch = Activity.FindViewById<Switch>(Resource.Id.PostponedSw);
            postponedForTV = Activity.FindViewById<TextView>(Resource.Id.PostponedForTV);
            postponedforPicker = Activity.FindViewById<DatePicker>(Resource.Id.PostponedForDP);
            nextBtn = Activity.FindViewById<Button>(Resource.Id.NextButton);
            prevBtn = Activity.FindViewById<Button>(Resource.Id.PreviousButton);
            P2WSWitch = Activity.FindViewById<Switch>(Resource.Id.Per2WeeksSW);
            ChosenTypeTV = Activity.FindViewById<TextView>(Resource.Id.ChosenType);
            dayP = Activity.FindViewById<DatePicker>(Resource.Id.DayDP);
            LessonName = Activity.FindViewById<EditText>(Resource.Id.LessonNameET);
            Semester = Activity.FindViewById<EditText>(Resource.Id.SemesterET);
            Typebar = Activity.FindViewById<SeekBar>(Resource.Id.TypeSB);
            BegginingHourET = Activity.FindViewById<EditText>(Resource.Id.BegginingHourET);
            EndingHourET = Activity.FindViewById<EditText>(Resource.Id.EndingHourET);
            PostponedEndingHourET = Activity.FindViewById<EditText>(Resource.Id.AddLesson_PostponedEndingHourET);
            PostponedStartingHourET = Activity.FindViewById<EditText>(Resource.Id.AddLesson_PostponedBegginingHourET);
            PostponedTimeTextOutputTV = Activity.FindViewById<TextView>(Resource.Id.AddLesson_PostponedTimeTextOutput);
            keyboardSelect = Activity.FindViewById<ImageButton>(Resource.Id.AddLessonFragment_KeyboardSelection);
            ClassroomET = Activity.FindViewById<EditText>(Resource.Id.AddLessonFragment_ClassroomET);
        }
        private void PostponedSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (postponedSwitch.Checked)
            {
                isPostponed = true;
                postponedForTV.Visibility = ViewStates.Visible;
                postponedforPicker.Visibility = ViewStates.Visible;
            }
            else
            {
                isPostponed = false;
                postponedForTV.Visibility = ViewStates.Gone;
                postponedforPicker.Visibility = ViewStates.Gone;
            }
            lesson.PostponeTime = isPostponed;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
             return inflater.Inflate(Resource.Layout.AddLessonFragment, container, false);

        }
    }
}