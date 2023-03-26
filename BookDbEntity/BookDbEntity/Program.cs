// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BookData
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
    }

    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }
    }

    public class BookDataContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BookDbAtore;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Insert a book
            using (var context = new BookDataContext())
            {
                var publisher = new Publisher { Name = "ABC Publishing" };
                var book = new Book { Title = "Book Title", Author = "Author Name", Publisher = publisher };

                context.Add(book);
                context.SaveChanges();
            }

            // Update a book
            using (var context = new BookDataContext())
            {
                var book = context.Books.Find(1);

                if (book != null)
                {
                    book.Title = "New Book Title";
                    book.Author = "New Author Name";

                    context.SaveChanges();
                }
            }

            // Get a book by id
            using (var context = new BookDataContext())
            {
                var book = context.Books.Find(1);

                if (book != null)
                {
                    Console.WriteLine($"Book Title: {book.Title}, Author: {book.Author}");
                }
            }

            // Get all books
            using (var context = new BookDataContext())
            {
                var books = context.Books.Include(b => b.Publisher);

                foreach (var book in books)
                {
                    Console.WriteLine($"Book Title: {book.Title}, Author: {book.Author}, Publisher: {book.Publisher.Name}");
                }
            }

            Console.ReadLine();
        }
    }
}
