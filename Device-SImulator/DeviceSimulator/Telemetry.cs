using System.IO;
using System.Collections.Generic;

/// <summary>
/// Handles loading data from data file
/// </summary>
public class Telemetry
{
	private const string dataFile = "./data.csv";

	public static List<TelemetryData> GetDataLines()
    {
		using (StreamReader sr = new StreamReader(dataFile))
		{
			List<TelemetryData> allTelemetryData = new List<TelemetryData>();
			sr.ReadLine();
			while (!sr.EndOfStream)
			{
				allTelemetryData.Add(CreateTelemetryData(sr.ReadLine()));
			}
			return allTelemetryData;
		}
    }

	private static TelemetryData CreateTelemetryData(string line)
    {
		TelemetryData data = new TelemetryData();
		string[] split = line.Split(',');
		data.turbineId = split[0];
		data.timeInterval = split[1];
		int.TryParse(split[2], out data.eventCode);
		data.eventCodeDescription = split[3];
		double.TryParse(split[4], out data.windSpeed);
		double.TryParse(split[5], out data.temperature);
		double.TryParse(split[6], out data.rotorSpeed);
		double.TryParse(split[7], out data.power);
		return data;
	}
}
