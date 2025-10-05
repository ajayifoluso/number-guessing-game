Here’s a **summarized, high-level version** of your entire project guide — perfect for a **GitHub README.md**:

---

# 🚀 Team 2: Secure Web Application with Azure Web App & Key Vault

## Overview

This project demonstrates how to build and deploy a secure ASP.NET Core web application on **Azure App Service** integrated with **Azure Key Vault** for secret management. It follows best practices for **identity-based authentication**, **resource governance**, and **application monitoring**.

---

## 🧱 Architecture Summary

* **Azure Resources:** Resource Group, App Service, Key Vault, Application Insights
* **Security:** Managed Identity, Role-Based Access Control (RBAC), HTTPS-only, Key Vault secret integration
* **Monitoring:** Application Insights for performance, logs, and telemetry
* **Automation:** Azure CLI, Git integration, and Visual Studio Code for deployment

---

## ⚙️ Deployment Steps (High Level)

### 1️⃣ Environment Setup

Install **Azure CLI**, **VS Code (with Azure extensions)**, and **Git**.
Verify access to your Azure subscription.

### 2️⃣ Naming Convention

Use consistent naming for all resources:
`[project]-[environment]-[resource-type]-[location]`
Example: `team2-dev-webapp-eastus`

### 3️⃣ Resource Group & Service Principal

Create a **Resource Group** (`team2-dev-rg-eastus`) and a **Service Principal** for secure, automated access.

### 4️⃣ Azure Key Vault

Create a **Key Vault** to store sensitive credentials (e.g., database connection strings, API keys).
Add and manage secrets securely—no hardcoded values.

### 5️⃣ ASP.NET Core App Setup

Build a simple **ASP.NET Core** app that retrieves secrets at runtime from Key Vault using:

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["KeyVault:Url"]),
    new DefaultAzureCredential());
```

### 6️⃣ Web App Creation

Deploy the app on **Azure App Service (Windows)** with a **system-assigned managed identity**.
Assign the **Key Vault Secrets User** role to allow secret access.

### 7️⃣ Configuration

Set environment variables in App Service:

```
KeyVault__Url = https://team2-dev-kv-eastus.vault.azure.net/
ASPNETCORE_ENVIRONMENT = Development
```

### 8️⃣ Deployment

Use **VS Code → Deploy to Azure App Service** or `az webapp deployment` to publish.
Confirm app loads successfully at:
👉 `https://team2-dev-webapp-eastus.azurewebsites.net`

### 9️⃣ Security & Monitoring

* Enforce **HTTPS-only** access.
* Integrate **Application Insights** for logging and telemetry.
* Enable **Key Vault diagnostic logs** for auditing access.

### 🔒 Security Validation

✅ No secrets in source code
✅ Managed Identity authentication enabled
✅ HTTPS enforced
✅ Access logging & RBAC configured

### 🔍 Testing

Verify:

* Web app loads correctly
* Secrets load from Key Vault
* Monitoring and logs appear in Azure Portal

### 📄 Documentation & Cleanup

Document:

* Architecture diagram
* Deployment process
* Lessons learned and challenges

Clean up resources after testing to avoid costs:

```bash
az group delete --name team2-dev-rg-eastus --yes --no-wait
```

---

