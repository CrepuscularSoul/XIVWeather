using System.Collections.Generic;
using WeatherApp.Domain.Models;
using WeatherApp.Domain.Models.Locations;

namespace WeatherApp.Domain.Repository
{
    /// <summary>
    /// Repository for retrieving Region information.
    /// </summary>
    /// <remarks>
    /// Currently just uses in memory collections.
    /// </remarks>
    public class RegionRepository
    {
        /// <summary>
        /// Gets all regions and associated zone and weather information.
        /// </summary>
        public static List<Region> GetRegions()
        {
            return new List<Region>
            {
                LaNoscea,
                BlackShroud,
                Thanalan,
                IshgardAndSurrounding,
                GyrAbania,
                FarEast,
                Others,
                Norvrandt
            };
        }

        /// <summary>
        /// Gets the details for La Noscea
        /// </summary>
        private static Region LaNoscea =>
            new Region
            {
                Name = Constants.Regions.LaNoscea,
                Zones = new List<Zone>
                {
                    new Zone
                    {
                        Name = Constants.Zones.LimsaLominsa,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 50, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Rain}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.MiddleLaNoscea,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 50, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.Wind},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Rain},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.LowerLaNoscea,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 50, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.Wind},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Rain},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.EasternLaNoscea,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 5, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 50, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 95, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Showers},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.WesternLaNoscea,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Wind},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Gales},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.UpperLaNoscea,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 50, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Thunder},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Thunderstorms},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.OuterLaNoscea,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 50, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Rain},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.Mist,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 50, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Rain},
                        }
                    },
                }
            };

        /// <summary>
        /// Gets the details for The Black Shroud
        /// </summary>
        private static Region BlackShroud
            => new Region
            {
                Name = Constants.Regions.BlackShroud,
                Zones = new List<Zone>
                {
                    new Zone
                    {
                        Name = Constants.Zones.Gridania,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 55, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.FairSkies},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.CentralShroud,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 5, Weather = Enums.Weather.Thunder},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 55, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.FairSkies}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.EastShroud,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 5, Weather = Enums.Weather.Thunder},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 55, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.FairSkies}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.SouthShroud,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 5, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Thunderstorms},
                            new WeatherBreakpoint { Value = 25, Weather = Enums.Weather.Thunder},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.ClearSkies}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.NorthShroud,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 5, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Showers},
                            new WeatherBreakpoint { Value = 25, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.ClearSkies}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.LavenderBeds,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 5, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 55, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.FairSkies}
                        }
                    },
                }
            };

        /// <summary>
        /// Gets the details for Thanalan
        /// </summary>
        private static Region Thanalan
            => new Region
            {
                Name = Constants.Regions.Thanalan,
                Zones = new List<Zone>
                {
                    new Zone
                    {
                        Name = Constants.Zones.Uldah,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 95, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Rain}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.WesternThanalan,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 95, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Rain}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.CentralThanalan,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 15, Weather = Enums.Weather.DustStorms},
                            new WeatherBreakpoint { Value = 55, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 95, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Rain}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.EasternThanalan,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Showers},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.SouthernThanalan,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.HeatWaves},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Fog}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.NorthernThanalan,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 5, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 50, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Fog}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.Goblet,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 95, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Rain}
                        }
                    },
                }
            };

        /// <summary>
        /// Gets the details for Ishgard and its surrounding areas.
        /// </summary>
        private static Region IshgardAndSurrounding
            => new Region
            {
                Name = Constants.Regions.Ishgard,
                Zones = new List<Zone>
                {
                    new Zone
                    {
                        Name = Constants.Zones.Ishgard,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.Snow},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Fog}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.CoerthasCentralHighlands,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Blizzards},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.Snow},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Fog}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.CoerthasWesternHighlands,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Blizzards},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.Snow},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Fog}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.SeaOfClouds,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Wind},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.UmbralWind}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.AzysLla,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 35, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Thunder}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.DravanianForelands,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Thunder},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.DustStorms},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.FairSkies}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.DravanianHinterlands,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.Showers},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.FairSkies}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.ChurningMists,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Gales},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.UmbralStatic},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.FairSkies}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.Idyllshire,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.Showers},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.FairSkies}
                        }
                    },
                }
            };

        /// <summary>
        /// Gets the details for miscellaneous areas such as Eureka or Mor Dhona.
        /// </summary>
        private static Region Others
            => new Region
            {
                Name = Constants.Regions.Others,
                Zones = new List<Zone>
                {
                    new Zone
                    {
                        Name = Constants.Zones.MorDhona,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 15, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.Gloom},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.FairSkies},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.EurekaAnemos,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.Gales},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Showers},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Snow},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.EurekaPagos,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 28, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 46, Weather = Enums.Weather.HeatWaves},
                            new WeatherBreakpoint { Value = 64, Weather = Enums.Weather.Snow},
                            new WeatherBreakpoint { Value = 82, Weather = Enums.Weather.Thunder},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Blizzards}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.EurekaPyros,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 28, Weather = Enums.Weather.HeatWaves},
                            new WeatherBreakpoint { Value = 46, Weather = Enums.Weather.Thunder},
                            new WeatherBreakpoint { Value = 64, Weather = Enums.Weather.Blizzards},
                            new WeatherBreakpoint { Value = 82, Weather = Enums.Weather.UmbralWind},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Snow}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.EurekaHydatos,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 12, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 34, Weather = Enums.Weather.Showers},
                            new WeatherBreakpoint { Value = 56, Weather = Enums.Weather.Gloom},
                            new WeatherBreakpoint { Value = 78, Weather = Enums.Weather.Thunderstorms},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Snow},
                        }
                    },
                }
            };

        /// <summary>
        /// Gets the details for the far east zones.
        /// </summary>
        private static Region FarEast
            => new Region
            {
                Name = Constants.Regions.FarEast,
                Zones = new List<Zone>
                {
                    new Zone
                    {
                        Name = Constants.Zones.Kugane,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.ClearSkies},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.RubySea,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Thunder},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Wind},
                            new WeatherBreakpoint { Value = 35, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.ClearSkies},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.Yanxia,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 5, Weather = Enums.Weather.Showers},
                            new WeatherBreakpoint { Value = 15, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 25, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 40, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.ClearSkies},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.AzimSteppe,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 5, Weather = Enums.Weather.Gales},
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Wind},
                            new WeatherBreakpoint { Value = 17, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 25, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 35, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint {Value = 100, Weather = Enums.Weather.ClearSkies}
                        }
                    },
                }
            };

        /// <summary>
        /// Gets the details for Gyr Abania
        /// </summary>
        private static Region GyrAbania
            => new Region
            {
                Name = Constants.Regions.GyrAbania,
                Zones = new List<Zone>
                {
                    new Zone
                    {
                        Name = Constants.Zones.RhalgrsReach,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 15, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Thunder},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.Fringes,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 15, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Thunder},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.Peaks,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 95, Weather = Enums.Weather.Wind},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.DustStorms},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.Lochs,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 90, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Thunderstorms},
                        }
                    },
                }
            };

        /// <summary>
        /// Gets the details for Norvrandt.
        /// </summary>
        private static Region Norvrandt
            => new Region
            {
                Name = Constants.Regions.Norvrandt,
                Zones = new List<Zone>
                {
                    new Zone
                    {
                        Name = Constants.Zones.Crystarium,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 95, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value =  100, Weather = Enums.Weather.Thunderstorms}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.Eulmore,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Gales},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 45, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.ClearSkies}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.Lakeland,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 75, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 95, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Thunderstorms}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.Kholusia,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Gales},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 35, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 45, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.ClearSkies}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.AmhAraeng,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 45, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 70, Weather = Enums.Weather.DustStorms},
                            new WeatherBreakpoint { Value = 80, Weather = Enums.Weather.HeatWaves},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.ClearSkies},
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.IlMheg,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 35, Weather = Enums.Weather.Clouds},
                            new WeatherBreakpoint { Value = 45, Weather = Enums.Weather.Thunderstorms},
                            new WeatherBreakpoint { Value = 60, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.FairSkies}
                        }
                    },
                    new Zone
                    {
                        Name = Constants.Zones.RaktikaGreatwood,
                        WeatherBreakpoints = new List<WeatherBreakpoint>
                        {
                            new WeatherBreakpoint { Value = 10, Weather = Enums.Weather.Fog},
                            new WeatherBreakpoint { Value = 20, Weather = Enums.Weather.Rain},
                            new WeatherBreakpoint { Value = 30, Weather = Enums.Weather.UmbralWind},
                            new WeatherBreakpoint { Value = 45, Weather = Enums.Weather.ClearSkies},
                            new WeatherBreakpoint { Value = 85, Weather = Enums.Weather.FairSkies},
                            new WeatherBreakpoint { Value = 100, Weather = Enums.Weather.Clouds}
                        }
                    },
                }
            };
    }
}