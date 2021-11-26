namespace Videooverflow.Server.Core;

public record ResourceDTOs(DateTime date, Type type , int ID, Uri site_url, string site_title, User creator, Uri content_source, float lixNumber , SkillLevel skillLevel , string language );

public record CharacterDetailsDto(int Id, string? GivenName, string? Surname, string? AlterEgo, string? City, Gender Gender, int? FirstAppearance, string? Occupation, string? ImageUrl, IReadOnlySet<string> Powers) : CharacterDto(Id, GivenName, Surname, AlterEgo);

public record CharacterCreateDto
{
    [StringLength(50)]
    public string? GivenName { get; init; }

    [StringLength(50)]
    public string? Surname { get; init; }

    [StringLength(50)]
    public string? AlterEgo { get; init; }

    [Range(1900, 2100)]
    public int? FirstAppearance { get; init; }

    [StringLength(50)]
    public string? Occupation { get; init; }

    public string? City { get; init; }

    public Gender Gender { get; init; }

    [Required]
    public ISet<string> Powers { get; init; } = null!;
}

public record CharacterUpdateDto : CharacterCreateDto
{
    public int Id { get; init; }
}