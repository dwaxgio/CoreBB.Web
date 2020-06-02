﻿// 21. Se agrega UserController, para contener la logica de negocio de gestion de los usuarios (registro, login y logout)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreBB.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreBB.Web.Controllers
{
    [Authorize] // Indica que el usuario debe estar registrado, para poder acceder a este controlador
    public class UserController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private CoreBBContext _dbContext; // Nombre de la instancia, utilizada para interactuar con la DB CoreBB
        
        // Controlador
        public UserController(CoreBBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 22. Se agrega el siguiente código
        [AllowAnonymous, HttpGet] // El atributo AllowAnonymous sobreescribe el atributo Authorize
        public async Task<IActionResult> Register()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        // 24. Se agrega el metodo POST
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Invalid registration information.");
            }

            model.Name = model.Name.Trim();
            model.Password = model.Password.Trim();
            model.RepeatPassword = model.RepeatPassword.Trim();

            var targetUser = _dbContext.User
                .SingleOrDefault(u => u.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));

            if (targetUser != null)
            {
                throw new Exception("User name already exists.");
            }

            if (!model.Password.Equals(model.RepeatPassword))
            {
                throw new Exception("Passwords are not identical.");
            }

            var hasher = new PasswordHasher<User>(); // El hasher cifra la contraseña y luego la almacena en la DB, nunca se guarda la contraseña como texto plano en la DB
            targetUser = new User { Name = model.Name, RegisterDateTime = DateTime.Now, Description = model.Description };
            targetUser.PasswordHash = hasher.HashPassword(targetUser, model.Password);

            if (_dbContext.User.Count() == 0)
            {
                targetUser.IsAdministrator = true;
            }

            await _dbContext.User.AddAsync(targetUser);
            await _dbContext.SaveChangesAsync();

            await LogInUserAsync(targetUser);

            return RedirectToAction("Index", "Home");
        }

        private async Task LogInUserAsync(User user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            if (user.IsAdministrator)
            {
                claims.Add(new Claim(ClaimTypes.Role, Roles.Administrator));
            }

            var claimsIndentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIndentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            user.LastLogInDateTime = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }

        // 25. Se agrega el metodo get, para el login
        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> LogIn()
        {
            await
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        // 28. Se agrega el metodo post para el log in
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel model)
        {
            if(!ModelState.IsValid)
            {
                throw new Exception("Información de usuario invalida.");
            }

            var targetUser = _dbContext.User.SingleOrDefault(u => u.Name.Equals(model.Name,
                StringComparison.CurrentCultureIgnoreCase));
            if(targetUser == null)
            {
                throw new Exception("El usuario no existe.");
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(targetUser, targetUser.PasswordHash, model.Password);
            if (result != PasswordVerificationResult.Success)
            {
                throw new Exception("La contraseña es erronea.");
            }

            await LogInUserAsync(targetUser);
            return RedirectToAction("Index", "Home");
        }

        // 29. Se agrega la accion para LogOut
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // 30. Se agrega el siguiente método para mostrar el detalle del usuario
        [HttpGet]
        public IActionResult Detail(string name)
        {
            var user = _dbContext.User.SingleOrDefault(u => u.Name == name);
            if (user == null)
            {
                throw new Exception($"El usuario '{name}' no existe");
            }
            return View(user);
        }

        // 32. Se agrega metodo para encontrar usuario por nombre
        [HttpGet]
        public IActionResult Edit(string name)
        {
            if (User.Identity.Name != name && !User.IsInRole(Roles.Administrator))
            {
                throw new Exception("Operation is denied.");
            }

            var user = _dbContext.User.SingleOrDefault(u => u.Name == name);
            if (user == null)
            {
                throw new Exception($"User '{name}' does not exist.");
            }

            var model = UserEditViewModel.FromUser(user);
            return View(model);
        }

        // 34. Se agrega el siguiente método para verificar la informacion actualizada y salvarla en la base de datos
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Invalid user information.");
            }

            var user = _dbContext.User
                .SingleOrDefault(u => u.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));

            if (user == null)
            {
                throw new Exception("User does not exist.");
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                model.Password = model.Password.Trim();
                model.RepeatPassword = model.RepeatPassword.Trim();
                if (!model.Password.Equals(model.RepeatPassword))
                {
                    throw new Exception("Passwords are not identical.");
                }

                var hasher = new PasswordHasher<User>();
                if (!User.IsInRole(Roles.Administrator))
                {
                    var vr = hasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);
                    if (vr != PasswordVerificationResult.Success)
                    {
                        throw new Exception("Please provide correct current password.");
                    }
                }

                user.PasswordHash = hasher.HashPassword(user, model.Password);
            }

            user.Description = model.Description;

            if (User.IsInRole(Roles.Administrator))
            {
                user.IsAdministrator = model.IsAdministrator;
                user.IsLocked = model.IsLocked;
            }

            _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Detail", new { name = user.Name });
        }
    }
}