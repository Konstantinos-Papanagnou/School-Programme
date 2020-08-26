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
using Android.Database.Sqlite;
using Android.Database;
using Java.Lang;
using System.IO;
using School_Programme.Model;
using School_Programme.Query;
using System.Globalization;

namespace School_Programme.Database
{
    public class SQLite_db : SQLiteOpenHelper
    {

        public static readonly string STORE = "DatabaseUpdated";
        public static readonly string Version = "Version";
        private const string DATABASE_NAME = "Lessons.db";
        private const string LESSON_TABLE_NAME = "Lessons";
        private const string LESSONID = "ID";
        private const string LESSONNAME = "LESSON_NAME";
        private const string BEGINNINGHOUR = "BEGGINING_HOUR";
        private const string ENDINGHOUR = "ENDING_HOUR";
        private const string DAY = "DAY";
        private const string SEMESTER = "SEMESTER";
        private const string PER2WEEKS = "PER2WEEKS";
        private const string POSTPONED = "POSTPONED";
        private const string POSTPONEDFOR = "POSTPONEDFOR";
        private const string TYPE = "TYPE";
        private const string POSTPONETIME = "POSTPONETIME";
        private const string POSTPONEDSTARTINGHOUR = "POSTPONEDSTARTINGHOUR";
        private const string POSTPONEDENDINGHOUR = "POSTPONEDENDINGHOUR";
        private const string CLASSROOM = "CLASSROOM";
        public SQLite_db(Context context, int version) : base(context, DATABASE_NAME, null, version)
        { }

        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL("CREATE TABLE " + LESSON_TABLE_NAME + "  (" + LESSONID + " INTEGER PRIMARY KEY AUTOINCREMENT, "
                + LESSONNAME + " TEXT, " + BEGINNINGHOUR + " TEXT, " + ENDINGHOUR + " TEXT, " + DAY + " DATE, "
                + SEMESTER + " INTEGER, " + PER2WEEKS + " BOOLEAN, " + POSTPONED + " BOOLEAN, " + POSTPONEDFOR
                + " DATE, " + TYPE + " TEXT, " + POSTPONETIME + " BOOLEAN, " + POSTPONEDSTARTINGHOUR
                + " TEXT, " + POSTPONEDENDINGHOUR + " TEXT " + CLASSROOM + " TEXT)" );
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL("DROP TABLE IF EXISTS " + LESSON_TABLE_NAME);
            OnCreate(db);
        }

        public void UpdateDb(int version)
        {
            SQLiteDatabase db = this.WritableDatabase;
            if (version == 0)
                db.ExecSQL($"ALTER TABLE {LESSON_TABLE_NAME} ADD COLUMN {CLASSROOM} TEXT");
            else throw new System.Exception("Updated");
        }

        public bool insertLesson(Lesson lesson)
        {
            using (SQLiteDatabase db = this.WritableDatabase)
            {
                ContentValues values = new ContentValues();
                values.Put(LESSONNAME, lesson.LessonName);
                values.Put(BEGINNINGHOUR, lesson.BegginingHour);
                values.Put(ENDINGHOUR, lesson.EndingHour);
                values.Put(DAY, lesson.day.ToString("dd/MM/yyyy"));
                values.Put(SEMESTER, lesson.Semester);
                values.Put(PER2WEEKS, lesson.Per2Weeks);
                values.Put(POSTPONED, lesson.Postponed);
                values.Put(POSTPONEDFOR, lesson.Postponedfor.ToString("dd/MM/yyyy"));
                values.Put(TYPE, lesson.Type);
                values.Put(POSTPONETIME, lesson.PostponeTime);
                values.Put(POSTPONEDSTARTINGHOUR, lesson.PostponedStartingHour);
                values.Put(POSTPONEDENDINGHOUR, lesson.PostponedEndingHour);
                values.Put(CLASSROOM, lesson.Classroom);
                long result = db.Insert(LESSON_TABLE_NAME, null, values);
                if (result == -1)
                    return false;
                return true;
            }
        }

        public List<Lesson> getallLessons()
        {
            List<Lesson> lessons = new List<Lesson>();

            using (SQLiteDatabase db = this.WritableDatabase)
            {
                ICursor datastream = db.RawQuery("SELECT * FROM " + LESSON_TABLE_NAME, null);
                while (datastream.MoveToNext())
                {
                    lessons.Add(new Lesson
                    {
                        LessonID = datastream.GetInt(0),
                        LessonName = datastream.GetString(1),
                        BegginingHour = datastream.GetString(2),
                        EndingHour = datastream.GetString(3),
                        day = DateTime.ParseExact(datastream.GetString(4), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Semester = datastream.GetInt(5),
                        Per2Weeks = boolean(datastream.GetString(6)),
                        Postponed = boolean(datastream.GetString(7)),
                        Postponedfor = DateTime.ParseExact(datastream.GetString(8), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Type = datastream.GetString(9),
                        PostponeTime = boolean(datastream.GetString(10)),
                        PostponedStartingHour = datastream.GetString(11),
                        PostponedEndingHour = datastream.GetString(12),
                        Classroom = datastream.GetString(13)
                    });
                }
                datastream.Close();
                datastream.Dispose();
            }
            lessons = Sorting.SortByNameAndType(lessons);
            return lessons;
            

        }

        public List<Lesson> getUpcommingLessons(Context context)
        {
            List<Lesson> Ulessons = new List<Lesson>();
            bool updated = false;
            using (SQLiteDatabase db = this.WritableDatabase)
            {
                    DateTime today = DateTime.Today;
                    ICursor datastream = db.RawQuery("SELECT * FROM " + LESSON_TABLE_NAME, null);
                while (datastream.MoveToNext())
                {
                    bool itemUpdated = false;
                    DateTime time = DateTime.ParseExact(datastream.GetString(4), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (DateTime.Compare(time, today) > 0)
                    {
                        Ulessons.Add(new Lesson
                        {
                            LessonID = datastream.GetInt(0),
                            LessonName = datastream.GetString(1),
                            BegginingHour = datastream.GetString(2),
                            EndingHour = datastream.GetString(3),
                            day = DateTime.ParseExact(datastream.GetString(4), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Semester = datastream.GetInt(5),
                            Per2Weeks = boolean(datastream.GetString(6)),
                            Postponed = boolean(datastream.GetString(7)),
                            Postponedfor = DateTime.ParseExact(datastream.GetString(8), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Type = datastream.GetString(9),
                            PostponeTime = boolean(datastream.GetString(10)),
                            PostponedStartingHour = datastream.GetString(11),
                            PostponedEndingHour = datastream.GetString(12),
                            Classroom = datastream.GetString(13)
                        });
                    }
                    else if (DateTime.Compare(time, today) == 0)
                    {
                        Lesson temp = new Lesson
                        {
                            LessonID = datastream.GetInt(0),
                            LessonName = datastream.GetString(1),
                            BegginingHour = datastream.GetString(2),
                            EndingHour = datastream.GetString(3),
                            day = DateTime.ParseExact(datastream.GetString(4), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Semester = datastream.GetInt(5),
                            Per2Weeks = boolean(datastream.GetString(6)),
                            Postponed = boolean(datastream.GetString(7)),
                            Postponedfor = DateTime.ParseExact(datastream.GetString(8), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Type = datastream.GetString(9),
                            PostponeTime = boolean(datastream.GetString(10)),
                            PostponedStartingHour = datastream.GetString(11),
                            PostponedEndingHour = datastream.GetString(12),
                            Classroom = datastream.GetString(13)
                        };
                        Ulessons.Add(temp);
                        TimeSpan now = DateTime.Now.TimeOfDay;
                        TimeSpan beggining = TimeSpan.Parse(temp.BegginingHour);
                        if (TimeSpan.Compare(beggining, now) < 0)
                        {
                            if (UpdateLesson(temp, true) > 0)
                            {
                                updated = true;
                                itemUpdated = true;
                                Toast.MakeText(context, "Updated", ToastLength.Short).Show();
                            }
                            else Toast.MakeText(context, "Not Updated", ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Lesson temp = new Lesson
                        {
                            LessonID = datastream.GetInt(0),
                            LessonName = datastream.GetString(1),
                            BegginingHour = datastream.GetString(2),
                            EndingHour = datastream.GetString(3),
                            day = DateTime.ParseExact(datastream.GetString(4), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Semester = datastream.GetInt(5),
                            Per2Weeks = boolean(datastream.GetString(6)),
                            Postponed = boolean(datastream.GetString(7)),
                            Postponedfor = DateTime.ParseExact(datastream.GetString(8), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Type = datastream.GetString(9),
                            PostponeTime = boolean(datastream.GetString(10)),
                            PostponedStartingHour = datastream.GetString(11),
                            PostponedEndingHour = datastream.GetString(12),
                            Classroom = datastream.GetString(13)
                        };
                        if (UpdateLesson(temp, true) > 0)
                        {
                            updated = true;
                            itemUpdated = true;
                            Toast.MakeText(context, "Updated", ToastLength.Short).Show();
                        }
                        else Toast.MakeText(context, "Not Updated", ToastLength.Short).Show();
                    }


                    if (!itemUpdated && boolean(datastream.GetString(7)))
                    {
                        DateTime postponed = DateTime.Parse(datastream.GetString(8));
                        if (DateTime.Compare(postponed, today) < 0)
                        {
                            Lesson temp = new Lesson
                            {
                                LessonID = datastream.GetInt(0),
                                LessonName = datastream.GetString(1),
                                BegginingHour = datastream.GetString(2),
                                EndingHour = datastream.GetString(3),
                                day = DateTime.ParseExact(datastream.GetString(4), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                Semester = datastream.GetInt(5),
                                Per2Weeks = boolean(datastream.GetString(6)),
                                Postponed = boolean(datastream.GetString(7)),
                                Postponedfor = DateTime.ParseExact(datastream.GetString(8), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                Type = datastream.GetString(9),
                                PostponeTime = boolean(datastream.GetString(10)),
                                PostponedStartingHour = datastream.GetString(11),
                                PostponedEndingHour = datastream.GetString(12),
                                Classroom = datastream.GetString(13)
                            };
                            if (UpdateLesson(temp, true) > 0)
                            {
                                updated = true;
                                Toast.MakeText(context, "Updated", ToastLength.Short).Show();
                            }
                            else Toast.MakeText(context, "Not Updated", ToastLength.Short).Show();
                        }
                        else if (DateTime.Compare(postponed, today) == 0)
                        {
                            Lesson temp = new Lesson
                            {
                                LessonID = datastream.GetInt(0),
                                LessonName = datastream.GetString(1),
                                BegginingHour = datastream.GetString(2),
                                EndingHour = datastream.GetString(3),
                                day = DateTime.ParseExact(datastream.GetString(4), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                Semester = datastream.GetInt(5),
                                Per2Weeks = boolean(datastream.GetString(6)),
                                Postponed = boolean(datastream.GetString(7)),
                                Postponedfor = DateTime.ParseExact(datastream.GetString(8), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                Type = datastream.GetString(9),
                                PostponeTime = boolean(datastream.GetString(10)),
                                PostponedStartingHour = datastream.GetString(11),
                                PostponedEndingHour = datastream.GetString(12),
                                Classroom = datastream.GetString(13)
                            };
                            // > 0 success if its -3 note that there is nothing to change!!!
                            int result = UpdateLesson(temp, true);
                            if (result > 0)
                            {
                                updated = true;
                                Toast.MakeText(context, "Updated", ToastLength.Short).Show();
                            }
                            else if (result != -3) Toast.MakeText(context, "Not Updated", ToastLength.Short).Show();
                        }
                    }
                }
                datastream.Close();
                datastream.Dispose();
            }
            if (updated)
                return getUpcommingLessons(context);
            Ulessons = Sorting.SortByDateAndTimeAndPostponeStartingHour(Ulessons);
            return Ulessons;
        }

        public int UpdateLesson(Lesson lessonToUpdate, bool SmartUd = false)
        {
            using(SQLiteDatabase db = this.WritableDatabase)
            {
                try
                {
                    ContentValues values = new ContentValues();
                    values.Put(LESSONNAME, lessonToUpdate.LessonName);
                    values.Put(BEGINNINGHOUR, lessonToUpdate.BegginingHour);
                    values.Put(ENDINGHOUR, lessonToUpdate.EndingHour);

                    if (SmartUd)
                    {
                        bool hasChanged = false;
                        if (DateTime.Compare(lessonToUpdate.day, DateTime.Today) < 0 || DateTime.Compare(lessonToUpdate.day, DateTime.Today) <= 0 && TimeSpan.Compare(TimeSpan.Parse(lessonToUpdate.BegginingHour), DateTime.Now.TimeOfDay) < 0) {
                            if (!lessonToUpdate.Per2Weeks)
                                values.Put(DAY, lessonToUpdate.day.AddDays(7).ToString("dd/MM/yyyy"));
                            else
                                values.Put(DAY, lessonToUpdate.day.AddDays(14).ToString("dd/MM/yyyy"));
                            hasChanged = true;
                        }

                        if (!hasChanged)
                        {
                            if (lessonToUpdate.Postponed && DateTime.Compare(lessonToUpdate.Postponedfor, DateTime.Today) <= 0)
                            {
                                if (lessonToUpdate.PostponeTime && TimeSpan.Compare(TimeSpan.Parse(lessonToUpdate.PostponedStartingHour), DateTime.Now.TimeOfDay) <= 0)
                                {
                                    values.Put(POSTPONEDFOR, DateTime.MinValue.ToString("dd/MM/yyyy"));
                                    values.Put(POSTPONED, false);
                                    values.Put(POSTPONETIME, false);
                                    values.Put(POSTPONEDSTARTINGHOUR, string.Empty);
                                    values.Put(POSTPONEDENDINGHOUR, string.Empty);
                                }
                                else if (!lessonToUpdate.PostponeTime)
                                {
                                    values.Put(POSTPONEDFOR, DateTime.MinValue.ToString("dd/MM/yyyy"));
                                    values.Put(POSTPONED, false);
                                }
                                else return -3;
                            }
                        }                        
                    }
                    else
                    {
                        values.Put(POSTPONED, lessonToUpdate.Postponed);
                        values.Put(DAY, lessonToUpdate.day.ToString("dd/MM/yyyy"));
                        values.Put(POSTPONEDFOR, lessonToUpdate.Postponedfor.ToString("dd/MM/yyyy"));
                        values.Put(POSTPONETIME, lessonToUpdate.PostponeTime);
                        values.Put(POSTPONEDSTARTINGHOUR, lessonToUpdate.PostponedStartingHour);
                        values.Put(POSTPONEDENDINGHOUR, lessonToUpdate.PostponedEndingHour);
                    }
                    values.Put(SEMESTER, lessonToUpdate.Semester);
                    values.Put(PER2WEEKS, lessonToUpdate.Per2Weeks);
                    values.Put(TYPE, lessonToUpdate.Type);

                    return db.Update(LESSON_TABLE_NAME, values, LESSONID + " = ?", new string[] {
                    lessonToUpdate.LessonID.ToString()
                });
                }
                catch
                {
                    return -1;
                }

            }

        }


        private bool boolean(string toConvert)
        {
            if (toConvert == "1")
                return true;
            return false;
        }

        public bool RemoveLesson(Lesson lessonToRemove)
        {
            try
            {
                using (SQLiteDatabase db = this.WritableDatabase)
                {
                    db.Delete(LESSON_TABLE_NAME, LESSONID + " = " + lessonToRemove.LessonID, null);
                }
                return true;
            }
            catch { return false; }
        }

        public bool ModifyLesson(Lesson lesson)
        {
            bool status = false;
            using(SQLiteDatabase db = this.WritableDatabase)
            {
                ContentValues values = new ContentValues();
                values.Put(PER2WEEKS, lesson.Per2Weeks);
                values.Put(POSTPONED, lesson.Postponed);
                values.Put(DAY, lesson.day.ToString("dd/MM/yyyy"));
                values.Put(POSTPONEDFOR, lesson.Postponedfor.ToString("dd/MM/yyyy"));
                values.Put(BEGINNINGHOUR, lesson.BegginingHour);
                values.Put(ENDINGHOUR, lesson.EndingHour);
                values.Put(POSTPONETIME, lesson.PostponeTime);
                values.Put(POSTPONEDSTARTINGHOUR, lesson.PostponedStartingHour);
                values.Put(POSTPONEDENDINGHOUR, lesson.PostponedEndingHour);
                values.Put(CLASSROOM, lesson.Classroom);
                if (db.Update(LESSON_TABLE_NAME, values, LESSONID + " = ?", new string[] { lesson.LessonID.ToString() }) > 0)
                    status = true;
            }
            return status;
        }
    }
}