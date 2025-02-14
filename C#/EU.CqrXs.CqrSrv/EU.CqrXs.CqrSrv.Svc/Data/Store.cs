using System;
using System.Collections.Generic;

namespace EU.CqrXs.CqrSrv.Svc.Data
{
    public class Store
    {
        static Store()
        {
            Books = new List<Book>();
            Authors = new List<Author>();
            
            var authorMiguelCervantes = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Miguel de Cervantes"
            };
            var authorCharlesDickens = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Charles Dickens"
            };
            var authorJRRTolkien = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "J. R. R. Tolkien"
            };
            var authorAntoineSaintExupery = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Antoine de Saint-Exupéry"
            };
            var authorJKRowling = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "J. K. Rowling"
            };
            var authorAgathaChristie = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Agatha Christie"
            };
            var authorCaoXueqin = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Cao Xueqin"
            };
            var authorHRiderHaggard = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "H. Rider Haggard"
            };
            Authors.Add(authorMiguelCervantes);
            Authors.Add(authorCharlesDickens);
            Authors.Add(authorJRRTolkien);
            Authors.Add(authorAntoineSaintExupery);
            Authors.Add(authorJKRowling);
            Authors.Add(authorAgathaChristie);
            Authors.Add(authorCaoXueqin);
            Authors.Add(authorHRiderHaggard);

            var bookDonQuixote = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Don Quixote",
                Author = authorMiguelCervantes,
                FirstPublished = 1605,
                Language = new Language("en-UK")
            };
            var bookATaleOfTwoCities = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "A Tale Of Two Cities",
                Author = authorCharlesDickens,
                FirstPublished = 1859,
                Language = new Language("en-UK")
            };
            var bookTheLordOfTheRings = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "The Lord of the Rings",
                Author = authorJRRTolkien,
                FirstPublished = 1954,
                Language = new Language("es-ES")
            };
            var bookTheHobbit = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "The Hobbit",
                Author = authorJRRTolkien,
                FirstPublished = 1937,
                Language = new Language("en-UK")
            };
            var bookLePetitPrince = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Le Petit Prince",
                Author = authorAntoineSaintExupery,
                FirstPublished = 1943,
                Language = new Language("fr-FR")
            };
            var bookHarryPotterAndThePhilosopherStone = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Harry Potter and the Philosopher's Stone",
                Author = authorJKRowling,
                FirstPublished = 1997,
                Language = new Language("en-US")
            };
            var bookAndThenThereWereNone = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "And Then There Were None",
                Author = authorAgathaChristie,
                FirstPublished = 1939,
                Language = new Language("en-US")
            };
            var bookDreamOfTheRedChamber = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "紅樓夢/红楼梦 (Dream of the Red Chamber)",
                Author = authorCaoXueqin,
                FirstPublished = 1754,
                Language = new Language("zh")
            };
            var bookSheAHistoryOfAdventure = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "She: A History of Adventure",
                Author = authorHRiderHaggard,
                FirstPublished = 1887,
                Language = new Language("en")
            };
            Books.Add(bookDonQuixote);
            Books.Add(bookATaleOfTwoCities);
            Books.Add(bookTheLordOfTheRings);
            Books.Add(bookTheHobbit);
            Books.Add(bookLePetitPrince);
            Books.Add(bookHarryPotterAndThePhilosopherStone);
            Books.Add(bookAndThenThereWereNone);
            Books.Add(bookDreamOfTheRedChamber);
            Books.Add(bookSheAHistoryOfAdventure);
        }

        public static readonly List<Book> Books;
        public static readonly List<Author> Authors;
    }
}
