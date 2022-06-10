namespace SmartWatts.Server.Utilities
{
    public static class PowerUtilities
    {
        public static PowerData CalculatePowerFromDataStream(List<StravaDataStream> sdss)
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


            KeyValuePair<int, int> susTH = new(60 * 15, 0);
            KeyValuePair<int, int> susVO2 = new(60 * 3, 0);
            KeyValuePair<int, int> susAn = new(30, 0);

            var powerPointsInRide = Constants.PowerPoints.Where(pp => pp <= powerStream.data.Length);

            foreach(int pp in powerPointsInRide)
            {
                rollingWatts.Add(pp, new List<float>());
                powerPoints.Add(pp, 0);
            }

            for(int i = 0; i < powerStream.data.Length; i++)
            {
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
    }
}
