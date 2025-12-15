namespace POS.Domain.Models.CQRS.Commands.Base;

public abstract class BaseCommand
{
}

public class CreateOrUpdateCommand : BaseCommand
{
}

public class CreateCommand : CreateOrUpdateCommand
{
}

public class UpdateCommand : CreateOrUpdateCommand
{
    public Guid Id { get; set; }

    public bool IsDeleted { get; set; } = false;
}

public class DeleteCommand : BaseCommand
{
    public Guid Id { get; set; }

    public bool IsPermanent { get; set; } = false;
}