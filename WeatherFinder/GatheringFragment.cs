using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using WeatherApp.Domain.Models;
using WeatherApp.Domain.Models.Gathering;
using WeatherApp.Domain.Services;

namespace WeatherFinder
{
    public class GatheringFragment : Fragment
    {
        private View _view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.gathering_layout, container, false);
            ConfigureViews(_view);
            return _view;
        }

        /// <summary>
        /// Configures click events for the view.
        /// </summary>
        private void ConfigureViews(View view)
        {
            view.FindViewById<Button>(Resource.Id.ephemeral_nodes).Click += GetEphemeralNodeDetails;
            view.FindViewById<Button>(Resource.Id.unspoiled_nodes).Click += GetUnspoiledNodeDetails;
            view.FindViewById<Button>(Resource.Id.legendary_nodes).Click += GetLegendaryNodeDetails;
            view.FindViewById<Button>(Resource.Id.yellow_scrip_rotation).Click += GetYellowScripRotation;
            view.FindViewById<Button>(Resource.Id.white_scrip_rotation).Click += GetWhiteScripRotation;
            view.FindViewById<ImageButton>(Resource.Id.search_items).Click += SearchItems;
        }

        /// <summary>
        /// Gets the ephemeral node details.
        /// </summary>
        private void GetEphemeralNodeDetails(object sender, EventArgs args)
        {
            var items = GatheringService.GetItemsOfType(Enums.NodeType.Ephemeral, true);
            PopulateTableView(items);
        }

        /// <summary>
        /// Gets the unspoiled node details.
        /// </summary>
        private void GetUnspoiledNodeDetails(object sender, EventArgs args)
        {
            var items = GatheringService.GetItemsOfType(Enums.NodeType.Unspoiled, false);
            PopulateTableView(items);
        }

        /// <summary>
        /// Gets the legendary node details.
        /// </summary>
        private void GetLegendaryNodeDetails(object sender, EventArgs args)
        {
            var items = GatheringService.GetItemsOfType(Enums.NodeType.Legendary, false);
            PopulateTableView(items);
        }

        /// <summary>
        /// Gets the yellow scrip rotation.
        /// </summary>
        private void GetYellowScripRotation(object sender, EventArgs args)
        {
            var items = GatheringService.GetScripRotation(Enums.ScripType.Yellow);
            PopulateTableView(items);
        }

        /// <summary>
        /// Gets the white scrip rotation.
        /// </summary>
        private void GetWhiteScripRotation(object sender, EventArgs args)
        {
            var items = GatheringService.GetScripRotation(Enums.ScripType.White);
            PopulateTableView(items);
        }

        /// <summary>
        /// Searches for items that contain the search text as all or part of their name.
        /// </summary>
        private void SearchItems(object sender, EventArgs args)
        {
            var view = _view.FindViewById<EditText>(Resource.Id.search_edittext);
            var items = GatheringService.Search(view.Text);
            PopulateTableView(items);
        }

        /// <summary>
        /// Populates the table view.
        /// </summary>
        private void PopulateTableView(List<GatheringItem> items)
        {
            var view = _view.FindViewById<HorizontalScrollView>(Resource.Id.gathering_scrollview);
            view.RemoveAllViews();
            
            var table = new TableLayout(_view.Context);
            table.AddNewRow(GatheringItem.GetRelevantPropertyNames());
            items.ForEach(x => table.AddNewRow(x.ToStringArray(), x.Gatherer == Enums.Gatherer.Botanist ? Color.ForestGreen : Color.IndianRed));
            view.AddView(table);
            view.RequestLayout();
        }
    }
}