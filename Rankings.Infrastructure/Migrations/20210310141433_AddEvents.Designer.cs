﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rankings.Infrastructure.Data;

namespace Rankings.Infrastructure.Migrations
{
    [DbContext(typeof(RankingContext))]
    [Migration("20210310141433_AddEvents")]
    partial class AddEvents
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Rankings.Core.Entities.DoubleGame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GameTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Identifier")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Player1Team1Id")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Player1Team2Id")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Player2Team1Id")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Player2Team2Id")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("RegistrationDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Score1")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Score2")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("VenueId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameTypeId");

                    b.HasIndex("Player1Team1Id");

                    b.HasIndex("Player1Team2Id");

                    b.HasIndex("Player2Team1Id");

                    b.HasIndex("Player2Team2Id");

                    b.HasIndex("VenueId");

                    b.ToTable("DoubleGames");
                });

            modelBuilder.Entity("Rankings.Core.Entities.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Body")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Identifier")
                        .HasColumnType("TEXT");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Rankings.Core.Entities.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GameTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Identifier")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Player1Id")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Player2Id")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Score1")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Score2")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SetScores1")
                        .HasColumnType("TEXT");

                    b.Property<string>("SetScores2")
                        .HasColumnType("TEXT");

                    b.Property<int?>("VenueId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameTypeId");

                    b.HasIndex("Player1Id");

                    b.HasIndex("Player2Id");

                    b.HasIndex("VenueId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Rankings.Core.Entities.GameType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("GameTypes");
                });

            modelBuilder.Entity("Rankings.Core.Entities.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("Identifier")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Rankings.Core.Entities.Venue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Venues");
                });

            modelBuilder.Entity("Rankings.Core.Entities.DoubleGame", b =>
                {
                    b.HasOne("Rankings.Core.Entities.GameType", "GameType")
                        .WithMany()
                        .HasForeignKey("GameTypeId");

                    b.HasOne("Rankings.Core.Entities.Profile", "Player1Team1")
                        .WithMany()
                        .HasForeignKey("Player1Team1Id");

                    b.HasOne("Rankings.Core.Entities.Profile", "Player1Team2")
                        .WithMany()
                        .HasForeignKey("Player1Team2Id");

                    b.HasOne("Rankings.Core.Entities.Profile", "Player2Team1")
                        .WithMany()
                        .HasForeignKey("Player2Team1Id");

                    b.HasOne("Rankings.Core.Entities.Profile", "Player2Team2")
                        .WithMany()
                        .HasForeignKey("Player2Team2Id");

                    b.HasOne("Rankings.Core.Entities.Venue", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId");

                    b.Navigation("GameType");

                    b.Navigation("Player1Team1");

                    b.Navigation("Player1Team2");

                    b.Navigation("Player2Team1");

                    b.Navigation("Player2Team2");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("Rankings.Core.Entities.Game", b =>
                {
                    b.HasOne("Rankings.Core.Entities.GameType", "GameType")
                        .WithMany()
                        .HasForeignKey("GameTypeId");

                    b.HasOne("Rankings.Core.Entities.Profile", "Player1")
                        .WithMany()
                        .HasForeignKey("Player1Id");

                    b.HasOne("Rankings.Core.Entities.Profile", "Player2")
                        .WithMany()
                        .HasForeignKey("Player2Id");

                    b.HasOne("Rankings.Core.Entities.Venue", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId");

                    b.Navigation("GameType");

                    b.Navigation("Player1");

                    b.Navigation("Player2");

                    b.Navigation("Venue");
                });
#pragma warning restore 612, 618
        }
    }
}