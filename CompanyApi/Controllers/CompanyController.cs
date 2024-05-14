using CompanyApi.Data;
using CompanyApi.DTOs;
using CompanyApi.Models;

namespace CompanyApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[Authorize]
[ApiController]
[Route("api/[controller]")]  // Установка маршрута к контроллеру

public class CompanyController : ControllerBase  // Объявление класса контроллера CompanyController, унаследованного от ControllerBase
{
    private readonly ApplicationDbContext _context;  // Приватное поле для взаимодействия с контекстом базы данных

    public CompanyController(ApplicationDbContext context)  // Конструктор класса, инициализация контекста базы данных через внедрение зависимостей
    {
        _context = context;  // Присвоение контекста базы данных приватному полю
    }

    [HttpPost("create")]  // Обработчик HTTP POST запроса по пути /api/Company/create
    public async Task<IActionResult> CreateCompany(CreateCompanyDto companyDto)  // Метод создания новой компании
    {
        var company = new Company  // Создание нового объекта компании на основе переданных данных
        {
            Name = companyDto.Name,  // Назначение имени компании
            Country = companyDto.Country,  // Назначение страны
            BigBoss = companyDto.BigBoss  // Назначение руководителя компании
        };

        await _context.Companies.AddAsync(company);  // Асинхронное добавление компании в контекст базы данных
        await _context.SaveChangesAsync();  // Асинхронное сохранение изменений в базе данных

        return Ok(company);  // Возврат успешного ответа с созданной компанией
    }

    [HttpGet("get/{companyId}")]  // Обработчик HTTP GET запроса по пути /api/Company/get/{companyId}
    public IActionResult GetCompany(Guid companyId)  // Метод получения компании по идентификатору
    {
        var company = _context.Companies.Find(companyId);  // Получение компании из базы данных по идентификатору
        if (company == null)
            return NotFound("Company not found.");  // Возврат ошибки 404 с сообщением, если компания не найдена

        return Ok(company);  // Возврат успешного ответа с найденной компанией
    }
}