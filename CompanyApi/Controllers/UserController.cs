using CompanyApi.DTOs;
using CompanyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]  // Установка маршрута к контроллеру

public class UserController : ControllerBase  // Объявление класса контроллера UserController, унаследованного от ControllerBase
{
   private readonly UserManager<User> _userManager;  // Приватное поле UserManager для управления пользователями

   public UserController(UserManager<User> userManager)  // Конструктор класса UserController, внедрение зависимости UserManager
   {
       _userManager = userManager;  // Присвоение переданного экземпляра UserManager полю _userManager
   }

   [HttpPut("edit/{id}")]  // Обработчик HTTP PUT запроса для изменения данных пользователя по идентификатору
   public async Task<IActionResult> EditUser(string id, UserEditDto userEditDto)  // Метод редактирования данных пользователя
   {
       var user = await _userManager.FindByIdAsync(id);  // Поиск пользователя по идентификатору
       if (user == null) return NotFound();  // Возврат ошибки 404, если пользователь не найден

       user.PhoneNumber = userEditDto.PhoneNumber;  // Обновление номера телефона пользователя
       user.Email = userEditDto.Email;  // Обновление электронной почты пользователя

       var result = await _userManager.UpdateAsync(user);  // Асинхронное обновление данных пользователя
       if (result.Succeeded)  // Если операция успешно выполнена
       {
           return Ok();  // Возврат успешного ответа
       }

       return BadRequest(result.Errors);  // Возврат ошибки с описанием, если произошла ошибка при обновлении
   }

   [HttpDelete("delete/{id}")]  // Обработчик HTTP DELETE запроса для удаления пользователя по идентификатору
   public async Task<IActionResult> DeleteUser(string id)  // Метод удаления пользователя
   {
       var user = await _userManager.FindByIdAsync(id);  // Поиск пользователя по идентификатору
       if (user == null) return NotFound();  // Возврат ошибки 404, если пользователь не найден

       var result = await _userManager.DeleteAsync(user);  // Асинхронное удаление пользователя
       if (result.Succeeded)  // Если операция успешно выполнена
       {
           return Ok();  // Возврат успешного ответа
       }

       return BadRequest(result.Errors);  // Возврат ошибки с описанием, если произошла ошибка при удалении
   }

   [HttpGet("search")]  // Обработчик HTTP GET запроса для поиска пользователей
   public IActionResult SearchUsers([FromQuery] UserSearchDto searchDto)  // Метод поиска пользователей
   {
       var users = _userManager.Users  // Получение списка пользователей
           .Where(u => (searchDto.Name == null || u.UserName.Contains(searchDto.Name)) &&  // Фильтрация по имени
                       (searchDto.Email == null || u.Email.Contains(searchDto.Email)))  // Фильтрация по электронной почте
           .ToList();  // Преобразование в список

       return Ok(users);  // Возврат найденных пользователей
   }

}
