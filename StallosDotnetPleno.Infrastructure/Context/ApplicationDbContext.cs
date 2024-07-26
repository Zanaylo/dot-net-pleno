using Microsoft.EntityFrameworkCore;
using StallosDotnetPleno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Infrastructure.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TipoPessoa> TipoPessoas { get; set; }
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<PessoaEndereco> PessoaEnderecos { get; set; }
    public DbSet<PessoaLista> PessoaListas { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // Configure primary keys
        modelBuilder.Entity<TipoPessoa>().HasKey(tp => tp.Id);
        modelBuilder.Entity<Pessoa>().HasKey(p => p.Id);
        modelBuilder.Entity<Endereco>().HasKey(e => e.Id);
        modelBuilder.Entity<PessoaEndereco>().HasKey(pe => new { pe.IdPessoa, pe.IdEndereco });
        modelBuilder.Entity<PessoaLista>().HasKey(pl => pl.Id);
        modelBuilder.Entity<User>().HasKey(u => u.Id);

        // Configure relationships
        modelBuilder.Entity<Pessoa>()
            .HasOne(p => p.TipoPessoa)
            .WithMany()
            .HasForeignKey(p => p.IdTipoPessoa);

        modelBuilder.Entity<PessoaEndereco>()
            .HasOne(pe => pe.Pessoa)
            .WithMany(p => p.PessoaEnderecos)
            .HasForeignKey(pe => pe.IdPessoa);

        modelBuilder.Entity<PessoaEndereco>()
            .HasOne(pe => pe.Endereco)
            .WithMany(e => e.PessoaEnderecos)
            .HasForeignKey(pe => pe.IdEndereco);

        modelBuilder.Entity<PessoaLista>()
            .HasOne(pl => pl.Pessoa)
            .WithMany(p => p.PessoaListas)
            .HasForeignKey(pl => pl.IdPessoa);
    }
}