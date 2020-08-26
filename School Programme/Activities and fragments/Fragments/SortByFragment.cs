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
using School_Programme.Adapters;


namespace School_Programme.Activities_and_fragments.Fragments
{
    public class SortByFragment : DialogFragment
    {
        public interface OnInputSelectedI
        {
            void SendData(int sortMethod);
        }

        public OnInputSelectedI OnInputSelected;
        private ListView lv;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            Dialog.SetTitle("Sort By:");

            View view = inflater.Inflate(Resource.Layout.SortByLayout, container, false);
            lv = view.FindViewById<ListView>(Resource.Id.SortingListView);
            return view;
        }

        private void Lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            OnInputSelected = (OnInputSelectedI)TargetFragment;
            OnInputSelected.SendData(e.Position);
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            lv.Adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItemSingleChoice, Sortingitems.items);
            lv.ItemClick += Lv_ItemClick;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                OnInputSelected = (OnInputSelectedI)TargetFragment;
            }
            catch (Exception e)
            {
                Toast.MakeText(this.Activity, "CastException " + e.Message, ToastLength.Long).Show();
            }
        }
    }
}