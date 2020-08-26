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
using School_Programme.Database;
using School_Programme.Model;
using School_Programme.Animations.Hold_Fading_animation;
using School_Programme.Query;
using School_Programme.Activities_and_fragments.Fragments;
using School_Programme.Adapters;

namespace School_Programme
{
    public class PostponeLessonsFragment : Fragment, SortByFragment.OnInputSelectedI
    {
        // Listing bellow the objects..

        private LinearLayout _postponeListViewLL, _postponeTimeLL;
        private ScrollView _postponeScrollView;
        private ListView _postponeListView;
        private TextView _labelStatus;
        private Button _submitChangesBtn, _sortBy, _backBtn;
        private DatePicker _postponeDP;
        private Switch _postponeTimeSW;
        private EditText _startingPoint, _endingPoint;
        private ImageButton _keyboardSelector;
        private KeyboardHandeler.KeyboardType type;
        // Listing variables..
        private SQLite_db db;
        private List<Lesson> _lessons;
        private HoldFadingAnim anims;
        private int _sortMethod = 0;
        private Lesson _lessonToPostpone;
        #region HoldAnimMethods

        private void _postponeListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            anims = new HoldFadingAnim();
            if (_lessons[e.Position].Postponed)
                Dialog("Selection", "Choose what you wanna do", e.Position);
            else
            {
                anims.ControlInput(0, _postponeListViewLL, _postponeScrollView);
                InsertSetup(e.Position);
            }
            _lessonToPostpone = _lessons[e.Position];

        }

        private void Dialog(string title, string message, int position)
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(Activity);
            dialog.SetMessage(message);
            dialog.SetTitle(title);
            dialog.SetIcon(Resource.Drawable.Question);
            dialog.SetNegativeButton("Remove Status",
                delegate
                {
                    AlertDialog.Builder confirmation = new AlertDialog.Builder(Activity);
                    confirmation.SetMessage("Are you sure you want to remove the postpone status from " + _lessons[position].LessonName + "?");
                    confirmation.SetTitle("Confirmation");
                    confirmation.SetIcon(Resource.Drawable.Question);
                    confirmation.SetPositiveButton("Confirm", 
                        delegate {
                            Lesson lessonToUpdate = _lessons[position];
                            lessonToUpdate.Postponed = false;
                            lessonToUpdate.Postponedfor = DateTime.MinValue;
                            lessonToUpdate.PostponeTime = false;
                            lessonToUpdate.PostponedStartingHour = string.Empty;
                            lessonToUpdate.PostponedStartingHour = string.Empty;
                            db.UpdateLesson(lessonToUpdate);
                            UpdateLessonsFromDb();
                            RefreshListView();
                        });
                    confirmation.SetNegativeButton("Cancel", delegate { Toast.MakeText(Activity, "Removal process cancelled by user", ToastLength.Short).Show(); });
                    confirmation.Show();
                });
            dialog.SetPositiveButton("Update Status",
                delegate
                {
                    anims.ControlInput(0, _postponeListViewLL, _postponeScrollView);
                    UpdateSetup(position);
                });
            dialog.Show();
        }

        private void UpdateSetup(int position)
        {
            _labelStatus.Text = "Update Postpone Status of " + _lessons[position].LessonName;
            _postponeDP.DateTime = _lessons[position].Postponedfor;
            if (_lessons[position].PostponeTime)
            {
                _postponeTimeSW.Checked = true;
                _startingPoint.Text = _lessons[position].PostponedStartingHour;
                _endingPoint.Text = _lessons[position].PostponedEndingHour;
            }else
            {
                _postponeTimeSW.Checked = false;
                _startingPoint.Text = string.Empty;
                _endingPoint.Text = string.Empty;
            }

        }

        private void InsertSetup(int position)
        {
            _labelStatus.Text = "Insert Postpone Status for " + _lessons[position].LessonName;
            _postponeDP.DateTime = DateTime.Today;
            _startingPoint.Text = string.Empty;
            _endingPoint.Text = string.Empty;
            _postponeTimeSW.Checked = false;
        }

        private void _postponeTimeSW_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            anims.SwitchVisibilityHandeler(e.IsChecked, _postponeTimeLL);
        }

        private void _backBtn_Click(object sender, EventArgs e)
        {
            anims.ControlInput(1, _postponeListViewLL, _postponeScrollView);
        }

        private void _submitChangesBtn_Click(object sender, EventArgs e)
        {
            // validate data 
            if (DateTime.Compare(_postponeDP.DateTime, DateTime.Today) < 0) {
                Toast.MakeText(Activity, "Postpone date should be higher than " + DateTime.Today.ToShortDateString() + " !", ToastLength.Short).Show();
                return;
            }

            // Also make the validations of the time postponed settings 
            if (_postponeTimeSW.Checked)
            {
                TimeSpan span;
                if (TimeSpan.TryParse(_startingPoint.Text, out span) && TimeSpan.TryParse(_endingPoint.Text, out span))
                {
                    _lessonToPostpone.PostponeTime = true;
                    _lessonToPostpone.PostponedStartingHour = _startingPoint.Text;
                    _lessonToPostpone.PostponedEndingHour = _endingPoint.Text;
                }
                else
                {
                    Toast.MakeText(Activity, "Invalid time inputs." , ToastLength.Long).Show();
                    return;
                }
            }

            if(!_lessonToPostpone.Postponed)
                _lessonToPostpone.Postponed = true;
            _lessonToPostpone.Postponedfor = _postponeDP.DateTime;

            // call the database method to update data.
            if (db.UpdateLesson(_lessonToPostpone) > 0)
                Toast.MakeText(Activity, "Successfully postponed lesson", ToastLength.Short).Show();
            else
                Toast.MakeText(Activity, "Oops something went wrong", ToastLength.Short).Show();

            // reset the form
            anims.ControlInput(1, _postponeListViewLL, _postponeScrollView);
            UpdateLessonsFromDb();
            RefreshListView();
        }

        #endregion
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FindViews();
            // Create your fragment here
            db = new SQLite_db(Activity, 1);
            _lessons = db.getallLessons();
            Sort();
            _postponeListView.Adapter = new LessonAdapter(Activity, _lessons);
            _sortBy.Click += _sortBy_Click;
            _postponeListView.ItemLongClick += _postponeListView_ItemLongClick;
            _postponeTimeSW.CheckedChange += _postponeTimeSW_CheckedChange;
            _backBtn.Click += _backBtn_Click;
            _submitChangesBtn.Click += _submitChangesBtn_Click;

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
            handeler.ChangeKeyboardRestrictions(_startingPoint, _endingPoint, type);
        }

        #region SortByMethods
        private void _sortBy_Click(object sender, EventArgs e)
        {
            FragmentTransaction trans = FragmentManager.BeginTransaction();
            SortByFragment sortby = new SortByFragment();
            sortby.SetTargetFragment(this, 105);
            sortby.Show(trans, "Sort_By");
        }

        private void Sort()
        {
            switch (_sortMethod)
            {
                case 0: // is Sort by name Ascending
                    _lessons = Sorting.SortByName(_lessons);
                    break;
                case 1: // is Sort by name Descending
                    _lessons = Sorting.SortByNameReverse(_lessons);
                    break;
                case 2: // is Sort by date
                    _lessons = Sorting.SortByDate(_lessons);
                    break;
                case 3: // is sort by time
                    _lessons = Sorting.SortByTime(_lessons);
                    break;
                case 4:  // is sort by date and time
                    _lessons = Sorting.SortByDateAndTime(_lessons);
                    break;
                case 5:  // is sort by name and type
                    _lessons = Sorting.SortByNameAndType(_lessons);
                    break;
                case 6: // is sort by postpone Status Ascending
                    _lessons = Sorting.SortByStatus(_lessons);
                    break;
                case 7: // is sort by postpone Status Descending
                    _lessons = Sorting.SortByStatusReverse(_lessons);
                    break;
                case 8:
                    _lessons = Sorting.SortByDateAndTimeAndPostponeStartingHour(_lessons);
                    break;
            }
            RefreshListView();
        }

        private void RefreshListView()
        {
            _postponeListView.Adapter = new LessonAdapter(Activity, _lessons);
        }
        
        private void UpdateLessonsFromDb()
        {
            _lessons = db.getallLessons();
            Sort();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.PostponeLessonsFragment, container, false);
            
        }

        public void SendData(int sortMethod)
        {
            _sortMethod = sortMethod;
            Sort();
        }
        #endregion
        private void FindViews()
        {
            _postponeListViewLL = Activity.FindViewById<LinearLayout>(Resource.Id.ListViewPostponeLL);
            _postponeTimeLL = Activity.FindViewById<LinearLayout>(Resource.Id.PostponeTimePSLL);
            _postponeScrollView = Activity.FindViewById<ScrollView>(Resource.Id.PostponeScrollViewHolder);
            _postponeListView = Activity.FindViewById<ListView>(Resource.Id.PostponeListView);
            _labelStatus = Activity.FindViewById<TextView>(Resource.Id.LabelStatus);
            _submitChangesBtn = Activity.FindViewById<Button>(Resource.Id.SubmitUpdatesPBtn);
            _sortBy = Activity.FindViewById<Button>(Resource.Id.SortButton);
            _postponeDP = Activity.FindViewById<DatePicker>(Resource.Id.PostponePostponedDP);
            _postponeTimeSW = Activity.FindViewById<Switch>(Resource.Id.PostponeTimePSSW);
            _startingPoint = Activity.FindViewById<EditText>(Resource.Id.PostponeBegginingHourET);
            _endingPoint = Activity.FindViewById<EditText>(Resource.Id.PostponeEndingHourET);
            _backBtn = Activity.FindViewById<Button>(Resource.Id.PostponeFrag_BackBtn);
            _keyboardSelector = Activity.FindViewById<ImageButton>(Resource.Id.PostponeLessonsFragment_KeyboardSelection);
        }
    }
}