using System.Net;
using App.Models;

namespace App.Views;

public class HtmlRenderer
{
    public string Layout(string title, string content, User? currentUser = null)
    {
        var authLinks = currentUser is null
            ? """
              <a href="/register">Register</a>
              <a href="/login">Login</a>
              """
            : """
              <a href="/profile">Profile</a>
              <form method="post" action="/logout" style="display:inline;">
                  <button type="submit">Logout</button>
              </form>
              """;

        return $$$"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1" />
            <title>{{{Encode(title)}}}</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    margin: 0;
                    padding: 0;
                    background: #f4f4f4;
                }

                nav {
                    background: #222;
                    color: white;
                    padding: 1rem;
                    display: flex;
                    gap: 1rem;
                    align-items: center;
                }

                nav a, nav button {
                    color: white;
                    text-decoration: none;
                    background: none;
                    border: none;
                    cursor: pointer;
                    font: inherit;
                }

                .container {
                    max-width: 600px;
                    margin: 40px auto;
                    background: white;
                    padding: 24px;
                    border-radius: 10px;
                    box-shadow: 0 2px 10px rgba(0,0,0,0.08);
                }

                input {
                    width: 100%;
                    padding: 10px;
                    margin-top: 6px;
                    margin-bottom: 16px;
                    box-sizing: border-box;
                }

                button {
                    padding: 10px 14px;
                    cursor: pointer;
                }

                .error {
                    background: #ffe0e0;
                    color: #8a0000;
                    padding: 10px;
                    border-radius: 6px;
                    margin-bottom: 16px;
                }

                .success {
                    background: #e4ffe7;
                    color: #145c1f;
                    padding: 10px;
                    border-radius: 6px;
                    margin-bottom: 16px;
                }

                .captcha {
                    margin: 12px 0;
                    padding: 10px;
                    background: #fafafa;
                    border: 1px solid #ddd;
                }

                label {
                    font-weight: bold;
                }
            </style>
        </head>
        <body>
            <nav>
                <a href="/">Home</a>
                {{{authLinks}}}
            </nav>
            <div class="container">
                {{{content}}}
            </div>
        </body>
        </html>
        """;
    }

    public string HomePage(User? currentUser)
    {
        var content = currentUser is null
            ? """
              <h1>Registration App</h1>
              <p>Simple client-server registration system without a web framework.</p>
              <p><a href="/register">Create account</a> or <a href="/login">log in</a>.</p>
              """
            : $"""
              <h1>Welcome, {Encode(currentUser.FirstName)}!</h1>
              <p>You are logged in.</p>
              <p><a href="/profile">Go to your profile</a></p>
              """;

        return Layout("Home", content, currentUser);
    }

    public string RegisterPage(string captchaSvg, IEnumerable<string>? errors = null)
    {
        var errorHtml = RenderErrors(errors);

        var content = $"""
        <h1>Register</h1>
        {errorHtml}
        <form method="post" action="/register">
            <label>Email</label>
            <input name="email" type="email" required />

            <label>First name</label>
            <input name="firstName" type="text" required />

            <label>Last name</label>
            <input name="lastName" type="text" required />

            <label>Password</label>
            <input name="password" type="password" required />

            <label>CAPTCHA</label>
            <div class="captcha">{captchaSvg}</div>
            <input name="captcha" type="text" required />

            <button type="submit">Register</button>
        </form>
        """;

        return Layout("Register", content);
    }

    public string LoginPage(IEnumerable<string>? errors = null)
    {
        var errorHtml = RenderErrors(errors);

        var content = $"""
        <h1>Login</h1>
        {errorHtml}
        <form method="post" action="/login">
            <label>Email</label>
            <input name="email" type="email" required />

            <label>Password</label>
            <input name="password" type="password" required />

            <button type="submit">Login</button>
        </form>
        """;

        return Layout("Login", content);
    }

    public string ProfilePage(User user, string? successMessage = null)
    {
        var successHtml = string.IsNullOrWhiteSpace(successMessage)
            ? string.Empty
            : $"""<div class="success">{Encode(successMessage)}</div>""";

        var content = $"""
        <h1>Profile</h1>
        {successHtml}
        <p><strong>Email:</strong> {Encode(user.Email)}</p>
        <p><strong>First name:</strong> {Encode(user.FirstName)}</p>
        <p><strong>Last name:</strong> {Encode(user.LastName)}</p>
        <p><a href="/profile/edit">Edit profile</a></p>
        """;

        return Layout("Profile", content, user);
    }

    public string EditProfilePage(User user, IEnumerable<string>? errors = null)
    {
        var errorHtml = RenderErrors(errors);

        var content = $"""
        <h1>Edit Profile</h1>
        {errorHtml}
        <form method="post" action="/profile/edit">
            <label>First name</label>
            <input name="firstName" type="text" value="{Encode(user.FirstName)}" required />

            <label>Last name</label>
            <input name="lastName" type="text" value="{Encode(user.LastName)}" required />

            <label>New password</label>
            <input name="newPassword" type="password" />

            <p style="font-size: 0.9rem; color: #555;">Leave password empty if you do not want to change it.</p>

            <button type="submit">Save changes</button>
        </form>
        """;

        return Layout("Edit Profile", content, user);
    }

    public string NotFoundPage()
    {
        return Layout("Not Found", "<h1>404</h1><p>Page not found.</p>");
    }

    private static string RenderErrors(IEnumerable<string>? errors)
    {
        if (errors is null || !errors.Any())
        {
            return string.Empty;
        }

        var items = string.Join("", errors.Select(e => $"<li>{Encode(e)}</li>"));
        return $"""<div class="error"><ul>{items}</ul></div>""";
    }

    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}