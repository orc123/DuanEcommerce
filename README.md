# 🛒 DuanEcommerce - Hệ thống Thương mại Điện tử (ABP Framework & Angular)

**DuanEcommerce** là giải pháp phần mềm Thương mại Điện tử đa phân hệ (Enterprise E-Commerce Solution) được thiết kế và xây dựng trên nền tảng **[ABP Framework](https://abp.io)**, tuân thủ chặt chẽ các nguyên lý **Domain-Driven Design (DDD)** và kiến trúc phân lớp chuẩn hóa. Hệ thống tách biệt rõ ràng giữa các phân hệ Quản trị (Admin), Phân hệ Công khai (Public Storefront), và giao diện điều khiển Angular SPA hiện đại.

---

## 🛠️ Công nghệ Sử dụng (Technology Stack)

### 🔮 Backend Framework & Core (.NET 10)
- **Nền tảng**: .NET 10.0 (C#)
- **Core Framework**: [ABP Framework v10.5](https://abp.io) (Hỗ trợ DDD, Multi-Tenancy, Dependency Injection, Event Bus, Audit Logging)
- **ORM & Database**: Entity Framework Core 10.0 & Microsoft SQL Server
- **Xác thực & Phân quyền**: OpenIddict (OAuth 2.0 / OpenID Connect), ABP Identity & Tenant Management
- **API Documentation**: Swagger / Swashbuckle UI
- **Logging & Monitoring**: Serilog, ASP.NET Core Health Checks UI
- **Testing**: xUnit, EF Core InMemory Testing

### 🎨 Frontend Framework & UI (Angular 21)
- **Framework**: Angular v21.2 (Single Page Application)
- **ABP Angular Packages**: `@abp/ng.core`, `@abp/ng.theme.lepton-x` (v5.5), `@abp/ng.identity`, `@abp/ng.account`
- **UI Components**: [PrimeNG v19](https://primeng.org/), PrimeFlex v4, PrimeIcons v8
- **Rich Text Editor**: Quill Editor (`ngx-quill` v30)
- **Styling & Layout**: PrimeFlex CSS & Custom SCSS

### 🐳 Containerization & Deployment
- **Docker**: Các dịch vụ đã có sẵn `Dockerfile` để đóng gói container độc lập.
- **Nginx**: Phục vụ ứng dụng Frontend Angular ở môi trường production.

---

## 📁 Cấu trúc Dự án (Project Structure)

Dự án được cấu trúc theo mô hình phân lớp DDD kết hợp với giải pháp đa phân hệ (Modular / Multi-host):

```text
DuanEcommerce/
├── angular/                                # 🎨 Ứng dụng Frontend Angular SPA (Admin Portal)
│   ├── src/                                # Mã nguồn Angular (app, assets, environments)
│   ├── Dockerfile                          # 🐳 Containerize Angular SPA bằng Nginx Alpine
│   ├── angular.json                        # Cấu hình Angular CLI
│   └── package.json                        # Khai báo phụ thuộc NPM (Angular, PrimeNG, ABP)
├── src/                                    # ⚙️ Mã nguồn Backend .NET
│   ├── admin/                              # 🛡️ Phân hệ Quản trị (Admin Subsystem)
│   │   ├── DuanEcommerce.Admin.Application          # Application Services dành cho Admin
│   │   ├── DuanEcommerce.Admin.Application.Contracts# DTOs, Interfaces & Permissions
│   │   ├── DuanEcommerce.Admin.HttpApi              # REST API Controllers cho Admin
│   │   ├── DuanEcommerce.Admin.HttpApi.Client       # Client Proxies cho Admin APIs
│   │   └── DuanEcommerce.Admin.HttpApi.Host         # Web Host Service (có Dockerfile & Dockerfile.local)
│   ├── public/                             # 🌐 Phân hệ Cửa hàng Công khai (Public Storefront)
│   │   ├── DuanEcommerce.Public.Application         # Application Services cho Khách hàng
│   │   ├── DuanEcommerce.Public.Application.Contracts# DTOs & Interfaces cho Public API
│   │   ├── DuanEcommerce.Public.HttpApi             # REST API Controllers cho Public Storefront
│   │   ├── DuanEcommerce.Public.HttpApi.Client      # Client Proxies cho Public APIs
│   │   └── DuanEcommerce.Public.Web                 # Public Web Application (có Dockerfile & Dockerfile.local)
│   └── common/                             # 📦 Các thành phần dùng chung (Shared Infrastructure & Domain)
│       ├── domain/                         # Tầng Business Logic trung tâm
│       │   ├── DuanEcommerce.Domain                 # Entities, Aggregates, Domain Services
│       │   └── DuanEcommerce.Domain.Shared          # Enums, Constants, Multi-language Localization
│       └── infrastructure/                 # Tầng Hạ tầng & Lưu trữ Dữ liệu
│           ├── DuanEcommerce.EntityFrameworkCore    # DbContext, Mappings, Migrations & Repositories
│           └── DuanEcommerce.DbMigrator             # Console App khởi tạo DB (có Dockerfile & Dockerfile.local)
├── test/                                   # 🧪 Các dự án Unit Test & Integration Test
│   ├── DuanEcommerce.Application.Tests          # Tests cho Application Layer
│   ├── DuanEcommerce.Domain.Tests               # Tests cho Domain Layer
│   └── DuanEcommerce.EntityFrameworkCore.Tests   # Tests cho EF Core Repositories & Database
├── DuanEcommerce.abpsln                    # Cấu hình ABP Studio Solution
└── README.md                               # Tài liệu hướng dẫn dự án
```

---

## 🚀 Hướng dẫn Khởi chạy (Getting Started)

### 📋 Yêu cầu tiên quyết (Prerequisites)
- **[.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet)** trở lên.
- **[Node.js](https://nodejs.org/)** (v18 hoặc v20) & **npm / yarn**.
- **Microsoft SQL Server** (LocalDB hoặc Server Instance).
- **[ABP CLI](https://abp.io/docs/latest/cli)** (Tùy chọn, phục vụ quản lý module/code generator).

---

### 🔨 Cách 1: Khởi chạy Trực tiếp trên Máy Cục bộ (Local Run)

#### Step 1: Clone và Cài đặt Client-Side Libraries
Chạy lệnh sau tại thư mục gốc của giải pháp để cài đặt các thư viện client-side phụ thuộc:
```bash
abp install-libs
```

#### Step 2: Cấu hình Chuỗi Kết nối Database (Connection Strings)
Mở file `appsettings.json` trong các dự án sau và thay đổi chuỗi kết nối `Default` phù hợp với SQL Server của bạn:
- `src/admin/DuanEcommerce.Admin.HttpApi.Host/appsettings.json`
- `src/common/infrastructure/DuanEcommerce.DbMigrator/appsettings.json`

#### Step 3: Khởi tạo Database & Seed Data
Chạy ứng dụng console **DbMigrator** để tạo Database, áp dụng EF Core Migrations và nạp dữ liệu mẫu ban đầu:
```bash
dotnet run --project src/common/infrastructure/DuanEcommerce.DbMigrator/DuanEcommerce.DbMigrator.csproj
```

#### Step 4: Tạo Certificate Xác thực OpenIddict (Production / Dev)
Để ứng dụng Identity Server (OpenIddict) hoạt động chính xác với token signing:
```bash
dotnet dev-certs https -v -ep openiddict.pfx -p 6deab80b-354e-4296-bfe9-483ac1770c3f
```

#### Step 5: Khởi chạy Backend Admin API Server
Khởi chạy API Host cho phân hệ Admin:
```bash
dotnet run --project src/admin/DuanEcommerce.Admin.HttpApi.Host/DuanEcommerce.Admin.HttpApi.Host.csproj
```
> Trình duyệt sẽ mở giao diện Swagger API Docs tại địa chỉ `https://localhost:44300` (hoặc port được cấu hình).

#### Step 6: Khởi chạy Frontend Angular
Mở một cửa sổ Terminal mới, di chuyển vào thư mục `angular` và khởi chạy giao diện người dùng:
```bash
cd angular
npm install
npm start
```
> Ứng dụng Angular sẽ chạy tại địa chỉ `http://localhost:4200`.

---

### 🐳 Cách 2: Khởi chạy bằng Docker (Docker Deployment)

Tất cả các dịch vụ chính trong hệ thống đều đã được tích hợp sẵn file `Dockerfile` và `Dockerfile.local`.

#### 1. Build & Run Frontend Angular với Docker:
```bash
cd angular
docker build -t duanecommerce-angular .
docker run -d -p 4200:80 --name duanecommerce-frontend duanecommerce-angular
```

#### 2. Build & Run Backend Admin Host API:
Trước khi build Docker image cho backend, hãy publish project sang bản phát hành:
```bash
dotnet publish src/admin/DuanEcommerce.Admin.HttpApi.Host/DuanEcommerce.Admin.HttpApi.Host.csproj -c Release
cd src/admin/DuanEcommerce.Admin.HttpApi.Host
docker build -t duanecommerce-admin-api .
docker run -d -p 8080:80 --name duanecommerce-backend duanecommerce-admin-api
```

#### 3. Build & Run Public Web:
```bash
dotnet publish src/public/DuanEcommerce.Public.Web/DuanEcommerce.Public.Web.csproj -c Release
cd src/public/DuanEcommerce.Public.Web
docker build -t duanecommerce-public-web .
docker run -d -p 8081:80 --name duanecommerce-public duanecommerce-public-web
```

#### 4. Run Database Migrator trong Container:
```bash
dotnet publish src/common/infrastructure/DuanEcommerce.DbMigrator/DuanEcommerce.DbMigrator.csproj -c Release
cd src/common/infrastructure/DuanEcommerce.DbMigrator
docker build -t duanecommerce-db-migrator .
docker run --rm duanecommerce-db-migrator
```

---

## 🧪 Kiểm thử (Testing)

Để thực hiện chạy toàn bộ Unit Tests và Integration Tests trong dự án:
```bash
dotnet test
```

Đối với ứng dụng Angular:
```bash
cd angular
npm run test
```

---

## 📚 Tài liệu tham khảo thêm

- [ABP Framework Documentation](https://abp.io/docs/latest/framework/architecture/domain-driven-design)
- [Angular Setup Guide trong dự án](./angular/README.md)
- [OpenIddict Certificate Configuration](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html)
