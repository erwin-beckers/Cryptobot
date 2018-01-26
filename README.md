# Cryptobot

Framework for creating crypto bots on Binance.com
At the moment the bot implements the Simple Trend Reversal Strategy

See https://www.forexfactory.com/showthread.php?t=713593 for more details


#### Binance API KEYS

Cryptobot communicates with binance.com to get the latest pricing info.
For this it needs an api key and api secret. You can find those in binance.com under your account details.
Please fill in your personal api key & api secret in the app.config file:

#### Telegram bot
If you want to send signals to your telegram channel then you will need to:

1) Create a telegram bot (google, plenty on info on how to do this
2) Fill in the telegram bot api and telegram chat id in the app.config


### app.config
```c#
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1" />
    </startup>
  <appSettings>
    <add key="apikey" value="the api key you get from binance.com"/>
    <add key="apisecret" value="the api secret key you get from binance.com"/>
    <add key="telegramapi" value="the telegram bot api (optional)"/>
    <add key="telegramchatid" value="the telegram chat id (optional)"/>
  </appSettings>
</configuration>
```


## History

v1.01
- added trend reversal strategy
- added S&R indicator
- added ZigZag indicator
- added MBFX indicator
- added Trendline indicator
- added Telegram alerts

v1.00
- initial commit
