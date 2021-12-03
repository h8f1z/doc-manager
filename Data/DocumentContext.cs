using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace doc_manager.Data;
public class DocumentContext : DbContext
{
  public DocumentContext(DbContextOptions options) : base(options)
  {
  }

  public DbSet<Models.Document> Document { get; set; }
}