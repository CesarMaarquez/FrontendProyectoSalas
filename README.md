# Aplicación de Reuniones Virtuales para Terapia

##  Descripción General
Este proyecto consiste en el desarrollo de una aplicación de reuniones virtuales enfocada en servicios de terapia o asesoría. La aplicación permitirá la interacción entre terapeutas y clientes mediante sesiones de videollamada, gestión de citas y mensajería en tiempo real.

##  Objetivo
Desarrollar una aplicación multiplataforma utilizando **.NET MAUI** para el frontend y **C#** para el backend. La aplicación se desplegará y gestionará en **Azure**, aprovechando sus servicios para autenticación, almacenamiento y monitoreo.

## Tecnologías Utilizadas
###  Frontend
- .NET MAUI (Android, iOS, Windows, macOS)


###  Backend
- C#
- Entity Framework Core
- API RESTful para la gestión de usuarios, sesiones y videollamadas
- Autenticación con **Azure AD B2C**
- Integración de videoconferencias con **Azure Communication Services** o alternativa

###  Base de Datos
- **Azure SQL Database** u otra opción relacional(a concretar)
- Modelo de datos relacional con usuarios, sesiones y roles
- Cifrado de datos sensibles y cumplimiento con normativas de privacidad

###  Infraestructura en Azure
- **Azure App Service** para desplegar la API
- **Azure Storage** (opcional, hay que valorarlo) para almacenamiento de archivos
- **Azure Notification Hubs** para notificaciones push (opcional)
- **Azure Monitor / Application Insights** para monitoreo y logs

##  Funcionalidades Principales
- **Registro y autenticación de usuarios** (clientes y terapeutas)
- **Gestión de perfiles** con información básica y profesional
- **Agendamiento y gestión de sesiones** (creación, modificación, recordatorios)
- **Videollamadas en tiempo real** con controles de audio y video
- **Mensajería en tiempo real** (chat entre usuarios, opcional)
- **Panel de administración** (opcional, para gestión avanzada)

##  Seguridad y Cumplimiento
- Autenticación segura con **JWT** o **Azure AD B2C**
- Logs y auditoría con **Azure Monitor**

##  Herramientas de Desarrollo
- **IDE:** Visual Studio / Visual Studio Code
- **Control de Versiones:** Git (GitHub)
- **CI/CD:** Azure DevOps / GitHub Actions
- **Pruebas:** Unitarias, integración y UI Testing

##  Despliegue
- **Despliegue en Azure App Service** mediante CI/CD


