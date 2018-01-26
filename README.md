# Cryptobot

Framework for creating crypto bots on Binance.com

At the moment the bot contains a implementation of the Simple Trend Reversal Strategy

But things can easily be changed/adapted to other strategies

See https://www.forexfactory.com/showthread.php?t=713593 for more details on this strategy


#### Binance API KEYS

Cryptobot communicates with binance.com to get the latest pricing info for all crypto currencies.
For this it needs a so called api key and api secret. You can find those in binance.com under your account details.
Please fill in your personal api key & api secret in the app.config file (see below)

#### Sending alerts to a Telegram bot
If you want to send signals to your telegram channel then you will need to:

1) Create a telegram bot using botfather (google, there is plenty on info on how to do this)
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
