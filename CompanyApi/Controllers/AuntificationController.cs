using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CompanyApi.DTOs;
using CompanyApi.Models;

[ApiController]
[Route("[controller]")]
public class AuntificationController : ControllerBase
{
   private readonly UserManager<User> _userManager;
   private readonly IConfiguration _configuration;

   public AuntificationController(UserManager<User> userManager, IConfiguration configuration)
   {
       _userManager = userManager;  // Инициализация объекта для управления пользователями
       _configuration = configuration;  // Инициализация объекта для доступа к конфигурации приложения
   }

   [HttpPost("register")]
   public async Task<IActionResult> Register(UserDto userDto)
   {
       var user = new User
       {
           UserName = userDto.UserName,
           Email = userDto.Email,
           Login = userDto.UserName, // Предполагается, что 'Login' - обязательный уникальный идентификатор
           Name = userDto.Name ?? "Default Name",  // Назначение имени пользователя или значения "Default Name" по умолчанию
           PhoneNumber = userDto.PhoneNumber  // Назначение номера телефона пользователя
       };

       var result = await _userManager.CreateAsync(user, userDto.Password);  // Создание нового пользователя через UserManager

       if (result.Succeeded)  // Если операция создания пользователя выполнена успешно
       {
           return Ok();  // Возврат успешного результата
       }

       var errors = result.Errors.Select(e => e.Description);
       return BadRequest(new { Errors = errors });  // Возврат списка ошибок в случае неудачи
   }
   

   [HttpPost("login")]
   public async Task<IActionResult> Login(UserLoginDto userLoginDto)
   {
       var user = await _userManager.FindByNameAsync(userLoginDto.UserName);  // Поиск пользователя по имени пользователя

       if (user != null && await _userManager.CheckPasswordAsync(user, userLoginDto.Password))  // Если пользователь найден и пароль верен
       {
           var authClaims = new List<Claim>  // Создание списка утверждений для токена
           {
               new Claim(ClaimTypes.Name, user.UserName),  // Утверждение с именем пользователя
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  // Утверждение с уникальным идентификатором
           };

           var secretKey = _configuration["JWT:Secret"];
           if (string.IsNullOrEmpty(secretKey))  // Проверка, установлен ли секретный ключ для JWT
           {
               return Unauthorized("JWT secret key is not set in the configuration");  // Возврат ошибки Unauthorized, если секретный ключ JWT не установлен в конфигурации
           }

           var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));  // Создание ключа подписи для токена

           var token = new JwtSecurityToken(  // Создание JWT токена
               issuer: _configuration["JWT:ValidIssuer"],  // Установка издателя
               audience: _configuration["JWT:ValidAudience"],  // Установка аудитории
               expires: DateTime.Now.AddHours(3),  // Установка срока действия токена
               claims: authClaims,  // Утверждения
               signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)  // Установка учетных данных подписи
           );

           return Ok(new  // Возврат успешного результата с JWT токеном и его сроком действия
           {
               token = new JwtSecurityTokenHandler().WriteToken(token),
               expiration = token.ValidTo
           });
       }

       return Unauthorized();  // Возврат ошибки 401 Unauthorized в случае неудачной аутентификации
   }
}
