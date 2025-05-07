
```markdown
# Fliq Social Platform - Web API

## 🌟 Project Overview
Fliq is a revolutionary social connection platform targeting global citizens seeking meaningful relationships, friendships, and event-based interactions. Our ASP.NET Core Web API (v8) powers secure social interactions with AI-enhanced features.

**Key Objectives:**
- Combat social isolation through intelligent matching
- Provide safe, verified social interactions
- Enable event-based connections through shared interests
- Offer AI-powered relationship support tools

[Figma Design Mockups](#design-mockups)
![image](https://github.com/user-attachments/assets/b537dc87-035d-423b-bb5e-4207a98d556c)
<img src="https://github.com/user-attachments/assets/b537dc87-035d-423b-bb5e-4207a98d556c" alt="Fliq Logo" width="200"/>

## 🚀 Features

### Core Functionality
- **Secure Authentication System**
  - JWT-based authentication
  - Facial verification integration
  - Multi-factor authentication
- **AI-Powered Services**
  - Profile Bio Generator
  - Psychological Support Chatbot
  - Smart Conversation Assistant
- **Event Management**
  - Creation/Hosting tools
  - RSVP system with QR codes
  - Paid/Free event ticketing
- **Social Features**
  - Multi-mode dating (Speed, Blind, Group)
  - Interest-based matching algorithm
  - Secure in-app messaging

### Technical Highlights
- RESTful API Design
- Clean Architecture implementation
- TDD with xUnit/NUnit
- CI/CD Pipeline Integration
- Real-time notifications (SignalR)
- EF Core with MS-SQL

## 🛠 Getting Started

### Prerequisites
- .NET 8 SDK
- Docker (for containerized DB)
- MSSQL 2018+
- IDE (VS 2022+/Rider/VSCode)

### Installation
```bash
# Clone repository
git clone -b development https://github.com/chimexy8alluvial/Fliq-WebAPI.git

# Restore dependencies
dotnet restore

# Run migrations
dotnet ef database update

# Start application
dotnet run --project Fliq.Api
```

## 📚 API Documentation
Explore our API endpoints through:
- Swagger UI: `https://fliqapidev.azurewebsites.net/swagger/index.html`

## 🧪 Testing (TDD Mandatory)
```bash
# Run unit tests
dotnet test Fliq.Tests


## 🤝 Contributing
**Branch Strategy:**
```
development (protected) ← feature/* ← contributors
```

### Contribution Process
1. Create issue in Trello board https://trello.com/b/FOIeHctl/backend-dev-board
2. Branch from `development`:
   ```bash
   git checkout -b feature/your-feature development
   ```
3. Implement with TDD approach
4. Submit PR to `development` with:
   - Passing tests
   - Updated documentation
   - API versioning compliance
5. Await 1+ approvals from core team

**Prohibited:**
- Direct commits to `development`
- Untested code submissions
- Breaking changes without migration plan

## 📐 Design Mockups
View UI/UX specifications:  
[Figma Design System](https://www.figma.com/design/u3SHuD2nGCZ5LxrW1L6m8q/Fliq-Final?node-id=4206-49193&t=Q4fFCxsyRAqdPqVC-0)  
*(Contact project admins for access)*

## 📜 License
MIT License - See [LICENSE](LICENSE) for details

## 📞 Contact
**Technical Lead:** Chimezirim Bassey 
**Email:** fliqtech@getfliq.com 
---

**Let's Build Meaningful Connections Together!**  
*"Alone we can do so little; together we can do so much" - Helen Keller*
