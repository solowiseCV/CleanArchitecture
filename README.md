# Clean Architecture - Movie & Payment API 

A modern  .NET API built using Clean Architecture principles, featuring movie management, user authentication, and integrated payment processing via Paystack.

## 📋 Overview

This project implements **Clean Architecture** with clear separation of concerns across multiple layers:

- **API Layer** (`CleanArchitecture.API`) – HTTP endpoints and request handling
- **Application Layer** (`CleanArchitecture.Application`) – Business logic, CQRS commands & queries, DTOs
- **Domain Layer** (`CleanArchitecture.Domain`) – Core entities and business rules
- **Infrastructure Layer** (`CleanArchitecture.Infra`) – Data access, external integrations (Paystack), migrations

## ✨ Key Features

- **Movie Management** – Create, read, and manage movies with premium tiers
- **User Authentication** – JWT-based auth with refresh tokens
- **Payment Processing** – Paystack integration for secure payments
- **Premium User System** – Grant premium status upon successful payment
- **Webhook Support** – Handle Paystack webhook events for asynchronous confirmations
- **Concurrency Control** – Optimistic locking with row versions to prevent race conditions
- **Structured Logging** – Serilog integration with file & console output

## 🏗️ Project Structure

```
CleanArchitecture/
├── CleanArchitecture.API/           # REST API controllers & middleware
│   ├── Controllers/                 # MovieController, PaymentController, UserController
│   ├── Middleware/                  # GlobalException handling
│   ├── Extensions/                  # Service registration (ApplicationService, etc.)
│   ├── Program.cs                   # Startup configuration
│   └── appsettings.json             # Configuration (DB, JWT, Paystack, Logging)
│
├── CleanArchitecture.Application/   # Application business logic
│   ├── DTOs/                        # Data Transfer Objects (PaystackDTOs, etc.)
│   ├── Features/                    # Feature-based organization
│   ├── Payments/
│   │   ├── Commands/                # MediatR commands
│   │   ├── Handlers/                # Command/Query handlers
│   │   │   ├── InitializePaymentHandler      # Create payment, call Paystack API
│   │   │   ├── VerifyPaymentHandler          # Verify transaction, handle concurrency
│   │   │   └── HandlePaystackWebhookHandler  # Async webhook processing
│   │   └── Queries/                 # Queries for payment data
│   ├── IRepository/                 # Repository abstractions
│   ├── IService/                    # Service abstractions (Paystack, etc.)
│   └── Mappings/                    # AutoMapper profiles
│
├── CleanArchitecture.Domain/        # Domain entities & rules
│   └── Entities/
│       ├── Movie.cs                 # Movie entity with premium flag
│       └── Payment.cs               # Payment entity with concurrency token
│
├── CleanArchitecture.Infrastructure/  # Infrastructure & data access
│   ├── Context/                     # ApplicationDbContext (EF Core)
│   ├── Repository/                  # Concrete repositories
│   │   ├── PaymentRepository        # CRUD + atomic operations
│   │   └── UnitOfWork               # Transaction management
│   ├── Services/
│   │   └── PaystackService          # Paystack API integration
│   ├── Migrations/                  # EF Core migrations
│   └── Extensions/                  # DI setup
│
└── CleanArchitecture.sln            # Solution file
```

## � Secrets & Configuration Management

⚠️ **IMPORTANT: Never commit `appsettings.json` or `.env` files containing secrets to version control!**

### Local Development Setup

1. **Create `appsettings.Development.json`** in `CleanArchitecture.API/` (excluded from Git):
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MovieDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     },
     "JwtSettings": {
       "Key": "your_very_long_and_secure_secret_key_at_least_32_chars",
       "ValidIssuer": "CleanArchitectureApi",
       "ValidAudience": "CleanArchitectureClient",
       "Expires": 60
     },
     "Paystack": {
       "SecretKey": "sk_test_YOUR_TEST_SECRET_KEY",
       "CallbackUrl": "https://localhost:5001/api/v1/payments/webhook"
     },
     "Serilog": {
       "MinimumLevel": {
         "Default": "Information"
       }
     }
   }
   ```

2. **Using User Secrets (Recommended for local dev)**  
   Instead of `appsettings.Development.json`, use .NET User Secrets:
   ```bash
   cd CleanArchitecture.API
   dotnet user-secrets init
   dotnet user-secrets set "JwtSettings:Key" "your_very_long_secure_key"
   dotnet user-secrets set "Paystack:SecretKey" "sk_test_YOUR_KEY"
   dotnet user-secrets set "Paystack:CallbackUrl" "https://localhost:5001/api/v1/payments/webhook"
   ```
   Secrets stored in `%APPDATA%\\Microsoft\\UserSecrets\\` are never committed.

3. **Production Deployment**  
   Use environment variables or your platform's secret management:
   - **Docker**: Pass via `--env` or `.env` file (add `.env*` to `.gitignore`)
   - **Azure**: Use Azure Key Vault
   - **AWS**: Use AWS Secrets Manager
   - **Local Server**: Set environment variables on the host

### Gitignore Rules

Currently configured to exclude:
- `appsettings.*.json` – All environment-specific configs
- `.env*` – Environment variable files

Verify the following are in `.gitignore`:
```gitignore
appsettings.*.json
appsettings.Development.json
appsettings.Production.json
*.env
.env
.env.local
.env.*.local
```

### If Secrets Were Already Pushed

If you've already committed `appsettings.json` with real keys:

1. **Rotate all compromised secrets immediately:**
   - Generate a new Paystack secret key in your dashboard
   - Change the database password
   - Invalidate old JWT key and regenerate

2. **Remove from Git history:**
   ```bash
   # Remove from latest commit (if not yet pushed)
   git rm --cached CleanArchitecture.API/appsettings.json
   git commit --amend -m "Remove appsettings.json with secrets"
   
   # If already pushed, use git-filter-repo or BFG Repo-Cleaner
   # (more involved, consult GitHub's removal docs)
   ```

3. **Add template file for developers:**
   Create `appsettings.example.json` with placeholder values to guide setup.

## �🚀 Getting Started

### Prerequisites

- **.NET 8.0** or higher
- **SQL Server** (LocalDB or full instance)
- **Paystack Account** with Secret Key and configured callback URL

### Setup

1. **Clone & navigate to project:**
   ```bash
   cd CleanArchitecture
   ```

2. **Configure database connection** in `CleanArchitecture.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MovieDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. **Configure Paystack credentials** in `appsettings.json`:
   ```json
   "Paystack": {
     "SecretKey": "sk_test_YOUR_SECRET_KEY",
     "CallbackUrl": "https://yourdomain.com/api/v1/payments/webhook"
   }
   ```

4. **Apply database migrations:**
   ```bash
   cd CleanArchitecture.Infra
   dotnet ef database update
   ```

5. **Run the API:**
   ```bash
   cd CleanArchitecture.API
   dotnet run
   ```

   API will be available at `https://localhost:5001`

## 💳 Payment Flow

### 1. **Initialize Payment**
- Client calls `POST /api/v1/payments/initialize` with email and amount
- `InitializePaymentHandler` calls Paystack API to create a transaction
- Payment record created in DB with status `"Pending"`
- Returns authorization URL for client to redirect user to Paystack

### 2. **Verify Payment**
- Client calls `GET /api/v1/payments/verify/{reference}` after user returns from Paystack
- `VerifyPaymentHandler` fetches payment status from Paystack
- If successful, payment status updated to `"Success"` (with optimistic concurrency control)
- User granted premium access
- Returns verification result to client

### 3. **Webhook Handling**
- Paystack sends `charge.success` event to `POST /api/v1/payments/webhook`
- Signature verified using Paystack secret key
- `HandlePaystackWebhookHandler` processes the event atomically
- Same premium grant logic as manual verification
- Idempotency check prevents duplicate processing

## 🔒 Concurrency & Safety Features

### Pessimistic Idempotency
- Both `VerifyPaymentHandler` and webhook handler check payment status before updating
- Prevent duplicate premium grants

### Optimistic Locking
- `Payment` entity includes `byte[] RowVersion` property (SQL Server `rowversion` type)
- EF Core throws `DbUpdateConcurrencyException` if two threads update simultaneously
- `TryUpdateStatusAsync` in repository catches this and returns false, allowing safe retry logic

### Unique Reference Index
- Database enforces unique constraint on `Reference` column
- Prevents duplicate Paystack references from being stored
- `AddAsync` checks and throws `InvalidOperationException` if duplicate detected

### Atomic Updates
- New `TryUpdateStatusAsync(reference, newStatus)` method performs lookup + update in single operation
- Returns boolean indicating success/failure
- Handlers use this instead of separate load/update/save calls

## 🔧 Configuration

### JWT Settings
```json
"JwtSettings": {
  "Key": "a_very_long_and_secure_secret_key_at_least_32_chars",
  "ValidIssuer": "CleanArchitectureApi",
  "ValidAudience": "CleanArchitectureClient",
  "Expires": 60
}
```

### Logging (Serilog)
```json
"Serilog": {
  "MinimumLevel": "Information",
  "WriteTo": [
    { "Name": "Console" },
    { "Name": "File", "path": "logs/app-.log" }
  ]
}
```

Logs are written to:
- **Console** – real-time development feedback
- **File** – `logs/app-*.log` (daily rolling, 30-day retention)

## 🔗 API Endpoints

### Movies
- `GET /api/v1/movies` – List all movies
- `POST /api/v1/movies` – Create movie (admin only)
- `GET /api/v1/movies/{id}` – Get movie details
- `DELETE /api/v1/movies/{id}` – Delete movie (admin only)

### Payments
- `POST /api/v1/payments/initialize` – Start payment transaction
- `GET /api/v1/payments/verify/{reference}` – Verify transaction status
- `POST /api/v1/payments/webhook` – Paystack webhook (no auth required)
- `GET /api/v1/payments/my-transactions` – Get user's transaction history

### Users
- `POST /api/v1/users/register` – Register new user
- `POST /api/v1/users/login` – Authenticate and get JWT
- `POST /api/v1/users/refresh` – Refresh JWT token

## 📊 Database Schema

### Payments Table
| Column | Type | Constraints |
|--------|------|-------------|
| `Id` | UNIQUEIDENTIFIER | Primary Key |
| `UserId` | NVARCHAR | Foreign Key → AspNetUsers |
| `Amount` | DECIMAL(18,2) | Payment amount in Naira |
| `Reference` | NVARCHAR | Paystack reference (Unique Index) |
| `Status` | NVARCHAR | Pending / Success / Failed |
| `TransactionDate` | DATETIME2 | When payment was initiated |
| `RowVersion` | ROWVERSION | Concurrency token for optimistic locking |

## 🧪 Testing the Payment Flow

### Manual Test (Postman/cURL)

1. **Register user:**
   ```bash
   POST /api/v1/users/register
   {
     "email": "test@example.com",
     "password": "SecurePassword123!",
     "firstName": "John",
     "lastName": "Doe",
     "gender": "Male"
   }
   ```

2. **Login:**
   ```bash
   POST /api/v1/users/login
   {
     "email": "test@example.com",
     "password": "SecurePassword123!"
   }
   ```
   Copy the returned JWT token.

3. **Initialize payment:**
   ```bash
   POST /api/v1/payments/initialize
   Authorization: Bearer <JWT_TOKEN>
   {
     "email": "test@example.com",
     "amount": 5000
   }
   ```
   Response includes Paystack authorization URL.

4. **Verify (after user completes Paystack redirect):**
   ```bash
   GET /api/v1/payments/verify/<reference>
   Authorization: Bearer <JWT_TOKEN>
   ```

## 📝 Recent Changes

### Concurrency & Idempotency Hardening (Feb 27, 2026)

- ✅ Added `RowVersion` to `Payment` entity for optimistic locking
- ✅ Database enforces unique constraint on `Reference`
- ✅ New atomic `TryUpdateStatusAsync` method in repository
- ✅ Handlers catch duplicate references gracefully
- ✅ Webhook processing now fully idempotent
- ✅ Latest migration: `AddPaymentConcurrency`

### Payment Callback Configuration (Feb 27, 2026)

- ✅ Removed `CallbackUrl` from request DTO
- ✅ Callback URL now read from `appsettings.json` only
- ✅ Prevents client tampering with callback destination

## 🐛 Troubleshooting

### Migration Issues
If `add-migration` reports data loss:
```bash
dotnet ef migrations remove
# Review Payment entity changes, then:
dotnet ef migrations add AddPaymentConcurrency
dotnet ef database update
```

### Paystack Integration Issues
- Verify `SecretKey` is correct in `appsettings.json`
- Check `CallbackUrl` matches registered webhook in Paystack dashboard
- Review logs in `logs/` directory for detailed error traces

### Database Connection
- Ensure LocalDB is running: `sqllocaldb start mssqllocaldb`
- Or configure full SQL Server connection string

## 📚 Architecture Principles

This project follows SOLID principles and Clean Architecture:

- **S** – Single Responsibility: Each class has one reason to change
- **O** – Open/Closed: Open for extension, closed for modification
- **L** – Liskov Substitution: Interfaces are properly implemented
- **I** – Interface Segregation: Small, focused interfaces
- **D** – Dependency Inversion: Depend on abstractions, not concrete implementations

Using **MediatR** for CQRS pattern keeps business logic separate from HTTP concerns.

## 📄 License

This project is provided as-is for educational and development purposes.

## 💬 Support

For questions or issues, review the log files in `logs/` or check the console output when running locally.
