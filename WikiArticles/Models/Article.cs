using System;

namespace WikiArticles.Models
{
    public class Article
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        protected bool Equals(Article other)
        {
            return Id == other.Id && Title == other.Title;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Article) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title);
        }
    }
}
