using System.Collections.Generic;
using Android.Content;
using Android.Widget;
using School_Programme.Model;
using School_Programme.Query;

namespace School_Programme.Adapters
{
    public class Sortingitems
    {
        public static readonly string[] items =
        {
            "Name (Ascending)",
            "Name (Descending)",
            "Date",
            "Time",
            "Date and Time",
            "Name And Type",
            "Postpone Status (Ascending)",
            "Postpone Status (Descending)",
            "Auto mix Postpone Status/Normal Sort"
        };

        public static void Sort(Context context, int sortMethod, ListView lv, ref List<Lesson> lessons)
        {
            switch (sortMethod)
            {
                case 0: // is Sort by name Ascending
                    lessons = Sorting.SortByName(lessons);
                    break;
                case 1: // is Sort by name Descending
                    lessons = Sorting.SortByNameReverse(lessons);
                    break;
                case 2: // is Sort by date
                    lessons = Sorting.SortByDate(lessons);
                    break;
                case 3: // is sort by time
                    lessons = Sorting.SortByTime(lessons);
                    break;
                case 4:  // is sort by date and time
                    lessons = Sorting.SortByDateAndTime(lessons);
                    break;
                case 5:  // is sort by name and type
                    lessons = Sorting.SortByNameAndType(lessons);
                    break;
                case 6: // is sort by postpone Status Ascending
                    lessons = Sorting.SortByStatus(lessons);
                    break;
                case 7: // is sort by postpone Status Descending
                    lessons = Sorting.SortByStatusReverse(lessons);
                    break;
                case 8:
                    lessons = Sorting.SortByDateAndTimeAndPostponeStartingHour(lessons);
                    break;
            }
            lv.Adapter = new LessonAdapter(context, lessons);
        }
    }      
}