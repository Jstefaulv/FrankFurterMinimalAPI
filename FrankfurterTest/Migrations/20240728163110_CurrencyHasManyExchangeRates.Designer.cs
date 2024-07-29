﻿// <auto-generated />
using System;
using FrankfurterTest;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FrankfurterTest.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240728163110_CurrencyHasManyExchangeRates")]
    partial class CurrencyHasManyExchangeRates
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FrankfurterTest.Entities.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("FrankfurterTest.Entities.ExchangeRate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BaseCurrencyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Rate")
                        .HasColumnType("decimal(18,4)");

                    b.Property<int>("TargetCurrencyId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BaseCurrencyId");

                    b.HasIndex("TargetCurrencyId");

                    b.ToTable("ExchangesRates");
                });

            modelBuilder.Entity("FrankfurterTest.Entities.ExchangeRate", b =>
                {
                    b.HasOne("FrankfurterTest.Entities.Currency", "BaseCurrency")
                        .WithMany("BaseExchangeRates")
                        .HasForeignKey("BaseCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FrankfurterTest.Entities.Currency", "TargetCurrency")
                        .WithMany("TargetExchangeRates")
                        .HasForeignKey("TargetCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BaseCurrency");

                    b.Navigation("TargetCurrency");
                });

            modelBuilder.Entity("FrankfurterTest.Entities.Currency", b =>
                {
                    b.Navigation("BaseExchangeRates");

                    b.Navigation("TargetExchangeRates");
                });
#pragma warning restore 612, 618
        }
    }
}
