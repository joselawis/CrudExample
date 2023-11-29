using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
                CREATE OR REPLACE FUNCTION insert_person(
                    p_person_id uuid,
                    p_person_name varchar(40),
                    p_email varchar(40),
                    p_date_of_birth date,
                    p_gender varchar(6),
                    p_country_id uuid,
                    p_address varchar(200),
                    p_receive_newsletters boolean
                )
                    RETURNS void AS $$
                BEGIN
                    INSERT INTO public.""Persons"" (
                        ""PersonId"",
                        ""PersonName"",
                        ""Email"",
                        ""DateOfBirth"",
                        ""Gender"",
                        ""CountryId"",
                        ""Address"",
                        ""ReceiveNewsLetters""
                    ) VALUES (
                                 p_person_id,
                                 p_person_name,
                                 p_email,
                                 p_date_of_birth,
                                 p_gender,
                                 p_country_id,
                                 p_address,
                                 p_receive_newsletters
                             );
                END;
                $$ LANGUAGE plpgsql;
                ";
            migrationBuilder.Sql(sp_InsertPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
                DROP FUNCTION public.insert_person();
                ";
            migrationBuilder.Sql(sp_InsertPerson);
        }
    }
}
