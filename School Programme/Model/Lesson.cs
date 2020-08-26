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

namespace School_Programme.Model
{
    public struct Lesson
    {
        public int LessonID { get; set; }
        public string LessonName { get; set; }
        public string BegginingHour { get; set; }
        public string EndingHour { get; set; }
        public DateTime day { get; set; }
        public int Semester { get; set; }
        public bool Per2Weeks { get; set; }
        public bool Postponed { get; set; }
        public DateTime Postponedfor { get; set; }
        public string Type { get; set; }
        public bool PostponeTime { get; set; }
        public string PostponedStartingHour { get; set; }
        public string PostponedEndingHour { get; set; }
        public string Classroom { get; set; }
        
    }
}