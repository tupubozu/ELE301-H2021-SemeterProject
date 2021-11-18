using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace SemesterProject.DatabaseCommunication
{
    public interface IBased
    {
        NpgsqlConnection Database { get; set; }
    }
}
