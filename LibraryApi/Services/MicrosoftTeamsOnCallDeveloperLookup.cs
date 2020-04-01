using LibraryApi.Controllers;
using LibraryApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Services
{
    public class MicrosoftTeamsOnCallDeveloperLookup : ILookupOnCallDevelopers
    {
        public Task<OnCallDeveloperResponse> GetOnCallDeveloper()
        {
            // so here is where we will write all tha code to call the Teams API,
            // put it in a cache, etc.
            return Task.FromResult(new OnCallDeveloperResponse { Email = "totallyfake@gmail.com" });
        }
    }
}
