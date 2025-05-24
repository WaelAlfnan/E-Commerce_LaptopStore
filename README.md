# **E-Commerce Website - LapStore**

## **Project Overview**
LapStore is an e-commerce platform offering a wide range of laptops at competitive prices, providing comprehensive product information, excellent user experience, and outstanding customer support. This graduation project demonstrates a complete e-commerce solution with modern web technologies and secure transaction capabilities.

---

## **Objectives**
- Deliver a seamless user experience
- Increase sales through intuitive design
- Improve customer satisfaction with reliable service
- Provide a comprehensive laptop marketplace

---

## **Key Features**
- **User-friendly interface**: Easy navigation and advanced search functionality
- **Product management system**: Complete inventory management for sellers
- **Shopping cart functionality**: Streamlined checkout process
- **Secure payment integration**: Multiple payment gateway support
- **Order tracking system**: Real-time order status updates
- **User authentication**: Secure registration and login system
- **Admin panel**: Complete platform management tools

---

## **Technologies Used**
- **Frontend**: HTML, CSS, JavaScript
- **Backend**: C# (.NET Core)
- **Database**: SQL Server with Entity Framework
- **Architecture**: N-tier Architecture (Presentation Layer, Business Logic Layer, Data Access Layer)
- **Template**: Bootstrap-based responsive design

---

## **Installation & Setup**
1. **Clone the repository**:
   ```bash
   git clone https://github.com/WaelAlfnan/E-Commerce_LaptopStore/
   ```
2. **Set up the database**:
   - Run the Entity Framework migrations to create the necessary database structure
   - Update the connection string in `appsettings.json` to point to your SQL Server instance
3. **Configure the backend**:
   - Ensure all NuGet packages are restored
   - Configure payment gateway settings (if applicable)
4. **Run the application**:
   - Use Visual Studio or the .NET CLI to build and run the project
   ```bash
   dotnet run
   ```

---

## **System Architecture**
The system follows an **n-tier architecture** pattern:

### **Presentation Layer**
- User interface components
- Web controllers and views
- Client-side JavaScript interactions

### **Business Logic Layer**
- Core business rules and validation
- Service classes and business entities
- Application workflow management

### **Data Access Layer**
- Entity Framework implementation
- Repository pattern for data operations
- Database connection management

---

## **Database Design**
### **Entity-Relationship Diagram (ERD)**
The ERD illustrates the relationships between key entities including users, products, orders, categories, and payment information.

![ERD](/Media/ECommerceERD.png)

### **Logical Schema**
The normalized database schema ensures data integrity and optimal performance with proper indexing and relationships.

![Logical Schema](/Media/LapStoreSchema.png)

---

## **Use Case Diagram**
The system supports three main user types with distinct functionalities:

![Use Case Diagram](/Media/UseCaseDiagramForE_CommerceApplication.png)

### **User Stories**

#### **Buyer User Stories**
- Search for laptops easily and find products quickly
- Securely complete payments with confidence
- Track orders and know delivery status
- Manage personal account and order history

#### **Seller User Stories**
- Manage products and inventory efficiently
- Analyze sales performance and optimize business
- Handle customer inquiries and support

#### **Admin User Stories**
- Manage user accounts and maintain platform integrity
- Resolve disputes and ensure smooth transactions
- Monitor system performance and security

---

## **Project Timeline**
### **Development Phases**
| Phase | Task | Duration |
|-------|------|----------|
| **Phase 1** | Search for Bootstrap Template and Install | 1 Day |
| **Phase 2** | Design ERD and Relationships | 3 Days |
| **Phase 3** | Database Normalization | 1 Day |
| **Phase 4** | Create Migration using Entity Framework | 3 Days |
| **Phase 5** | Frontend Development | 2 Weeks |
| **Phase 6** | Backend Development | 6 Weeks |
| **Phase 7** | Testing & Debugging | 1 Week |

**Project Duration**: March 9 to May 9

![Timeline](/Media/project-timeline.png)

---

## **Requirements**

### **Functional Requirements**
- User authentication and registration system
- Product listing, categorization, and management
- Shopping cart and checkout system
- Payment gateway integration
- Order management and tracking system
- Search and filtering capabilities

### **Non-Functional Requirements**
- **Reliability**: 99% system uptime
- **Performance**: API response time < 500ms
- **Usability**: Intuitive and accessible user interface
- **Flexibility**: Scalable system architecture
- **Security**: Encrypted data transmission and secure payment processing

---

## **Key Performance Indicators (KPIs)**
| KPI | Target | Description |
|-----|--------|-------------|
| **System Uptime** | 99% | Platform availability and reliability |
| **Response Time** | < 500ms | API and page load performance |
| **User Adoption Rate** | Growth tracking | New user registrations |
| **Order Completion Rate** | High percentage | Successful transaction ratio |
| **Customer Satisfaction** | 90%+ | Based on reviews and feedback |

---

## **Security Measures**
- **SSL Encryption**: All payment data encrypted using SSL/TLS
- **Input Validation**: Comprehensive validation to prevent injection attacks
- **Secure Authentication**: Password hashing and session management
- **Regular Security Audits**: Ongoing security assessment and updates
- **Secure Coding Practices**: Following industry security standards

---

## **Risk Assessment & Mitigation**
| Risk | Impact | Mitigation Strategy |
|------|---------|-------------------|
| Development Delays | High | Regular progress tracking, buffer time |
| Security Vulnerabilities | High | Secure coding practices, regular audits |
| Payment Gateway Failures | Medium | Multiple payment providers |
| Poor User Adoption | Medium | Marketing campaigns, user-friendly design |

---

## **Testing Strategy**
### **Unit Testing**
- Individual component testing
- Business logic validation
- Data access layer testing

### **Integration Testing**
- Frontend-backend integration
- Database connectivity testing
- Payment gateway integration testing
- End-to-end workflow testing

---

## **UI/UX Guidelines**
- **Color Scheme**: Neutral colors with blue accents for technology focus
- **Typography**: Clean, easy-to-read fonts for optimal readability
- **Responsive Design**: Mobile-first approach with Bootstrap framework
- **User Experience**: Intuitive navigation and streamlined user flows

---

## **Team Members & Roles**
| Team Member | Role | Responsibility | Contact |
|-------------|------|----------------|---------|
| **[Wael Bahaa Aldien](https://github.com/WaelAlfnan)** | Backend Developer & Team Lead | APIs, Data Access Layer, Business Logic | Waelbahaa01@hotmail.com |
| **[Abdelrahman Hassan](https://github.com/AbdelRahmanHassan01)** | Frontend Developer | UI Components, User Interactions | abdelraalabdelrahman@gmail.com |
| **[Shehab Eissa](https://github.com/ShehabEisa)** | Database Designer | ERD Design, Database Schema | Shehabeissa472@gmail.com |
| **[Dina Gamal Kamal](https://github.com/Dina-Hawas)** | Software Tester | Unit Tests, Quality Assurance | dinahawas2004@gmail.com |

**Supervisor**: Eng. Mahmoud Shahaly

---

## **Project Documentation**
For comprehensive project details including stakeholder analysis, detailed requirements, and technical specifications, refer to:

- [LapStore Project Documentation](Media/LapStoreDocumentation.pdf)

---

## **Repository Structure**
```
E-Commerce_LaptopStore/
├── Frontend/          # HTML, CSS, JavaScript files
├── Backend/           # .NET Core application
├── Database/          # SQL scripts and migrations
├── Media/             # Project images and diagrams
├── Tests/             # Unit and integration tests
└── Documentation/     # Project documentation files
```

---

## **Getting Started**
1. Review the project documentation for complete understanding
2. Set up your development environment with .NET Core and SQL Server
3. Clone the repository and follow installation steps
4. Run initial database migrations
5. Start the application and begin development

---

## **Support & Contact**
For questions, issues, or contributions:
- **Issues**: [GitHub Issue Tracker](https://github.com/WaelAlfnan/E-Commerce_LaptopStore/issues/new)
- **Repository**: [LapStore GitHub Repository](https://github.com/WaelAlfnan/E-Commerce_LaptopStore)

---

## **Conclusion**
LapStore represents a comprehensive e-commerce solution designed with modern web technologies, secure practices, and user-centric design. The project demonstrates proficiency in full-stack development, database design, and software engineering principles, making it a robust platform for laptop retail operations.
