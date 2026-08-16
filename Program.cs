using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace minimalEF;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument();
        var schoolConnectionString = builder.Configuration.GetConnectionString("School");
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(schoolConnectionString));
        var app = builder.Build();

        app.UseOpenApi();
        app.UseSwaggerUi();

        app.MapGet("/", () => "School Days!");
        app.MapGet("/students", async (AppDbContext dbContext) =>
        {
            var students = await dbContext.Student.ToListAsync();
            return Results.Ok(students);
        });
        app.MapPost("/student", async (AppDbContext dbContext, Student student) =>
        {
            dbContext.Add(student);
            var rowsInserted = await dbContext.SaveChangesAsync();
            return Results.Ok($"{rowsInserted} rows inserted");
        });

        app.Run();
    }
}
public class Student
{
    public int StudentId { get; set; }

    [MaxLength(150)]
    public string Name { get; set; }
    public DateOnly DateOfBirth { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Student> Student { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions)
    : base(dbContextOptions)
    {

    }
}
