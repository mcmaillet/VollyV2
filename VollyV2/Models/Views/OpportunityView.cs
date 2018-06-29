﻿using System;
using System.Collections.Generic;
using System.Linq;
using VollyV2.Data.Volly;

namespace VollyV2.Models.Views
{
    public class OpportunityView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationLink { get; set; }
        public string CauseName { get; set; }
        public string CategoryName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImageUrl { get; set; }
        public List<OccurrenceView> OccurrenceViews { get; set; }
        public static OpportunityView FromOpportunity(Opportunity opportunity, List<DateTime> valiDateTimes)
        {
            return new OpportunityView()
            {
                Id = opportunity.Id,
                Name = opportunity.Name,
                Description = opportunity.Description,
                Address = opportunity.Address,
                OrganizationName = opportunity.Organization.Name,
                OrganizationLink = opportunity.Organization.WebsiteLink,
                CauseName = opportunity.Organization.Cause.Name,
                CategoryName = opportunity.Category.Name,
                Latitude = opportunity.Location.Latitude,
                Longitude = opportunity.Location.Longitude,
                ImageUrl = opportunity.ImageUrl,
                OccurrenceViews = valiDateTimes.Count > 0 ? opportunity.Occurrences
                    .Where(oc => valiDateTimes.Contains(oc.StartTime.Date))
                    .OrderBy(o => o.StartTime)
                    .Select(OccurrenceView.FromOccurrence)
                    .ToList() : opportunity.Occurrences.Select(OccurrenceView.FromOccurrence).ToList()
            };
        }
    }
}
