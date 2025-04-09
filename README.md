# PPS Register

A .NET application for managing Personal Property Security registrations.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/downloads)

## Local Development Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/samofwise/pps-register.git
   cd pps-register
   ```

2. Open the pps-register.code-workspace in VS Code:

   ```bash
   code pps-register.code-workspace
   ```

3. Start the Project with f5 or via cli:

   ```bash
   dotnet build
   ```

4. Run the tests:
   ```bash
   dotnet test
   ```

## Running Tests

The project uses TestContainers for database testing. Make sure Docker Desktop is running before executing tests.

### Database Tests

The database tests use SQL Server 2022 running in a Docker container. The container is automatically:

- Started before tests run
- Configured with the necessary schema
- Cleaned up after tests complete

### Test Configuration

- SQL Server password: `Str0ngP@ssw0rd!2024#Test`
- Database name: `PPSRegisterTest`
- Port: Dynamically assigned by TestContainers

## CI/CD

The project uses GitHub Actions for continuous integration. The workflow:

- Runs on every push to main
- Runs on every pull request to main
- Executes all tests using TestContainers
- Requires Docker to be available in the CI environment

## Project Structure

```
pps-register/
├── pps-register-api/           # Backend API
│   ├── PPSRegister.Api/        # API project
│   ├── PPSRegister.Data/       # Data access layer
│   ├── PPSRegister.PPSUploader/# Background service
│   └── PPSRegister.Tests/      # Test project
└── pps-register-app/           # Frontend React application
```

## Contributing

1. Create a feature branch from `main`
2. Make your changes
3. Run tests locally
4. Create a pull request

## License

[Add your license information here]
