using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarWarsMovies.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    episode_id = table.Column<int>(type: "integer", nullable: false),
                    opening_crawl = table.Column<string>(type: "text", nullable: false),
                    director = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    producer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    release_date = table.Column<DateOnly>(type: "date", nullable: false),
                    source_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_from_external_api = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movie_external_resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    movie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_external_resources", x => x.id);
                    table.ForeignKey(
                        name: "FK_movie_external_resources_movies_movie_id",
                        column: x => x.movie_id,
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_movie_external_resources_movie_id_type_url",
                table: "movie_external_resources",
                columns: new[] { "movie_id", "type", "url" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movies_external_id",
                table: "movies",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movie_external_resources");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "movies");
        }
    }
}
