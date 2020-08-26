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

namespace School_Programme.Animations.Hold_Fading_animation
{
    public class HoldFadingAnim
    {
        /// <summary>
        /// Changes the visibility between 2 layouts based on the command parameter
        /// </summary>
        /// <param name="command">If command = 0 the linear layout will be gone and
        /// the scroll view will be visible
        /// Else the linear layout will be visible and the
        /// Scroll View will be gone</param>
        /// <param name="ll">The Linear Layout</param>
        /// <param name="sv">The Scroll View</param>
        public void ControlInput(int command, LinearLayout ll, ScrollView sv)
        {
            if(command == 0)
            {
                ll.Visibility = ViewStates.Gone;
                sv.Visibility = ViewStates.Visible;
            }
            else
            {
                ll.Visibility = ViewStates.Visible;
                sv.Visibility = ViewStates.Gone;
            }
        }

        /// <summary>
        /// Changes the visibility between 2 layouts based on the command parameter
        /// </summary>
        /// <param name="command">If command = 0 the first linear layout will be gone and
        /// the second Linear Layout will be visible.
        /// Else the first parameter will be visible and the
        /// Second will be gone</param>
        /// <param name="ll">The Linear Layout</param>
        /// <param name="ll2">The Second Linear Layout</param>
        public void ControlInput(int command, LinearLayout ll, LinearLayout ll2)
        {
            if (command == 0)
            {
                ll.Visibility = ViewStates.Gone;
                ll2.Visibility = ViewStates.Visible;
            }
            else
            {
                ll.Visibility = ViewStates.Visible;
                ll2.Visibility = ViewStates.Gone;
            }
        }


        /// <summary>
        /// Changes the visibility of a DatePicker based on the isChecked parameter
        /// </summary>
        /// <param name="isChecked">true to set to visible, false to set to gone</param>
        /// <param name="picker">object to handle</param>
        public void SwitchVisibilityHandeler(bool isChecked, DatePicker picker)
        {
            if (isChecked)
                picker.Visibility = ViewStates.Visible;
            else
                picker.Visibility = ViewStates.Gone;
                
        }

        /// <summary>
        /// Changes the visibility of a Linear Layout based on the isChecked parameter
        /// </summary>
        /// <param name="isChecked">true to set to visible, false to set to gone</param>
        /// <param name="ll">layout to handle</param>
        public void SwitchVisibilityHandeler(bool isChecked, LinearLayout ll)
        {
            if (isChecked)
                ll.Visibility = ViewStates.Visible;
            else
                ll.Visibility = ViewStates.Gone;

        }
    }
}