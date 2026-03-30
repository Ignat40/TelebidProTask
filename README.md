
# For the Project 

This is a client-server type web application for registration and managing user profile written without any frameworks

#### The app has the following functionality: 
 
* Register an user
* Validate the entered data
* Login/logout 
* Edit the user's data
* Generate CAPTCHA to avoid bots 

---

</br>

# How To Run The App



## Locally

Make sure you have active PostgreSQL

Then:

```bash
dotnet build CSApp.slnx
dotnet run --project App/App.csproj
```

Access it through:

```text
http://localhost:8080
```

</br>

## Docker Compose


```bash
docker compose down -v
docker compose up --build
```


Access it through

```text
http://localhost:8080
```
</br>

## Running The Tests

```bash
dotnet test CSApp.slnx
```
</br>


## Generating the code coverage

```bash
dotnet test CSApp.slnx --collect:"XPlat Code Coverage"
```

If `reportgenerator` is installed:

```bash
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report
```

</br>

# Tech Stack

#### C# / .NET 

The developing language of choice 

#### Custom HTTP Server
Insted of using framework, the HTTP communicaiton was created using TcpListener, parsing the HTTP request, routing and generating a HTTP response

#### PostgreSQL 
Using a relational database for storing the user data and active sessions

#### Npqsql 
.NET driver for working with PostgreSQL through ADO.NET

#### Docker & docker-compose 
The whole applicaiton is containerized and could be ran either locally: 
```
dotnet run --project App/App.csproj
```

Or through docker
```
docker compose up --build
```

#### HTML/CSS 
The very simple web interface was written with plain HTML & CSS as a interpolated string 

#### xUnit
Used for covering over 80% of the code base

#### Coverlet / XPlay Code Coverage
Used to calculate the above-mentioned code base coverage 

![Code %](/TelebigProTask/SecondRound/code_coverage.png "Screenshot of the code coverage")

---

</br>

# Architecture 

The app is created in layers for ease for developing, maintaining, testing and understanding

#### 1. Server layer 

Responds and accepts HTTP requests

#### 2. Controller layer 

Processes the routes and connect the HTTP with the logic layer

#### 3. Service Layer 
Handles the core logic
* Registering 
* Login 
* Captcha
* Validating User Input

#### 4. Data access layer

Works directly with PostreSQL through Npsql and SQL queries

#### 5. View layer

Loads the HTML content of the pages 

#### 6. Tests layer

Stores the unit test for the main functions

---
</br>


## Key Components

### HTTP Server

A custom HTTP server is writting using `TcpListener`, which:


* listens for incoming TCP connections
* handles HTTP request line, headers and body
* maintains `GET` & `POST`
* works with cookies
* sends request to Router

### Router

The Router handles which controller/action to process based on the either

* HTTP method
* path

### Controllers

Implemented controllers to:

* register and login/logout
* edit the user profile

### Services

The logic is handled by `services` so it can be easily tested

### Repositories

Repository (Repo) classes encapsulate the work with the database


---

</br>

## Implemented functionalities and method

### User Registration

**Registration page with the following fields:**

* email
* first name
* last name
* password
* CAPTCHA

**Once the form is sent:**

1. Data is read from the HTTP request body
2. Input validation is performed
3. CAPTCHA is checked
4. Checks if email already exists in the database
5. Password is hashed
6. User is saved to PostgreSQL


### Data Validation

Implemented proprietary validation logic for:

#### Email
Checks:

* whether it is not empty
* whether it is in a valid format

#### Username

Checks:

* whether it is not empty
* `min` & `max` length
* use of forbidden characters

#### Password

Check if it contains:

* `min` length 
* Uppercase letter
* Lowercase letters
* Number
* Special Character

**Validation is handles by separate `ValidationService`**

---

</br>

## Saving to the Databse

The app uses PostgreSQL database with two tables:

### `users`

Contains:

* `id`
* `email`
* `first_name`
* `last_name`
* `password_hash`
* `password_salt`
* `created_at`

### `sessions`

Contains:

* `id`
* `user_id`
* `session_token`
* `expires_at`
* `created_at`

Database access is done through:

* `NpgsqlConnection`
* `NpgsqlCommand`
* parameterized SQL queries

This way we dodge SQL injection the access to the data remains explicit

---
</br>

## Login / Logout

### Login

When login:

1. The user provides an email and password
2. A user is searched for by email
3. The provided password is compared with the hash in the database
4. If successful, a session token is created
5. The session token is saved in the `sessions` table
6. A cookie with a session token is set for the client

### Logout

When logout:

1. The cookie with the session token is read
2. The corresponding session is deleted from the database
3. The cookie is cleared from the browser

---
</br>

## Profile page and profile editing

**Implemented:**

* profile view page
* name editing page
* password change option

**When editing:**

* new names are validated
* if a new password is submitted, it is also validated
* if the password is valid, it is hashed again and saved in the database

#### Access to these pages is allowed only for logged in users

---

</br>

## CAPTCHA

The CAPTCHA is implemented entirely in code, without an external service

Implementation:

1. Random code is generated
2. An SVG image with noise/lines is generated from the code
3. The expected value is stored in a cookie
4. When submitting the registration, the entered value is compared with the expected one

---

</br>

## Built-in Methods

I have used standard and built-in functions where appropriate, without relying on a web framework for faster and easier development

### .NET / C#

* `TcpListener`
  To accpets HTTP requests

* `StreamReader` / `NetworkStream`
  To read HTTP request

* `Regex`
  For part of the user data validation

* `RandomNumberGenerator`
  For cryptographically secure generation of salt, session token and CAPTCHA characters
  

* `Rfc2898DeriveBytes.Pbkdf2`
  For secure password hashing

* `CryptographicOperations.FixedTimeEquals`
  For secure password comparison

* `Uri.UnescapeDataString`
  For decoding the `form` data 

* `WebUtility.HtmlEncode`
  For escape the HTML content 

### For The Database

* `NpgsqlConnection`
* `NpgsqlCommand`
* `NpgsqlDataReader`

### For Testing

* `xUnit`
* `coverlet.collector`

### For Containerization

* `Docker`
* `Docker Compose`

---

</br>

## Class Breakdown
### `App/Program.cs`

The main entry point of the application

Loads the configuration, initializes the database, creates the dependency objects and starts the HTTP server

### `App/Config/AppSettings.cs`

Reads the settings from the environment variables:

* DB host
* DB port
* DB name
* DB user
* DB password
* app port

### `App/Server/`

Contains the logic for the HTTP layer:

#### `SimpleHttpServer.cs`

Custom HTTP server with `TcpListener`

#### `HttpRequest.cs`

Incoming HTTP request model

#### `HttpResponse.cs`

HTTP response model

#### `Router.cs`

Distributes requests to the appropriate controller

#### `FormParser.cs`

Parses form-urlencoded data and query string

#### `CookieHelper.cs`

Works with cookies for session token and CAPTCHA

### `App/Controllers/`

Contains the logic for processing routes

#### `AuthController.cs`

Registration, login, logout

#### `ProfileController.cs`

Profile, profile edit, name/password change

### `App/Services/`

Contains business logic

#### `ValidationService.cs`

Email, name and password validation

#### `AuthService.cs`

Registration and login logic

#### `SessionService.cs`

Creation, validation and removal of sessions

#### `CaptchaService.cs`

Captcha generation and validation

### `App/Security/PasswordHasher.cs`

Password hashing and verification using `PBKDF2`

### `App/Data/`

Working with PostgreSQL

#### `DbConnection.cs` / `DbConnectionFactory.cs`

Creates a connection to the database

#### `DbInit.cs`

Creates tables on initial startup

#### `UserRepository.cs`

CRUD operations for users

#### `SessionRepository.cs`

Working with the session table

#### `IUserRepository.cs`, `ISessionRepository.cs`

Interfaces for the repository layer

### `App/Models/`

Data models:

* `User.cs`
* `Session.cs`
* `ServiceResult.cs`

#### `App/Views/HtmlRenderer.cs`

Generates HTML pages for:

* home
* register
* login
* profile
* edit profile

#### `Tests/`

Unit tests

#### `ValidationServiceTests.cs`

Validation tests

#### `PasswordHasherTests.cs`

Password hashing tests

#### `CaptchaServiceTests.cs`

CAPTCHA tests

#### `AuthServiceTests.cs`

Registration and login tests

#### `SessionServiceTests.cs`

Session tests

#### `Fakes/`

Contains fake repository implementations for unit tests

---


## Security & Good Practices

( in my opinion :D )

* passwords are not stored in plain text
* secure hashing with salt is used
* parameterized SQL queries are used
* session cookies are used instead of storing sensitive data on the client
* HTML content is encoded to reduce the risk of injection into pages
* logic is separated so that it is testable

---

