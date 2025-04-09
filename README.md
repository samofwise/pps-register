# [Personal Property Securities Register](https://github.com/samofwise/pps-register)

https://github.com/samofwise/pps-register

[![Tests](https://github.com/samofwise/pps-register/actions/workflows/tests-pipeline.yml/badge.svg)](https://github.com/samofwise/pps-register/actions/workflows/tests-pipeline.yml)

A .NET Aspire and React application for managing Personal Property Security registrations.

## Setup

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/downloads)

### Local Development Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/samofwise/pps-register.git
   cd pps-register
   ```

2. Open the pps-register.code-workspace in VS Code:

   ```bash
   code pps-register.code-workspace
   ```

3. Start the project via the cli or f5:

   ```bash
   dotnet run --project pps-register-api/PPSRegister.AppHost/PPSRegister.AppHost.csproj
   ```

## Using the Personal Property Security Register

Here is an outline of the basic features implemented in the react app:

- **Uploading Registrations**

  - Click the file upload and select a valid file
  - Drag a file from your computer onto the file upload
  - Drag one of the many example files onto the file upload
  - Displays a simple and clear response message

- **Temporary Client Switching**

  - On the top right you are able to switch between clients
  - This allows you to quickly reupload the same file for different clients
  - Implemented with an Http Header which would be replaced with proper OAuth Tokens and Indentity

- **Data Clearing**
  - On the top right you can also quickly clear all the data for the current client
  - This is for testing and demo purposes only

## Details

### Domain design

- For the domain design I selected a simple representation of two main entities. `PersonalPropertySecurities` and `PersonalPropertySecurityUploads`.
- This was to keep the domain design simple and only focus on the necessary entities to allow for time to be focused around building out the microservices architecture
- `PersonalPropertySecurityUploads` tracks all file uploads per client, using stored filenames to prevent duplicate submissions. Also it provides a denormalized summary of the processing results
- `PersonalPropertySecurities` stores all of the unique personal property securities per client in the registry.
- Also I do admit the names are too similar and may easily lead to confusion

### Scaleability

- I implemented a microservices architecture using .NET Aspire to enable independent scaling and load balancing of services
- I specifically separated the PPSUpload Worker into its own microservice to handle resource-intensive file processing independently
- I integrated AWS SQS to ensure reliable file processing during high load
- I configured health checks to quickly detect and respond to service issues
- I utilized React Query to provide instant feedback and reduce server load
- I implemented Docker containerization for consistent deployments
- I set up GitHub Actions to catch issues early and maintain quality

### Memory footprint

- I utilized Streams and StreamReaders to avoid loading entire files into memory
- I implemented batch processing in the SQS worker to improve throughput
- I implemented early validation to prevent unnecessary processing of invalid files, reducing memory usage and improving performance

#### Future Optimizations

- Implement parallel upload processing to decrease the time each item in the batch is held in memory
- Implement pagination to decrease the amount of data sets in Api responses

### Idempotency

- Implemented unique constraints in the database to prevent duplicate file uploads and VIN registrations
- Used AWS SQS message queue with message deletion after successful processing to ensure each upload is processed exactly once
- Added file name validation to prevent duplicate file uploads for the same client
- Implemented record-level idempotency by checking for existing records based on unique combinations of GrantorFirstName, GrantorLastName, VIN, and SpgAcn
- Added comprehensive unit tests to verify idempotent behavior across all operations

### Containerization

- Implemented containerization using .NET Aspire for service orchestration and management
- Containerized core services including API, PPS Uploader, Frontend, and SQL Server with persistent storage
- Configured secure HTTPS endpoints and service discovery for container communication
- Integrated Testcontainers.MsSql for reliable integration testing with automated cleanup
- Set up Docker Desktop for local development with consistent environment configuration

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

## CI/CD

The project uses GitHub Actions for continuous integration. The workflow:

- Runs on every push to main
- Runs on every pull request to main
- Executes all tests using TestContainers
- Requires Docker to be available in the CI environment

## Future Improvements

- Deploy this fully on AWS and link the demo
  - Utilize terraform in the AWS deployment
  - Trigger this via CI/CD
- Use AWS LocalStack for local development
- Double check the foreign key relationships to client (I think it isn't working correctly)
- Possibly implement a FK relationship between `PersonalPropertySecurities` and `PersonalPropertySecurityUploads`
- Implement EF Migrations
