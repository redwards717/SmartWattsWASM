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

            EffortCalculations anEffort = new(Constants.AnaerobicPZ, ftp);
            EffortCalculations vo2Effort = new(Constants.VO2PZ, ftp);
            EffortCalculations thEffort = new(Constants.ThresholdPZ, ftp);

            var powerPointsInRide = Constants.PowerPoints.Where(pp => pp <= powerStream.data.Length);

            foreach(int pp in powerPointsInRide)
            {
                rollingWatts.Add(pp, new List<float>());
                powerPoints.Add(pp, 0);
            }

            for(int i = 0; i < powerStream.data.Length; i++)
            {
                //finding sustained efforts
                anEffort.RollingValues.Add(powerStream.data[i]);
                TrackSustainedEfforts(anEffort);

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

            powerData.PowerPoints = powerPoints;
            powerData.JsonPowerPoints = JsonSerializer.Serialize(powerPoints);

            return powerData;
        }

        private static void TrackSustainedEfforts(EffortCalculations e)
        {
            if (e.InEffort)
            {
                if (e.RollingValues.Sum() < (e.Target * .99))
                {
                    e.TotalTime += e.RollingValues.Count;
                    e.InEffort = false;
                    e.RollingValues = new List<float>();
                }
            }
            else if (e.RollingValues.Count >= Constants.AnaerobicPZ.Time)
            {
                if (e.RollingValues.Sum() >= e.Target)
                {
                    e.InEffort = true;
                }
                else
                {
                    e.RollingValues.RemoveAt(0);
                }
            }
        }
    }
}
