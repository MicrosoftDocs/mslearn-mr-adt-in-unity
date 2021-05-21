using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// This class will be updated to follow our model of reading in device IDs from ConnectionStrings.csv, getting their strings from IoT hub itself.  
/// </summary>
public static class DeviceIdLoader
{
    static string fileLocation = "./DeviceIds.csv";
    
    public static List<string> GetDeviceIds()
    {
        try
        {
            using (StreamReader sr = new StreamReader(fileLocation))

            {
                List<string> deviceIds = new List<string>();
                while (!sr.EndOfStream)
                {
                    string name = sr.ReadLine();
                    Console.WriteLine(name);
                    deviceIds.Add(name);
                }
                sr.Close();
                return deviceIds;
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
        
    }
}
