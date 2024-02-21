using System.ComponentModel.DataAnnotations;

namespace Moment_4.Models;

    public class Song

    {

    public int Id { get; set; }


    [Required]
    public string? Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Msg from model. Length must be greater than 0")]
    
    //miniumm 1, max ej definerad
    public int LengthInSeconds { get; set; }

    public string? ImageFilePath { get; set; }


    // Foreign key for Category
    
    public int CategoryId { get; set; }

    // Navigation property back to Category
    
    public Category? Category { get; set; }

}
