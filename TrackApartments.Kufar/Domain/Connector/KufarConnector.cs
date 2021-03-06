﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TrackApartments.Contracts;
using TrackApartments.Contracts.Models;
using TrackApartments.Kufar.Domain.Connector.DTOs;
using TrackApartments.Kufar.Domain.Connector.Extensions;

namespace TrackApartments.Kufar.Domain.Connector
{
    public sealed class KufarConnector : IKufarConnector
    {
        private readonly ILoadEngine engine;
        private readonly IResponseParser parser;
        private readonly ILogger logger;

        public KufarConnector(ILoadEngine engine, IResponseParser parser, ILogger logger)
        {
            this.engine = engine;
            this.parser = parser;
            this.logger = logger;
        }

        public async Task<List<Apartment>> GetAsync(string url)
        {
            HttpResponseMessage data = await engine.LoadAsync(url);
            var kufarApartments = await parser.ParseAsync<KufarBoard>(data);

            var results = new List<Apartment>();

            foreach (var flat in kufarApartments.Apartments)
            {
                var detailsUrl = "https://re.kufar.by/api/search/detail/" + flat.Id;
                var detailsData = await engine.LoadAsync(detailsUrl);
                var details = await parser.ParseAsync<KufarDetails>(detailsData);

                if (details == null)
                {
                    logger.LogWarning($"Details are abscent for kufar item: {flat.Address}, url: {flat.Url}");
                }

                var result = flat.ToApartment(details?.Result);

                if (details != null && result.Phones.Count == 0)
                {
                    logger.LogWarning($"Item {result} does not contain proper phone number in details {details.Result}");
                }

                results.Add(result);
            }

            return results;
        }
    }
}
