
using Labb3FunctionsAPI.Data;
using Labb3FunctionsAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Labb3FunctionsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<DataContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapPost("/api/character", async (DataContext db, Character newCharacter) =>
            {
                if (string.IsNullOrEmpty(newCharacter.name) || string.IsNullOrEmpty(newCharacter.species) || string.IsNullOrEmpty(newCharacter.about))
                {
                    return Results.BadRequest("Inavlid character information");
                }
                await db.Characters.AddAsync(newCharacter);
                await db.SaveChangesAsync();
                return Results.Ok($"Added character {newCharacter.name}");

            });

            app.MapGet("/api/characters", async (DataContext db) =>
            {
                var characters = await db.Characters.ToListAsync();
                if (characters.IsNullOrEmpty())
                {
                    return Results.NotFound("No characters found");
                }
                return Results.Ok(characters);
            });

            app.MapPut("/api/character/{id}", async (DataContext db, Character newCharacter, int id) =>
            {
                var oldCharacter = await db.Characters.FindAsync(id);
                if (oldCharacter == null)
                {
                    return Results.NotFound($"Character with id:{id} was not found");
                }
                oldCharacter.name = newCharacter.name;
                oldCharacter.species = newCharacter.species;
                oldCharacter.about = newCharacter.about;
                await db.SaveChangesAsync();
                return Results.Ok($"Character {id} {oldCharacter.name} was updated");
            });

            app.MapDelete("/api/character/{id}", async (DataContext db, int id) =>
            {
                var character = await db.Characters.FindAsync(id);
                if (character == null)
                {
                    return Results.NotFound($"Character with id:{id} was not found");
                }
                db.Characters.Remove(character);
                await db.SaveChangesAsync();
                return Results.Ok($"Character {id} {character.name} was removed");
            });

            app.Run();
        }
    }
}
