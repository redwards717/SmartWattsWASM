﻿#pragma warning disable IDE1006 // Naming Styles
using System;

namespace SmartWatts.Shared
{
    public class StravaModels
    {
        public string token_type { get; set; }
        public int expires_at { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string access_token { get; set; }
        public Athlete athlete { get; set; }
    }

    public class StravaActivity
    {
        public int resource_state { get; set; }
        public Athlete athlete { get; set; }
        public string name { get; set; }
        public double distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public float total_elevation_gain { get; set; }
        public string type { get; set; }
        public int? workout_type { get; set; }
        public long id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime start_date_local { get; set; }
        public string timezone { get; set; }
        public float utc_offset { get; set; }
        public object location_city { get; set; }
        public object location_state { get; set; }
        public string location_country { get; set; }
        public int achievement_count { get; set; }
        public int kudos_count { get; set; }
        public int comment_count { get; set; }
        public int athlete_count { get; set; }
        public int photo_count { get; set; }
        public Map map { get; set; }
        public bool trainer { get; set; }
        public bool commute { get; set; }
        public bool manual { get; set; }
        public bool _private { get; set; }
        public string visibility { get; set; }
        public bool flagged { get; set; }
        public string gear_id { get; set; }
        public float[] start_latlng { get; set; }
        public float[] end_latlng { get; set; }
        public double average_speed { get; set; }
        public double max_speed { get; set; }
        public double average_cadence { get; set; }
        public double average_watts { get; set; }
        public int max_watts { get; set; }
        public int weighted_average_watts { get; set; }
        public double kilojoules { get; set; }
        public bool device_watts { get; set; }
        public bool has_heartrate { get; set; }
        public double average_heartrate { get; set; }
        public double max_heartrate { get; set; }
        public bool heartrate_opt_out { get; set; }
        public bool display_hide_heartrate_option { get; set; }
        public float elev_high { get; set; }
        public float elev_low { get; set; }
        public long? upload_id { get; set; }
        public string upload_id_str { get; set; }
        public string external_id { get; set; }
        public bool from_accepted_tag { get; set; }
        public int pr_count { get; set; }
        public int total_photo_count { get; set; }
        public bool has_kudoed { get; set; }
        public float suffer_score { get; set; }
    }

    public class StravaDataStream
    {
        public string type { get; set; }
        public float[] data { get; set; }
        public string series_type { get; set; }
        public int original_size { get; set; }
        public string resolution { get; set; }
    }

    public class Athlete
    {
        public int id { get; set; }
        public string username { get; set; }
        public int resource_state { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string bio { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string sex { get; set; }
        public bool premium { get; set; }
        public bool summit { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int badge_type_id { get; set; }
        public float weight { get; set; }
        public string profile_medium { get; set; }
        public string profile { get; set; }
        public object friend { get; set; }
        public object follower { get; set; }
    }

    public class Map
    {
        public string id { get; set; }
        public string summary_polyline { get; set; }
        public int resource_state { get; set; }
    }


    public class AthleteStats
    {
        public float biggest_ride_distance { get; set; }
        public float biggest_climb_elevation_gain { get; set; }
        public Recent_Ride_Totals recent_ride_totals { get; set; }
        public All_Ride_Totals all_ride_totals { get; set; }
        public Recent_Run_Totals recent_run_totals { get; set; }
        public All_Run_Totals all_run_totals { get; set; }
        public Recent_Swim_Totals recent_swim_totals { get; set; }
        public All_Swim_Totals all_swim_totals { get; set; }
        public Ytd_Ride_Totals ytd_ride_totals { get; set; }
        public Ytd_Run_Totals ytd_run_totals { get; set; }
        public Ytd_Swim_Totals ytd_swim_totals { get; set; }
    }

    public class Recent_Ride_Totals
    {
        public int count { get; set; }
        public float distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public float elevation_gain { get; set; }
        public int achievement_count { get; set; }
    }

    public class All_Ride_Totals
    {
        public int count { get; set; }
        public int distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public int elevation_gain { get; set; }
    }

    public class Recent_Run_Totals
    {
        public int count { get; set; }
        public float distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public float elevation_gain { get; set; }
        public int achievement_count { get; set; }
    }

    public class All_Run_Totals
    {
        public int count { get; set; }
        public int distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public int elevation_gain { get; set; }
    }

    public class Recent_Swim_Totals
    {
        public int count { get; set; }
        public float distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public float elevation_gain { get; set; }
        public int achievement_count { get; set; }
    }

    public class All_Swim_Totals
    {
        public int count { get; set; }
        public int distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public int elevation_gain { get; set; }
    }

    public class Ytd_Ride_Totals
    {
        public int count { get; set; }
        public int distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public int elevation_gain { get; set; }
    }

    public class Ytd_Run_Totals
    {
        public int count { get; set; }
        public int distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public int elevation_gain { get; set; }
    }

    public class Ytd_Swim_Totals
    {
        public int count { get; set; }
        public int distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public int elevation_gain { get; set; }
    }

}
