/// <summary>
/// Holds data for telemetry
/// </summary>
public class TelemetryData
{
	public string turbineId;
	public string timeInterval;
	public int eventCode;
	public string eventCodeDescription;
	public double windSpeed;
	public double temperature;
	public double rotorSpeed;
	public double power;

	public TelemetryData() { }
    
	public TelemetryData(TelemetryData data)
    {
		turbineId = data.turbineId;
		timeInterval = data.timeInterval;
		eventCode = data.eventCode;
		eventCodeDescription = data.eventCodeDescription;
		windSpeed = data.windSpeed;
		temperature = data.temperature;
		rotorSpeed = data.rotorSpeed;
		power = data.power;
    }
}
