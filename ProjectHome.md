This .NET 4.0 library provides a fluent interface to the NWX weather service hosted at navlost.eu.


From navlost.eu:

_The NWX Weather Service is a web service providing atmospheric weather information from a variety of sources through a common XML-based interface. It has the ability to instantly provide current weather forecast information on demand for almost anywhere in the world. This works by downloading elements from a current realisation of the Global Forecast System numeric weather model and storing them in the server in a manner that allows quick and efficient access to individual values._

_The service is also capable of producing derived information in a number of ways, notably in the form of charts and vertical profiles, and complements the weather data with other useful items such as a terrain elevation model and geomagnetic data._

_Its primary use is as an easy to access source of weather data for third party software, with a focus on aviation applications._

**_Main Features_**

  * _Winds aloft: NWX provides forecast winds aloft information up to four days in advance anywhere in the world, from the surface up to flight level 300, which is used by flight planning software to give a better estimate of flight time and fuel consumption en route, and to optimise both, e.g., by choosing the most rapid or economical route and flight level according to the forecast winds._
  * _Temperature: It provides forecast temperature information in the atmosphere as well as on the ground, which assists flight planning, e.g., by estimating the risk of icing._
  * _Terrain and Geopotential Altitude: This service can estimate how high you are actually flying over the ground by correcting the barometric altitude by pressure and density and thus assist in determining terrain clearance, gliding range, driftdowns, etc. It also incorporates a high resolution terrain database which gives the highest (not mean) recorded terrain elevation in each data point._
  * _Charts and Cross-sections: You can create on-the-fly charts showing atmospheric phenomena, such as forecast precipitation, cloud, winds, temperature, pressure, etc., and even obtain cross-sections of the atmosphere (e.g., along a flight route) with much the same information._
  * _METAR and TAF Reports: In addition to the forecast data, the system also provides a feed of observed weather in the form of worldwide METAR data access, as well as TAF reports. Both are archived and the historical reports are accessible via the web service interface._
  * _Ancillary Data: Other useful information, mainly for flight planning but also relevant to other activities is provided, including calculating the magnetic declination (variation) on any point of the Earth, sunrise/sunset times, etc._

NWX Data © 2010 Diego Berge http://navlost.eu