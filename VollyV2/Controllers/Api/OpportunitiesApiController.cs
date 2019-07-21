﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models.Views;
using VollyV2.Models.Volly;
using VollyV2.Services;

namespace VollyV2.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Opportunities")]
    public class OpportunitiesApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public OpportunitiesApiController(ApplicationDbContext context,
            IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        // GET: api/Opportunities
        [HttpGet]
        public async Task<IActionResult> GetOpportunities()
        {
            List<Opportunity> opportunities = await VollyMemoryCache.GetAllOpportunities(_memoryCache, _context);
            List<OpportunityView> opportunityViews = opportunities
                .Select(OpportunityView.FromOpportunity)
                .Where(o => o.OccurrenceViews.Count > 0)
                .ToList();

            return Ok(Sort(opportunityViews, 1));
        }

        // GET: api/Opportunities/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpportunity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Opportunity opportunity = (await VollyMemoryCache.GetAllOpportunities(_memoryCache, _context))
                .SingleOrDefault(m => m.Id == id);

            if (opportunity == null)
            {
                return NotFound();
            }

            return Ok(OpportunityView.FromOpportunity(opportunity));
        }

        [HttpPost]
        [Route("/api/Opportunities/Search")]
        public async Task<IActionResult> Search([FromBody] OpportunitySearch opportunitySearch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Opportunity> opportunities = await VollyMemoryCache.GetAllOpportunities(_memoryCache, _context);
            List<OpportunityView> opportunityViews = opportunities
                .Where(GetEligibleOpportunityPredicate(opportunitySearch))
                .Select(OpportunityView.FromOpportunity)
                .Where(o => o.OccurrenceViews.Count > 0)
                .ToList();

            return Ok(Sort(opportunityViews, opportunitySearch.Sort));
        }

        private static Func<Opportunity, bool> GetEligibleOpportunityPredicate(OpportunitySearch opportunitySearch)
        {
            return o =>
                (opportunitySearch.CauseIds == null || o.Organization.Cause != null &&
                 opportunitySearch.CauseIds.Contains(o.Organization.Cause.Id)) &&
                (opportunitySearch.CategoryIds == null ||
                 opportunitySearch.CategoryIds.Contains(o.Category.Id)) &&
                (opportunitySearch.OrganizationIds == null ||
                 opportunitySearch.OrganizationIds.Contains(o.Organization.Id)) &&
                (opportunitySearch.CommunityIds == null || o.Community != null &&
                 opportunitySearch.CommunityIds.Contains(o.Community.Id)) &&
                (opportunitySearch.OpportunityType == OpportunityType.All || opportunitySearch.OpportunityType == o.OpportunityType) &&
                o.Approved;
        }

        private List<OpportunityView> Sort(List<OpportunityView> opportunityViews, int sort)
        {
            switch (sort)
            {
                case 1:
                    return opportunityViews.OrderBy(o =>
                    {
                        OccurrenceView firstOcc = o.OccurrenceViews[0];
                        return firstOcc.StartTime;
                    }).ToList();
                case 2:
                    return opportunityViews.OrderBy(o => o.OrganizationName).ToList();
                case 3:
                    return opportunityViews.OrderBy(o => o.OccurrenceViews.Sum(occ => occ.Openings)).ToList();
                case 4:
                    return opportunityViews.OrderBy(o =>
                    {
                        OccurrenceView firstOcc = o.OccurrenceViews[0];
                        return firstOcc.EndTime - firstOcc.StartTime;
                    }).ToList();
                default:
                    return opportunityViews;
            }
        }
    }
}