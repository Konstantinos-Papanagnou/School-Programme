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
    public class KeyboardHandeler
    {
        private static readonly string File = "KeyboardType";
        private static readonly string Key = "Type";
        public enum KeyboardType
        {
            numeric,
            normal
        }
        ImageButton button;
        public KeyboardHandeler(ImageButton button)
        {
            this.button = button;
        }

        public void ChangeImageButtonIcon(KeyboardType type)
        {
            if (type == KeyboardType.numeric)
                button.SetImageResource(Resource.Drawable.NumericKeyboard);
            else button.SetImageResource(Resource.Drawable.Keyboard);
        }

        public void ChangeKeyboardRestrictions(EditText text1, EditText text2, KeyboardType type)
        {
            if (type == KeyboardType.normal)
            {
                text1.InputType = Android.Text.InputTypes.ClassText;
                text2.InputType = Android.Text.InputTypes.ClassText;
            }
            else
            {
                text1.InputType = Android.Text.InputTypes.ClassDatetime;
                text2.InputType = Android.Text.InputTypes.ClassDatetime;
            }
        }

        public static KeyboardType GetPrefsForKeyboard(Activity act)
        {            
            return (KeyboardType)act.GetSharedPreferences(File, FileCreationMode.Private).GetInt(Key, 0);
        }

        public static void SetPrefsForKeyboard(Activity act, KeyboardType type)
        {
            act.GetSharedPreferences(File, FileCreationMode.Append).Edit().PutInt(Key, (int)type).Commit();
        }
    }
}