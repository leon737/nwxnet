namespace NWXNet
{
    public static class Altitude
    {
        public static string EnumToUnitCode(AltitudeUnit unit)
        {
            switch(unit)
            {
                case AltitudeUnit.ImperialFlightLevel:
                    return "F";

                case AltitudeUnit.ImperialAltitude:
                    return "A";

                case AltitudeUnit.MetricFlightLevel:
                    return "S";

                case AltitudeUnit.MetricAltitude:
                    return "M";
            }
            return null;
        }

        public static AltitudeUnit UnitCodeToEnum(string code)
        {
            switch (code)
            {
                case "F":
                    return AltitudeUnit.ImperialFlightLevel;

                case "A":
                    return AltitudeUnit.ImperialAltitude;

                case "S":
                    return AltitudeUnit.MetricFlightLevel;

                case "M":
                    return AltitudeUnit.MetricAltitude;
            }
            return AltitudeUnit.ImperialFlightLevel;
        }

        public static string FormatUnitString(int altitude, AltitudeUnit unit)
        {
            switch (unit)
            {
                case AltitudeUnit.ImperialFlightLevel:
                    return "FL" + altitude;

                case AltitudeUnit.ImperialAltitude:
                    return altitude + "ft";

                case AltitudeUnit.MetricFlightLevel:
                    return "S" + altitude;

                case AltitudeUnit.MetricAltitude:
                    return altitude + "m";
            }
            return null;
        }
    }
}
