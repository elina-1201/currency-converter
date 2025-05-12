
# Currency Converter

A mobile application for currency conversion and real-time exchange rate preview.

## Technical Implementation

Developed in **C#** using **.NET MAUI (Multi-platform App UI)**.

The application uses an external API to retrieve the latest exchange rates. The API endpoint ensures that the application always has up-to-date data. Retrieved data is stored locally in JSON format to be available in offline mode. This local copy is used for all calculations.

## How It Works

- On launch, the app fetches the latest exchange rates from [this API](https://open.er-api.com/v6/latest).
- Retrieved data is stored locally in JSON format for offline use.
- Users input an amount and select currencies for conversion.
- The app uses locally stored data to calculate and display the conversion result.


## Features
-   Simple, intuitive and appealing user interface
-   Functionality in both offline and online modes
-   Accurate and up-to-date exchange rates
-   Support for multiple currencies
