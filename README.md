DataImporter

Overview

DataImporter is a .NET console application designed to synchronize and transfer structured data from one database to another in a controlled and scalable way.

The application reads large data files (e.g., CSV/TSV), processes them in batches, inserts them into a staging table, validates and transforms the data, and then moves valid records into the target production tables.

The project focuses on performance, scalability, and clean data processing architecture.

⸻

Purpose

The main goal of this project is to:
	•	Synchronize large datasets between systems
	•	Handle high-volume data imports efficiently
	•	Separate raw data ingestion from validated data storage
	•	Ensure data integrity during transformation
	•	Demonstrate batch processing techniques in .NET

This approach is commonly used in real-world enterprise systems where data must be validated before entering production tables.

⸻

Architecture

The import process follows a structured multi-step pipeline:

1. File Reading
	•	Reads large input files (up to 1,000,000+ rows)
	•	Parses rows into in-memory objects
	•	Processes data in configurable batch sizes (e.g., 10,000 rows)

2. Staging Table Insert
	•	Inserts raw data into a staging table
	•	Uses batch processing to improve performance
	•	Reduces memory pressure and transaction size

3. Data Validation & Transformation
	•	Converts string fields into correct data types (int, decimal, etc.)
	•	Applies business rules
	•	Filters invalid records

4. Final Table Synchronization
	•	Transfers valid records into the production table
	•	Ensures only properly formatted and verified data is stored

⸻

Why Use a Staging Table?

The staging table provides:
	•	Isolation of raw imported data
	•	Validation before production usage
	•	Error handling without corrupting final tables
	•	Better control over large-scale data imports

This pattern is widely used in enterprise ETL (Extract, Transform, Load) systems.

⸻

Performance Strategy

To handle large datasets efficiently, the application:
	•	Processes records in batches (e.g., 10,000 rows per batch)
	•	Avoids loading the entire file into memory
	•	Uses optimized database insert operations
	•	Minimizes transaction overhead

This allows the system to process millions of rows reliably.

⸻

Technologies Used
	•	.NET
	•	C#
	•	SQL Server
	•	ADO.NET
	•	Batch Processing
	•	Git

⸻

How It Works (Step-by-Step)
	1.	Application reads the source data file
	2.	Data is parsed into row objects
	3.	Rows are inserted into the staging table in batches
	4.	Validation logic is executed
	5.	Valid rows are moved into the final target table
	6.	Invalid rows can be logged or reviewed separately
