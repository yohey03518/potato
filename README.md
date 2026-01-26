# Potato Trading System

This is a stock trading simulation system that connects to the Fugle API to fetch real-time market data.

## Prerequisites

- .NET 8 SDK installed on your machine.
- A Fugle API Token (Get one from [Fugle Developer](https://developer.fugle.tw/)).

## Setup

1.  **Clone the repository**:
    ```bash
    git clone <repository_url>
    cd potato
    ```

2.  **Set up the Fugle API Key**:
    The application reads the API key from the `FugleApi:ApiKey` configuration. You can set this using an environment variable.

    **macOS / Linux**:
    ```bash
    export FugleApi__ApiKey=your_api_token_here
    ```

    **Windows (PowerShell)**:
    ```powershell
    $env:FugleApi__ApiKey="your_api_token_here"
    ```

    *Note: The double underscore `__` is the separator for nested configuration sections in environment variables.*

## Running the Application

To run the `Potato.Client` which fetches and logs the stock price for TSMC (2330):

```bash
dotnet run --project src/Potato.Client/Potato.Client.csproj
```

The application will start, fetch the quote, log the result to the console, and then automatically shut down.
