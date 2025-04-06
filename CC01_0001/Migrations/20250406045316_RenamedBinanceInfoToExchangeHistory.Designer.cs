﻿// <auto-generated />
using System;
using CC01_0001.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CC01_0001.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250406045316_RenamedBinanceInfoToExchangeHistory")]
    partial class RenamedBinanceInfoToExchangeHistory
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.14");

            modelBuilder.Entity("CC01_0001.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("CC01_0001.Models.BinanceTrade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("EventTime")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "E");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "e");

                    b.Property<bool>("Ignore")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "M");

                    b.Property<bool>("IsBuyerMarketMaker")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "m");

                    b.Property<string>("Price")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "p");

                    b.Property<string>("Quantity")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "q");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "s");

                    b.Property<long>("TradeId")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "t");

                    b.Property<long>("TradeTime")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "T");

                    b.HasKey("Id");

                    b.ToTable("BinanceTrades");
                });

            modelBuilder.Entity("CC01_0001.Models.ExchangeFilter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BinanceExchangeInfoId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BinanceExchangeInfoId");

                    b.ToTable("ExchangeFilters");

                    b.HasAnnotation("Relational:JsonPropertyName", "exchangeFilters");
                });

            modelBuilder.Entity("CC01_0001.Models.ExchangeHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("ServerTime")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "serverTime");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("Timezone")
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "timezone");

                    b.HasKey("Id");

                    b.ToTable("BinanceExchangeInfos");
                });

            modelBuilder.Entity("CC01_0001.Models.Filter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FilterType")
                        .HasColumnType("TEXT");

                    b.Property<string>("MaxPrice")
                        .HasColumnType("TEXT");

                    b.Property<string>("MaxQty")
                        .HasColumnType("TEXT");

                    b.Property<string>("MinPrice")
                        .HasColumnType("TEXT");

                    b.Property<string>("MinQty")
                        .HasColumnType("TEXT");

                    b.Property<string>("StepSize")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SymbolId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TickSize")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SymbolId");

                    b.ToTable("Filters");
                });

            modelBuilder.Entity("CC01_0001.Models.OrderType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("TypeTag")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("OrderTypes");
                });

            modelBuilder.Entity("CC01_0001.Models.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("PermissionTag")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("CC01_0001.Models.PermissionSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SymbolId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SymbolId");

                    b.ToTable("PermissionSets");
                });

            modelBuilder.Entity("CC01_0001.Models.PermissionSetPermissions", b =>
                {
                    b.Property<int>("PermissionSetId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PermissionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PermissionSetId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("PermissionSetPermissions");
                });

            modelBuilder.Entity("CC01_0001.Models.RateLimit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BinanceExchangeInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Interval")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("IntervalNum")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Limit")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RateLimitType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BinanceExchangeInfoId");

                    b.ToTable("RateLimits");

                    b.HasAnnotation("Relational:JsonPropertyName", "rateLimits");
                });

            modelBuilder.Entity("CC01_0001.Models.Symbol", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AllowTrailingStop")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BaseAsset")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("BaseAssetPrecision")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BaseCommissionPrecision")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BinanceExchangeInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CancelReplaceAllowed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IcebergAllowed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMarginTradingAllowed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSpotTradingAllowed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("OcoAllowed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("OtoAllowed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("QuoteAsset")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("QuoteAssetPrecision")
                        .HasColumnType("INTEGER");

                    b.Property<int>("QuoteCommissionPrecision")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("QuoteOrderQtyMarketAllowed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("QuotePrecision")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SymbolName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BinanceExchangeInfoId");

                    b.ToTable("Symbols");

                    b.HasAnnotation("Relational:JsonPropertyName", "symbols");
                });

            modelBuilder.Entity("CC01_0001.Models.SymbolOrderType", b =>
                {
                    b.Property<int>("SymbolId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OrderTypeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("SymbolId", "OrderTypeId");

                    b.HasIndex("OrderTypeId");

                    b.ToTable("SymbolOrderTypes");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("CC01_0001.Models.ExchangeFilter", b =>
                {
                    b.HasOne("CC01_0001.Models.ExchangeHistory", "BinanceExchangeInfo")
                        .WithMany("ExchangeFilters")
                        .HasForeignKey("BinanceExchangeInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BinanceExchangeInfo");
                });

            modelBuilder.Entity("CC01_0001.Models.Filter", b =>
                {
                    b.HasOne("CC01_0001.Models.Symbol", "Symbol")
                        .WithMany("Filters")
                        .HasForeignKey("SymbolId");

                    b.Navigation("Symbol");
                });

            modelBuilder.Entity("CC01_0001.Models.PermissionSet", b =>
                {
                    b.HasOne("CC01_0001.Models.Symbol", null)
                        .WithMany("PermissionSets")
                        .HasForeignKey("SymbolId");
                });

            modelBuilder.Entity("CC01_0001.Models.PermissionSetPermissions", b =>
                {
                    b.HasOne("CC01_0001.Models.Permission", "Permission")
                        .WithMany("PermissionSetPermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CC01_0001.Models.PermissionSet", "PermissionSet")
                        .WithMany("PermissionSetPermissions")
                        .HasForeignKey("PermissionSetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("PermissionSet");
                });

            modelBuilder.Entity("CC01_0001.Models.RateLimit", b =>
                {
                    b.HasOne("CC01_0001.Models.ExchangeHistory", "BinanceExchangeInfo")
                        .WithMany("RateLimits")
                        .HasForeignKey("BinanceExchangeInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BinanceExchangeInfo");
                });

            modelBuilder.Entity("CC01_0001.Models.Symbol", b =>
                {
                    b.HasOne("CC01_0001.Models.ExchangeHistory", "BinanceExchangeInfo")
                        .WithMany("Symbols")
                        .HasForeignKey("BinanceExchangeInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BinanceExchangeInfo");
                });

            modelBuilder.Entity("CC01_0001.Models.SymbolOrderType", b =>
                {
                    b.HasOne("CC01_0001.Models.OrderType", "OrderType")
                        .WithMany("SymbolOrderTypes")
                        .HasForeignKey("OrderTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CC01_0001.Models.Symbol", "Symbol")
                        .WithMany("SymbolOrderTypes")
                        .HasForeignKey("SymbolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderType");

                    b.Navigation("Symbol");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("CC01_0001.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("CC01_0001.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CC01_0001.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("CC01_0001.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CC01_0001.Models.ExchangeHistory", b =>
                {
                    b.Navigation("ExchangeFilters");

                    b.Navigation("RateLimits");

                    b.Navigation("Symbols");
                });

            modelBuilder.Entity("CC01_0001.Models.OrderType", b =>
                {
                    b.Navigation("SymbolOrderTypes");
                });

            modelBuilder.Entity("CC01_0001.Models.Permission", b =>
                {
                    b.Navigation("PermissionSetPermissions");
                });

            modelBuilder.Entity("CC01_0001.Models.PermissionSet", b =>
                {
                    b.Navigation("PermissionSetPermissions");
                });

            modelBuilder.Entity("CC01_0001.Models.Symbol", b =>
                {
                    b.Navigation("Filters");

                    b.Navigation("PermissionSets");

                    b.Navigation("SymbolOrderTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
