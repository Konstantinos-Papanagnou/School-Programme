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

namespace School_Programme.Query
{
    public abstract class Sorting
    {

        /// <summary>
        /// Sort by Date in ascending order
        /// </summary>
        /// <param name="lessons">The list of lessons to be sorted</param>
        /// <returns>The Sorted List</returns>
        public static List<Lesson> SortByDate(List<Lesson> lessons)
        {
            lessons.Sort((x, y) => DateTime.Compare(x.day, y.day));

            return lessons;
        }

        /// <summary>
        /// Sort by Time in ascending order
        /// </summary>
        /// <param name="lessons">The list of lessons to be sorted</param>
        /// <returns>The Sorted List</returns>
        public static List<Lesson> SortByTime(List<Lesson> lessons)
        {
            lessons.Sort((x, y) => TimeSpan.Compare(TimeSpan.Parse(x.BegginingHour), TimeSpan.Parse(y.BegginingHour)));
            return lessons;
        }

        /// <summary>
        /// Sort by Date and Time in ascending onrder
        /// </summary>
        /// <param name="lessons">The list of lessons to be sorted</param>
        /// <returns>The Sorted List</returns>
        public static List<Lesson> SortByDateAndTime(List<Lesson> lessons)
        {
            lessons = lessons.OrderBy(x => x.day).ThenBy(y => TimeSpan.Parse(y.BegginingHour)).ToList();
            return lessons;
        }

        /// <summary>
        /// Sorts automatically in mix of postponed and normal order
        /// </summary>
        /// <param name="lessons">List of lessons to sort</param>
        /// <returns>The Sorted List</returns>
        public static List<Lesson> SortByDateAndTimeAndPostponeStartingHour(List<Lesson> lessons)
        {
            return lessons.OrderBy(x => x.Postponed? x.Postponedfor: x.day).ThenBy(m => m.PostponeTime? TimeSpan.Parse(m.PostponedStartingHour): TimeSpan.Parse(m.BegginingHour)).ToList();
        }

        /// <summary>
        /// Sort by name and type in asceding order 
        /// </summary>
        /// <param name="lessons">The list of lessons to be sorted</param>
        /// <returns>The Sorted List</returns>
        public static List<Lesson> SortByNameAndType(List<Lesson> lessons)
        {
            return lessons.OrderBy(x => x.LessonName).ThenBy(y => y.Type).ToList();
        }

        /// <summary>
        /// Postponed items First
        /// </summary>
        /// <param name="lessons">The list of lessons to be sorted</param>
        /// <returns>The Sorted List</returns>
        public static List<Lesson> SortByStatus(List<Lesson> lessons)
        {
            return lessons.OrderBy(x => x.Postponed).ToList();
        }

        /// <summary>
        /// Postponed items last
        /// </summary>
        /// <param name="lessons">The list of lessons to be sorted</param>
        /// <returns>The Sorted List</returns>
        public static List<Lesson> SortByStatusReverse(List<Lesson> lessons)
        {
            return lessons.OrderByDescending(x => x.Postponed).ToList();
        }

        /// <summary>
        /// Sort by Name in ascending order
        /// </summary>
        /// <param name="lessons">The list of lessons to be sorted</param>
        /// <returns>The Sorted List</returns>
        public static List<Lesson> SortByName(List<Lesson> lessons)
        {
            return lessons.OrderBy(x => x.LessonName).ToList();
        }

        /// <summary>
        /// Sort by Name in descending order
        /// </summary>
        /// <param name="lessons">The list of lessons to be sorted</param>
        /// <returns>The Sorted List</returns>
        public static List<Lesson> SortByNameReverse(List<Lesson> lessons)
        {
            return lessons.OrderByDescending(x => x.LessonName).ToList();
        }
    }
}