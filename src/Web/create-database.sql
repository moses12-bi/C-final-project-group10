-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PTMS_DB')
BEGIN
    CREATE DATABASE PTMS_DB;
END

-- Use the database
USE PTMS_DB;

-- Create Identity tables (simplified version)
CREATE TABLE IF NOT EXISTS __EFMigrationsHistory (
    MigrationId NVARCHAR(150) NOT NULL,
    ProductVersion NVARCHAR(32) NOT NULL,
    CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
);

-- The actual Identity tables will be created by EnsureCreated()
-- This script just ensures the database exists
