using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetPersons_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                CREATE OR REPLACE FUNCTION get_all_persons()
                    RETURNS TABLE (
                        PersonId UUID,
                        PersonName VARCHAR(255),
                        Email VARCHAR(255),
                        DateOfBirth date,
                        Gender VARCHAR(10),
                        CountryId UUID,
                        Address VARCHAR(255),
                        ReceiveNewsLetters BOOLEAN
                    )
                AS $$
                BEGIN
                    RETURN QUERY SELECT
                        ""PersonId"",
                        ""PersonName"",
                        ""Email"",
                        ""DateOfBirth"",
                        ""Gender"",
                        ""CountryId"",
                        ""Address"",
                        ""ReceiveNewsLetters""
                    FROM public.""Persons"";
                END;
                $$ LANGUAGE plpgsql;
                ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                DROP FUNCTION public.get_all_persons();
                ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
