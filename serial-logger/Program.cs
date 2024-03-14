using System.IO.Ports;
using Newtonsoft.Json;

RespostaApi respostaApi = null;

SerialPort serial = new SerialPort("COM12", 9600);
serial.ReadTimeout = 2500;
serial.WriteTimeout = 2500;
serial.Open();

string caminho = "C:\\Users\\gabriel\\Documents\\temp-log.txt";
if (!File.Exists(caminho))
{
    using (StreamWriter sw = File.CreateText(caminho))
    {
        sw.WriteLine("Data temperatura-local humidade-local temperatura-cidade humidade-cidade sensacao-cidade clima-cidade");
    }
}

Console.WriteLine("data - temp - humidade - temp api - humidade api - sensacao termica api - clima cidade");
while (true)
{
    try
    {
        var tempo = DateTime.Now.ToString("HH:mm:ss");
        tempo = tempo.Split(":")[1];
        int minutos = Convert.ToInt32(tempo);
        string msg = serial.ReadLine();

        if (!string.IsNullOrEmpty(msg))
        {
            decimal temp = respostaApi == null ? 0M : respostaApi.Main.Temp;
            decimal hum = respostaApi == null ? 0M : respostaApi.Main.Humidity;
            decimal rel = respostaApi == null ? 0M : respostaApi.Main.Feels_like;
            string clima = respostaApi == null ? "" : respostaApi.Weather[0].Main + " " + respostaApi.Weather[0].Description;

            Console.WriteLine(DateTime.Now.ToString() + "; " + msg.Replace("\r", "") + "; " + temp + "; " + hum + "; " + rel + "; " + clima);

            if ((minutos >= 30 && minutos < 32) || (minutos >= 0 && minutos < 2))
            {
                respostaApi = await GetHttpResponse();
                using (StreamWriter sw = new StreamWriter(caminho, true))
                {
                    Console.WriteLine("...::Escrevendo no arquivo::...");
                    sw.WriteLine(DateTime.Now.ToString() + "; " + msg.Replace("\r", "") + "; " + temp + "; " + hum + "; " + rel + "; " + clima);
                }
                Console.WriteLine("...::Arquivo fechado::...");
            }
        }
        Thread.Sleep(10000);
    }
    catch (Exception e)
    {
        Console.WriteLine("...::Erro::...");
        Console.WriteLine(e.Message);
        Console.WriteLine("...::Fim msg de erro::...");
    }
};

static async Task<RespostaApi?> GetHttpResponse()
{
    string apiKey = ""; // sua apiKey
    string lat = ""; // a latitude da sua localização
    string lon = ""; // a longitude da sua localização

    string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&appid={apiKey}";

    using (HttpClient client = new HttpClient())
    {
        HttpResponseMessage response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var responsestring = await response.Content.ReadAsStringAsync();

            if (responsestring.Contains("main"))
            {
                var mainTemp = JsonConvert.DeserializeObject<RespostaApi>(responsestring);
                return mainTemp;
            }
        }
        return null;
    }
}

public class Main
{
    public decimal Temp { get; set; }
    public decimal Feels_like { get; set; }
    public decimal Humidity { get; set; }
}

public class Weather
{
    public string Main { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class RespostaApi
{
    public Main Main { get; set; }
    public List<Weather> Weather { get; set; }
}