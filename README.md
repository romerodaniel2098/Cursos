# Plataforma de Cursos Online (Assessment)

Este proyecto implementa una API REST con .NET 9 y Clean Architecture, consumida por una aplicación frontend en React + Vite.

## Requisitos Previos

- [.NET SDK 9.0](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18 o superior)
- [MySQL](https://www.mysql.com/)
- Docker (Opcional, si deseas contenerizar la BD)

## Configuración de Base de Datos

1.  Asegúrate de tener un servidor MySQL corriendo (puedes usar XAMPP, Docker, etc.).
2.  Verifica la cadena de conexión en `OnlineCourses.Api/appsettings.json`.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "server=localhost;port=3306;database=onlinecourses_db;user=root;password=tu_password;TreatTinyAsBoolean=true"
    }
    ```
3.  La aplicación intentará crear la base de datos y sembrar un usuario de prueba automáticamente al arrancar.

## Ejecución

### Backend (API)

```bash
cd OnlineCourses.Api
dotnet run
```
La API estará disponible en `http://localhost:5259`.
- Swagger UI: `http://localhost:5259/swagger`
- Usuario de prueba: `test@demo.com` / `Test123$`

### Frontend (App)

```bash
cd frontend
npm install
npm run dev
```
La aplicación estará disponible en `http://localhost:5173`.

## Documentación de la API

### Autenticación
Los endpoints de autenticación generan un JWT Bearer Token que debe enviarse en el header `Authorization: Bearer <token>` para las peticiones protegidas.

- `POST /api/auth/register`: Registro de nuevos usuarios.
    - Body: `{ "email": "user@example.com", "password": "Password123", "fullName": "John Doe" }`
- `POST /api/auth/login`: Inicio de sesión.
    - Body: `{ "email": "user@example.com", "password": "Password123" }`

### Cursos
Requieren autenticación.

- `GET /api/courses/search?q=&status=`: Búsqueda paginada de cursos.
- `GET /api/courses/{id}`: Obtener detalles de un curso.
- `GET /api/courses/{id}/summary`: Resumen del curso (info básica + total lecciones).
- `POST /api/courses`: Crear un nuevo curso (estado Draft).
    - Body: `{ "title": "Nuevo Curso" }`
- `PUT /api/courses/{id}`: Actualizar título de un curso.
- `DELETE /api/courses/{id}`: Eliminación lógica (soft delete).
- `PATCH /api/courses/{id}/publish`: Publicar un curso (Debe tener lecciones activas).
- `PATCH /api/courses/{id}/unpublish`: Despublicar un curso.

### Lecciones
Requieren autenticación.

- `GET /api/lessons/course/{courseId}`: Listar todas las lecciones de un curso, ordenadas.
- `POST /api/lessons`: Crear una lección.
    - Body: `{ "courseId": "guid...", "title": "Lección 1", "order": 1 }`
- `PUT /api/lessons/{id}`: Actualizar lección (título u orden).
- `DELETE /api/lessons/{id}`: Eliminación lógica (soft delete).

## Tests

Para ejecutar las pruebas unitarias que validan las reglas de negocio:
```bash
dotnet test OnlineCourses.Tests
```
