# .NET Libraries for Eikon Data APIs Quick Start
The Eikon Data APIs provide simple access to users who require programmatic access to Thomson Reuters data on the desktop. These APIs are intended for Eikon users.

The APIs provide access to the following content sets:

* Fundamental and reference data
* Price time series
* News
* Symbology conversion

This document will explain how to install and use .NET libraries for Eikon Data APIs.

## Installation
The .NET Eikon Data APIs are 64-bit libraries which target the following .NET frameworks: .NET 4.5, .NET 4.5.1. .NET 4.5.2, .NET 4.6, .NET 4.6.1, and .NET Standard 2.0. It is available in [NuGet](https://www.nuget.org/packages/EikonDataAPI/). It can be installed by using the following commands.

* **Package Manager**
>```
>PM> Install-Package EikonDataAPI -Version 0.4.3
>```
* **.NET CLI**
>```
> > dotnet add package EikonDataAPI --version 0.4.3
>```
* **Paket CLI**
>```
> > paket add EikonDataAPI --version 0.4.3
>```

## Dependencies
The .NET Eikon Data APIs depend on the following libraries.

**.NET Framework 4.5, 4.5.1, 4.5.2**
> * Newtonsoft.Json (>=11.0.1)
> * Microsoft.Extensions.Logging (>=1.1.2)
> * Deedle (>=1.2.5)

**.NET Framework 4.6, 4.6.1**
> * Newtonsoft.Json (>=11.0.1)
> * Microsoft.Extensions.Logging (>=2.0.1)
> * Deedle (>=1.2.5)

**.NET Standard 2.0**
> * Newtonsoft.Json (>=11.0.1)
> * Microsoft.Extensions.Logging (>=2.0.1)

_Note: Eikon Data APIs for .NET Standard 2.0 only support raw interfaces which return the data as JSON strings. It doesn't support Deedle data frame._

## Usage
After installing **EikonDataAPI**, the application can use the API by calling the **Eikon.CreateDataAPI()** static method to create an **Eikon** interface. This interface is an access point to all functionalities in Eikon Data APIs.

Then, the first method that must be called with the Eikon interface is **Eikon.SetAppKey()**. This method accepts one string argument that represents the application ID. The application ID identifies your application on Thomson Reuters Platform. You can get an application ID using the Application ID generator. For more information, please refer to [the Quick Start guide](https://developers.thomsonreuters.com/eikon-data-apis/quick-start).

```csharp
using EikonDataAPI;
...

IEikon eikon = Eikon.CreateDataAPI();
eikon.SetAppKey("xxxxx");
...
```

After that, the application can call other retrieval methods prefixed with **Get** to retrieve the data. If there is an error when retrieving the data, these methods can throw **EikonException**. Therefore, the application needs to call retrieval methods inside **try/catch** block.

```csharp
 try{
    var response = eikon.GetNewsHeadlines();
    ...
 }catch(EikonException ex){
     ...
 }
```

## Data Frame vs Raw Interfaces
There are two types of retrieval methods in the libraries:
* The methods which return data in data frames, such as **GetData**, **GetTimeSeries**, and **GetNewsHeadlines**
* The methods which suffixed with **Raw** which return a data as a JSON string, such as **GetDataRaw**, **GetTimeSeriesRaw**, and **GetNewsHeadlinesRaw** 

The libraries use [Deedle](http://bluemountaincapital.github.io/Deedle/index.html) to create a data frame from the retrieved raw data. A data frame is a structure containing multiple columns that share the same row key similar to a data table or a spreadsheet. Deedle provides many convenient methods to manipulate and analyze a data frame.

```
     Security TIMESTAMP             HIGH    CLOSE   LOW     OPEN    COUNT   VOLUME     
0 -> TRI.N    3/12/2018 12:00:00 AM 40.64   40.49   40.3    40.4    913     171658
1 -> TRI.N    3/13/2018 12:00:00 AM 40.68   39.87   39.81   40.49   1201    198656
2 -> TRI.N    3/14/2018 12:00:00 AM 40.06   39.69   39.48   40.04   1081    188959
3 -> IBM.N    3/12/2018 12:00:00 AM 161.01  160.26  158.93  159.76  5963    1273673
4 -> IBM.N    3/13/2018 12:00:00 AM 162.11  159.32  158.84  160.09  5658    1027086
5 -> IBM.N    3/14/2018 12:00:00 AM 160.59  158.12  157.75  160.17  4636    925920
```

However, you can also use raw interfaces to retrieve a raw JSON string. Then, it can be passed to other methods or libraries for logging, parsing, or analyzing.

```json
{
  "mappedSymbols": [
    {
      "bestMatch": {
        "CUSIP": "884903105",
        "ISIN": "CA8849031056"
      },
      "symbol": "TRI.N"
    }
  ]
}
```

Currently, Deedle doesn't support .NET Standard so Eikon Data APIs library for .NET Standard 2.0 only supports raw interfaces which return the data as JSON strings.

_Note: There is one exception for the **GetNewsStory** method which returns a string instead of a data frame._

## Fundamental and reference data
You can use **GetData()** methods to retrieve fundamental and reference data. These methods accept three parameters.

|Name         |Type      |Description   |
|-------------|----------|--------------|
|instrument(s)|string, or list of string|**Required**: Single instrument or list of instruments to request.|
|fields      |string, list of string, or list of TRField      |**Required**: <p> A single field as a string, such as _'TR.PriceClose'_</p> <p>A list of fields, such as <br/> _new List<string> { "TR.TotalReturn.Date", "TR.TotalReturn" }_</p><p>A list of TRFields, such as<br/>_new List<TRField> {eikon.TRField("TR.TotalReturn.Date"), eikon.TRField("TR.TotalReturn") }_</p>|
|parameters        |dictionary    |**Optional**: Dictionary of global parameters to request, such as <p>_new Dictionary<string, string> { { "SDate", "2017-10-23"},{"EDate","2017-11-29" },{"Frq","D"},{"CH","Fd" },{"RH","IN" } }_</p>| 

### Examples

The following code retrieves fundamental data - Revenue and Gross Profit - for Google, Microsoft, and Facebook.

```csharp
List<string> instruments = new List<string> {
   "GOOG.O",
   "MSFT.O",
   "FB.O"
};

List<string> fields = new List<string> {
    "TR.Revenue",
    "TR.GrossProfit"
};              

var data = eikon.GetData(
    instruments,
    fields
    );

data.Print(); //print Deedle frame
```
The output is:
```
     Instrument Revenue      Gross Profit
0 -> GOOG.O     110855000000 65272000000
1 -> MSFT.O     89950000000  55689000000
2 -> FB.O       40653000000  35199000000
```
You can specify additional parameters and request full year revenue and gross profit for the last two years scaled to millions and converted to Euros.
```csharp
List<string> instruments = new List<string> {
    "GOOG.O",
    "MSFT.O",
    "FB.O",
    "AMZN.O",
    "TWTR.K"
};

List<string> fields = new List<string> {
    "TR.Revenue.date",
    "TR.Revenue",
    "TR.GrossProfit"
};

Dictionary<string, string> parameters = new Dictionary<string, string> {
    { "Scale", "6" },
    { "SDate", "0" },
    { "EDate","-2"},
    { "FRQ","FY" },
    { "Curn","EUR" }
};

var data = eikon.GetData(
    instruments,
    fields,
    parameters
    );

data.Print();         //print Deedle frame
```
The output is:
```
      Instrument Date                   Revenue       Gross Profit
0  -> GOOG.O     12/31/2017 12:00:00 AM 92409.83655   54411.39192
1  -> GOOG.O     12/31/2016 12:00:00 AM 85866.7264    52443.4608
2  -> GOOG.O     12/31/2015 12:00:00 AM 69050.62109   43116.92825
3  -> MSFT.O     6/30/2017 12:00:00 AM  78744.9285    48751.82127
4  -> MSFT.O     6/30/2016 12:00:00 AM  76837.4856    47316.4732
5  -> MSFT.O     6/30/2015 12:00:00 AM  84041.3906    54370.95394
6  -> FB.O       12/31/2017 12:00:00 AM 33888.74733   29342.23839
7  -> FB.O       12/31/2016 12:00:00 AM 26289.2656    22685.1688
8  -> FB.O       12/31/2015 12:00:00 AM 16508.28168   13868.31941
9  -> AMZN.O     12/31/2017 12:00:00 AM 148270.87626  54961.57452
10 -> AMZN.O     12/31/2016 12:00:00 AM 129350.8344   45393.1664
11 -> AMZN.O     12/31/2015 12:00:00 AM 98532.19486   32555.23755
12 -> TWTR.K     12/31/2017 12:00:00 AM 2036.75847939 1318.81853577
13 -> TWTR.K     12/31/2016 12:00:00 AM 2406.1735928  1519.4269048
14 -> TWTR.K     12/31/2015 12:00:00 AM 2042.38604592 1370.87982856
```
## Price time series
You can use **GetTimeSeries()** methods to retrieve historical data on one or several RICs. These methods accept eight parameters.

|Name         |Type      |Description   |
|-------------|----------|--------------|
|ric(s)|string or list of strings|**Required**: Single RIC or List of RICs to retrieve historical data for|
|startDate |string or DateTime |**Required**: Starting date and time of the historical range. String format is "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:sszzz", or "yyyy-MM-ddTHH:mm:ssZ"|
|endDate |string or DateTime |**Required**: End date and time of the historical range. String format is "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:sszzz", or "yyyy-MM-ddTHH:mm:ssZ"|
|interval|**Interval** enumeration|**Optional**: Data interval. Possible values: tick, minute, hour, daily, weekly, monthly, quarterly, yearly (Default: daily)|
|fields|List of **TimeSeriesField** enumerations|**Optional**: Use this parameter to filter the returned fields set. Available fields: TIMESTAMP, VALUE, VOLUME, HIGH, LOW, OPEN, CLOSE, COUNT. By default all fields are returned|
|count|integer|**Optional**: 	Max number of data points retrieved |
|calendar|**Calendar** enumeration|**Optional**: Possible values: native, tradingdays, calendardays |
|Corax|**Corax** enumeration|**Optional**: 	Possible values: adjusted, unadjusted |

### Example

The following code returns time series of daily price history for Microsoft Corp ordinary share between 1st of Jan and 10th of Jan 2018.

```csharp
var data = eikon.GetTimeSeries(
    "MSFT.O",                  //a single RIC to retrieve historical data for
    new DateTime(2018, 1, 1),  //starting date of the historical range
    new DateTime(2018, 1, 10)  //end date of the historical range
    );

data.Print();                  //print Deedle frame
```
The output is:
```
     Security TIMESTAMP            HIGH    CLOSE LOW     OPEN   COUNT  VOLUME
0 -> MSFT.O   1/2/2018 12:00:00 AM 86.31   85.95 85.5    86.125 119264 22483797
1 -> MSFT.O   1/3/2018 12:00:00 AM 86.51   86.35 85.97   86.055 126562 26061439
2 -> MSFT.O   1/4/2018 12:00:00 AM 87.66   87.11 86.57   86.59  124561 21911974
3 -> MSFT.O   1/5/2018 12:00:00 AM 88.41   88.19 87.43   87.66  118899 23407110
4 -> MSFT.O   1/8/2018 12:00:00 AM 88.58   88.28 87.6046 88.2   104870 22113049
5 -> MSFT.O   1/9/2018 12:00:00 AM 88.7272 88.22 87.86   88.65  129008 19484317
```
## News
You can use **GetNewsHeadlines()** methods to get a list of news headlines. These methods accept four parameters.


|Name         |Type      |Description   |
|-------------|----------|--------------|
|query|string|**Optional**: News headlines search criteria. The text can contain RIC codes, company names, country names and operators (AND, OR, NOT, IN, parentheses and quotes for explicit search…). <p> (Default: Top News written in English) </p>|
|count|integer|**Optional**: Max number of headlines retrieved. Value Range: [1-100]. (Default: 10)|
|dateFrom|string or DateTime|**Optional**: Beginning of date range. String format is "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:sszzz", or "yyyy-MM-ddTHH:mm:ssZ" |
|dateTo|string or DateTime|**Optional**: End of date range. String format is "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:sszzz", or "yyyy-MM-ddTHH:mm:ssZ" |

### Examples

The following code retrieves news headlines on Deutsche Lufthansa AG (equity RIC: LHAG.DE), between 09:00 and 18:00 GMT on the 5th of Apr 2017.

```csharp
var news = eikon.GetNewsHeadlines(
    "R:LHAG.DE",             //Retrieve news headlines related to LHAG.DE
    null,                    //count parameter is null so it will use the default value which is 10
    "2017-04-05T09:00:00Z",  //Beginning of date range
    "2017-04-05T18:00:00Z"   //End of date range
    );

news.Print();                //print Deedle frame

```
The output is:

```
     FirstCreated         VersionCreated       Text        StoryId          SourceCode
0 -> 4/5/2017 2:01:51 PM  4/5/2017 2:01:56 PM  Austrian Airlines enhances Boeing 777 Jet with Blue Danube Waltz<ODDN.UL> urn:newsml:reuters.com:20170405:nNRA3m80yv:1 NS:DATMTR
1 -> 4/5/2017 1:12:17 PM  4/5/2017 1:12:17 PM  FOKUS 1-Tiergesundheit gibt Boehringer Schub - Zuversicht für 2017 urn:newsml:reuters.com:20170405:nL5N1HD1RK:1 NS:RTRS
2 -> 4/5/2017 10:41:12 AM 4/5/2017 10:41:12 AM Airline services group dnata looking for more catering acquisitions urn:newsml:reuters.com:20170405:nL5N1HD26H:1 NS:RTRS
3 -> 4/5/2017 9:05:06 AM  4/5/2017 9:05:19 AM  Restaurant Brands International announces agreement to grow the BURGER KING brand in sub-Saharan Africa<BKCBK.UL><QSR.TO> urn:newsml:reuters.com:20170405:nNRA3m66rw:1 NS:ENPNWS
```

Then, you can use the **GetNewsStory()** method to retrieve a single news story corresponding to the identifier provided in **StoryId**. This method accepts one parameter.

|Name         |Type      |Description   |
|-------------|----------|--------------|
|storyId|string|**Required**: The story Id of the news story

This method returns a string of story text.

## Example

The following code retrieves the news story of the first retrieved news headline.

```csharp
var headlines = eikon.GetNewsHeadlines(
    "R:LHAG.DE",            //Retrieve news headlines related to LHAG.DE
    null,                   //count parameter is null so it will use the default value which is 10
    "2017-04-05T09:00:00Z", //Beginning of date range
    "2017-04-05T18:00:00Z"  //End of date range
    );

string storyId = headlines.GetColumn<string>("StoryId").FirstValue(); //Get the StoryId of the first headline
    
Console.WriteLine(eikon.GetNewsStory(storyId));     //get and print story text

```

The output is the HTML of the news story.
```html
<div class="storyContent" lang="en"><p>Austrian Airlines AG, an airline in Austria, has enhanced the Boeing 777 Jet...
```

## Symbology conversion
You can call **GetSymbology()** methods to get a list of instrument names converted into another instrument code, such as converting SEDOL instrument names to RIC names. These methods accept four parameters.

|Name         |Type      |Description   |
|-------------|----------|--------------|
|symbol(s)|string or list of strings|**Required**:Single instrument or list of instruments to convert|
|fromSymbolType|**SymbologyType** enumeration|**Optional**: Instrument code to convert from. Possible values: RIC,ISIN,CUSIP,SEDOL,ticker,lipperID,OAPermID,or IMO (Default: RIC)|
|toSymbolType|List of **SymbologyType** enumeration|**Optional**: List of instrument code to convert to. Possible values: RIC,ISIN,CUSIP,SEDOL,ticker,lipperID,OAPermID,or IMO|
|bestMatch|boolean|**Optional**:Defines whether "**bestMatch**" section only is required. (Default: true)

## Example
The following code converts RICs to ISINs and tickers and returns only best match.

```csharp
List<string> instruments = new List<string> {
    "MSFT.O",
    "GOOG.O",
    "IBM.N"
};

List<SymbologyType> toSymbolType = new List<SymbologyType> {
    SymbologyType.ticker,
    SymbologyType.ISIN
};

var responseSymbol = eikon.GetSymbology(
    instruments,
    SymbologyType.RIC, 
    toSymbolType
    );

responseSymbol.Print();       //print Deedle frame
```
The output is:
```
          ISIN         ticker
MSFT.O -> US5949181045 MSFT
GOOG.O -> US02079K1079 GOOG
IBM.N  -> US4592001014 IBM
```
## Logging
The libraries use the **Microsoft.Extensions.Logging** package for logging. To support console log, it requires the **Microsoft.Extensions.Logging.Console** package. After adding this package, you can use the following code to enable console logging with debug level.

```csharp
using Microsoft.Extensions.Logging;
...
eikon.GetLoggerFactory().AddConsole(LogLevel.Debug);
```
## Summary
.NET 64 bit Eikon Data APIs are available in  [nuget](https://www.nuget.org/packages/EikonDataAPI/). It supports .NET 4.5, .NET 4.5.1. .NET 4.5.2, .NET 4.6, .NET 4.6.1, and .NET Standard 2.0. 

It provides access to the following content sets:

* Fundamental and reference data
* Price time series
* News
* Symbology conversion

The retrieval methods format and return data as a [Deedle](http://bluemountaincapital.github.io/Deedle/index.html) data frame for ease of use. However, users can decide to retrieve data as a raw JSON string by using raw interfaces. However, for .NET Standard 2.0, it only supports raw interfaces.

## Reference
1. [.NET Eikon Data APIs](https://www.nuget.org/packages/EikonDataAPI/)
2. [Quick Start guide for Eikon Data APIs](https://developers.thomsonreuters.com/eikon-data-apis/quick-start)
3. [Deedle](http://bluemountaincapital.github.io/Deedle/index.html)
