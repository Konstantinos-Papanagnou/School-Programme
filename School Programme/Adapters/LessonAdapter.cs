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
using School_Programme.Model;
using Android.Graphics;

namespace School_Programme.Adapters
{
    public class LessonAdapter : BaseAdapter
    {

        Context context;
        private List<Lesson> lessons;

        public LessonAdapter(Context context, List<Lesson> lessons)
        {
            this.context = context;
            this.lessons = lessons;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        //GetView is called as many times as is the value of position which is the elements to load.
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            LessonAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as LessonAdapterViewHolder;

            if (holder == null)
            {
                holder = new LessonAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();

                view = inflater.Inflate(Resource.Layout.LessonVIewRow, parent, false);
                holder.txtLessonName = view.FindViewById<TextView>(Resource.Id.LessonNameView);
                holder.txtDay = view.FindViewById<TextView>(Resource.Id.DayView);
                holder.txtBegginingHour = view.FindViewById<TextView>(Resource.Id.BeggingHourView);
                holder.txtEndingHour = view.FindViewById<TextView>(Resource.Id.EndingHourView);
                holder.txtSemester = view.FindViewById<TextView>(Resource.Id.SemesterView);
                holder.txtPer2Weeks = view.FindViewById<TextView>(Resource.Id.Per2WeeksView);
                holder.txtType = view.FindViewById<TextView>(Resource.Id.TypeView);
                holder.txtPostponed = view.FindViewById<TextView>(Resource.Id.PostponedAdapterTV);
                holder.txtPostponedfor = view.FindViewById<TextView>(Resource.Id.PostponedForAdapterTV);
                holder.txtPostponedStartingHour = view.FindViewById<TextView>(Resource.Id.PostponedStartingHourAdapterTV);
                holder.txtPostponedEndingHour = view.FindViewById<TextView>(Resource.Id.PostponedEndingHourAdapterTV);
                holder.txtClassroom = view.FindViewById<TextView>(Resource.Id.ClassroomView);
                view.Tag = holder;
            }
            //setting their visibility to gone for each itteration.
            holder.txtPostponed.Visibility = ViewStates.Gone;
            holder.txtPostponedfor.Visibility = ViewStates.Gone;
            holder.txtPostponedEndingHour.Visibility = ViewStates.Gone;
            holder.txtPostponedStartingHour.Visibility = ViewStates.Gone;

            DateTime date = lessons[position].day;
            Color color;
            if (lessons[position].Type == "Theory")
                color = Color.White;
            else if (lessons[position].Type == "Excercise/Practice")
                color = Color.Black;
            else
                color = Color.Yellow;

            if (lessons[position].Postponed || lessons[position].PostponeTime)
                color = Color.DarkRed;
            // coloring the fields based on type of lesson: white for theory, yellow for lab, black for exercise/practice
            // and red for postponed status
            holder.txtLessonName.SetTextColor(color);
            holder.txtDay.SetTextColor(color);
            holder.txtBegginingHour.SetTextColor(color);
            holder.txtEndingHour.SetTextColor(color);
            holder.txtPer2Weeks.SetTextColor(color);
            holder.txtSemester.SetTextColor(color);
            holder.txtType.SetTextColor(color);
            holder.txtClassroom.SetTextColor(color);

            holder.txtLessonName.Text = "Lesson: " + lessons[position].LessonName;
            holder.txtDay.Text = "Day: " + date.ToLongDateString();
            holder.txtBegginingHour.Text = "Starting at: " + lessons[position].BegginingHour;
            holder.txtEndingHour.Text = "Ending at: " + lessons[position].EndingHour;
            holder.txtSemester.Text = "Semester: " + lessons[position].Semester.ToString();
            holder.txtClassroom.Text = "Classroom: " + lessons[position].Classroom;
            if (lessons[position].Per2Weeks)
                holder.txtPer2Weeks.Text = "Per 2 Weeks: Yes";
            else
                holder.txtPer2Weeks.Text = "Per 2 Weeks: No";
            holder.txtType.Text = "Type: " + lessons[position].Type;

            if (lessons[position].Postponed)
            {
                holder.txtPostponed.SetTextColor(color);
                holder.txtPostponed.Visibility = ViewStates.Visible;
                holder.txtPostponed.Text = "Postponed: Yes";
               // if(lessons[position].Postponedfor != DateTime.MinValue)
               // {
                holder.txtPostponedfor.SetTextColor(color);
                holder.txtPostponedfor.Visibility = ViewStates.Visible;
                holder.txtPostponedfor.Text = "Postponed for: " + lessons[position].Postponedfor.ToLongDateString();    
               // }
                if (lessons[position].PostponeTime)
                {
                    holder.txtPostponedEndingHour.Visibility = ViewStates.Visible;
                    holder.txtPostponedStartingHour.Visibility = ViewStates.Visible;
                    holder.txtPostponedStartingHour.SetTextColor(color);
                    holder.txtPostponedEndingHour.SetTextColor(color);
                    holder.txtPostponedStartingHour.Text = "Postponed Starting Hour: " + lessons[position].PostponedStartingHour;
                    holder.txtPostponedEndingHour.Text = "Postponed Ending Hour: " + lessons[position].PostponedEndingHour;
                }
            }


            return view;
        }

        public override int Count
        {
            get
            {
                return lessons.Count;
            }
        }

    }

    internal class LessonAdapterViewHolder : Java.Lang.Object
    {
        public TextView txtLessonName { get; set; }
        public TextView txtDay { get; set; }
        public TextView txtBegginingHour { get; set; }
        public TextView txtEndingHour { get; set; }
        public TextView txtSemester { get; set; }
        public TextView txtPer2Weeks { get; set; }
        public TextView txtPostponed { get; set; }
        public TextView txtPostponedfor { get; set; }
        public TextView txtType { get; set; }
        public TextView txtPostponedStartingHour { get; set; }
        public TextView txtPostponedEndingHour { get; set; }
        public TextView txtClassroom { get; set; }
    }
}