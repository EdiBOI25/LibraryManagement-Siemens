# Library Management System
## Description
Command-line application created for 2025 Siemens .NET Internship by Cioropariu Eduard

## Features
### Books
- Add books with title, author, quantity, and categories `book add --title ... --author ... --quantity n [--categories cat1,cat2,...]`
- Prevent duplicates: if a book with the same title and author exists, quantity is increased instead
- Delete books by ID (`book delete --id n`)
- List all books with filtering and/or sorting (`book list [--title ...] [--author ...] [--categories cat2,cat1,...] [--sortby id|title|author|rating]`)
- Display categories and average rating for each book

### Categories
- View all categories (`categories list`)
- Associate multiple categories with a book on creation

### Lending
- Borrow books by ID and borrower name (`lending borrow --bookid n --name ...`)
- Prevent borrow if no stock or if user already borrowed it
- Return books and provide an optional rating (`lending return --bookid ... --name ... [--rating n]`)
- Auto-update:
  - Book quantity
  - Average rating based on all return ratings
- List all or active lendings
  - Filter by book ID or borrower name (`lending list [--bookid n] [--name ...] [--active]`)

## Bonus Feature

### Category System + Rating-based Recommendations

The system supports:
- Associating books with one or more categories on creation (`--categories fantasy,romance`)
- Filtering books by category when listing
- Sorting books by average rating (`--sortby rating`)
- Viewing ratings directly in book list

## How to run/compile

### Debug Mode (from IDE)

1. Open the solution in Rider or Visual Studio
2. Set `LibraryManagement` as the startup project
3. Run the application
4. Use the commands mentioned above

### Release Mode (standalone exe)
1. Run the following command in the root folder (containing the .sln file):

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

The executable should be located at `LibraryManagement\bin\Release\net8.0\win-x64\publish\`
2. Use the commands mentioned above

## Regarding the database
You must drag and drop the `library.db` file inside the terminal and press Enter. The file should be located in the root folder.
