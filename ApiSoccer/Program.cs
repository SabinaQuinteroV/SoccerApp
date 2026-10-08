using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SoccerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Angular");

app.MapGet("/", () => "API Soccer arriba!!!");

// ==========================================
// TABLE LEAGUE
// ==========================================

// GET ver registros 
app.MapGet("/leagues", async (SoccerDbContext db) =>
{
    return await db.Leagues.ToListAsync();
});

// POST crear registro
app.MapPost("/leagues", async (League league, [FromServices] SoccerDbContext db) =>
{
    if (league.StartDate == default)
        league.StartDate = DateTime.Now;

    if (league.EndDate == default)
        league.EndDate = DateTime.Now;
    
    if (league.StartDate > league.EndDate)
    {
        return Results.BadRequest("La fecha de inicio debe ser menor a la fecha de fin");
    }

    db.Leagues.Add(league);

    await db.SaveChangesAsync();
    return Results.Created($"/leagues/{league.id}", league);
});

//PUT Actualizar registro 
app.MapPut("/leagues/{id}", async (int id, League leagueUpdate, [FromServices] SoccerDbContext db) =>
{
    var leagueDB = await db.Leagues.FindAsync(id);
    if (leagueDB is null) return Results.NotFound();

    leagueDB.Name = leagueUpdate.Name;
    leagueDB.Country = leagueUpdate.Country;
    leagueDB.StartDate = leagueUpdate.StartDate;
    leagueDB.EndDate = leagueUpdate.EndDate;
    leagueDB.Enabled = leagueUpdate.Enabled;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE: Borrar 
app.MapDelete("/leagues/{id}", async (int id, [FromServices]  SoccerDbContext db) =>
{
    var leagueDB = await db.Leagues.FindAsync(id);
    if (leagueDB is null) return Results.NotFound();

    db.Leagues.Remove(leagueDB);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


// ==========================================
// TABLE TEAM
// ==========================================

// GET ver registros 
app.MapGet("/teams", async (SoccerDbContext db) =>
{
    return await db.Teams.ToListAsync();
});

// POST crear registro
app.MapPost("/teams", async (Team team, [FromServices] SoccerDbContext db) =>
{
    if (team.PlayersQuantity < 11 || team.PlayersQuantity > 22)
    {
        return Results.BadRequest("El equipo debe tener entre 11 y 22 jugadores.");
    }

    db.Teams.Add(team);
    await db.SaveChangesAsync();
    return Results.Created($"/teams/{team.id}", team);
});

//PUT Actualizar registro 
app.MapPut("/teams/{id}", async (int id, Team teamsUpdate, [FromServices] SoccerDbContext db) =>
{
    var teamsDB = await db.Teams.FindAsync(id);

    if (teamsDB is null) return Results.NotFound();

        if (teamsUpdate.PlayersQuantity < 11 || teamsUpdate.PlayersQuantity > 22)
        {
            return Results.BadRequest("El equipo debe tener entre 11 y 22 jugadores.");
        }

    teamsDB.Name = teamsUpdate.Name;
    teamsDB.Country = teamsUpdate.Country;
    teamsDB.PlayersQuantity = teamsUpdate.PlayersQuantity;
    teamsDB.Enabled = teamsUpdate.Enabled;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE: Borrar 
app.MapDelete("/teams/{id}", async (int id, [FromServices] SoccerDbContext db) =>
{
    var teamsDB = await db.Teams.FindAsync(id);
    if (teamsDB is null) return Results.NotFound();

    db.Teams.Remove(teamsDB);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

// ==========================================
public class League{
    public int id {get; set;}
    public string Name {get; set;} = "";
    public string Country {get; set;} = "";
    public DateTime StartDate {get; set;}
    public DateTime EndDate {get; set;}
    public bool Enabled {get; set;} = true;
}

public class Team{
    public int id {get; set;}
    public string Name {get; set;} = "";
    public string Country {get; set;} = "";
    public int PlayersQuantity {get; set;}
    public bool Enabled {get; set;} = true;
}
public class LeagueTeam
{
    public int LeagueId { get; set; }
    public int TeamId { get; set; }
}

public class SoccerDbContext : DbContext
{
    public DbSet<League> Leagues => Set<League>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<LeagueTeam> LeagueTeams => Set<LeagueTeam>();
    public SoccerDbContext(DbContextOptions<SoccerDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<League>().ToTable("League");
        modelBuilder.Entity<Team>().ToTable("Team");
        modelBuilder.Entity<LeagueTeam>().ToTable("LeagueTeam");

        modelBuilder.Entity<LeagueTeam>()
            .HasKey(x => new { x.LeagueId, x.TeamId });
    }

}

