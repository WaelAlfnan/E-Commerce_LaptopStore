# **E-Commerce Website - LapStore**

## **Project Overview**
LapStore is an e-commerce platform designed to offer a wide range of laptops at competitive prices. The platform provides comprehensive product information, an excellent user experience, and outstanding customer support.

---

## **Key Features**
- **User-friendly interface**: Easy navigation and search functionality.
- **Product management**: Add, modify, and delete products.
- **Shopping cart**: Add products and complete purchases.
- **Payment system**: Supports at least one payment method.

---

## **Technologies Used**
- **Frontend**: HTML, CSS, JavaScript.
- **Backend**: C# (.NET Core).
- **Database**: SQL Server.
- **Architecture**: MVC (Model-View-Controller) within an n-tier structure.

---

## **Installation & Setup**
1. **Clone the repository**:
   ```bash
   git clone https://github.com/WaelAlfnan/E-Commerce_LaptopStore/
   ```
2. **Set up the database**:
   - Run the SQL scripts provided in the `Database` folder to create the necessary tables.
3. **Configure the backend**:
   - Update the connection string in `appsettings.json` to point to your SQL Server instance.
4. **Run the application**:
   - Use Visual Studio or the .NET CLI to build and run the project.
   ```bash
   dotnet run
   ```

---

## **Database Design**
### **ERD (Entity-Relationship Diagram)**
The ERD outlines the relationships between different entities in the database, such as users, products, and orders. Below is the ERD for the LapStore database:

![ERD](/Media/ECommerceERD.png)

### **Logical Schema**
The logical schema provides a detailed structure of the database tables, including attributes and relationships. Below is the logical schema for the LapStore database:

![Logical Schema](/Media/LapStoreSchema.png)

---

## **Use Case Diagram**
The use case diagram illustrates the interactions between users (customers and merchants) and the system, including searching for products, managing inventory, and completing purchases. Below is the use case diagram for LapStore:

![Use Case Diagram](/Media/UseCaseDiagramForE_CommerceApplication.png)

---

## **Project Timeline**
The project timeline is outlined in the Gantt chart below, covering the period from **March 9 to May 9**. Key milestones include:
- Completion of design.
- Development of frontend and backend.
- Testing and final delivery.

![Timeline](/Media/project-timeline.png)

---
## **Project Documentation**
For detailed information about the project, including the project proposal, plan, risk assessment, and more, please refer to the Project Documentation file:

- [LapStore Project Documentation](Media/LapStoreDocumentation.pdf)
---
## **Key Performance Indicators (KPIs)**
- **Page load time**: Less than 3 seconds.
- **Conversion rate**: 5%.
- **Customer satisfaction**: 90%.

---

## **Security Measures**
- **Encryption**: Payment data is encrypted using SSL.
- **Input Validation**: All user inputs are validated to prevent security attacks.

---

## **Testing**
- **Unit Testing**: Each component is tested individually.
- **Integration Testing**: Ensures seamless operation between frontend and backend.

---

## **Team Members**
- **[Wael B.Alfnan](https://github.com/WaelAlfnan)**: Team Lead & Backend Developer.
- **[Abdelrahman Hassan](https://github.com/AbdelRahmanHassan01)**: Frontend Developer.
- **[Shehab Eisa](https://github.com/ShehabEisa)**: Database Designer.
- **[Dina Hawas](https://github.com/Dina-Hawas)**: Quality Assurance Engineer.

---

## **Contact Information**
For any questions or support, please contact [here](https://github.com/WaelAlfnan/E-Commerce_LaptopStore/issues/new) through the repository's issue tracker.

---

## **Conclusion**
LapStore aims to provide a reliable, user-friendly, and secure platform for purchasing laptops online. With a focus on performance and customer satisfaction, LapStore is set to become a leading e-commerce platform.
