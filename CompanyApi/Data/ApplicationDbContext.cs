using CompanyApi.Models;  
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 

using Microsoft.EntityFrameworkCore;  

namespace CompanyApi.Data  
{
    public class ApplicationDbContext : IdentityDbContext<User>  // Определение класса контекста базы данных, унаследованного от IdentityDbContext с пользовательской моделью User
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}  // Конструктор класса, принимающий параметр options и передающий его в базовый конструктор

        public DbSet<Company> Companies { get; set; }  // Определение свойства DbSet для работы с таблицей Companies в базе данных
    }
}