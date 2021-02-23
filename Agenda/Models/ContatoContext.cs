using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Agenda.Models
{
    public class ContatoContext : DbContext
    {
        public ContatoContext() : base("MyDbConnection")
        {
        }

        public DbSet<Contato> contatos { get; set; }
    }
}