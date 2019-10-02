using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using System.Text;

namespace WeatherFinder
{
    /// <summary>
    /// Multi-select dialog component 
    /// </summary>
    public class MultiSelectionSpinner : Spinner, IDialogInterfaceOnMultiChoiceClickListener
    {
        private List<string> _items;
        private bool[] _selection;
        private readonly ArrayAdapter<string> _simpleAdapter;

        /// <summary>
        /// Get or set the items associated with the multi-select
        /// </summary>
        public List<string> Items
        {
            get => _items;
            set
            {
                _items = value;
                _selection = new bool[value.Count];
                _simpleAdapter.Clear();
                _simpleAdapter.Add(BuildSelectedItemString());
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MultiSelectionSpinner(Context context) : base(context)
        {
            _simpleAdapter = new ArrayAdapter<string>(context,
                    Resource.Layout.support_simple_spinner_dropdown_item);
            base.Adapter = _simpleAdapter;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MultiSelectionSpinner(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _simpleAdapter = new ArrayAdapter<string>(context,
                Resource.Layout.support_simple_spinner_dropdown_item);
            base.Adapter = _simpleAdapter;
        }

        /// <summary>
        /// Click event for when an item in the dialog is clicked
        /// </summary>
        public void OnClick(IDialogInterface dialog, int which, bool isChecked)
        {
            if (_selection == null || which >= _selection.Length) 
                return;

            _selection[which] = isChecked;
            _simpleAdapter.Clear();
            _simpleAdapter.Add(BuildSelectedItemString());
        }

        /// <summary>
        /// Click event for when the element is clicked, which pops up the alert dialog
        /// </summary>
        public override bool PerformClick()
        {
            using var _ = new AlertDialog.Builder(Context)
                .SetMultiChoiceItems(Items.ToArray(), _selection, this)
                .SetPositiveButton("Ok", (sender, args) => { })
                .Show();
            return true;
        }

        /// <summary>
        /// Get a list of the items selected
        /// </summary>
        public List<string> GetSelectedStrings()
        {
            return Items.Where((t, i) => _selection[i]).ToList();
        }

        /// <summary>
        /// Return a list of the selected indicies
        /// </summary>
        /// <returns></returns>
        public List<int> GetSelectedIndicies()
        {
            var selection = new List<int>();
            for (var i = 0; i < Items.Count; i++)
            {
                if (_selection[i])
                    selection.Add(i);
            }
            return selection;
        }

        /// <summary>
        /// Returns a comma-separated list of selected items
        /// </summary>
        /// <returns></returns>
        private string BuildSelectedItemString()
        {
            if (!_selection.Any(x => x))
                return "\tSelect...";

            return "\t" + string.Join(", ", GetSelectedStrings());
        }
    }
}