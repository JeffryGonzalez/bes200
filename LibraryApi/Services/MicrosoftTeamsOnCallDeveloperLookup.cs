using LibraryApi.Controllers;
using LibraryApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApi.Services
{
    public class MicrosoftTeamsOnCallDeveloperLookup : ILookupOnCallDevelopers
    {
        IDistributedCache Cache;

        public MicrosoftTeamsOnCallDeveloperLookup(IDistributedCache cache)
        {
            Cache = cache;
        }

        public async Task<OnCallDeveloperResponse> GetOnCallDeveloper()
        {
            var email = await Cache.GetAsync("email");
            string emailAddress = null;
            if (email == null)
            {
                // call the Microsoft Teams API, get the email address.
                var emailToSave = $"bob-{DateTime.Now.ToLongTimeString()}@aol.com";
                var encodedEmail = Encoding.UTF8.GetBytes(emailToSave);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddSeconds(15));
                await Cache.SetAsync("email", encodedEmail, options);
                // Add it the cache with an expiration
                // return that email addres.
                emailAddress = emailToSave;
            } else
            {
                emailAddress = Encoding.UTF8.GetString(email);
            }

            return new OnCallDeveloperResponse { Email = emailAddress };

        }
    }
}
