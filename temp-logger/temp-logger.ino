#include <DHT11.h>

#define DHTPIN 2

DHT11 dht11(DHTPIN);

void setup() {
  Serial.begin(9600);
}

void loop() {
  delay(10000);
  int temperature = 0;
  int humidity = 0;

  // Attempt to read the temperature and humidity values from the DHT11 sensor.
  int result = dht11.readTemperatureHumidity(temperature, humidity);

  // Check the results of the readings.
  // If the reading is successful, print the temperature and humidity values.
  // If there are errors, print the appropriate error messages.
  if (result == 0) {
    Serial.print(temperature);
    Serial.print("; ");
    Serial.print(humidity);
    Serial.println();
  } 
  else {
    // Print error message based on the error code.
    Serial.println(DHT11::getErrorString(result));
  }
}
