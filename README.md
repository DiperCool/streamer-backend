# 📺 Streamer - Modern Streaming Platform

[![Build Status](https://img.shields.io/badge/.NET-8.0-512bd4.svg?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Vertical_Slices-blue?style=for-the-badge)](https://github.com/jbogard/MediatR)
[![GraphQL](https://img.shields.io/badge/API-GraphQL-e10098?style=for-the-badge&logo=graphql)](https://chillicream.com/docs/hotchocolate)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](./LICENSE)

**Streamer** is a high-performance backend platform for streaming, built with .NET 8. The project demonstrates the practical application of modern software architecture patterns: **Vertical Slice Architecture** and **CQRS**.

The core highlight of this project is the use of **.NET Aspire** for local orchestration and **GraphQL (Hot Chocolate)** with Source Generators for maximum API performance.

---

## 🚀 Key Features

*   **Vertical Slice Architecture**: Each feature is self-contained and encapsulated.
*   **GraphQL First**: A powerful and flexible API powered by Hot Chocolate.
*   **CQRS & Mediator**: Clean separation of commands and queries using a custom mediator implementation.
*   **Event-Driven**: Asynchronous communication via RabbitMQ.
*   **Cloud Ready**: Integrations with AWS S3, Stripe, and Auth0.
*   **Aspire Orchestration**: Seamless startup of the entire infrastructure (Redis, RabbitMQ, DB) with a single click.

---

## 🛠 Technology Stack

### Core
- **.NET 8** — The latest and most performant version of the framework.
- **Entity Framework Core** — The primary ORM for data access.
- **Hot Chocolate** — GraphQL server supporting Source Generators.
- **.NET Aspire** — Orchestration for cloud-native applications and local development.

### Infrastructure
- **Redis** — Caching and distributed locks.
- **RabbitMQ** — Message broker for the event-driven architecture.
- **Hangfire** — Background tasks and scheduled jobs.
- **PostgreSQL** — The primary relational data store.
- **MongoDB** — Document-oriented database dedicated to bot-related state and metadata.

### Integrations
- **Stripe** — Payment and subscription processing.
- **Auth0** — Modern authentication and authorization.
- **AWS S3** — Media content and object storage.

---

## 🏗 System Architecture

The application is composed of several specialized services working together to provide a robust streaming experience:

*   **Traefik Gateway**: Acts as the entry point for all traffic, handling HTTPS termination (via ACME/Let's Encrypt) and routing RTMP/HLS traffic.
*   **Media Server (MediaMTX)**: A high-performance media server that manages live RTMP ingestion and HLS distribution.
*   **Streamer API**: The core backend service that handles business logic, user management, and exposes the GraphQL API.
*   **Background Processors**:
    *   **VOD Processor**: Handles the conversion of live streams into Video-on-Demand content, uploading them to S3 storage.
    *   **Preview Processor**: Generates real-time stream previews for the discovery interface.
    *   **Bot Service**: Orchestrates automated streaming bots that simulate live broadcasts by pushing pre-recorded video content to RTMP endpoints via FFmpeg.
*   **Storage Tier**:
    *   **PostgreSQL**: Stores the main relational business data (users, streamers, settings, etc.).
    *   **MongoDB**: Serves as the primary data store for the **Bot Service**, managing bot configurations, operational logs, and real-time execution data.
    *   **Redis**: High-speed caching and real-time state management.
    *   **RabbitMQ**: The central nervous system for inter-service communication and task distribution.

---

## 📁 Project Structure

*   **`src/Shared`**: Shared abstractions and wrappers for external services (Stripe, S3, Auth0).
*   **`src/streamer.AppHost`**: The .NET Aspire orchestration project.
*   **`src/Streamers.Api`**: The host for the GraphQL API and specific Minimal APIs.
*   **`src/Streamers.Features`**: The core of the system, containing all vertical slices of functionality.
*   **`tests/`**: Unit and integration tests to verify business logic and feature flows.

---

## 🚦 Getting Started

### Prerequisites
1.  **Docker Desktop** (required for running containers via Aspire).
2.  **.NET 8 SDK**.
3.  **IDE** (JetBrains Rider, Visual Studio 2022, or VS Code).

### How to Run
1. Clone the repository.
2. Open the `streamer.sln` solution.
3. Set `streamer.AppHost` as the startup project.
4. Press **F5** (or run `dotnet run` from the AppHost directory).

.NET Aspire will automatically provision and start the database, Redis, RabbitMQ, and open the Aspire Dashboard. From there, you can navigate to the GraphQL Playground endpoint (`/graphql`).

---

## 🧪 Testing

The project includes:
*   **Unit Tests**: Verifying domain logic and individual handlers.
*   **Integration Tests**: Comprehensive testing of feature slices using `WebApplicationFactory`. The setup utilizes **Testcontainers** to spin up real **PostgreSQL** and **Redis** instances, providing a production-like environment. **Respawn** is used to ensure state isolation by resetting the database between tests, while external integrations (Stripe, AWS S3, Auth0) are mocked using **NSubstitute**.

---

## 📜 License

This project is licensed under the **MIT** License. See the [LICENSE](./LICENSE) file for details.
