﻿// <auto-generated />
using System;
using JOS.StreamDatabase.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JOS.StreamDatabase.Database.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("JOS.StreamDatabase.Core.RealEstate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_real_estate");

                    b.ToTable("real_estate", (string)null);
                });

            modelBuilder.Entity("JOS.StreamDatabase.Core.RealEstateImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("RealEstateId")
                        .HasColumnType("uuid")
                        .HasColumnName("real_estate_id");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<byte[]>("data")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("data");

                    b.HasKey("Id")
                        .HasName("pk_real_estate_images");

                    b.HasIndex("RealEstateId")
                        .HasDatabaseName("ix_real_estate_images_real_estate_id");

                    b.ToTable("real_estate_images", (string)null);
                });

            modelBuilder.Entity("JOS.StreamDatabase.Core.RealEstateImage", b =>
                {
                    b.HasOne("JOS.StreamDatabase.Core.RealEstate", null)
                        .WithMany("_images")
                        .HasForeignKey("RealEstateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_real_estate_images_real_estate_real_estate_id");

                    b.OwnsOne("JOS.StreamDatabase.Core.ImageMetadata", "Metadata", b1 =>
                        {
                            b1.Property<Guid>("RealEstateImageId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Extension")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Filename")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("MimeType")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<long>("Size")
                                .HasColumnType("bigint");

                            b1.HasKey("RealEstateImageId");

                            b1.ToTable("real_estate_images");

                            b1.ToJson("metadata");

                            b1.WithOwner()
                                .HasForeignKey("RealEstateImageId")
                                .HasConstraintName("fk_real_estate_images_real_estate_images_id");
                        });

                    b.Navigation("Metadata")
                        .IsRequired();
                });

            modelBuilder.Entity("JOS.StreamDatabase.Core.RealEstate", b =>
                {
                    b.Navigation("_images");
                });
#pragma warning restore 612, 618
        }
    }
}
