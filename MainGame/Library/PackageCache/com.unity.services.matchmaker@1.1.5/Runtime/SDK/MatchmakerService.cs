using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Services.Matchmaker.Tests")]

namespace Unity.Services.Matchmaker 
{
    /// <summary>
    /// The entry class of the Matchmaker Ticketing Service enables clients to connect to matchmaking queues and resolve to target online server instances.
    /// </summary>
    public static class MatchmakerService
    {
        private static IMatchmakerService service;
        private static readonly Configuration configuration;

        static MatchmakerService() 
        {
            configuration = new Configuration("https://matchmaker.services.api.unity.com", 10, 4, null);
        }

        /// <summary>
        /// A static instance of the Matchmaker Ticketing Client.
        /// </summary>
        public static IMatchmakerService Instance 
        {
            get 
            {
                if (service != null) 
                {
                    return service;
                }

                //Prevent construction if CoreRegistry was not initialized.
                var serviceSdk = MatchmakerServiceSdk.Instance;
                if (serviceSdk == null) 
                {
                    throw new InvalidOperationException("Attempting to call Matchmaker Services requires initializing Core Registry. Call 'UnityServices.InitializeAsync' first!");
                }

                return new WrappedMatchmakerService(MatchmakerServiceSdk.Instance);
            }
        }
    }
}