namespace SmartWatts.Server.Utilities
{
    public static class PelotonUtilities
    {
        //public static void AddMissingDataPoints(Activity activity, List<StravaDataStream> sdss)
        //{
        //    var wattsAry = sdss.Find(s => s.type == "watts").data;
        //    var timeAry = sdss.Find(s => s.type == "time").data;

        //    if(wattsAry.Length != timeAry.Length)
        //    {
        //        throw new Exception("Data for peloton ride isn't compatible");
        //    }

        //    List<float> correctedWatts = new();

        //    float prev = 0;

        //    for(int i = 0; i < timeAry.Length; i++)
        //    {
        //        var diff = (timeAry[i] - prev);

        //        if (diff > 1 && diff < 10)
        //        {
        //            correctedWatts.AddRange(Enumerable.Repeat(wattsAry[i], (int)diff));
        //        }
        //        else if(diff >= 10)
        //        {
        //            correctedWatts.AddRange(Enumerable.Repeat(0f, (int)diff));
        //        }
        //        else
        //        {
        //            correctedWatts.Add(wattsAry[i]);
        //        }

        //        prev = timeAry[i];
        //    }

        //    correctedWatts.AddRange(Enumerable.Repeat(0f, 5));
        //    activity.MovingTime = correctedWatts.Count();

        //    sdss[0].data = correctedWatts.ToArray();
        //}

        public static void AddMissingDataPoints(Activity activity, List<StravaDataStream> sdss, int percentAdj = 0)
        {
            var wattsAry = sdss.Find(s => s.type == "watts").data;
            var timeAry = sdss.Find(s => s.type == "time").data;

            double multiplier = (double)(percentAdj + 100) / 100;

            if (wattsAry.Length != timeAry.Length)
            {
                throw new Exception("Data for peloton ride isn't compatible");
            }

            List<float> correctedWatts = new();

            float prev = 0;

            for (int i = 0; i < timeAry.Length; i++)
            {
                var diff = (timeAry[i] - prev);
                var correctedValue = (float)(wattsAry[i] * multiplier);

                if (diff > 1 && diff < 10)
                {
                    correctedWatts.AddRange(Enumerable.Repeat(correctedValue, (int)diff));
                }
                else if (diff >= 10)
                {
                    correctedWatts.AddRange(Enumerable.Repeat(0f, (int)diff));
                }
                else
                {
                    correctedWatts.Add(correctedValue);
                }

                prev = timeAry[i];
            }

            correctedWatts.AddRange(Enumerable.Repeat(0f, 5));
            activity.MovingTime = correctedWatts.Count();

            sdss[0].data = correctedWatts.ToArray();
        }
        public static void NormalizePowerMetaData(Activity activity, int percentAdj)
        {
            double multiplier = (double)(percentAdj + 100) / 100;

            activity.Kilojoules *= multiplier;
            activity.WeightedAvgWatts *= multiplier;
            activity.AvgWatts *= multiplier;
            activity.MaxWatts *= multiplier;
        }
    }
}
