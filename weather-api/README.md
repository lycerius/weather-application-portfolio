# Weather API

A simple ASP.NET Core Web API for weather forecasts.

## Prerequisites

- Docker installed on your machine.

## Building the Docker Image

To build the Docker image, run the following command from the `weather-api` directory:

```bash
docker build -t weather-api .
```

## Running the Application

To run the application in a Docker container, use:

```bash
docker run -p 8080:80 weather-api
```

The API will be available at `http://localhost:8080`.

## API Endpoints

- GET /weatherforecast - Returns a list of weather forecasts.

## Troubleshooting

- Ensure Docker is running and you have permissions to build and run containers.
- If you encounter build errors, check that all dependencies are correctly restored.
- For runtime issues, verify the port mapping and that no other service is using port 8080.