# RestaurantOrderManagement Project

## Overview

The `RestaurantOrderManagement` project is a microservices-based application designed to manage restaurant orders and menu items. The project consists of three main services:

1. **OrderService**: Handles the management of customer orders.
2. **MenuService**: Manages the restaurant's menu items.
3. **RestaurantOrderManagement**: The main application that integrates with the above services and provides user interfaces for managing orders and menu items.

Each service is built using ASP.NET Core and communicates via HTTP and RabbitMQ for messaging.

## Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- Services
  - [OrderService](#orderservice)
  - [MenuService](#menuservice)
  - [RestaurantOrderManagement](#restaurantordermanagement)
- [Technologies Used](#technologies-used)
- [Configuration](#configuration)
- [Deployment](#deployment)
- [Future Improvements](#future-improvements)
- [Contact](#contact)

## Project Structure

The project is organized into three main folders, each representing a separate service:

- `OrderService` - Manages order operations including creating, updating, and retrieving orders.
- `MenuService` - Handles menu item operations such as adding, updating, and retrieving menu items.
- `RestaurantOrderManagement` - Acts as the main UI and integrates with the `OrderService` and `MenuService`.

Each service has its own `Program.cs` and `Startup.cs` files for configuration and setup.

```
RestaurantOrderManagement/
│
├── OrderService/
│   ├── Controllers/
│   ├── Models/
│   ├── DB/
│   ├── Services/
│   ├── Program.cs
│   ├── Startup.cs
│
├── MenuService/
│   ├── Controllers/
│   ├── Models/
│   ├── DB/
│   ├── Services/
│   ├── Program.cs
│   ├── Startup.cs
│
└── RestaurantOrderManagement/
    ├── Controllers/
    ├── Models/
    ├── Views/
    ├── Program.cs
    ├── Startup.cs
```

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- RabbitMQ
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or any other SQL-compatible database)

### Installation

1. **Clone the repository**:

   ```
   git clone https://github.com/yourusername/RestaurantOrderManagement.git
   ```

2. **Navigate to the project directory**:

   ```
   cd RestaurantOrderManagement
   ```

3. **Install dependencies** for each service:

   ```
   dotnet restore
   ```

4. **Set up the database** by applying migrations for each service:

   ```
   dotnet ef database update --project OrderService
   dotnet ef database update --project MenuService
   ```

5. **Run RabbitMQ** (ensure RabbitMQ server is running locally).

### Running the Services

Run each service in separate terminal windows:

- **OrderService**:

  ```
  OrderService
  dotnet run
  ```

- **MenuService**:

  ```
  MenuService
  dotnet run
  ```

- **RestaurantOrderManagement**:

  ```
  RestaurantOrderManagement
  dotnet run
  ```

## Services

### OrderService

- **Purpose**: Manages customer orders.
- **Endpoints**:
  - `GET /api/orders` - Retrieve all orders.
  - `GET /api/orders/{id}` - Retrieve a specific order by ID.
  - `POST /api/orders` - Create a new order.
  - `PUT /api/orders/{id}` - Update an existing order.
  - `DELETE /api/orders/{id}` - Delete an order.
- **Messaging**:
  - Uses RabbitMQ to publish order events to the `orderExchange` with routing keys `order` and `status`.

### MenuService

- **Purpose**: Manages restaurant menu items.
- Endpoints
  - `GET /api/menuitems` - Retrieve all menu items.
  - `GET /api/menuitems/{id}` - Retrieve a specific menu item by ID.
  - `POST /api/menuitems` - Add a new menu item.
  - `PUT /api/menuitems/{id}` - Update an existing menu item.
  - `DELETE /api/menuitems/{id}` - Delete a menu item.

### RestaurantOrderManagement

- **Purpose**: Main application providing user interfaces for managing orders and menu items.

- Endpoints

  :

  - `GET /Orders` - View and manage customer orders.
  - `GET /Menu` - View and manage menu items.

## Technologies Used

- **ASP.NET Core** - For building web APIs and MVC applications.
- **RabbitMQ** - For message queueing between services.
- **Entity Framework Core** - For database access and ORM.
- **SQL Server** - As the database for storing order and menu data.
- **Swagger/OpenAPI** - For API documentation and testing.
- **Bootstrap** - For responsive UI components.

## Configuration

### RabbitMQ

The `RabbitMQService` class handles the connection to RabbitMQ and message publishing. Ensure RabbitMQ is running locally or provide the necessary connection details in the configuration.

### Database

Each service uses Entity Framework Core to interact with a SQL Server database. Connection strings should be configured in the `appsettings.json` file of each service.

```
jsonКопировать код{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RestaurantOrderDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Application Settings

Configure application-specific settings in `appsettings.json` or environment variables.

## Deployment

To deploy the project:

1. **Build Docker images** for each service if containerization is preferred.
2. **Deploy to a cloud provider** like Azure, AWS, or any Kubernetes cluster.
3. **Set up continuous integration and deployment (CI/CD)** pipelines using GitHub Actions, Azure DevOps, or any other CI/CD tool.

## Future Improvements

- **Authentication and Authorization**: Implement JWT-based authentication and role-based access control.
- **Enhanced Error Handling**: Improve error handling and logging across all services.
- **Scalability**: Introduce load balancing and container orchestration with Kubernetes.
- **Caching**: Use Redis for caching frequently accessed data to improve performance.
- **Monitoring and Logging**: Integrate tools like Prometheus and Grafana for monitoring and ELK stack for centralized logging.
- **API Gateway**: Implement an API Gateway for better API management and routing.

## Contact

For any questions or contributions, please contact:

- **Email**: rustampulatovwh@gmail.com
- **GitHub**: [Warhammer2000](https://github.com/Warhammer2000)
