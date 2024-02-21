using System.ComponentModel.DataAnnotations;

namespace Moment_4.Models;

    public class Category
    {


    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public List<Song>? Songs { get; set; }
}

