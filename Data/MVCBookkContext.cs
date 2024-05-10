﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MVCBookk.Models;

namespace MVCBookk.Data
{
    public class MVCBookkContext : DbContext
    {
        public MVCBookkContext (DbContextOptions<MVCBookkContext> options)
            : base(options)
        {
        }

        public DbSet<MVCBookk.Models.Author> Author { get; set; } = default!;
        public DbSet<MVCBookk.Models.Book> Book { get; set; } = default!;
        public DbSet<MVCBookk.Models.BookGenre> BookGenre { get; set; } = default!;
        public DbSet<MVCBookk.Models.Genre> Genre { get; set; } = default!;
        public DbSet<MVCBookk.Models.Review> Review { get; set; } = default!;
        public DbSet<MVCBookk.Models.UserBooks> UserBooks { get; set; } = default!;
    }
}
