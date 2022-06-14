namespace SmartWatts.Server.Utilities
{
    public static class PowerUtilities
    {
        public static PowerData CalculatePowerFromDataStream(List<StravaDataStream> sdss, int ftp)
        {
            PowerData powerData = new();
            Dictionary<int, int> powerPoints = new();
            Dictionary<int, List<float>> rollingWatts = new();

            //var timeStream = sdss.Find(s => s.type == "time");  // will eventually need to use time stream to input missing values into peloton activities
            var powerStream = sdss.Find(s => s.type == "watts");

            //if(timeStream.data.Length != powerStream.data.Length)
            //{
            //    throw new Exception("DataStream sizes are not compatible");
            //}

            var powerPointsInRide = Constants.PowerPoints.Where(pp => pp <= powerStream.data.Length);

            foreach(int pp in powerPointsInRide)
            {
                rollingWatts.Add(pp, new List<float>());
                powerPoints.Add(pp, 0);
            }

            Dictionary<int, int> sustainedEfforts = new();
            EffortCalculations anEffort = new(Constants.AnaerobicPZ, ftp);
            EffortCalculations vo2Effort = new(Constants.VO2PZ, ftp);
            EffortCalculations thEffort = new(Constants.ThresholdPZ, ftp);

            for(int i = 0; i < powerStream.data.Length; i++)
            {
                //finding sustained efforts
                anEffort.RollingValues.Add(powerStream.data[i]);
                vo2Effort.RollingValues.Add(powerStream.data[i]);
                thEffort.RollingValues.Add(powerStream.data[i]);
                if(i % 5 == 0)  // potentially do this to the power curve portion to speed that up
                {
                    TrackSustainedEfforts(anEffort);
                    TrackSustainedEfforts(vo2Effort);
                    TrackSustainedEfforts(thEffort);
                }

                // calculating power curve
                foreach(int pp in powerPointsInRide)
                {
                    rollingWatts[pp].Add(powerStream.data[i]);
                    if (rollingWatts[pp].Count >= pp)
                    {
                        var rollingAvg = rollingWatts[pp].Sum() / pp;
                        if (powerPoints[pp] < rollingAvg)
                        {
                            powerPoints[pp] = (int)rollingAvg;
                        }
                        rollingWatts[pp].RemoveAt(0);
                    }
                }
            }

            ScanForFinalEfforts(anEffort);
            ScanForFinalEfforts(vo2Effort);
            ScanForFinalEfforts(thEffort);

            sustainedEfforts.Add(anEffort.MinTime, anEffort.TotalTime);
            sustainedEfforts.Add(vo2Effort.MinTime, vo2Effort.TotalTime);
            sustainedEfforts.Add(thEffort.MinTime, thEffort.TotalTime);

            powerData.SustainedEfforts = sustainedEfforts;
            powerData.JsonSustainedEfforts = JsonSerializer.Serialize(sustainedEfforts);
            powerData.PowerPoints = powerPoints;
            powerData.JsonPowerPoints = JsonSerializer.Serialize(powerPoints);

            return powerData;
        }

        private static void TrackSustainedEfforts(EffortCalculations e)
        {
            try
            {
                if (e.InEffort)
                {
                    if (e.RollingValues.Average() < (e.Target * .99))
                    {
                        if(e.RollingValues.Count - 5 >= e.MinTime)
                        {
                            e.TotalTime += e.RollingValues.Count;
                        }
                        e.InEffort = false;
                        e.RollingValues.Clear();
                    }
                }
                else if (e.RollingValues.Average() >= e.Target)
                {
                    e.InEffort = true;
                }
                else
                {
                    e.RollingValues.Clear();
                }
            }
            catch(Exception ex)
            {
                var test = ex;
                throw;
            }
        }

        private static void ScanForFinalEfforts(EffortCalculations e)
        {
            if (e.InEffort)
            {
                e.TotalTime += (e.RollingValues.Count >= e.MinTime && e.RollingValues.Average() >= e.Target) ? e.RollingValues.Count : 0;
            }
        }
    }
}
